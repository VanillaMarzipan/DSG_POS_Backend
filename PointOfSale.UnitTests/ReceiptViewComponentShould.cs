using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using PointOfSale.Models;
using PointOfSale.ViewComponents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.UnitTests
{
    public class ReceiptViewComponentShould
    {
        [Fact]
        public async Task ReturnReceiptViewWithGivenTransaction()
        {
            Response.Transaction transaction = new Response.Transaction()
            {
                Header = new Response.Header()
                {
                    StoreNumber = 879,
                    RegisterNumber = 1,
                    TransactionNumber = 2,
                    StartDateTime = DateTime.UtcNow,
                    TransactionStatus = DSG.POS.Common.Enumerations.TransactionStatus.Active
                },
                Items = new List<Response.Item>()
                {
                    new Response.Item()
                    {
                        Description = "Test Item",
                        TransactionItemIdentifier = 1,
                        UnitPrice = 19.99M,
                        Quantity = 1,
                        Upc = "01234567890",
                        NonTaxable = false,
                        ItemTaxes = new List<Response.ItemTax>()
                    }
                },
                Tenders = new List<Response.Tender>()
                {
                    new Response.Tender()
                    {
                        Amount = 19.99M,
                        TenderType = DSG.POS.Common.Enumerations.TenderType.Cash
                    }
                },
                Total = new Response.Total()
                {
                    SubTotal = 19.99M,
                    Tax = 0.00M,
                    GrandTotal = 20.00M,
                    ChangeDue = 0.01M
                }
            };

            ReceiptViewComponent receiptItemViewComponent = new ReceiptViewComponent();
            IViewComponentResult result = await receiptItemViewComponent.InvokeAsync(transaction, true, false);

            Assert.IsType<ViewViewComponentResult>(result);

            ViewViewComponentResult componentResult = ((ViewViewComponentResult)result);
            Assert.IsType<ReceiptViewModel>(componentResult.ViewData.Model);
            ReceiptViewModel receiptViewModel = (ReceiptViewModel)componentResult.ViewData.Model;

            Assert.IsType<Response.Transaction>(receiptViewModel.Transaction);

            Response.Transaction actualTransaction = (Response.Transaction)receiptViewModel.Transaction;
            Assert.Equal(transaction, actualTransaction);
        }
    }
}
