using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PointOfSale.Models;
using PointOfSale.Pages.Components;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class ReceiptItemViewComponentShould
    {
        [Fact]
        public async Task ReturnViewViewComponentResultOnSuccessfulInvokeAsync()
        {
            string upc = "012345678901";
            string description = "Test Item";
            decimal price = (decimal)19.99;
            int quantity = 1;

            ReceiptItem receiptItem = new ReceiptItem()
            {
                UPC = upc,
                Description = description,
                Price = price,
                Quantity = quantity
            };

            ReceiptItemViewComponent receiptItemViewComponent = new ReceiptItemViewComponent();
            IViewComponentResult result = await receiptItemViewComponent.InvokeAsync(receiptItem);

            Assert.IsType<ViewViewComponentResult>(result);

            ViewViewComponentResult componentResult = ((ViewViewComponentResult)result);
            Assert.IsType<ReceiptItem>(componentResult.ViewData.Model);

            ReceiptItem resultReceiptItem = (ReceiptItem)componentResult.ViewData.Model;
            Assert.Equal(receiptItem.UPC, resultReceiptItem.UPC);
            Assert.Equal(receiptItem.Description, resultReceiptItem.Description);
            Assert.Equal(receiptItem.Price, resultReceiptItem.Price);
            Assert.Equal(receiptItem.Quantity, resultReceiptItem.Quantity);
        }
    }
}
