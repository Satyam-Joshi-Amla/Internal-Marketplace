using Microsoft.AspNetCore.Mvc;
using AmlaMarketPlace.BAL.Agent.Agents.Product;
using AmlaMarketPlace.Models.ViewModels.Product;

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
            int itemsToShow = 8;
            ViewData["Products"] = products.Take(itemsToShow).ToList(); // To show only the first 8 products
            ViewData["TotalProducts"] = products.Count; // sending the total product count to the view
            return View();
        }
    }
}
