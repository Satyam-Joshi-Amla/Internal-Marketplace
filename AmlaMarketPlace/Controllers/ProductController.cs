using Microsoft.AspNetCore.Mvc;
using AmlaMarketPlace.BAL.Agent.Agents.Product;
using AmlaMarketPlace.Models.ViewModels.Product;
using AmlaMarketPlace.DAL.Service.Services.Product;
using Microsoft.AspNetCore.Authorization;

namespace AmlaMarketPlace.Controllers
{
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
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            // This will give you the "UserId" claim from the current user
            //int userId = 3;    //User.FindFirst("UserId")?.Value;
            return View();
        }


        [HttpPost]
        public IActionResult AddProduct(AddProductViewModel model)
        {
            model.UserId = 3;
            if (ModelState.IsValid)
            {
                bool result = _productAgent.AddProduct(model);

                if (result)
                {
                    return RedirectToAction("ProductListing");
                }
                else
                {
                    ModelState.AddModelError("", "Error uploading the product.");
                    return View(model);
                }
            }

            return View(model);
        }
    }
}
