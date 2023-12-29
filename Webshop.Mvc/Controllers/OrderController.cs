using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Services.OrderService;
using System.Threading.Tasks;
using Utility;

namespace Webshop.Mvc.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrderController : BaseController
    {
        private readonly IOrderService _service;

        [BindProperty]
        public OrderListVM OrderListVM { get; set; }

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IOrderService service)
        {
            _service = service;
        }
        public IActionResult Index(string name = null, string email = null, string phone = null, string status = null)
        {
            OrderListVM = _service.Search(name, email, phone, status);
            return View(OrderListVM);
        }

        public IActionResult Details(int id)
        {
            OrderVM = _service.Details(id);

            return View(OrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            _service.StartProcessing(OrderVM.OrderHeader.Id);

            return RedirectToActionSuccess(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShippOrder()
        {
            _service.ShippOrder(OrderVM.OrderHeader.Id);

            return RedirectToActionSuccess(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            await _service.CancelOrder(OrderVM.OrderHeader.Id);

            return RedirectToActionSuccess(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeader = _service.UpdateAndReturn(OrderVM.OrderHeader);

            TempData[WC.Success] = "Order updated successfully!";

            return RedirectToAction(nameof(Details), routeValues: new { id = orderHeader.Id });
        }
    }
}
