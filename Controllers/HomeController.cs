using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging; // Ensure you have this using directive for ILogger
using WebApplication1.Models;
using LegosWithAurora.Models;
using Microsoft.CodeAnalysis;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ILegoRepository _repo;

        private Cart Cart = new Cart();

        public HomeController(ILegoRepository temp)
        {
            _repo = temp;
        }

        public IActionResult Index()
        {
            var products = _repo.Products;

            products.ToList();

            // Read the user's cookie consent status from the cookies
            var userConsent = Request.Cookies["userConsent"];
            // Pass the consent status to the view via ViewBag
            ViewBag.UserConsent = userConsent;

            return View(products);
        }

        public IActionResult Privacy()
        {
            // You might also want to check and pass the consent status in the Privacy view
            var userConsent = Request.Cookies["userConsent"];
            ViewBag.UserConsent = userConsent;

            return View();
        }
        public IActionResult Products()
        {
            var products = _repo.Products;

            products.ToList();

            return View(products);
        }

        public IActionResult ProductDetails(int id, string returnUrl)
        {
            ViewBag.returnUrl = returnUrl ?? "/";

            var products = _repo.Products
                            .Where(x => x.ProductId == id).Single();

            return View(products);
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, string returnUrl)
        {
            Product? prod = _repo.Products
                            .FirstOrDefault(x => x.ProductId == productId);

            if (prod != null)
            {
                Cart.AddItem(prod, 1);
            }

            return RedirectToAction("CartConfirmation", new { id = productId, returnUrl = returnUrl });
        }
        public IActionResult CartConfirmation(int id, string returnUrl)
        {
            Product? prod = _repo.Products
                            .FirstOrDefault(x => x.ProductId == id);

            ViewBag.returnUrl = returnUrl ?? "/";
            return View(prod);
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Checkout() { return View(); }
        public IActionResult AddProduct() { return View(); }
        public IActionResult AddUser() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AdminOrderCancelled()
        {
            return View();
        }
        public IActionResult AdminOrderApproved()
        {
            return View();
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult AdminOrderReview()
        {
            return View();
        }

        public IActionResult AdminAllProducts()
        {
            var products = _repo.Products;

            products.ToList();

            return View(products);
        }

        public IActionResult AdminAllUsers()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AdminProductDelete(int id)
        {
            Product delete = _repo.Remove(id);
            
            return View(delete);
        }

        [HttpPost]
        public IActionResult AdminProductDelete(Product id)
        {
            _repo.Remove(id);

            return RedirectToAction("AdminAllProducts");
        }
    }
}
