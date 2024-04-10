using Microsoft.EntityFrameworkCore;
using SQLitePCL;
namespace LegosWithAurora.Models
{
    public class EFLegoRepository: ILegoRepository
    {
        private IntexContext _context;
        public EFLegoRepository(IntexContext temp) {
            _context = temp;
        }
        public IQueryable<Order> Orders => _context.Orders;
        public IQueryable<Product> Products => _context.Products;
        public IQueryable<LineItem> LineItems => _context.LineItems;
        public IQueryable<Customer> Customers => _context.Customers;

        public Product RemoveProduct(int id) => _context.Products
            .Single(x => x.ProductId == id);

        public void RemoveProdudct(Product p)
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
    }
}
