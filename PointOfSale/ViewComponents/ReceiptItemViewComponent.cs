using Microsoft.AspNetCore.Mvc;
using PointOfSale.Models;
using System.Threading.Tasks;

namespace PointOfSale.Pages.Components
{
    public class ReceiptItemViewComponent : ViewComponent
    {
        //public Task<IViewComponentResult> InvokeAsync(ReceiptItem receiptItem, bool isSelected = false)
        public Task<IViewComponentResult> InvokeAsync(ReceiptItem receiptItem)
        {
            return Task.FromResult<IViewComponentResult>(View("ReceiptItem", receiptItem));
        }
    }
}
