using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PointOfSale.ViewComponents
{
    public class PreviousPageViewComponent : ViewComponent
    {
        public PreviousPageViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync(string previousPageUrl)
        {
            return Task.FromResult<IViewComponentResult>(View("PreviousPage", previousPageUrl));
        }
    }
}
