using Microsoft.CodeAnalysis;

namespace LegosWithAurora.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        public virtual void AddItem(Product p, int quantity)
        {
            CartLine? line = Lines
                .Where(x => x.product.ProductId == p.ProductId)
                .FirstOrDefault();

            // has this item already been added to our cart?
            if (line == null)
            {
                Lines.Add(new CartLine()
                {
                    product = p,
                    Quantity = quantity,
                    Price = p.Price
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public virtual void RemoveLine(Product proj) => Lines.RemoveAll(x => x.product.ProductId == proj.ProductId);

        public virtual void Clear() => Lines.Clear();
        public int CalculateTotal() => (int)Lines.Sum(x => x.Price * x.Quantity);
        public class CartLine
        {
            public int CartLineId { get; set; }
            public Product product { get; set; }
            public int Quantity { get; set; }
            public int? Price { get; set; }
        }
    }
}
