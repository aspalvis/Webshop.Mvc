using Braintree;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utility;
using Utility.BrainTree;

namespace Webshop.Mvc.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusInProcess;
            _orderDetailsRepository.Save();

            return RedirectToActionSuccess(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShippOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderDetailsRepository.Save();

            return RedirectToActionSuccess(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);

            var gateway = _brainTreeGate.GetGateWay();
            Transaction transaction = await gateway.Transaction.FindAsync(orderHeader.TransactionId);

            if (transaction.Status is TransactionStatus.AUTHORIZED or TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                // no refund
                Result<Transaction> resultvoid = await gateway.Transaction.VoidAsync(orderHeader.TransactionId);
            }
            else
            {
                // refund
                Result<Transaction> resultvoid = await gateway.Transaction.RefundAsync(orderHeader.TransactionId);
            }

            orderHeader.OrderStatus = WC.StatusRefunded;
            _orderDetailsRepository.Save();

            return RedirectToActionSuccess(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);

            orderHeader.FullName = OrderVM.OrderHeader.FullName;
            orderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeader.Email = OrderVM.OrderHeader.Email;
            orderHeader.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeader.City = OrderVM.OrderHeader.City;
            orderHeader.State = OrderVM.OrderHeader.State;
            orderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;

            _orderHeaderRepository.Save();

            TempData[WC.Success] = "Order updated successfully!";

            return RedirectToAction(nameof(Details), routeValues: new { id = orderHeader.Id });
        }
    }
}
