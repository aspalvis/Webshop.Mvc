﻿using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utility;

namespace Webshop.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var vm = new HomeVM()
            {
                Products = _productRepository.GetAll(includeProperties: $"{nameof(ApplicationType)},{nameof(Category)}", isTracking: false),
                Categories = _categoryRepository.GetAll(isTracking: false)
            };
            return View(vm);
        }
        public IActionResult Details(int id)
        {

            List<ShoppingCart> list = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();

            var vm = new DetailsVM()
            {

                Product = _productRepository.FirstOrDefault(x => x.Id == id, includeProperties: $"{nameof(Category)},{nameof(ApplicationType)}", isTracking: false),

                ExistsInCart = list.Any(x => x.ProductId == id)
            };

            return View(vm);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(Product product)
        {
            List<ShoppingCart> cartItems = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();

            if (!cartItems.Any(item => item.ProductId == product.Id))
            {
                ShoppingCart newCartItem = new ShoppingCart
                {
                    ProductId = product.Id,
                    SqFt = product.TempSqFt
                };

                cartItems.Add(newCartItem);
            }

            HttpContext.Session.Set(WC.SessionCart, cartItems);

            return RedirectToActionSuccess(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> items = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) ?? new List<ShoppingCart>();
            HttpContext.Session.Set(WC.SessionCart, items.Where(x => x.ProductId != id).ToList());
            return RedirectToActionSuccess(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
