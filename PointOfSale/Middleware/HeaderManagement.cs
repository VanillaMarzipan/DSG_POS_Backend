using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PointOfSale.Services;
using System;
using System.Threading.Tasks;

namespace PointOfSale.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HeaderManagement
    {
        private static readonly string _validationCookieKey = "validation";
        private static readonly string _validationCookieValue = "true";
        private readonly RequestDelegate _next;

        public HeaderManagement(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IHeaderService headerService, ICookieManagement cookieManagement)
        {
            if (!cookieManagement.IsCookieDataValid(httpContext.Request.Cookies))
            {
                cookieManagement.ClearAllCookies(httpContext.Request.Cookies, httpContext.Response.Cookies);
            }

            string validationString = cookieManagement.GetRequestCookieValue(httpContext.Request.Cookies, _validationCookieKey);

            if (validationString != _validationCookieValue)
            {
                await UpdateHeaderAndCookieValues(httpContext, headerService, cookieManagement);
            }
            else
            {
                ExtractCookieHeaderValues(httpContext, cookieManagement, out int storeNumber, out int registerNumber, out Guid registerId);

                if (storeNumber == 0 || registerNumber == 0 || registerId == Guid.Empty)
                {
                    await UpdateHeaderAndCookieValues(httpContext, headerService, cookieManagement);
                }
                else
                {
                    headerService.StoreNumber = storeNumber;
                    headerService.RegisterNumber = registerNumber;
                    headerService.RegisterId = registerId;
                }
            }

            await _next(httpContext);
        }

        private static void ExtractCookieHeaderValues(HttpContext httpContext, ICookieManagement cookieManagement, out int storeNumber, out int registerNumber, out Guid registerId)
        {
            int.TryParse(cookieManagement.GetRequestCookieValue(httpContext.Request.Cookies, "storenumber"), out storeNumber);
            int.TryParse(cookieManagement.GetRequestCookieValue(httpContext.Request.Cookies, "registernumber"), out registerNumber);
            Guid.TryParse(cookieManagement.GetRequestCookieValue(httpContext.Request.Cookies, "registerid"), out registerId);
        }

        private static async Task UpdateHeaderAndCookieValues(HttpContext httpContext, IHeaderService headerService, ICookieManagement cookieManagement)
        {
            cookieManagement.UpdateResponseCookieWithValue(httpContext.Response.Cookies, _validationCookieKey, _validationCookieValue, new TimeSpan(0, 5, 0));

            headerService.SetStoreNumber();
            await headerService.SetRegisterNumberAsync(httpContext);

            cookieManagement.UpdateResponseCookieWithValue(httpContext.Response.Cookies, "storenumber", headerService.StoreNumber);
            cookieManagement.UpdateResponseCookieWithValue(httpContext.Response.Cookies, "registernumber", headerService.RegisterNumber);
            cookieManagement.UpdateResponseCookieWithValue(httpContext.Response.Cookies, "registerid", headerService.RegisterId);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class HeaderManagementExtensions
    {
        public static IApplicationBuilder UseHeaderService(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HeaderManagement>();
        }
    }
}
