using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Webshop.Mvc.Areas.Identity.IdentityHostingStartup))]
namespace Webshop.Mvc.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}