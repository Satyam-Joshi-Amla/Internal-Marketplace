using AmlaMarketPlace.DAL.Service.Services.Product;
using AmlaMarketPlace.Models.ViewModels.Product;
using AmlaMarketPlace.Models.DTO;
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

        public bool AddProduct(AddProductViewModel model)
        {
            try
            {
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = timeStamp + Path.GetExtension(model.Image.FileName);
                string imagesDirectory = Path.Combine(wwwRootPath, "images");
                string filePath = Path.Combine(imagesDirectory, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(stream);
                }
                AddProductDto Dto = new AddProductDto();
                Dto.UserId = model.UserId;
                Dto.ProductName = model.Name;
                Dto.Price = model.Price;
                Dto.Description = model.Description;
                Dto.Inventory = model.Inventory;
                Dto.ProductId = 0;
                Dto.ImageName = fileName;
                Dto.ImagePath = GetRelativeImagePath(filePath);

                _productService.AddProduct(Dto);
            } 
            catch
            {

            }
            return true;
        }

        public string GetRelativeImagePath(string fullPath)
        {
            // Find the index of "wwwroot" in the full path
            int index = fullPath.IndexOf("wwwroot");

            if (index >= 0)
            {
                // Extract everything after "wwwroot"
                string relativePath = fullPath.Substring(index + "wwwroot".Length); // Add the length of "wwwroot" to exclude it
                return relativePath;
            }

            // Return an empty string if "wwwroot" is not found
            return string.Empty;
        }
    }
}
