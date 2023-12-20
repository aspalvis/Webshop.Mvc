using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Webshop.Mvc.Controllers
{
    public class BaseController : Controller
    {
        public Claim GetClaimByType(string type) => ((ClaimsIdentity)User.Identity).FindFirst(type);

        public string GetControllerName(string fullName) => fullName.Replace("Controller", "");
    }
}
