using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        public OrderDetailsRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(OrderDetails orderDetails) => dbSet.Update(orderDetails);
    }
}
