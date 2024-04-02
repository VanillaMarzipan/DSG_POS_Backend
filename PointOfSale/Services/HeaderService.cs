using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;
using RegisterResponse = DSG.POS.PosRegisterManager.Models.Response;

namespace PointOfSale.Services
{
    public interface IHeaderService
    {
        int StoreNumber { get; set; }
        int RegisterNumber { get; set; }
        Guid RegisterId { get; set; }
        void SetStoreNumber();
        Task SetRegisterNumberAsync(HttpContext context);
    }

    public class HeaderService : IHeaderService
    {
        private int _storeNumber;
        private int _registerNumber;
        private Guid _registerId;

        public int StoreNumber { get { return _storeNumber; } set { if (_storeNumber == 0) _storeNumber = value; } }
        public int RegisterNumber { get { return _registerNumber; } set { if (_registerNumber == 0) _registerNumber = value; } }
        public Guid RegisterId { get { return _registerId; } set { if (_registerId == Guid.Empty) _registerId = value; } }

        private readonly ICookieManagement _cookieManagement;
        private readonly IRegisterManagerClient _registerManagerClient;
        private readonly IConfiguration _configuration;
        private readonly IDnsService _dnsService;

        public HeaderService(ICookieManagement cookieManagement, IRegisterManagerClient registerManagerClient, IConfiguration configuration, IDnsService dnsService)
        {
            _cookieManagement = cookieManagement;
            _registerManagerClient = registerManagerClient;
            _configuration = configuration;
            _dnsService = dnsService;
        }

        public void SetStoreNumber()
        {
            if (int.TryParse(_configuration["developmentStoreNumber"], out int storeNumber))
                _storeNumber = storeNumber;
            else
                _storeNumber = 879;
        }

        public async Task SetRegisterNumberAsync(HttpContext context)
        {
            Guid.TryParse(_cookieManagement.GetRequestCookieValue(context.Request.Cookies, CookieManagement.RegisterIdCookieName), out Guid registerId);
            int.TryParse(_cookieManagement.GetRequestCookieValue(context.Request.Cookies, CookieManagement.RegisterNumberCookieName), out int registerNumber);

            IPAddress clientIp = context.Connection.RemoteIpAddress;
            string hostname = await _dnsService.GetHostnameFromIp(clientIp);

            RegisterResponse.RegisterValidation registerValidation = new RegisterResponse.RegisterValidation()
            {
                IsValid = false
            };

            // When cookie values are present, validate the register
            if (registerNumber != 0 &&
                registerId != Guid.Empty)
            {
                registerValidation = await _registerManagerClient.Validation(registerId, StoreNumber, registerNumber, hostname);
            }

            // Get new register number if current is unknown or invalid
            if (!registerValidation.IsValid)
            {
                RegisterResponse.Register register = await _registerManagerClient.RegisterNumber(StoreNumber, hostname);

                _cookieManagement.UpdateResponseCookieWithValue(context.Response.Cookies, CookieManagement.RegisterIdCookieName, register.RegisterId);
                _cookieManagement.UpdateResponseCookieWithValue(context.Response.Cookies, CookieManagement.RegisterNumberCookieName, register.RegisterNumber);
                _registerNumber = register.RegisterNumber;
                _registerId = register.RegisterId;
            }
            else
            {
                _registerNumber = registerNumber;
                _registerId = registerId;
            }
        }
    }
}
