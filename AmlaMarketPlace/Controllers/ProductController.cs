using Microsoft.AspNetCore.Mvc;
using AmlaMarketPlace.BAL.Agent.Agents.Product;
using AmlaMarketPlace.Models.ViewModels.Product;
using AmlaMarketPlace.DAL.Service.Services.Product;
using Microsoft.AspNetCore.Authorization;

namespace AmlaMarketPlace.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ProductAgent _productAgent;
        public ProductController(ProductAgent productAgent)
        {
            _productAgent = productAgent;
        }

        [HttpGet]
        public IActionResult ProductListing()
        {
            List<ProductListViewModel> products = _productAgent.GetProducts();
            int itemsToShow = 8;
            ViewData["Products"] = products.Take(itemsToShow).ToList(); // To show only the first 8 products
            ViewData["TotalProducts"] = products.Count; // sending the total product count to the view
            return View();
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            // This will give you the "UserId" claim from the current user
            //int userId = 3;    //User.FindFirst("UserId")?.Value;
            ViewData["EnableUserSidePanel"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(AddProductViewModel model, string action)
        {
            model.UserId = int.Parse(User.FindFirst("UserId")?.Value);
            if (string.IsNullOrEmpty(model.Description))
            {
                model.Description = "";  // Set to empty string if null or empty
            }
            if (ModelState.IsValid)
            {
                bool result = _productAgent.AddProduct(model);

                if (result)
                {
                    if (action == "Save and Close")
                    {
                        return RedirectToAction("ProductListing");
                    }
                    else if (action == "Save and Add More")
                    {
                        TempData["SuccessMessage"] = "Product uploaded successfully!";
                        return RedirectToAction("AddProduct");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error uploading the product.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ProductDetails(int id)
        {
            var productDetails = _productAgent.GetIndividualProduct(id);
            if (productDetails == null)
            {
                return NotFound();
            }

            return View(productDetails);
        }

        public IActionResult PlaceOrder(int productId)
        {
            _productAgent.PlaceOrder(productId, int.Parse(User.FindFirst("UserId")?.Value));
            return Ok();
        }

        public IActionResult GetUserUploadedProductsList(int id)
        {
            var userUploadedProducts = _productAgent.GetUserUploadedProducts(id);
            ViewData["EnableUserSidePanel"] = true;
            return View(userUploadedProducts);
        }

        [HttpPost]
        public IActionResult Publish(int id)
        {
            bool publishedSuccessfully = _productAgent.PublishProductSuccessfully(id);

            if (publishedSuccessfully)
            {
                TempData["PublishedSuccess"] = "Product is now published.";
            }
            else
            {
                TempData["PublishedFailed"] = "Product cannot be published. Please contact admin.";
            }

            // Redirect back to the GetUserUploadedProductsList with the same user id
            int userId = int.Parse(User.FindFirst("UserId")?.Value); // Extract the logged-in user's id
            return RedirectToAction("GetUserUploadedProductsList", new { id = userId });
        }



        //[HttpPost]
        //public IActionResult ProductDetails()
        //{
        //    return View();
        //}
    }
}
