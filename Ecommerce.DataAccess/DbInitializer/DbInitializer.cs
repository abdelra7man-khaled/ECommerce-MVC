using Ecommerce.DataAccess.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(AppDbContext context, UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            // Migrations if they are not applied
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            // Create roles if they are not created
            if (!_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Seller")).GetAwaiter().GetResult();

                // If roles are not created, then we will create admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Abdelrahman",
                    LastName = "Khaled",
                    PhoneNumber = "01128490013",
                    Address = "Nasr City, 67 Main St",
                    City = "Cairo",
                    CreatedAt = DateTime.Now
                }, "Admin1234*").GetAwaiter().GetResult();

                ApplicationUser user = _context.ApplicationUsers
                    .FirstOrDefault(u => u.Email == "admin@gmail.com")!;

                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }
            return;
        }
    }
}
