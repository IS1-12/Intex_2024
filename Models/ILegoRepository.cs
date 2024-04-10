namespace LegosWithAurora.Models
{
    public interface ILegoRepository
    {
        IQueryable<Order> Orders { get; }
        IQueryable<Product> Products { get; }
        IQueryable<LineItem> LineItems { get; }
        IQueryable<Customer> Customers { get; }
        public void AddProduct(Product p);
        public Product RemoveProduct(int id);
        public void RemoveProduct(Product p);
        public Product EditProduct(int id);
        public void EditProduct(Product p);
        public AspNetUser UpdateUser(int id);
    }
}
