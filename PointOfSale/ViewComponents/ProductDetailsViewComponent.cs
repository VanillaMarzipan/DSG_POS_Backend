using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PointOfSale.ViewComponents
{
    public class ProductDetailsViewComponent : ViewComponent
    {
        public ProductDetailsViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View("ProductDetails"));
        }
    }
}
