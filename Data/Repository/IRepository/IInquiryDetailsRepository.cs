using Models;

namespace DataAccess.Repository.IRepository
{
    public interface IInquiryDetailsRepository : IRepository<InquiryDetails>
    {
        void Update(InquiryDetails inquiryDetails);
    }
}
