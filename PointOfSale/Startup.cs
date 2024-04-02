using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pivotal.Discovery.Client;
using PointOfSale.Extensions;
using PointOfSale.Middleware;
using PointOfSale.Options;
using Steeltoe.Common.Http.Discovery;

namespace PointOfSale
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_hostingEnvironment.IsProduction())
            {
                services.AddTransient<DiscoveryHttpMessageHandler>();
                services.AddDiscoveryClient(_configuration);

                services.AddProductClient().AddServiceDiscovery();
                services.AddTransactionManagerClient().AddServiceDiscovery();
                services.AddRegisterManagerClient().AddServiceDiscovery();
            }
            else
            {
                services.AddProductClient();
                services.AddTransactionManagerClient();
                services.AddRegisterManagerClient();
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOptions();
            services.Configure<TransactionManagerOptions>(_configuration.GetSection("TransactionManagerOptions"));
            services.Configure<RegisterManagerOptions>(_configuration.GetSection("RegisterManagerOptions"));
            services.Configure<ProductServiceOptions>(_configuration.GetSection("ProductServiceOptions"));

            services.AddHttpContextAccessor();

            services.AddCookieManagement();
            services.AddDnsService();
            services.AddHeaderService();
            services.AddApplicationInsightsTelemetry(_configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseHeaderService();
            app.UseRequestLogging();
            app.UseMvc();

            if (_hostingEnvironment.IsProduction())
                app.UseDiscoveryClient();
        }
    }
}
