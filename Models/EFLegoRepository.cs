using Microsoft.EntityFrameworkCore;
using SQLitePCL;
namespace LegosWithAurora.Models
{
    public class EFLegoRepository: ILegoRepository
    {
        private MfalabContext _context;
        public EFLegoRepository(MfalabContext temp) {
            _context = temp;
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

        public Product RemoveProduct(int id) => _context.Products
            .Single(x => x.ProductId == id);

        public void RemoveProduct(Product p)
        {
            _context.Products.Remove(p);
            _context.SaveChanges();
        }
        public Product EditProduct(int id) => _context.Products
            .Single(x => x.ProductId == id);

        public void EditProduct(Product p)
        {
            _context.Products.Update(p);
            _context.SaveChanges();
        }
        //public AspNetUser UpdateUser(int id) =>
        //    _context.AspNetUsers
        //    .Single(x => x.CustomerId == id);

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
    }
}
