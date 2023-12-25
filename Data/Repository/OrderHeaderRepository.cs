using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(OrderHeader orderHeader) => dbSet.Update(orderHeader);
    }
}
