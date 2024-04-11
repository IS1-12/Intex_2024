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

        public void AddOrder(Order o)
        {
            _context.Add(o);
            _context.SaveChanges();
        }

        public void AddLineItem(LineItem l)
        {
            _context.Add(l);
            _context.SaveChanges();
        }
    }
}
