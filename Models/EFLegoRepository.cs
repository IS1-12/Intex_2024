using LegosWithAurora.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
namespace LegosWithAurora.Models
{
    public class EFLegoRepository : ILegoRepository
    {
        private MfalabContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public EFLegoRepository(MfalabContext temp, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = temp;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IQueryable<Order> Orders => _context.Orders;
        public IQueryable<Product> Products => _context.Products;
        public IQueryable<LineItem> LineItems => _context.LineItems;
        public IQueryable<Customer> Customers => _context.Customers;
        public IQueryable<AspNetUser> AspNetUsers => _context.AspNetUsers;
        public IQueryable<AspNetRole> AspNetRoles => _context.AspNetRoles;
        public IQueryable<AspNetRoleClaim> AspNetRoleClaims => _context.AspNetRoleClaims;
        public IQueryable<AspNetUserLogin> AspNetUserLogins => _context.AspNetUserLogins;
        public IQueryable<AspNetUserToken> AspNetUserTokens => _context.AspNetUserTokens;
        public IQueryable<Recommendations> recommendations => _context.recommendations;


        public void RemoveProduct(int p)
        {
            var pToDelete = _context.Products
                .Single(x => x.ProductId == p);
            _context.Products.Remove(pToDelete);
            _context.SaveChanges();
        }
        public Product EditProduct(int id) => _context.Products
            .Single(x => x.ProductId == id);

        public void EditProduct(Product p)
        {
            _context.Products.Update(p);
            _context.SaveChanges();
        }
        public AspNetUser UpdateUser(string id) =>
            _context.AspNetUsers
            .Single(x => x.Id == id);

        public void SaveUser(AspNetUser user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public void AddProduct(Product p)
        {
            _context.Add(p);
            _context.SaveChanges();
        }

        public Order RejectOrder(int id) => _context.Orders
            .Single(x => x.TransactionId == id);
        public void RejectOrder(Order o)
        {
            _context.Orders.Remove(o);
            _context.SaveChanges();
        }

        public void CorrectOrder(int id)
        {
            var orderCorrect = _context.Orders
                .FirstOrDefault(x => x.TransactionId == id);

            orderCorrect.Fraud = 0;
            _context.SaveChanges();
        }

        public void AddOrder(Order o)
        { _context.Orders.Add(o); _context.SaveChanges(); }

        public void AddLineItem(LineItem l)
        {
            _context.Add(l);
            _context.SaveChanges();
        }

        public void DelUser(string id)
        {
            var user = _context.AspNetUsers.Single(x => x.Id == id);
            _context.AspNetUsers.Remove(user);
            _context.SaveChanges();
        }
        
        public void EditExistingProduct(Product product)
        {
            _context.Update(product);
            _context.SaveChanges();
        }
    }
}
