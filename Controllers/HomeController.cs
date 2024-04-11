using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging; // Ensure you have this using directive for ILogger
using WebApplication1.Models;
using LegosWithAurora.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using System.Formats.Tar;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.AspNetCore.Http;
using LegosWithAurora.Infrastructure;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ILegoRepository _repo;

        private Cart Cart = new Cart();
        private readonly InferenceSession _session;

        public HomeController(ILegoRepository temp)
        {
            _repo = temp;

            try
            {
                _session = new InferenceSession("./decision_tree_model.onnx");
                //_logger.LogInformation("ONNX model loaded successfully.");
                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error loading the ONNX model: {ex.Message}");
                Console.WriteLine("Aaaaahhhhhh noooooooo" + ex.Message);
            }
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
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                Cart.AddItem(prod, 1);
                HttpContext.Session.SetJson("cart", Cart);
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

        public IActionResult ViewCart()
        {
            ViewBag.Cart = Cart;
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        [Authorize(Roles = "Member")]
        public IActionResult Checkout()
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart")
                ?? new Cart();
            return View(Cart);
        }
        [Authorize(Roles = "Member")]
        [HttpPost]
        public IActionResult Checkout(Cart c)
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();

            try
            {
                var input = new List<float> { DateTime.Now.Hour, Cart.CalculateTotal() };
                var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
                };

                using (var results = _session.Run(inputs)) // makes the prediction with the inputs from the form (i.e. class_type 1-7)
                {
                    var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                    if (prediction != null && prediction.Length > 0)
                    {
                        // Use the prediction to get the animal type from the dictionary
                        var isFraud = (int)prediction[0];
                        ViewBag.Prediction = isFraud;
                    }
                    else
                    {
                        ViewBag.Prediction = "Error: Unable to make a prediction.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Prediction = "Error during prediction.";
            }
            var submit = new Order
            {
                CustomerId = 29135,
                Amount = Cart.CalculateTotal(),
                CountryOfTransaction = "USA",
                Fraud = ViewBag.Prediction
            };
            _repo.AddOrder(submit);

            foreach (Cart.CartLine cl in Cart.Lines)
            {
                var lineItem = new LineItem
                {
                    ProductId = cl.product.ProductId,
                    TransactionId = submit.TransactionId,
                    Qty = cl.Quantity,
                    Rating = 5
                };

                _repo.AddLineItem(lineItem);
            }

            Cart.Clear();
            HttpContext.Session.SetJson("cart", Cart);

            return View("OrderConfirmation");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminAddUser() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderCancelled(int id)
        {
            //Order delete = _repo.RejectOrder(id);
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AdminOrderCancelled(Order o)
        {
            //_repo.RejectOrder(o);
            return RedirectToAction("");
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderReview()
        {
            var orders = _repo.Orders
                .Where(x => x.Fraud == 1);

            orders.ToList();
            
            return View(orders);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderReview(int id)
        {
            var acceptedID = id;
            //_repo.CorrectOrder(id);

            return RedirectToAction("AdminOrderAccept", acceptedID);
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
            var users = _repo.AspNetUsers;
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
        [HttpGet]
        public IActionResult AdminAddProduct()
        {
            return View();
        }
        //public IActionResult AdminUserEdit(int id)
        //{
        //    AspNetUser update = _repo.UpdateUser(id);
        //}

        //public IActionResult AdminUserEdit(int id)
        //{
        //AspNetUser update = _repo.UpdateUser(id);

        //    return View("AddUserForm", update);
        //}

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

            
        }

        //public IActionResult AdminOrderAccept(int id)
        //{
        //    ViewBag.Id = id;
            
        //    return View();
        //}
    }
