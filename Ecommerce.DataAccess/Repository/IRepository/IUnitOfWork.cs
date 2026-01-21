using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<Brand> Brands { get; }
        IRepository<ApplicationUser> ApplicationUsers { get; }
        Task<int> SaveChangesAsync();
    }
}
