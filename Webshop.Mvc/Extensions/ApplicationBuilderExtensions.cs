using DataAccess.Data;
using DataAccess.Seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Utility;

namespace Webshop.Mvc.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> RecreateDatabase(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await context.Database.EnsureDeletedAsync(); // Drop the database
                await context.Database.EnsureCreatedAsync(); // Recreate the database
            }
            return app;
        }
        public static async Task<IApplicationBuilder> ApplySeeds(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await roleManager.CreateAsync(new IdentityRole(WC.AdminRole));
                await roleManager.CreateAsync(new IdentityRole(WC.CustomerRole));

                await DataSeeder.SeedData(context);
            }
            return app;
        }
    }
}
