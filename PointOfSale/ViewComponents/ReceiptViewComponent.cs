using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Threading.Tasks;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.ViewComponents
{
    public class ReceiptViewComponent : ViewComponent
    {
        public ReceiptViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync(Response.Transaction transaction, bool showCompleteButton, bool showReceiptPrinted)
        {
            // TODO: Would like to default showCompleteButton & showReceiptPrinted to false, that way you only turn it on when you need it
            // However, it looks like it isnt ready yet: https://github.com/aspnet/Razor/issues/1266

            return Task.FromResult<IViewComponentResult>(View("Receipt", new ReceiptViewModel(transaction, showCompleteButton, showReceiptPrinted)));
        }
    }
}
