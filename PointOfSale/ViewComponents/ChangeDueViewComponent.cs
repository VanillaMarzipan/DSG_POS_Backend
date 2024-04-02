using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace PointOfSale.ViewComponents
{
    public class ChangeDueViewComponent : ViewComponent
    {
        public ChangeDueViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync(decimal changeDueAmount)
        {
            return Task.FromResult<IViewComponentResult>(View("ChangeDue", changeDueAmount));
        }
    }
}
