using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging; // Ensure you have this using directive for ILogger
using WebApplication1.Models;
using LegosWithAurora.Models;
<<<<<<< Updated upstream
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
=======
using System.Formats.Tar;
>>>>>>> Stashed changes

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
        [Authorize(Roles = "Member")]
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
        [Authorize(Roles = "Member")]
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

        [Authorize(Roles = "Member")]
        public IActionResult Checkout() { return View(); }
<<<<<<< Updated upstream
        public IActionResult AdminAddUser() { return View(); }
=======
        public IActionResult AddProduct() { return View("AddProductForm"); }
        public IActionResult AddUser() { return View("AddUserForm"); }
>>>>>>> Stashed changes

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderCancelled()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderApproved()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderReview()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminAllProducts()
        {
            var products = _repo.Products;

            products.ToList();

            return View(products);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminAllUsers()
        {
            var users = _repo.Customers;
            users.ToList();
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminProductDelete(int id)
        {
            Product delete = _repo.RemoveProduct(id);
            
            return View(delete);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminProductDelete(Product id)
        {
            _repo.RemoveProduct(id);

            return RedirectToAction("AdminAllProducts");
        }
<<<<<<< Updated upstream
<<<<<<< Updated upstream

=======
        
>>>>>>> Stashed changes
        [HttpGet]
        public IActionResult AdminAddProduct()
        {
            return View();
=======
        public IActionResult AdminUserEdit(int id)
        {
            AspNetUser update = _repo.UpdateUser(id);

            return View("AddUserForm", update);
>>>>>>> Stashed changes
        }

        [HttpPost]
        public IActionResult AdminAddProduct(Product p)
        {
            _repo.AddProduct(p);

            return RedirectToAction("AdminAllProducts");
        }
        
        public IActionResult AdminEditProduct(int id)
        {
            Product editProduct = _repo.EditProduct(id);

            return View("AdminAddProduct", editProduct);
        }

        [HttpPost]
        public IActionResult AdminEditProduct(Product p)
        {
            _repo.EditProduct(p);

            return RedirectToAction("AdminAllProducts");
        }
    }
}
