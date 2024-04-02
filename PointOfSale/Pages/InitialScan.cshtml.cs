using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSale.Models;
using PointOfSale.Services;
using System.Threading.Tasks;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.Pages
{
    public class InitialScanModel : PageModel
    {
        private readonly IHeaderService _headerService;
        private readonly IProductClient _productClient;
        private readonly ITransactionManagerClient _transactionManagerClient;

        private UpcLookupResult _upcLookupResult;

        public UpcLookupResult UpcLookupResult
        {
            get
            {
                return _upcLookupResult;
            }
        }

        public InitialScanModel(IHeaderService headerService, IProductClient productClient, ITransactionManagerClient transactionManagerClient)
        {
            _headerService = headerService;
            _productClient = productClient;
            _transactionManagerClient = transactionManagerClient;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Response.Transaction transaction = await _transactionManagerClient.ActiveTransaction(_headerService.StoreNumber, _headerService.RegisterNumber);

            if (transaction == null)
                return Page();
            else
                return RedirectToPage("Sale");
        }

        public async Task<IActionResult> OnPostEnterAsync(string barcodeData)
        {
            if (string.IsNullOrEmpty(barcodeData))
                return Page();

            ReceiptItem receiptItem = await _productClient.LookupProduct(_headerService.StoreNumber, barcodeData);

            if (receiptItem != null)
            {
                Response.Transaction transaction = await _transactionManagerClient.NewTransaction(_headerService.StoreNumber, _headerService.RegisterNumber);
                await _transactionManagerClient.NewItem(_headerService.StoreNumber, _headerService.RegisterNumber, receiptItem);
                return RedirectToPage("Sale");
            }
            else
            {
                // TODO: Show error on page that item does not exist
                _upcLookupResult = new UpcLookupResult()
                {
                    Success = false,
                    Upc = barcodeData
                };

                return Page();
            }
        }
    }
}