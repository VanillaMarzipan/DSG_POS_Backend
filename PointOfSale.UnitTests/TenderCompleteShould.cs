using DSG.POS.Common.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using PointOfSale.Pages;
using PointOfSale.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.UnitTests
{
    public class TenderCompleteShould
    {
        [Fact]
        public async Task PopulatePropertiesOnPageGet()
        {
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

            Mock<IHeaderService> mockHeaderService = new Mock<IHeaderService>();
            Mock<ITransactionManagerClient> mockTransactionManagerClient = new Mock<ITransactionManagerClient>();

            mockTransactionManagerClient.Setup(t => t.FinalizeTransaction(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(transaction);

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

            TenderCompleteModel tenderCompleteModel = new TenderCompleteModel(mockHeaderService.Object, mockTransactionManagerClient.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext)
            };

            await tenderCompleteModel.OnGetAsync();

            Assert.Equal(transaction.Total.ChangeDue, tenderCompleteModel.FinalizedTransaction.Total.ChangeDue);
            Assert.Equal(transaction.Tenders[0].Amount, tenderCompleteModel.FinalizedTransaction.Tenders[0].Amount);
        }
    }
}
