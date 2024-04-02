using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PointOfSale.ViewComponents
{
    public class ScannerInputViewComponent : ViewComponent
    {
        public ScannerInputViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync(Models.UpcLookupResult upcLookupResult)
        {
            return Task.FromResult<IViewComponentResult>(View("ScannerInput", upcLookupResult));
        }
    }
}