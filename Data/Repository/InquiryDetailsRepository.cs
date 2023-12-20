using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class InquiryDetailsRepository : Repository<InquiryDetails>, IInquiryDetailsRepository
    {
        public InquiryDetailsRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(InquiryDetails inquiryDetails) => dbSet.Update(inquiryDetails);
    }
}
