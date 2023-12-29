using Braintree;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utility;
using Utility.BrainTree;

namespace Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IBrainTreeGate _brainTreeGate;
        public OrderService(IOrderDetailsRepository orderDetailsRepository, IOrderHeaderRepository orderHeaderRepository, IBrainTreeGate brainTreeGate)
        {
            _orderDetailsRepository = orderDetailsRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _brainTreeGate = brainTreeGate;
        }
        public async Task CancelOrder(int id)
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == id);

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
        }
        public OrderVM Details(int id)
        {
            return new OrderVM()
            {
                OrderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == id, isTracking: false),
                OrderDetails = _orderDetailsRepository.GetAll(x => x.OrderHeaderId == id, includeProperties: nameof(Product), isTracking: false),
            };
        }
        public OrderListVM Search(string name = null, string email = null, string phone = null, string status = null)
        {
            return new OrderListVM()
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
        }
        public void ShippOrder(int id)
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == id);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderDetailsRepository.Save();
        }
        public void StartProcessing(int id)
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == id);
            orderHeader.OrderStatus = WC.StatusInProcess;
            _orderDetailsRepository.Save();
        }
        public OrderHeader UpdateAndReturn(OrderHeader orderHeader)
        {
            OrderHeader orderHeaderFromDb = _orderHeaderRepository.FirstOrDefault(x => x.Id == orderHeader.Id);

            orderHeaderFromDb.FullName = orderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = orderHeader.PhoneNumber;
            orderHeaderFromDb.Email = orderHeader.Email;
            orderHeaderFromDb.StreetAddress = orderHeader.StreetAddress;
            orderHeaderFromDb.City = orderHeader.City;
            orderHeaderFromDb.State = orderHeader.State;
            orderHeaderFromDb.PostalCode = orderHeader.PostalCode;

            _orderHeaderRepository.Save();

            return orderHeaderFromDb;
        }
    }
}
