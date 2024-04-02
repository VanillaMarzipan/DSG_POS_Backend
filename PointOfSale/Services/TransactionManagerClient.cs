using Flurl;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PointOfSale.Models;
using PointOfSale.Options;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Request = DSG.POS.PosTransactionManager.Models.Request;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.Services
{
    public interface ITransactionManagerClient
    {
        Task<Response.Transaction> NewTransaction(int storeNumber, int registerNumber);
        Task<Response.Transaction> ActiveTransaction(int storeNumber, int registerNumber);
        Task<Response.Transaction> FinalizeTransaction(int storeNumber, int registerNumber);
        Task<Response.Transaction> NewItem(int storeNumber, int registerNumber, ReceiptItem receiptItem);
        Task<Response.Transaction> NewTender(int storeNumber, int registerNumber, Request.Tender tender);
        Task<Response.Transaction> DeleteItem(int storeNumber, int registerNumber, int transactionItemIdentifier);
    }

    public class TransactionManagerClient : ITransactionManagerClient
    {
        private readonly HttpClient _httpClient;
        private TransactionManagerOptions _transactionManagerOptions { get; set; }
        private readonly string _transactionManagerBaseUrl;

        public TransactionManagerClient(HttpClient httpClient, IOptions<TransactionManagerOptions> transactionManagerOptions)
        {
            _httpClient = httpClient;
            _transactionManagerOptions = transactionManagerOptions.Value;
            _transactionManagerBaseUrl = _transactionManagerOptions.BaseUrl;
        }

        public async Task<Response.Transaction> NewTransaction(int storeNumber, int registerNumber)
        {
            Request.Transaction newTransactionRequest = new Request.Transaction()
            {
                StoreNumber = storeNumber,
                RegisterNumber = registerNumber
            };

            Response.Transaction transactionResponse = null;

            string requestUri = Url.Combine(_transactionManagerBaseUrl, _transactionManagerOptions.TransactionOptions.Endpoints.NewTransaction);

            var response = await _httpClient.PostAsJsonAsync(requestUri, newTransactionRequest);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                transactionResponse = JsonConvert.DeserializeObject<Response.Transaction>(responseString);
            }

            return transactionResponse;
        }

        public async Task<Response.Transaction> ActiveTransaction(int storeNumber, int registerNumber)
        {
            Response.Transaction transactionResponse = null;

            string requestUri = Url.Combine(_transactionManagerBaseUrl, _transactionManagerOptions.TransactionOptions.Endpoints.ActiveTransaction)
                                   .AppendPathSegments(storeNumber, registerNumber);

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    transactionResponse = JsonConvert.DeserializeObject<Response.Transaction>(responseString);

                    return transactionResponse;
                }
                else
                {
                    // TODO: Once error handling is implmented, we need to check for a status code of NoContent and handle that differently then an actual error.
                    return transactionResponse;
                }
            }
            else
            {
                return transactionResponse;
            }
        }

        public async Task<Response.Transaction> FinalizeTransaction(int storeNumber, int registerNumber)
        {
            Request.Transaction newTransactionRequest = new Request.Transaction()
            {
                StoreNumber = storeNumber,
                RegisterNumber = registerNumber
            };

            Response.Transaction transactionResponse = null;

            string requestUri = Url.Combine(_transactionManagerBaseUrl, _transactionManagerOptions.TransactionOptions.Endpoints.FinalizeTransaction);

            var response = await _httpClient.PostAsJsonAsync(requestUri, newTransactionRequest);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                transactionResponse = JsonConvert.DeserializeObject<Response.Transaction>(responseString);
            }

            return transactionResponse;
        }

        public async Task<Response.Transaction> NewItem(int storeNumber, int registerNumber, ReceiptItem receiptItem)
        {
            Request.Item newTransactionItemRequest = new Request.Item()
            {
                Upc = receiptItem.UPC,
                Description = receiptItem.Description,
                Quantity = receiptItem.Quantity,
                UnitPrice = receiptItem.Price,
                StoreNumber = storeNumber,
                RegisterNumber = registerNumber
            };

            Response.Transaction transactionResponse = null;

            string requestUri = Url.Combine(_transactionManagerBaseUrl, _transactionManagerOptions.ItemOptions.Endpoints.NewItem);

            var response = await _httpClient.PostAsJsonAsync(requestUri, newTransactionItemRequest);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                transactionResponse = JsonConvert.DeserializeObject<Response.Transaction>(responseString);
            }

            return transactionResponse;
        }

        public async Task<Response.Transaction> DeleteItem(int storeNumber, int registerNumber, int transactionItemIdentifier)
        {
            Response.Transaction transactionResponse = null;

            string requestUri = Url.Combine(_transactionManagerBaseUrl, _transactionManagerOptions.ItemOptions.Endpoints.DeleteItem)
                                   .AppendPathSegments(storeNumber, registerNumber, transactionItemIdentifier);

            var response = await _httpClient.PostAsync(requestUri, null);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                transactionResponse = JsonConvert.DeserializeObject<Response.Transaction>(responseString);
            }

            return transactionResponse;
        }

        public async Task<Response.Transaction> NewTender(int storeNumber, int registerNumber, Request.Tender tender)
        {
            Request.Tender newTenderRequest = new Request.Tender()
            {
                TenderType = tender.TenderType,
                Amount = tender.Amount,
                StoreNumber = storeNumber,
                RegisterNumber = registerNumber
            };

            Response.Transaction transaction = null;

            string requestUri = Url.Combine(_transactionManagerBaseUrl, _transactionManagerOptions.TenderOptions.Endpoints.NewTender);

            var response = await _httpClient.PostAsJsonAsync(requestUri, newTenderRequest);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                transaction = JsonConvert.DeserializeObject<Response.Transaction>(responseString);
            }

            return transaction;
        }
    }
}