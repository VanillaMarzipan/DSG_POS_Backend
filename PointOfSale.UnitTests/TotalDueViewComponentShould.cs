using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using PointOfSale.Pages.Components;
using PointOfSale.ViewComponents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class TotalDueViewComponentShould
    {
        [Fact]
        public async Task ReturnTotalDueViewWithGivenAmount()
        {
            decimal expectedResult = 11.99M;

            TotalDueViewComponent receiptItemViewComponent = new TotalDueViewComponent();
            IViewComponentResult result = await receiptItemViewComponent.InvokeAsync(expectedResult);

            Assert.IsType<ViewViewComponentResult>(result);

            ViewViewComponentResult componentResult = ((ViewViewComponentResult)result);
            Assert.IsType<decimal>(componentResult.ViewData.Model);

            decimal actualResult = (decimal)componentResult.ViewData.Model;
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
