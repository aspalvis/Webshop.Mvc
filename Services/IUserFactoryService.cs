using Microsoft.AspNetCore.Identity;
using Models;
using System.Threading.Tasks;

namespace Services
{

    public interface IUserFactoryService
    {
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password, string role);
        Task<IdentityResult> CreateAdminAsync(ApplicationUser user, string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
    }
}
