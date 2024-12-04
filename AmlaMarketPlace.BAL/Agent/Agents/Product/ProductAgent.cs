using AmlaMarketPlace.DAL.Service.Services.Product;
using AmlaMarketPlace.Models.ViewModels.Product;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.BAL.Agent.Agents.Product
{
    public class ProductAgent
    {
        private readonly ProductService _productService;
        public ProductAgent(ProductService productService)
        {
            _productService = productService;
        }

        public List<ProductListViewModel> GetProducts()
        {
            return _productService.GetProducts();
        }
    }
}
