using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.ViewComponents
{
    public class MainHeaderViewComponent : ViewComponent
    {
        public MainHeaderViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync(Response.Header header)
        {
            return Task.FromResult<IViewComponentResult>(View("MainHeader", header));
        }
    }
}