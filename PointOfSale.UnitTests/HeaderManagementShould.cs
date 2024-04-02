using Microsoft.AspNetCore.Http;
using Moq;
using PointOfSale.Middleware;
using PointOfSale.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class HeaderManagementShould
    {
        [Fact]
        public async Task ClearAllCookiesWhenCookiesAreInvalid()
        {
            HeaderManagement headerManagement = new HeaderManagement((httpContext) => { return Task.CompletedTask; });

            HttpContext mockHttpContext = new DefaultHttpContext();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();
            Mock<ICookieManagement> mockCookieManagement = new Mock<ICookieManagement>();
            mockCookieManagement.Setup(a => a.IsCookieDataValid(It.IsAny<IRequestCookieCollection>())).Returns(false);

            await headerManagement.Invoke(mockHttpContext, mockHeaderService.Object, mockCookieManagement.Object);
            mockCookieManagement.Verify(a => a.ClearAllCookies(It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()), Times.Once);
        }

        [Fact]
        public async Task UpdateHeaderAndCookieValuesWhenValidationCookieIsNotTrue()
        {
            HeaderManagement headerManagement = new HeaderManagement((httpContext) => { return Task.CompletedTask; });

            HttpContext mockHttpContext = new DefaultHttpContext();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();
            Mock<ICookieManagement> mockCookieManagement = new Mock<ICookieManagement>();
            mockCookieManagement.Setup(a => a.IsCookieDataValid(It.IsAny<IRequestCookieCollection>())).Returns(true);
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "validation")).Returns("");

            await headerManagement.Invoke(mockHttpContext, mockHeaderService.Object, mockCookieManagement.Object);
            mockHeaderService.Verify(a => a.SetStoreNumber(), Times.Once);
            mockHeaderService.Verify(a => a.SetRegisterNumberAsync(It.IsAny<HttpContext>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "validation", It.IsAny<string>(), new TimeSpan(0, 5, 0)), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "storenumber", It.IsAny<int>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "registernumber", It.IsAny<int>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "registerid", It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task ExtractCookieValuesAndUpdateHeaderWhenValuesAreValid()
        {
            int expectedStoreNumber = 42;
            int expectedRegisterNumber = 1;
            Guid expectedRegisterId = Guid.NewGuid();

            HeaderManagement headerManagement = new HeaderManagement((httpContext) => { return Task.CompletedTask; });

            HttpContext mockHttpContext = new DefaultHttpContext();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();

            Mock<ICookieManagement> mockCookieManagement = new Mock<ICookieManagement>();
            mockCookieManagement.Setup(a => a.IsCookieDataValid(It.IsAny<IRequestCookieCollection>())).Returns(true);
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "validation")).Returns("true");
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "storenumber")).Returns(expectedStoreNumber.ToString());
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "registernumber")).Returns(expectedRegisterNumber.ToString());
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "registerid")).Returns(expectedRegisterId.ToString());

            await headerManagement.Invoke(mockHttpContext, mockHeaderService.Object, mockCookieManagement.Object);

            mockHeaderService.VerifySet(a => a.StoreNumber = expectedStoreNumber);
            mockHeaderService.VerifySet(a => a.RegisterNumber = expectedRegisterNumber);
            mockHeaderService.VerifySet(a => a.RegisterId = expectedRegisterId);
        }

        [Fact]
        public async Task UpdateCookieAndHeaderValuesWhenStoreNumberInvalid()
        {
            int invalidStoreNumber = 0;
            int registerNumber = 1;
            Guid registerId = Guid.NewGuid();

            HeaderManagement headerManagement = new HeaderManagement((httpContext) => { return Task.CompletedTask; });

            HttpContext mockHttpContext = new DefaultHttpContext();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();
            Mock<ICookieManagement> mockCookieManagement = new Mock<ICookieManagement>();
            mockCookieManagement.Setup(a => a.IsCookieDataValid(It.IsAny<IRequestCookieCollection>())).Returns(true);
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "validation")).Returns("true");
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "storenumber")).Returns(invalidStoreNumber.ToString());
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "registernumber")).Returns(registerNumber.ToString());
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "registerid")).Returns(registerId.ToString());

            await headerManagement.Invoke(mockHttpContext, mockHeaderService.Object, mockCookieManagement.Object);
            mockHeaderService.Verify(a => a.SetStoreNumber(), Times.Once);
            mockHeaderService.Verify(a => a.SetRegisterNumberAsync(It.IsAny<HttpContext>()), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "storenumber"), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "registernumber"), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "registerid"), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "validation", It.IsAny<string>(), new TimeSpan(0, 5, 0)), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "storenumber", It.IsAny<int>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "registernumber", It.IsAny<int>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "registerid", It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCookieAndHeaderValuesWhenRegisterNumberInvalid()
        {
            int invalidStoreNumber = 42;
            int registerNumber = 0;
            Guid registerId = Guid.NewGuid();

            HeaderManagement headerManagement = new HeaderManagement((httpContext) => { return Task.CompletedTask; });

            HttpContext mockHttpContext = new DefaultHttpContext();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();
            Mock<ICookieManagement> mockCookieManagement = new Mock<ICookieManagement>();
            mockCookieManagement.Setup(a => a.IsCookieDataValid(It.IsAny<IRequestCookieCollection>())).Returns(true);
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "validation")).Returns("true");
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "storenumber")).Returns(invalidStoreNumber.ToString());
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "registernumber")).Returns(registerNumber.ToString());
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "registerid")).Returns(registerId.ToString());

            await headerManagement.Invoke(mockHttpContext, mockHeaderService.Object, mockCookieManagement.Object);
            mockHeaderService.Verify(a => a.SetStoreNumber(), Times.Once);
            mockHeaderService.Verify(a => a.SetRegisterNumberAsync(It.IsAny<HttpContext>()), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "storenumber"), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "registernumber"), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "registerid"), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "validation", It.IsAny<string>(), new TimeSpan(0, 5, 0)), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "storenumber", It.IsAny<int>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "registernumber", It.IsAny<int>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "registerid", It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCookieAndHeaderValuesWhenRegisterIdInvalid()
        {
            int invalidStoreNumber = 42;
            int registerNumber = 1;
            Guid registerId = Guid.Empty;

            HeaderManagement headerManagement = new HeaderManagement((httpContext) => { return Task.CompletedTask; });

            HttpContext mockHttpContext = new DefaultHttpContext();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();
            Mock<ICookieManagement> mockCookieManagement = new Mock<ICookieManagement>();
            mockCookieManagement.Setup(a => a.IsCookieDataValid(It.IsAny<IRequestCookieCollection>())).Returns(true);
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "validation")).Returns("true");
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "storenumber")).Returns(invalidStoreNumber.ToString());
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "registernumber")).Returns(registerNumber.ToString());
            mockCookieManagement.Setup(a => a.GetRequestCookieValue(It.IsAny<IRequestCookieCollection>(), "registerid")).Returns(registerId.ToString());

            await headerManagement.Invoke(mockHttpContext, mockHeaderService.Object, mockCookieManagement.Object);
            mockHeaderService.Verify(a => a.SetStoreNumber(), Times.Once);
            mockHeaderService.Verify(a => a.SetRegisterNumberAsync(It.IsAny<HttpContext>()), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "storenumber"), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "registernumber"), Times.Once);
            mockCookieManagement.Verify(a => a.GetRequestCookieValue(mockHttpContext.Request.Cookies, "registerid"), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "validation", It.IsAny<string>(), new TimeSpan(0, 5, 0)), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "storenumber", It.IsAny<int>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "registernumber", It.IsAny<int>()), Times.Once);
            mockCookieManagement.Verify(a => a.UpdateResponseCookieWithValue(mockHttpContext.Response.Cookies, "registerid", It.IsAny<Guid>()), Times.Once);
        }
    }
}
