using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Webshop.Mvc.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddRespositories(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            services.AddScoped<IInquiryDetailsRepository, InquiryDetailsRepository>();
            services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();
        }

        public static void AddFacebookAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = configuration["Facebook:AppId"];
                options.AppSecret = configuration["Facebook:AppSecret"];
            });
        }
    }
}
