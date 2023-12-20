using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utility;

namespace Webshop.Mvc.Controllers
{
    public class BaseController : Controller
    {
        public Claim RetrieveClaimByType(string type) => ((ClaimsIdentity)User.Identity).FindFirst(type);

        public string ExtractControllerName(string fullName) => fullName.Replace("Controller", "");

        public void SetUISuccessMessage(string msg) => TempData[WC.Success] = msg;

        public void SetUIErrorMessage(string msg) => TempData[WC.Error] = msg;

        public void SetActionSuccessMessage() => SetUISuccessMessage("Action completed successfuly!");

        public void SetActionErrorMessage() => SetUIErrorMessage("Error while executing action!");

        public ViewResult ViewWithError(string viewName = null, object model = null)
        {
            SetActionErrorMessage();
            return View(viewName, model);
        }
        public ViewResult ViewWithSuccess(string viewName = null, object model = null)
        {
            SetActionSuccessMessage();
            return View(viewName, model);
        }
        public RedirectToActionResult RedirectToActionErrorError(string actionName = null, string controllerName = null, object routeValues = null)
        {
            SetActionErrorMessage();
            return RedirectToAction(actionName, controllerName, routeValues);
        }
        public RedirectToActionResult RedirectToActionSuccess(string actionName = null, string controllerName = null, object routeValues = null)
        {
            SetActionSuccessMessage();
            return RedirectToAction(actionName, controllerName, routeValues);
        }
    }
}
