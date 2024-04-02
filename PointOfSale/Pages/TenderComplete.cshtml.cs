using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSale.Services;
using System.Threading.Tasks;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.Pages
{
    public class TenderCompleteModel : PageModel
    {
        private readonly IHeaderService _headerService;
        private readonly ITransactionManagerClient _transactionManagerClient;

        public Response.Transaction FinalizedTransaction { get; set; }

        public TenderCompleteModel(IHeaderService headerService, ITransactionManagerClient transactionManagerClient)
        {
            _headerService = headerService;
            _transactionManagerClient = transactionManagerClient;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            FinalizedTransaction = await _transactionManagerClient.FinalizeTransaction(_headerService.StoreNumber, _headerService.RegisterNumber);

            // TODO: If F5 is pressed and there is no active transaction to finalize, what do we want to do?
            if (FinalizedTransaction != null)
                return Page();
            else
                return RedirectToPage("Sale");
        }
    }
}