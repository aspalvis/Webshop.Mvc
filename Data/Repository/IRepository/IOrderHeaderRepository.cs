using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
    }
}
