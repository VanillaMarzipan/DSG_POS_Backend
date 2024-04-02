using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.ViewComponents
{
    public class TotalDueViewComponent : ViewComponent
    {
        public TotalDueViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync(decimal totalDue)
        {
            return Task.FromResult<IViewComponentResult>(View("TotalDue", totalDue));
        }
    }
}
