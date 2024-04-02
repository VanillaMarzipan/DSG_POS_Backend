using DSG.POS.Common.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSale.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Request = DSG.POS.PosTransactionManager.Models.Request;
using Response = DSG.POS.PosTransactionManager.Models.Response;


namespace PointOfSale.Pages
{
    public class TenderCashModel : PageModel
    {
        private readonly IHeaderService _headerService;
        private readonly ITransactionManagerClient _transactionManagerClient;

        public Response.Transaction Transaction { get; private set; }

        public TenderCashModel(IHeaderService headerService, ITransactionManagerClient transactionManagerClient)
        {
            _headerService = headerService;
            _transactionManagerClient = transactionManagerClient;
        }

        public async Task OnGetAsync()
        {
            Transaction = await _transactionManagerClient.ActiveTransaction(_headerService.StoreNumber, _headerService.RegisterNumber);
        }

        public async Task<IActionResult> OnPostTenderWithCashAsync(decimal cashTenderAmount)
        {
            Request.Tender tender = new Request.Tender()
            {
                TenderType = TenderType.Cash,
                Amount = cashTenderAmount
            };

            Transaction = await _transactionManagerClient.NewTender(_headerService.StoreNumber, _headerService.RegisterNumber, tender);

            return RedirectToPage("TenderComplete");

        }
    }
}