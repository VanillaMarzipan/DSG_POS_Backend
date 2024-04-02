using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PointOfSale.ViewComponents
{
    public class TenderMenuViewComponent : ViewComponent
    {
        public TenderMenuViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View("TenderMenu"));
        }
    }
}
