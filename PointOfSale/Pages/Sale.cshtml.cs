using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PointOfSale.Models;
using PointOfSale.Services;
using System.Threading.Tasks;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.Pages
{
    public class SaleModel : PageModel
    {
        private readonly IHeaderService _headerService;
        private readonly ITransactionManagerClient _transactionManagerClient;
        private readonly IProductClient _productClient;
        private readonly ILogger<SaleModel> _logger;

        private UpcLookupResult _upcLookupResult;

        public UpcLookupResult UpcLookupResult
        {
            get
            {
                return _upcLookupResult;
            }
        }

        public Response.Transaction Transaction { get; private set; }

        public SaleModel(IHeaderService headerService, ITransactionManagerClient transactionManagerClient, IProductClient productClient, ILogger<SaleModel> logger)
        {
            _headerService = headerService;
            _transactionManagerClient = transactionManagerClient;
            _productClient = productClient;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            Transaction = await _transactionManagerClient.ActiveTransaction(_headerService.StoreNumber, _headerService.RegisterNumber);

            if (Transaction == null)
                Transaction = await _transactionManagerClient.NewTransaction(_headerService.StoreNumber, _headerService.RegisterNumber);
        }

        public async Task<IActionResult> OnPostEnterAsync(string barcodeData)
        {
            _logger.LogTrace($"Received request for barcode [{barcodeData}]");
            //TODO: Need error handling
            //TODO: Need to revisit how this system executes a sequence of events upon an action (state machine) to simplify maintenance, code reuse, etc.
            // Lookup item
            ReceiptItem receiptItem = await _productClient.LookupProduct(_headerService.StoreNumber, barcodeData);

            if (receiptItem != null)
                Transaction = await _transactionManagerClient.NewItem(_headerService.StoreNumber, _headerService.RegisterNumber, receiptItem);
            else
            {
                _upcLookupResult = new UpcLookupResult()
                {
                    Success = false,
                    Upc = barcodeData
                };

                Transaction = await _transactionManagerClient.ActiveTransaction(_headerService.StoreNumber, _headerService.RegisterNumber);
            }

            return Page();
        }

        public async Task OnPostDeleteItemAsync(int transactionItemIdentifier)
        {
            _logger.LogTrace($"Received delete request for item [{transactionItemIdentifier}]");
            Transaction = await _transactionManagerClient.DeleteItem(_headerService.StoreNumber, _headerService.RegisterNumber, transactionItemIdentifier);
        }
    }
}
