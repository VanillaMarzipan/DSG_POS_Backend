using Microsoft.Extensions.DependencyInjection;
using PointOfSale.Services;

namespace PointOfSale.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddHeaderService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IHeaderService, HeaderService>();
        }

        public static void AddDnsService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDnsService, DnsService>();
        }

        public static void AddCookieManagement(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICookieManagement, CookieManagement>();
        }

        public static IHttpClientBuilder AddProductClient(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddHttpClient<IProductClient, ProductClient>();
        }

        public static IHttpClientBuilder AddTransactionManagerClient(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddHttpClient<ITransactionManagerClient, TransactionManagerClient>();
        }

        public static IHttpClientBuilder AddRegisterManagerClient(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddHttpClient<IRegisterManagerClient, RegisterManagerClient>();
        }
    }
}
