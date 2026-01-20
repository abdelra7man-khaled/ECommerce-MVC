using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IRepository<Product> Products { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Brand> Brands { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Products = new Repository<Product>(_context);
            Categories = new Repository<Category>(_context);
            Brands = new Repository<Brand>(_context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
