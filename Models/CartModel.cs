namespace LegosWithAurora.Models
{
    public class CartModel
    {
        private ILegoRepository _repo;
        public CartModel(ILegoRepository temp, Cart cartService)
        {
            _repo = temp;
            Cart = cartService;
        }

        public Cart Cart { get; set; }
    }
}
