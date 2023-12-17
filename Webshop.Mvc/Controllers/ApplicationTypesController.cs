using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using Utility;

namespace Webshop.Mvc.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypesController : Controller
    {
        private readonly IApplicationTypeRepository _applicationTypeRepository;

        public ApplicationTypesController(IApplicationTypeRepository applicationTypeRepository)
        {
            _applicationTypeRepository = applicationTypeRepository;
        }

        // GET: ApplicationTypes
        public IActionResult Index()
        {
            return View(_applicationTypeRepository.GetAll(isTracking: false));
        }

        // GET: ApplicationTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            _applicationTypeRepository.Add(applicationType);
            _applicationTypeRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: ApplicationTypes/Edit
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Index));
            }

            var applcationType = _applicationTypeRepository.Find(id.Value);

            if (applcationType == null)
            {
                return NotFound();
            }

            return View(applcationType);
        }

        // POST: ApplicationTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType applicationType)
        {
            _applicationTypeRepository.Update(applicationType);
            _applicationTypeRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: ApplicationTypes/Delete
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Index));
            }

            var applcationType = _applicationTypeRepository.Find(id.Value);

            if (applcationType == null)
            {
                return NotFound();
            }

            return View(applcationType);
        }

        // POST: ApplicationTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Index));
            }

            var applcationType = _applicationTypeRepository.Find(id.Value);

            if (applcationType == null)
            {
                return NotFound();
            }

            _applicationTypeRepository.Delete(applcationType);
            _applicationTypeRepository.Save();

            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationTypeExists(int id)
        {
            return _applicationTypeRepository.GetAll(isTracking: false).Any(e => e.Id == id);
        }
    }
}
