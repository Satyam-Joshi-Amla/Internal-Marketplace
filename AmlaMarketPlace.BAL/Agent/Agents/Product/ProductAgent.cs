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

        public PaginatedResultDto GetProducts(int pageNumber, int pageSize, int userId)
        {
            return _productService.GetProducts(userId, pageNumber, pageSize);
        }

        public List<ProductDTO> GetUserUploadedProducts(int userID)
        {
            return _productService.GetUserUploadedProducts(userID);
        }

        public ProductDetailsViewModel GetIndividualProduct(int productId)
        {
            try
            {
                return _productService.GetProductDetails(productId);
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while fetching product details. Please try again later. from agent");
            }
        }

        public bool AddProduct(AddProductViewModel model)
        {
            try
            {
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = timeStamp + Path.GetExtension(model.Image.FileName);
                string imagesDirectory = Path.Combine(wwwRootPath, "images/ProductImages");
                string filePath = Path.Combine(imagesDirectory, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(stream);
                }
                List<string> optImageNames = [];
                List<string> optImagePaths = [];
                if (model.OptionalImages != null)
                {
                    foreach (var img in model.OptionalImages)
                    {
                        string wwwRootPathOpt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        string fileNameOpt = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                        string imagesDirectoryOpt = Path.Combine(wwwRootPathOpt, "images/ProductImages");
                        string filePathOpt = Path.Combine(imagesDirectoryOpt, fileNameOpt);
                        using (var stream = new FileStream(filePathOpt, FileMode.Create))
                        {
                            img.CopyTo(stream);
                        }
                        optImageNames.Add(fileNameOpt);
                        optImagePaths.Add(GetRelativeImagePath(filePathOpt));
                    }
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
                Dto.OptionalImageNames = optImageNames;
                Dto.OptionalImagePaths = optImagePaths;

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
                string relativePath = fullPath.Substring(index + "wwwroot".Length);

                // Replace backslashes with forward slashes for web compatibility
                return relativePath.Replace("\\", "/");
            }

            // Return an empty string if "wwwroot" is not found
            return string.Empty;
        }

        public bool PlaceOrder(int productId, int buyerId, int orderQuantity)
        {
            _productService.PlaceOrder(productId, buyerId, orderQuantity);
            return true;
        }

        public bool PublishProductSuccessfully(int productID)
        {
            return _productService.PublishProductSuccessfully(productID);
        }

        public bool UnpublishProductSuccessfully(int productID)
        {
            return _productService.UnpublishProductSuccessfully(productID);
        }

        public EditProductViewModel GetEditDetails(int id)
        {
            return _productService.GetEditDetails(id);
        }

        public bool EditProduct(EditProductViewModel model)
        {
            return _productService.EditProduct(model);
        }

        public List<OrderDTO> GetOrderHistory(int userId)
        {
            return _productService.GetOrderHistory(userId);
        }

        public bool ChangeStatusTOPending(int productId)
        {
            return _productService.ChangeStatusTO(1, productId);
        }
        public bool ChangeStatusTOApproved(int productId)
        {
            return _productService.ChangeStatusTO(2, productId);
        }
        public bool ChangeStatusTORejected(int productId)
        {
            return _productService.ChangeStatusTO(3, productId);
        }

        public List<OrderDTO> GetMyRequests(int userId)
        {
            return _productService.GetMyRequests(userId);
        }

        public bool UpdateOrder(int orderId, int orderStatus)
        {
            _productService.UpdateOrder(orderId, orderStatus);
            return true;
        }
    }
}
