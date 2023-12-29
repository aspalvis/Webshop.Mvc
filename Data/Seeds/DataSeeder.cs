using Bogus;
using DataAccess.Data;
using Models;
using System;
using System.Threading.Tasks;

namespace DataAccess.Seeds
{
    public static class DataSeeder
    {
        private const int _productCount = 24;
        private const int _categoryCount = 8;
        private const int _applicationTypesCount = 4;

        public static async Task SeedData(ApplicationDbContext _db)
        {
            await SeedApplicationTypes(_db);
            await SeedCategories(_db);
            await SeedProducts(_db);
        }

        private static async Task SeedProducts(ApplicationDbContext _db)
        {
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.ShortDescription, f => f.Commerce.ProductAdjective())
                .RuleFor(p => p.Price, f => Math.Round(f.Random.Double(1, 10000), 2))
                .RuleFor(p => p.CategoryId, (f, p) => f.Random.Int(1, _categoryCount))
                .RuleFor(p => p.ApplicationTypeId, (f, p) => f.Random.Int(1, _applicationTypesCount));

            var products = productFaker.Generate(_productCount);

            await _db.Product.AddRangeAsync(products);
            await _db.SaveChangesAsync();
        }
        private static async Task SeedApplicationTypes(ApplicationDbContext _db)
        {
            var applicationTypeFaker = new Faker<ApplicationType>()
                .RuleFor(a => a.Name, f => f.Commerce.Department());

            var applicationTypes = applicationTypeFaker.Generate(_applicationTypesCount); // Generate 3 application types

            await _db.ApplicationType.AddRangeAsync(applicationTypes);
            await _db.SaveChangesAsync();
        }
        private static async Task SeedCategories(ApplicationDbContext _db)
        {
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Name, f => f.Commerce.ProductAdjective())
                .RuleFor(c => c.DisplayOrder, f => f.Random.Int(1, _categoryCount));

            var categories = categoryFaker.Generate(_categoryCount);

            await _db.Category.AddRangeAsync(categories);
            await _db.SaveChangesAsync();
        }
    }
}
