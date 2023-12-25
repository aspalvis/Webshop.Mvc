using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Utility
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
    }
}
