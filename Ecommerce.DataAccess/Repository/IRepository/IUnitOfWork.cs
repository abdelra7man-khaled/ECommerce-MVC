using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<Brand> Brands { get; }
        Task<int> SaveChangesAsync();
    }
}
