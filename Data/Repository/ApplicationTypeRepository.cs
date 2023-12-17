using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        public ApplicationTypeRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(ApplicationType applicationType)
        {
            ApplicationType objFromDb = FirstOrDefault(x => x.Id.Equals(applicationType.Id));

            if (objFromDb != null)
            {
                objFromDb.Name = applicationType.Name;
            }
        }
    }
}
