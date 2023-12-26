using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Utility.BrainTree;

namespace Webshop.Mvc.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IBrainTreeGate _brainTreeGate;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

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
            return View();
        }
    }
}
