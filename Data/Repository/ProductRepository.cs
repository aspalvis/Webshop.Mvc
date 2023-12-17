using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(Product product)
        {
            Product productFromDb = base.FirstOrDefault(x => x.Id.Equals(product.Id));

            if (productFromDb != null)
            {
                productFromDb.Name = product.Name;
                productFromDb.Price = product.Price;
                productFromDb.ShortDescription = product.ShortDescription;
                productFromDb.Description = product.Description;
                productFromDb.ApplicationTypeId = product.ApplicationTypeId;
                productFromDb.CategoryId = product.CategoryId;

                if (!string.IsNullOrEmpty(product.Image))
                {
                    productFromDb.Image = product.Image;
                }
            }
        }
    }
}
