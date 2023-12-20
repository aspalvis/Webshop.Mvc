using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Webshop.Mvc.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CartController : BaseController
    {

        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailsRepository _inquiryDetailsRepository;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IProductRepository productRepository, IApplicationUserRepository applicationUserRepository, IInquiryHeaderRepository inquiryHeaderRepository, IInquiryDetailsRepository inquiryDetailsRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _productRepository = productRepository;
            _applicationUserRepository = applicationUserRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailsRepository = inquiryDetailsRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<ShoppingCart> items = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();

            var prodInCart = items.Select(x => x.ProductId).ToList();

            var prodList = _productRepository.GetAll(x => prodInCart.Contains(x.Id), isTracking: false);

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = User.FindFirstValue(ClaimTypes.Name);

            IEnumerable<ShoppingCart> items = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();

            var prodInCart = items.Select(x => x.ProductId).ToList();

            IList<Product> prodList = _productRepository.GetAll(x => prodInCart.Contains(x.Id), isTracking: false);

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _applicationUserRepository.FirstOrDefault(x => x.Id.Equals(claim.Value), isTracking: false),
                ProductList = prodList,
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Summary))]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            var pathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
            string html = "";

            using (StreamReader sr = System.IO.File.OpenText(pathToTemplate))
            {
                html = sr.ReadToEnd();
            }

            var sb = new StringBuilder();

            foreach (var item in productUserVM.ProductList)
            {
                sb.Append($" - Name: {item.Name} <span style='font-size:14px;'> (ID: {item.Id})</span><br />");
            }

            string messageBody = string.Format(html
                , productUserVM.ApplicationUser.FullName
                , productUserVM.ApplicationUser.Email
                , productUserVM.ApplicationUser.PhoneNumber
                , sb.ToString()
                );

            await _emailSender.SendEmailAsync(WC.EmailAdmin, "New Inquiry", messageBody);

            var inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = GetClaimByType(ClaimTypes.NameIdentifier).Value,
                FullName = productUserVM.ApplicationUser.FullName,
                Email = productUserVM.ApplicationUser.Email,
                PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.Now
            };

            _inquiryHeaderRepository.Add(inquiryHeader);
            _inquiryHeaderRepository.Save();

            foreach (var item in productUserVM.ProductList)
            {
                InquiryDetails inquiryDetails = new()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = item.Id
                };
                _inquiryDetailsRepository.Add(inquiryDetails);
            }

            _inquiryDetailsRepository.Save();

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();

            return View();
        }


        public IActionResult Remove(int id)
        {
            List<ShoppingCart> items = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();
            HttpContext.Session.Set(WC.SessionCart, items.Where(x => x.ProductId != id).ToList());
            return RedirectToAction(nameof(Index));
        }
    }
}
