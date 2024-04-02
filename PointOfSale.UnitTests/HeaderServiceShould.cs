using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using PointOfSale.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using RegisterResponse = DSG.POS.PosRegisterManager.Models.Response;

namespace PointOfSale.UnitTests
{
    public class HeaderServiceShould
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ICookieManagement> _mockCookieManagement;
        private Mock<IRegisterManagerClient> _mockRegisterManagerClient;
        private Mock<IDnsService> _mockDnsService;
        private HeaderService _headerService;

        public HeaderServiceShould()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockCookieManagement = new Mock<ICookieManagement>();
            _mockRegisterManagerClient = new Mock<IRegisterManagerClient>();
            _mockDnsService = new Mock<IDnsService>();

            _headerService = new HeaderService(_mockCookieManagement.Object, _mockRegisterManagerClient.Object, _mockConfiguration.Object, _mockDnsService.Object);
        }

        [Fact]
        public void SetStoreNumberWhenFoundInConfiguration()
        {
            int expectedStoreNumber = 42;
            _mockConfiguration.Setup(a => a["developmentStoreNumber"]).Returns(expectedStoreNumber.ToString());
            _headerService.SetStoreNumber();
            Assert.Equal(expectedStoreNumber, _headerService.StoreNumber);
        }

        [Fact]
        public void SetStoreNumberTo879WhenNotFoundInConfiguration()
        {
            int expectedStoreNumber = 879;
            _headerService.SetStoreNumber();
            Assert.Equal(expectedStoreNumber, _headerService.StoreNumber);
        }

        [Fact]
        public async Task ValidateRegisterNumberWhenFoundInCookie()
        {
            int expectedRegisterNumber = 7;
            Guid expectedGuid = Guid.NewGuid();
            RegisterResponse.RegisterValidation expectedRegisterValidation = new RegisterResponse.RegisterValidation()
            {
                IsValid = true
            };

            RegisterResponse.Register expectedRegister = new RegisterResponse.Register()
            {
                RegisterId = Guid.NewGuid(),
                StoreNumber = 42,
                RegisterNumber = 1
            };

            _mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), CookieManagement.RegisterIdCookieName)).Returns(expectedGuid.ToString());
            _mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), CookieManagement.RegisterNumberCookieName)).Returns(expectedRegisterNumber.ToString());
            _mockDnsService.Setup(a => a.GetHostnameFromIp(It.IsAny<IPAddress>())).ReturnsAsync("MockHostname");
            _mockRegisterManagerClient.Setup(a => a.Validation(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(expectedRegisterValidation);
            _mockRegisterManagerClient.Setup(a => a.RegisterNumber(_headerService.StoreNumber, "MockHostname")).ReturnsAsync(expectedRegister);

            HttpContext httpContext = new DefaultHttpContext();

            await _headerService.SetRegisterNumberAsync(httpContext);

            Assert.Equal(expectedRegisterNumber, _headerService.RegisterNumber);
        }

        [Fact]
        public async Task GetRegisterNumberWhenRegisterNumberNotFoundInCookie()
        {
            int expectedRegisterNumber = 4;
            Guid expectedGuid = Guid.NewGuid();
            RegisterResponse.Register register = new RegisterResponse.Register()
            {
                RegisterId = expectedGuid,
                RegisterNumber = expectedRegisterNumber,
                StoreNumber = 1
            };

            _mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "RegisterId")).Returns(Guid.NewGuid().ToString());
            _mockDnsService.Setup(a => a.GetHostnameFromIp(It.IsAny<IPAddress>())).ReturnsAsync("MockHostname");
            _mockRegisterManagerClient.Setup(a => a.RegisterNumber(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(register);

            HttpContext httpContext = new DefaultHttpContext();

            await _headerService.SetRegisterNumberAsync(httpContext);

            Assert.Equal(expectedRegisterNumber, _headerService.RegisterNumber);
        }

        [Fact]
        public async Task GetRegisterNumberWhenRegisterIdNotFoundInCookie()
        {
            int expectedRegisterNumber = 4;
            Guid expectedGuid = Guid.NewGuid();
            RegisterResponse.Register register = new RegisterResponse.Register()
            {
                RegisterId = expectedGuid,
                RegisterNumber = expectedRegisterNumber,
                StoreNumber = 1
            };

            _mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "RegisterId")).Returns(Guid.Empty.ToString());
            _mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "RegisterNumber")).Returns("1");
            _mockDnsService.Setup(a => a.GetHostnameFromIp(It.IsAny<IPAddress>())).ReturnsAsync("MockHostname");
            _mockRegisterManagerClient.Setup(a => a.RegisterNumber(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(register);

            HttpContext httpContext = new DefaultHttpContext();

            await _headerService.SetRegisterNumberAsync(httpContext);

            Assert.Equal(expectedRegisterNumber, _headerService.RegisterNumber);
        }

        [Fact]
        public async Task GetRegisterNumberWhenFoundInCookieButFailsValidation()
        {
            int expectedRegisterNumber = 5;
            Guid expectedGuid = Guid.NewGuid();

            RegisterResponse.RegisterValidation expectedRegisterValidation = new RegisterResponse.RegisterValidation()
            {
                IsValid = false
            };

            RegisterResponse.Register register = new RegisterResponse.Register()
            {
                RegisterId = expectedGuid,
                RegisterNumber = expectedRegisterNumber,
                StoreNumber = 1
            };

            _mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "RegisterId")).Returns(expectedGuid.ToString());
            _mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "RegisterNumber")).Returns(expectedRegisterNumber.ToString());
            _mockDnsService.Setup(a => a.GetHostnameFromIp(It.IsAny<IPAddress>())).ReturnsAsync("MockHostname");
            _mockRegisterManagerClient.Setup(a => a.Validation(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(expectedRegisterValidation);
            _mockRegisterManagerClient.Setup(a => a.RegisterNumber(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(register);

            HttpContext httpContext = new DefaultHttpContext();

            await _headerService.SetRegisterNumberAsync(httpContext);

            Assert.Equal(expectedRegisterNumber, _headerService.RegisterNumber);
        }
    }
}
