using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;

namespace Webshop.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        private readonly ICategoryRepository _categoryRepository;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IApplicationTypeRepository _applicationTypeRepository;

        public ProductController(IWebHostEnvironment webHostEnvironment, IProductRepository productRepository, ICategoryRepository categoryRepository, IApplicationTypeRepository applicationTypeRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _applicationTypeRepository = applicationTypeRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _productRepository.GetAll(includeProperties: $"{nameof(Category)},{nameof(ApplicationType)}", isTracking: false);

            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _categoryRepository.GetAll(isTracking: false).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }),
                ApplicationTypeSelectList = _applicationTypeRepository.GetAll(isTracking: false).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }),
            };

            if (id.HasValue)
            {
                productVM.Product = _productRepository.FirstOrDefault(x => x.Id.Equals(id.Value), isTracking: false);

                return productVM.Product == null ? NotFound() : View(productVM);
            }
            else
            {
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (productVM.Product.Id == 0)
                {
                    string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;

                    string fileName = Guid.NewGuid().ToString();

                    string extension = Path.GetExtension(files[0].FileName);

                    using var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create);

                    files[0].CopyTo(fileStream);

                    productVM.Product.Image = fileName + extension;

                    _productRepository.Add(productVM.Product);
                }
                else
                {
                    Product productFromDb = _productRepository.FirstOrDefault(x => x.Id.Equals(productVM.Product.Id), isTracking: false);

                    if (files.Any())
                    {
                        string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        string oldFile = Path.Combine(upload, productFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create);

                        files[0].CopyTo(fileStream);

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = productFromDb.Image;
                    }

                    _productRepository.Update(productVM.Product);
                }

                _productRepository.Save();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategorySelectList = _categoryRepository.GetAll(isTracking: false).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
                productVM.ApplicationTypeSelectList = _applicationTypeRepository.GetAll(isTracking: false).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
                return View(productVM);
            }
        }

        public IActionResult Delete(int id)
        {
            var product = _productRepository.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;

            var photo = Path.Combine(upload, product.Image);

            if (System.IO.File.Exists(photo))
            {
                System.IO.File.Delete(photo);
            }

            _productRepository.Delete(product);
            _productRepository.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
