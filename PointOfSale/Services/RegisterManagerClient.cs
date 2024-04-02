using Flurl;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PointOfSale.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Response = DSG.POS.PosRegisterManager.Models.Response;

namespace PointOfSale.Services
{
    public interface IRegisterManagerClient
    {
        Task<Response.Register> RegisterNumber(int storeNumber, string hostname);
        Task<Response.RegisterValidation> Validation(Guid registerId, int storeNumber, int registerNumber, string hostname);
    }

    public class RegisterManagerClient : IRegisterManagerClient
    {
        private readonly HttpClient _httpClient;
        private RegisterManagerOptions _registerManagerOptions;
        private readonly string _registerManagerBaseUrl;


        public RegisterManagerClient(HttpClient httpClient, IOptions<RegisterManagerOptions> registerManagerOptions)
        {
            _httpClient = httpClient;
            _registerManagerOptions = registerManagerOptions.Value;
            _registerManagerBaseUrl = _registerManagerOptions.BaseUrl;
        }

        public async Task<Response.Register> RegisterNumber(int storeNumber, string hostname)
        {
            Response.Register registerResponse = null;

            string requestUri = Url.Combine(_registerManagerBaseUrl, _registerManagerOptions.RegisterOptions.Endpoints.RegisterNumber)
                                   .AppendPathSegments(storeNumber, hostname);

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    registerResponse = JsonConvert.DeserializeObject<Response.Register>(responseString);

                    return registerResponse;
                }
                else
                {
                    // TODO: Once error handling is implmented, we need to check for a status code of NoContent and handle that differently then an actual error.
                    return registerResponse;
                }
            }
            else
            {
                return registerResponse;
            }
        }

        public async Task<Response.RegisterValidation> Validation(Guid registerId, int storeNumber, int registerNumber, string hostname)
        {
            Response.RegisterValidation registerResponse = null;

            string requestUri = Url.Combine(_registerManagerBaseUrl, _registerManagerOptions.RegisterOptions.Endpoints.ValidateRegister)
                                   .AppendPathSegments(registerId, storeNumber, registerNumber, hostname);

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    registerResponse = JsonConvert.DeserializeObject<Response.RegisterValidation>(responseString);

                    return registerResponse;
                }
                else
                {
                    // TODO: Once error handling is implmented, we need to check for a status code of NoContent and handle that differently then an actual error.
                    return registerResponse;
                }
            }
            else
            {
                return registerResponse;
            }
        }
    }
}
