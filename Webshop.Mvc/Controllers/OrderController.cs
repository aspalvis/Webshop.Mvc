using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public OrderController(
            IOrderDetailsRepository orderDetailsRepository,
            IOrderHeaderRepository orderHeaderRepository,
            IBrainTreeGate brainTreeGate)
        {
            _orderDetailsRepository = orderDetailsRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _brainTreeGate = brainTreeGate;
        }
        public IActionResult Index()
        {
            OrderListVM = new OrderListVM()
            {
                OrderHeaders = _orderHeaderRepository.GetAll(isTracking: false),
                StatusList = WC.listStatus.ToList().Select(status => new SelectListItem
                {
                    Text = status,
                    Value = status
                })
            };
            return View();
        }
    }
}
