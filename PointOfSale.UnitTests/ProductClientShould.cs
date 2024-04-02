using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PointOfSale.Models;
using PointOfSale.Options;
using PointOfSale.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class ProductClientShould
    {
        [Fact]
        public async Task ReturnValidReceiptItem()
        {
            string upc = "012345678901";
            string description = "Test Item";
            decimal price = (decimal)19.99;

            Product product = new Product()
            {
                UPC = upc,
                Description = description,
                Price = price
            };

            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(product))
                });

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler.Object);

            IOptions<ProductServiceOptions> productServiceOptions = Microsoft.Extensions.Options.Options.Create(
                new ProductServiceOptions()
                {
                    BaseUrl = "http://localhost",
                    ProductLookupOptions = new ProductLookupOptions()
                    {
                        Endpoints = new ProductLookupOptions.ProductLookupEndpoints()
                        {
                            UpcLookup = string.Empty
                        }
                    }
                });

            ReceiptItem expectedReceiptItem = new ReceiptItem()
            {
                Description = description,
                Price = price,
                Quantity = 1,
                UPC = upc
            };

            ProductClient productClient = new ProductClient(httpClient, productServiceOptions);

            ReceiptItem actualReceiptItem = await productClient.LookupProduct(42, upc);

            Assert.NotNull(actualReceiptItem);

            Assert.Equal(expectedReceiptItem.UPC, actualReceiptItem.UPC);
            Assert.Equal(expectedReceiptItem.Description, actualReceiptItem.Description);
            Assert.Equal(expectedReceiptItem.Price, actualReceiptItem.Price);
            Assert.Equal(expectedReceiptItem.Quantity, actualReceiptItem.Quantity);
        }
    }
}
