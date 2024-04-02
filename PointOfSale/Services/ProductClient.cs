using Flurl;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PointOfSale.Models;
using PointOfSale.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace PointOfSale.Services
{
    public interface IProductClient
    {
        Task<ReceiptItem> LookupProduct(int storeNumber, string upc);
    }

    public class ProductClient : IProductClient
    {
        private readonly HttpClient _httpClient;
        private readonly ProductServiceOptions _productServiceOptions;
        private readonly string _productServiceBaseUrl;

        public ProductClient(HttpClient httpClient, IOptions<ProductServiceOptions> productServiceOptions)
        {
            _httpClient = httpClient;
            _productServiceOptions = productServiceOptions.Value;
            _productServiceBaseUrl = _productServiceOptions.BaseUrl;
        }

        public async Task<ReceiptItem> LookupProduct(int storeNumber, string upc)
        {
            string requestUri = Url.Combine(_productServiceBaseUrl, _productServiceOptions.ProductLookupOptions.Endpoints.UpcLookup)
                                   .AppendPathSegment(storeNumber)
                                   .AppendPathSegments(upc);

            ReceiptItem receiptItem = null;

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                Product product = JsonConvert.DeserializeObject<Product>(responseString);

                receiptItem = new ReceiptItem()
                {
                    Description = product.Description,
                    UPC = product.UPC,
                    Price = product.Price,
                    Quantity = 1
                };
            }

            return receiptItem;
        }
    }
}