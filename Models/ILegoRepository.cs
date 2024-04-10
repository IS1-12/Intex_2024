namespace LegosWithAurora.Models
{
    public interface ILegoRepository
    {
        IQueryable<Order> Orders { get; }
        IQueryable<Product> Products { get; }
        IQueryable<LineItem> LineItems { get; }
        IQueryable<Customer> Customers { get; }

        public Product Remove(int id);
        public void Remove(Product p);
    }
}
