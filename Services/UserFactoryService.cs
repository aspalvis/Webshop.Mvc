using Microsoft.AspNetCore.Identity;
using Models;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace Services
{
    public class UserFactoryService : IUserFactoryService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserFactoryService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> CreateAdminAsync(ApplicationUser user, string password)
        {
            return await CreateAsync(user, password, WC.AdminRole);
        }


        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password, string role = WC.CustomerRole)
        {
            var isFirst = !_userManager.Users.Any();

            var result = await _userManager.CreateAsync(user, password);

            await AddToRoleAsync(user, isFirst ? WC.AdminRole : role);

            return result;
        }
    }
}
