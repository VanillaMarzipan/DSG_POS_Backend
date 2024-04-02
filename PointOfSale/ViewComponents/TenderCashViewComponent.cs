using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PointOfSale.ViewComponents
{
    public class TenderCashViewComponent : ViewComponent
    {
        public TenderCashViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View("TenderCash"));
        }
    }
}
