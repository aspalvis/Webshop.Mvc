using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Webshop.Mvc.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : BaseController
    {

        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailsRepository _inquiryDetailsRepository;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepository, IInquiryDetailsRepository inquiryDetailsRepository)
        {
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailsRepository = inquiryDetailsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeaderRepository.FirstOrDefault(x => x.Id == id, isTracking: false),
                InquiryDetails = _inquiryDetailsRepository.GetAll(x => x.InquiryHeaderId.Equals(id), includeProperties: nameof(Product), isTracking: false)
            };
            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            InquiryVM.InquiryDetails = _inquiryDetailsRepository.GetAll(x => x.InquiryHeaderId.Equals(InquiryVM.InquiryHeader.Id));

            IEnumerable<ShoppingCart> shoppingCartList = InquiryVM.InquiryDetails.Select(x => new ShoppingCart() { ProductId = x.ProductId });

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);

            return RedirectToActionSuccess(nameof(Index), ExtractControllerName(nameof(CartController)));
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryVM.InquiryDetails = _inquiryDetailsRepository.GetAll(x => x.InquiryHeaderId.Equals(InquiryVM.InquiryHeader.Id));
            InquiryVM.InquiryHeader = _inquiryHeaderRepository.FirstOrDefault(x => x.Id.Equals(InquiryVM.InquiryHeader.Id));

            _inquiryHeaderRepository.Delete(InquiryVM.InquiryHeader);
            _inquiryDetailsRepository.DeleteRange(InquiryVM.InquiryDetails);

            _inquiryHeaderRepository.Save();

            return RedirectToActionSuccess(nameof(Index));
        }

        #region API CALLS  

        [HttpGet]
        public IActionResult InquiryList()
        {
            return Json(new { data = _inquiryHeaderRepository.GetAll(isTracking: false) });
        }

        #endregion

    }
}
