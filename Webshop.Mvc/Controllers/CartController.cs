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
using Utility.BrainTree;

namespace Webshop.Mvc.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {

        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailsRepository _inquiryDetailsRepository;
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IBrainTreeGate _brainTreeGate;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment,
            IEmailSender emailSender, IProductRepository productRepository,
            IApplicationUserRepository applicationUserRepository,
            IInquiryHeaderRepository inquiryHeaderRepository,
            IInquiryDetailsRepository inquiryDetailsRepository,
            IOrderDetailsRepository orderDetailsRepository,
            IOrderHeaderRepository orderHeaderRepository,
            IBrainTreeGate brainTreeGate)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _productRepository = productRepository;
            _applicationUserRepository = applicationUserRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailsRepository = inquiryDetailsRepository;
            _orderDetailsRepository = orderDetailsRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _brainTreeGate = brainTreeGate;
        }

        public IActionResult Index()
        {
            IEnumerable<ShoppingCart> items = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();

            var productIds = items.Select(u => u.ProductId).ToList();

            var prodList = _productRepository.GetAll(x => productIds.Contains(x.Id), isTracking: false);

            foreach (var cart in items)
            {
                var productTemp = prodList.FirstOrDefault(x => x.Id.Equals(cart.ProductId));
                if (productTemp != null)
                {
                    productTemp.TempSqFt = cart.SqFt;
                }
            }

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IList<Product> products)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

            foreach (var product in products)
            {
                shoppingCarts.Add(new ShoppingCart { ProductId = product.Id, SqFt = product.TempSqFt });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser appUser;
            if (User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionInquiryId) != 0)
                {
                    var inquiryHeader = _inquiryHeaderRepository
                        .FirstOrDefault(x => x.Id.Equals(HttpContext.Session.Get<int>(WC.SessionInquiryId)),
                            isTracking: false);

                    appUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber,
                    };
                }
                else
                {
                    appUser = new ApplicationUser();
                }

                var gateway = _brainTreeGate.GetGateWay();
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;
            }
            else
            {
                Claim claimsIdentity = base.RetrieveClaimByType(ClaimTypes.NameIdentifier);
                appUser = _applicationUserRepository.FirstOrDefault(x => x.Id.Equals(claimsIdentity.Value), isTracking: false);
            }

            IEnumerable<ShoppingCart> items = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();

            var prodInCart = items.Select(x => x.ProductId).ToList();

            IList<Product> prodList = _productRepository.GetAll(x => prodInCart.Contains(x.Id), isTracking: false);

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = appUser,
            };

            foreach (var item in items)
            {
                Product productTemp = _productRepository.FirstOrDefault(x => x.Id.Equals(item.ProductId));
                productTemp.TempSqFt = item.SqFt;
                ProductUserVM.ProductList.Add(productTemp);
            }

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Summary))]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            if (User.IsInRole(WC.AdminRole))
            {

                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = RetrieveClaimByType(ClaimTypes.NameIdentifier).Value,
                    FinalOrderTotal = productUserVM.ProductList.Sum(x => x.TempSqFt * x.Price),
                    FullName = productUserVM.ApplicationUser.FullName,
                    Email = productUserVM.ApplicationUser.Email,
                    PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                    City = productUserVM.ApplicationUser.City,
                    State = productUserVM.ApplicationUser.State,
                    StreetAddress = productUserVM.ApplicationUser.StreetAddress,
                    PostalCode = productUserVM.ApplicationUser.PostalCode,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending
                };

                _orderHeaderRepository.Add(orderHeader);
                _orderHeaderRepository.Save();

                foreach (var product in productUserVM.ProductList)
                {
                    OrderDetails orderDetails = new()
                    {
                        OrderHeaderId = orderHeader.Id,
                        ProductId = product.Id,
                        PricePerSqFt = product.Price,
                        Sqft = product.TempSqFt,
                    };
                    _orderDetailsRepository.Add(orderDetails);
                }

                _orderDetailsRepository.Save();

                return RedirectToActionSuccess(nameof(InquiryConfirmation), routeValues: new { id = orderHeader.Id });

            }
            else
            {
                var pathToTemplate = _webHostEnvironment.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "Inquiry.html";

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
                    ApplicationUserId = RetrieveClaimByType(ClaimTypes.NameIdentifier).Value,
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

                return RedirectToActionSuccess(nameof(InquiryConfirmation));
            }
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id.Equals(id), isTracking: false);

            HttpContext.Session.Clear();

            return View(orderHeader);
        }


        public IActionResult Remove(int id)
        {
            List<ShoppingCart> items = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();
            HttpContext.Session.Set(WC.SessionCart, items.Where(x => x.ProductId != id).ToList());
            return RedirectToActionSuccess(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IList<Product> products)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

            foreach (var product in products)
            {
                shoppingCarts.Add(new ShoppingCart { ProductId = product.Id, SqFt = product.TempSqFt });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);

            return RedirectToActionSuccess(nameof(Index));
        }
    }
}
