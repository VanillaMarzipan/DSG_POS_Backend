using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PointOfSale.Services
{
    public interface IDnsService
    {
        Task<string> GetHostnameFromIp(IPAddress address);
    }

    public class DnsService : IDnsService
    {
        private readonly ILogger<DnsService> _logger;

        public DnsService(ILogger<DnsService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetHostnameFromIp(IPAddress address)
        {
            string returnValue = String.Empty;

            try
            {
                returnValue = (await Dns.GetHostEntryAsync(address)).HostName.ToString();
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Hostname resolution of IP address [{address.ToString()}] failed.  Exception message - {e.ToString()}");
                return address.ToString();
            }

            return returnValue;
        }
    }
}
