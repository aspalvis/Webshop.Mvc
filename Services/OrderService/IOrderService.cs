using Models;
using Models.ViewModels;
using System.Threading.Tasks;

namespace Services.OrderService
{
    public interface IOrderService
    {
        OrderListVM Search(string name = null, string email = null, string phone = null, string status = null);
        OrderVM Details(int id);
        void StartProcessing(int id);
        void ShippOrder(int id);
        Task CancelOrder(int id);
        OrderHeader UpdateAndReturn(OrderHeader orderHeader);
    }
}
