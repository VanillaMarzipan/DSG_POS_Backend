using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSale.Services;
using System.Threading.Tasks;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.Pages
{
    public class TenderMenuModel : PageModel
    {
        private readonly IHeaderService _headerService;
        private readonly ITransactionManagerClient _transactionManagerClient;

        public Response.Transaction Transaction { get; private set; }

        public TenderMenuModel(IHeaderService headerService, ITransactionManagerClient transactionManagerClient)
        {
            _headerService = headerService;
            _transactionManagerClient = transactionManagerClient;
        }

        public async Task OnGetAsync()
        {
            // TODO: Need API method or an overload to get transaction when we already have the transaction number
            Transaction = await _transactionManagerClient.ActiveTransaction(_headerService.StoreNumber, _headerService.RegisterNumber);
        }
    }
}