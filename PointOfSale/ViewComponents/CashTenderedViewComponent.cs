using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PointOfSale.ViewComponents
{
    public class CashTenderedViewComponent : ViewComponent
    {
        public CashTenderedViewComponent() { }

        public Task<IViewComponentResult> InvokeAsync(decimal cashTendered)
        {
            return Task.FromResult<IViewComponentResult>(View("CashTendered", cashTendered));
        }
    }
}
