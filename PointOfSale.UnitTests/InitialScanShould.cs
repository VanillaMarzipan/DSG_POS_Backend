using DSG.POS.Common.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using PointOfSale.Models;
using PointOfSale.Pages;
using PointOfSale.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.UnitTests
{
    public class InitialScanShould
    {
        [Fact]
        public async Task DisplayInitialScanPageIfTransactionIsNull()
        {
            int storeNumber = 42;
            int registerNumber = 1;

            ReceiptItem receiptItem = new ReceiptItem()
            {
                UPC = "123456789012",
                Description = "Test Item 1",
                Price = 19.99M,
                Quantity = 1,
                SelectedItem = false
            };

            Mock<ITransactionManagerClient> mockTransactionManagerClient = new Mock<ITransactionManagerClient>();
            Mock<IProductClient> mockProductClient = new Mock<IProductClient>();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();

            mockTransactionManagerClient.Setup(t => t.ActiveTransaction(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Response.Transaction)null);
            mockHeaderService.SetupGet(a => a.StoreNumber).Returns(storeNumber);
            mockHeaderService.SetupGet(a => a.RegisterNumber).Returns(registerNumber);

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            InitialScanModel initialScanModel = new InitialScanModel(mockHeaderService.Object, mockProductClient.Object, mockTransactionManagerClient.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
            };

            var actionResult = await initialScanModel.OnGetAsync();
            Assert.IsType<PageResult>(actionResult);
        }

        [Fact]
        public async Task RedirectToSalePageIfTransactionIsNotNull()
        {
            int storeNumber = 42;
            int registerNumber = 1;

            Response.Transaction transaction = new Response.Transaction()
            {
                Header = new Response.Header()
                {
                    StoreNumber = 42,
                    RegisterNumber = 1,
                    TransactionNumber = 1234,
                    StartDateTime = DateTime.UtcNow,
                    TransactionStatus = TransactionStatus.Active
                },
                Items = new List<Response.Item>()
                {
                    new Response.Item()
                    {
                        Description = "Test Item 1",
                        UnitPrice = 17.98M,
                        Quantity = 1,
                        Upc = "123456789012"
                    },
                    new Response.Item()
                    {
                        Description = "Test Item 2",
                        UnitPrice = 199.97M,
                        Quantity = 1,
                        Upc = "987654321098"
                    }
                },
                Tenders = new List<Response.Tender>()
                {
                    new Response.Tender()
                    {
                        Amount = 220.00M,
                        TenderType = TenderType.Cash
                    }
                },
                Total = new Response.Total()
                {
                    SubTotal = 217.95M,
                    Tax = 0.00M,
                    GrandTotal = 217.95M,
                    ChangeDue = 2.05M
                }
            };

            ReceiptItem receiptItem = new ReceiptItem()
            {
                UPC = "123456789012",
                Description = "Test Item 1",
                Price = 19.99M,
                Quantity = 1,
                SelectedItem = false
            };

            Mock<ITransactionManagerClient> mockTransactionManagerClient = new Mock<ITransactionManagerClient>();
            Mock<IProductClient> mockProductClient = new Mock<IProductClient>();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();

            mockTransactionManagerClient.Setup(t => t.ActiveTransaction(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(transaction);
            mockHeaderService.SetupGet(a => a.StoreNumber).Returns(storeNumber);
            mockHeaderService.SetupGet(a => a.RegisterNumber).Returns(registerNumber);

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            InitialScanModel initialScanModel = new InitialScanModel(mockHeaderService.Object, mockProductClient.Object, mockTransactionManagerClient.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
            };

            var actionResult = await initialScanModel.OnGetAsync();
            Assert.IsType<RedirectToPageResult>(actionResult);

            var okResult = actionResult as RedirectToPageResult;
            Assert.Equal("Sale", okResult.PageName);
        }

        [Fact]
        public async Task StayOnSamePageOnFailedItemLookup()
        {
            int storeNumber = 42;
            int registerNumber = 1;

            Mock<ITransactionManagerClient> mockTransactionManagerClient = new Mock<ITransactionManagerClient>();
            Mock<IProductClient> mockProductClient = new Mock<IProductClient>();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();

            mockTransactionManagerClient.Setup(t => t.ActiveTransaction(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Response.Transaction)null);
            mockHeaderService.SetupGet(a => a.StoreNumber).Returns(storeNumber);
            mockHeaderService.SetupGet(a => a.RegisterNumber).Returns(registerNumber);

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            InitialScanModel initialScanModel = new InitialScanModel(mockHeaderService.Object, mockProductClient.Object, mockTransactionManagerClient.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
            };

            var actionResult = await initialScanModel.OnGetAsync();
            Assert.IsType<PageResult>(actionResult);
        }

        [Fact]
        public async Task NavigateToSalePageOnSuccessfulItemLookup()
        {
            int storeNumber = 42;
            int registerNumber = 1;

            ReceiptItem receiptItem = new ReceiptItem()
            {
                UPC = "123456789012",
                Description = "Test Item 1",
                Price = 19.99M,
                Quantity = 1,
                SelectedItem = false
            };

            Mock<ITransactionManagerClient> mockTransactionManagerClient = new Mock<ITransactionManagerClient>();
            Mock<IProductClient> mockProductClient = new Mock<IProductClient>();
            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();

            mockTransactionManagerClient.Setup(t => t.ActiveTransaction(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Response.Transaction)null);
            mockProductClient.Setup(p => p.LookupProduct(storeNumber, receiptItem.UPC)).ReturnsAsync(receiptItem);
            mockHeaderService.SetupGet(a => a.StoreNumber).Returns(42);
            mockHeaderService.SetupGet(a => a.RegisterNumber).Returns(1);

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            InitialScanModel initialScanModel = new InitialScanModel(mockHeaderService.Object, mockProductClient.Object, mockTransactionManagerClient.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
            };

            var actionResult = await initialScanModel.OnPostEnterAsync(receiptItem.UPC);
            Assert.IsType<RedirectToPageResult>(actionResult);

            var okResult = actionResult as RedirectToPageResult;
            Assert.Equal("Sale", okResult.PageName);
        }
    }
}
