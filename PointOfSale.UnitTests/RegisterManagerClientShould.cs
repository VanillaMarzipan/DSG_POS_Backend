using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PointOfSale.Options;
using PointOfSale.Services;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Response = DSG.POS.PosRegisterManager.Models.Response;

namespace PointOfSale.UnitTests
{
    public class RegisterManagerClientShould
    {
        [Fact]
        public async Task SuccessfullyValidateRegisterWhenSendingExistingRegister()
        {
            Response.RegisterValidation registerValidation = new Response.RegisterValidation()
            {
                IsValid = true
            };

            RegisterManagerClient registerManagerClient = GetRegisterManagerClient(registerValidation);
            Response.RegisterValidation goodRegister = await registerManagerClient.Validation(Guid.NewGuid(), 42, 1, "testHostname");

            Assert.True(goodRegister.IsValid);
        }

        [Fact]
        public async Task FailValidateRegisterWhenSendingNonMatchingRegister()
        {
            Response.RegisterValidation registerValidation = new Response.RegisterValidation()
            {
                IsValid = false
            };

            RegisterManagerClient registerManagerClient = GetRegisterManagerClient(registerValidation);
            Response.RegisterValidation registerOk = await registerManagerClient.Validation(Guid.NewGuid(), 42, 1, "testHostname");

            Assert.False(registerOk.IsValid);
        }

        [Fact]
        public async Task ReturnValidRegisterWhenRequestingNewRegister()
        {
            Response.Register expectedRegister = new Response.Register()
            {
                RegisterId = Guid.NewGuid(),
                StoreNumber = 42,
                RegisterNumber = 1
            };

            RegisterManagerClient registerManagerClient = GetRegisterManagerClient(expectedRegister);
            Response.Register actualRegister = await registerManagerClient.RegisterNumber(42, "testHostname");

            Assert.NotNull(actualRegister);

            Assert.Equal(expectedRegister.RegisterId, actualRegister.RegisterId);
            Assert.Equal(expectedRegister.StoreNumber, actualRegister.StoreNumber);
            Assert.Equal(expectedRegister.RegisterNumber, actualRegister.RegisterNumber);
        }

        private RegisterManagerClient GetRegisterManagerClient<T>(T testContentObject)
        {
            IOptions<RegisterManagerOptions> registerManagerOptions = Microsoft.Extensions.Options.Options.Create(
                new RegisterManagerOptions()
                {
                    BaseUrl = "http://localhost",
                    RegisterOptions = new RegisterOptions()
                    {
                        Endpoints = new RegisterOptions.RegisterEndpoints()
                        {
                            RegisterNumber = string.Empty,
                            ValidateRegister = string.Empty
                        }
                    }
                });

            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(testContentObject))
                });

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler.Object);
            RegisterManagerClient registerManagerClient = new RegisterManagerClient(httpClient, registerManagerOptions);

            return registerManagerClient;
        }
    }
}
