using LegosWithAurora.Infrastructure;
using LegosWithAurora.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Linq;

namespace LegosWithAurora.Pages
{
    public class CartModel : PageModel
    {
        private ILegoRepository repository;

        public CartModel(ILegoRepository repo, Cart cartService)
        {
            repository = repo;
            Cart = cartService;
        }

        public Cart? Cart { get; set; }
        public string ReturnUrl { get; set; } = "/";

        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
            Cart = HttpContext.Session.GetJson<Cart>("cart")
                ?? new Cart();
        }

        public IActionResult OnPostRemove(long ProductId, string returnUrl)
        {
            var test2 = Cart;
            var test1 = Cart.Lines;
            Cart.RemoveLine(Cart.Lines.First(x => x.product.ProductId == ProductId).product);

            return RedirectToPage(new { returnUrl = returnUrl });
        }
    }
}
