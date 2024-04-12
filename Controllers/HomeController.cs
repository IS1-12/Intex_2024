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
using Microsoft.AspNetCore.Identity;
using LegosWithAurora.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        //initialize the repo
        private ILegoRepository _repo;

        //initialize the cart
        private Cart Cart = new Cart();

        //prepare to initialize the Onnx model
        private readonly InferenceSession _session;

        public HomeController(ILegoRepository temp)
        {
            _repo = temp;

            //Start Onnx
            try
            {
                _session = new InferenceSession("./wwwroot/decision_tree_model.onnx");
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

            //This looks for if a user has purchase history. If they do, it loads it to a viewbag. It also loads a generic viewbag
            //in case a user is not logged in, or their history is empty
            try
            {
                int userId = 29135;

                var orderId = _repo.Orders.OrderBy(x => x.TransactionId).LastOrDefault(x => x.CustomerId == userId).TransactionId;

                var productId = _repo.LineItems.FirstOrDefault(x => x.TransactionId == orderId).ProductId;

                List<Product> RecProd = new List<Product>();

                var Rec = _repo.recommendations
                        .Where(x => x.product_ID == productId).Single();

                List<int> RecInt = new List<int>
                {
                    Rec.Rec1 + 1, Rec.Rec2 + 1, Rec.Rec3 + 1
                };

                foreach (var rec in RecInt)
                {
                    var conversionVar = _repo.Products.Where(x => x.ProductId == rec).Single();

                    RecProd.Add((Product)conversionVar);
                }

                ViewBag.RecommendationsSigned = RecProd;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }  

            List<Product> RecProds = new List<Product>();

            // This is the generic recommendation. It is based off of Stitch, the most popular product
            var Recs = _repo.recommendations
                    .Where(x => x.product_ID == 24).Single();

            List<int> RecInts = new List<int>
                {
                    Recs.Rec1 + 1, Recs.Rec2 + 1, Recs.Rec3 + 1
                };

            foreach (var rec in RecInts)
            {
                var conversionVar = _repo.Products.Where(x => x.ProductId == rec).Single();

                RecProds.Add((Product)conversionVar);
            }

            ViewBag.Recommendations = RecProds;

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
        public IActionResult Products(int pageNum, string categories, string color, int numProducts)
        {
            //This is for pagination. We were having some troubles with the numProducts being set to 0, so if it is, it returns to our default of 9
            int pageSize = numProducts;
            if (pageSize < 1) pageSize = 9;
            
            IQueryable<Product> products = _repo.Products;

            // Apply category filter if it is not null
            if (!string.IsNullOrEmpty(categories) && categories != "All")
            {
                products = products.Where(x => x.Category == categories);
            }
            
            // Apply color filter if it is not null
            if (!string.IsNullOrEmpty(color))
            {
                products = products.Where(x => x.PrimaryColor == color);
            }

            // Order and paginate the filtered products
            var paginatedProducts = products.OrderBy(x => x.ProductId)
                                            .Skip((pageNum - 1) * pageSize)
                                            .Take(pageSize);

            // Create the product list view model
            var productsPages = new ProductListViewModel
            {
                Products = paginatedProducts,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = products.Count() // Calculate total count based on filtered products
                },
                CurrentProductType = categories, // This might need adjustment if you want to display color or both.
                CurrentColor = color
            };
            return View(productsPages);
        }
        
        

        public IActionResult ProductDetails(int id, string returnUrl)
        {
            //Return URL to go to the place where they went to product details from
            ViewBag.returnUrl = returnUrl ?? "/";

            List<Product> RecProd = new List<Product>();

            var products = _repo.Products
                            .Where(x => x.ProductId == id).Single();

            var Recs = _repo.recommendations
                    .Where(x => x.product_ID == id).Single();

            List<int> RecInts = new List<int>
            {
                Recs.Rec1 + 1, Recs.Rec2 + 1, Recs.Rec3 + 1
            };

            foreach (var rec in RecInts)
            {
                var conversionVar = _repo.Products.Where(x => x.ProductId == rec).Single();

                RecProd.Add((Product)conversionVar);
            }

            ViewBag.Recommendations = RecProd;

            return View(products);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, string returnUrl)
        {
            Product? prod = _repo.Products
                            .FirstOrDefault(x => x.ProductId == productId);

            // add the selected item to the session cart
            if (prod != null)
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                Cart.AddItem(prod, 1);
                HttpContext.Session.SetJson("cart", Cart);
            }

            return RedirectToAction("CartConfirmation", new { id = productId, returnUrl = returnUrl });
        }

        //First authorized page. This requires users to log in to maintain a cart
        [Authorize]
        public IActionResult CartConfirmation(int id, string returnUrl)
        {
            Product? prod = _repo.Products
                            .FirstOrDefault(x => x.ProductId == id);

            ViewBag.returnUrl = returnUrl ?? "/";
            return View(prod);
        }

        [Authorize]
        public IActionResult ViewCart()
        {
            ViewBag.Cart = Cart;
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [Authorize]
        public IActionResult Checkout()
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart")
                ?? new Cart();
            return View(Cart);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Checkout(IFormCollection data)
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            var test = data;

            // This is the fraud prediction model
            try
            {
                // our model required only two inputs: time and amount. We can calculate both here so that a user can't fail to pass them
                var input = new List<float> { DateTime.Now.Hour, Cart.CalculateTotal() };
                var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

                // format the inputs correctly
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
                };

                using (var results = _session.Run(inputs)) // makes the prediction with the inputs from above
                {
                    var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                    if (prediction != null && prediction.Length > 0)
                    {
                        Console.WriteLine(prediction);
                        // Use the prediction to set the fraud flag in the database
                        var isFraud = (int)prediction[0];
                        ViewBag.Prediction = isFraud;
                    }
                    else
                    {
                        ViewBag.Prediction = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Prediction = 0;
            }
            // create a new Order using the cart and the prediction. Most of the fields are preset
            var submit = new Order
            {
                CustomerId = 29135,
                Amount = Cart.CalculateTotal(),
                CountryOfTransaction = "USA",
                Fraud = ViewBag.Prediction
            };
            _repo.AddOrder(submit);

            // create line item records
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

            //empty the cart
            Cart.Clear();
            HttpContext.Session.SetJson("cart", Cart);

            return View("OrderConfirmation");
        }


        //Start admin views
        [Authorize(Roles = "Admin")]
        public IActionResult AdminAddUser(string id) {

            var user = _repo.UpdateUser(id);
            
            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderCancelled(int id)
        {
            Order delete = _repo.RejectOrder(id);
            return View(delete);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AdminOrderCancelled(Order o)
        {
            _repo.RejectOrder(o);
            return RedirectToAction("AdminOrderReview");
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderReview()
        {
            //Filter the orders for just fraudulent orders in order to review them
            var orders = _repo.Orders
                .Where(x => x.Fraud == 1);
            
            return View(orders);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderReview(int id)
        {
            //Accept false positives
            var acceptedID = id;
            _repo.CorrectOrder(id);

            return RedirectToAction("AdminOrderAccept", acceptedID);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderAccept()
        {
            return View();
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult AdminAllProducts()
        {
            //View all products in database
            var products = _repo.Products;

            products.ToList();

            return View(products);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminAllUsers()
        {
            //view all users in database
            var users = _repo.AspNetUsers;
            users.ToList();
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminProductDelete(int id)
        {
            
            Product delete = _repo.EditProduct(id);
            return View(delete);
        }
        

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDeleteProduct(int ProductId)
        {
            //Delete products
            _repo.RemoveProduct(ProductId);
            return RedirectToAction("AdminAllProducts");
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminAddProduct()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminUserEdit(string id)
        {
            //Edit users
            AspNetUser update = _repo.UpdateUser(id);

            return View("AdminAddUser", update);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public IActionResult SaveUser(AspNetUser user)
        {
            _repo.SaveUser(user);
            return RedirectToAction("AdminAllUsers");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDelete(string id)
        {
            var user = _repo.UpdateUser(id);
            return View("AdminUserDelete", user);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public IActionResult AdminUserDelete(string id)
        {
            _repo.DelUser(id);
            return RedirectToAction("AdminAllUsers");
        }

        [HttpPost]
        public IActionResult AdminAddProduct(Product p)
        {
            //Add new product
            _repo.AddProduct(p);

            return RedirectToAction("AdminAllProducts");
        }

        [Authorize(Roles="Admin")]
        public IActionResult AdminEditProduct(int id)
        {
            Product editProduct = _repo.EditProduct(id);

            return View("AdminAddProduct", editProduct);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public IActionResult EditProduct(Product product)
        {
            _repo.EditExistingProduct(product);
            return RedirectToAction("AdminAllProducts");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminAllCustomers()
        {
            var customers = _repo.Customers;

            customers.ToList();
            
            return View(customers);
        }
    }
}
