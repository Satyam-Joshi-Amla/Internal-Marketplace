using Microsoft.AspNetCore.Mvc;
using AmlaMarketPlace.BAL.Agent.Agents.Product;
using AmlaMarketPlace.Models.ViewModels.Product;
using AmlaMarketPlace.DAL.Service.Services.Product;
using Microsoft.AspNetCore.Authorization;
using AmlaMarketPlace.Models.DTO;

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
        public IActionResult ProductListing(int pageNumber = 1, int pageSize = 8)
        {
            var paginatedResult = _productAgent.GetProducts(pageNumber, pageSize);

            List<ProductListViewModel> products = paginatedResult.Products;
            int totalProducts = paginatedResult.TotalCount;

            ViewData["Products"] = products;
            ViewData["TotalProducts"] = totalProducts;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

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
                        int userId = int.Parse(User.FindFirst("UserId")?.Value); // Extract the logged-in user's id
                        return RedirectToAction("GetUserUploadedProductsList", new { id = userId });
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

        public IActionResult PlaceOrder(int productId, int orderQuantity)
        {
            _productAgent.PlaceOrder(productId, int.Parse(User.FindFirst("UserId")?.Value), orderQuantity);
            TempData["OrderPlaced"] = true;
            return RedirectToAction("ProductDetails", "Product", new { id = productId });
        }

        public IActionResult GetUserUploadedProductsList(int id)
        {
            var userUploadedProducts = _productAgent.GetUserUploadedProducts(id);
            ViewData["EnableUserSidePanel"] = true;
            return View(userUploadedProducts);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var productDetails = _productAgent.GetEditDetails(id);
            if (productDetails == null)
            {
                return NotFound();
            }

            return View(productDetails);
        }

        [HttpPost]
        public IActionResult EditProduct(EditProductViewModel model)
        {
            try
            {
                _productAgent.EditProduct(model);
                _productAgent.ChangeStatusTOPending(model.ProductId);
                _productAgent.UnpublishProductSuccessfully(model.ProductId);
            }
            catch (Exception e)
            {
                var errorMessage = "An error occurred while editing the product. Please try again later. from controller";
                return RedirectToAction("Error", new { errorMessage = errorMessage });
            }
            
            return RedirectToAction("ProductListing");
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

        [HttpPost]
        public IActionResult Unpublish(int id)
        {
            bool UnpublishedSuccessfully = _productAgent.UnpublishProductSuccessfully(id);

            if (UnpublishedSuccessfully)
            {
                TempData["UnpublishedSuccess"] = "Product is now Unpublished.";
            }
            else
            {
                TempData["unpublishedFailed"] = "Product cannot be unpublished. Please contact admin.";
            }

            // Redirect back to the GetUserUploadedProductsList with the same user id
            int userId = int.Parse(User.FindFirst("UserId")?.Value); // Extract the logged-in user's id
            return RedirectToAction("GetUserUploadedProductsList", new { id = userId });
        }

        [HttpGet]
        public IActionResult OrderHistory()
        {
            List<OrderDTO> orderDTOs = _productAgent.GetOrderHistory(int.Parse(User.FindFirst("UserId")?.Value));
            ViewData["EnableUserSidePanel"] = true;
            return View(orderDTOs);
        }

        [HttpPost]
        public IActionResult OrderHistory(int orderId, int orderStatus)
        {
            _productAgent.UpdateOrder(orderId, orderStatus);
            return RedirectToAction("OrderHistory");
        }

        [HttpGet]
        public IActionResult GetMyRequests()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value);
            ViewData["EnableUserSidePanel"] = true;
            List<OrderDTO> MyRequests = _productAgent.GetMyRequests(userId);
            return View(MyRequests);
        }
    }
}
