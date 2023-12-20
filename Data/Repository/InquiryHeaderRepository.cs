using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        public InquiryHeaderRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(InquiryHeader inquiryHeader) => dbSet.Update(inquiryHeader);
    }
}
