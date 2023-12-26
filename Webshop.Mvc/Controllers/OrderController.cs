using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using System.Linq;
using Utility;
using Utility.BrainTree;

namespace Webshop.Mvc.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IBrainTreeGate _brainTreeGate;

        [BindProperty]
        public OrderListVM OrderListVM { get; set; }

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(
            IOrderDetailsRepository orderDetailsRepository,
            IOrderHeaderRepository orderHeaderRepository,
            IBrainTreeGate brainTreeGate)
        {
            _orderDetailsRepository = orderDetailsRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _brainTreeGate = brainTreeGate;
        }
        public IActionResult Index(string name = null, string email = null, string phone = null, string status = null)
        {
            OrderListVM = new OrderListVM()
            {
                OrderHeaders = _orderHeaderRepository.GetAll(
                    x => (string.IsNullOrEmpty(name) || x.FullName.ToLower().Contains(name.ToLower()))
                      && (string.IsNullOrEmpty(email) || x.Email.ToLower().Contains(email.ToLower()))
                      && (string.IsNullOrEmpty(phone) || x.PhoneNumber.ToLower().Contains(phone.ToLower()))
                      && (string.IsNullOrEmpty(status) || x.OrderStatus == status),
                    isTracking: false
                ),
                StatusList = WC.listStatus.ToList().Select(status => new SelectListItem
                {
                    Text = status,
                    Value = status
                })
            };
            return View(OrderListVM);
        }

        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == id, isTracking: false),
                OrderDetails = _orderDetailsRepository.GetAll(x => x.OrderHeaderId == id, includeProperties: nameof(Product), isTracking: false),
            };

            return View(OrderVM);
        }
    }
}
