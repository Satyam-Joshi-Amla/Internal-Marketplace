﻿using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.Models.ViewModels.Product;
using AmlaMarketPlace.Models.DTO;
using Microsoft.EntityFrameworkCore;
using AmlaMarketPlace.DAL.Service.Services.Account;

namespace AmlaMarketPlace.DAL.Service.Services.Product
{
    public class ProductService
    {
        private readonly AmlaMarketPlaceDbContext _context;
        private readonly AccountService _service;

        // Initialize the DbContext
        public ProductService(AmlaMarketPlaceDbContext context, AccountService service)
        {
            _context = context;
            _service = service;
        }

        public PaginatedResultDto GetProducts(int pageNumber = 1, int pageSize = 20)
        {
            var result = new List<ProductListViewModel>();
            int totalCount = 0; // This will store the total product count

            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open(); // Open the database connection

                using (var command = connection.CreateCommand())
                {
                    // Modify the stored procedure to take pageNumber and pageSize as parameters
                    command.CommandText = "EXEC GetPaginatedProductsWithDefaultImage @PageNumber, @PageSize"; // Name of your updated SP
                    command.CommandType = System.Data.CommandType.Text;

                    // Add the parameters for pagination
                    var pageNumberParam = command.CreateParameter();
                    pageNumberParam.ParameterName = "@PageNumber";
                    pageNumberParam.Value = pageNumber;
                    command.Parameters.Add(pageNumberParam);

                    var pageSizeParam = command.CreateParameter();
                    pageSizeParam.ParameterName = "@PageSize";
                    pageSizeParam.Value = pageSize;
                    command.Parameters.Add(pageSizeParam);

                    using (var reader = command.ExecuteReader())
                    {
                        // Read the products for the current page
                        while (reader.Read())
                        {
                            result.Add(new ProductListViewModel
                            {
                                ProductId = reader.GetInt32(0),       // Index 0: ProductId
                                Name = reader.GetString(1),           // Index 1: ProductName
                                Price = (float)reader.GetDecimal(2),  // Index 2: Price
                                Description = reader.GetString(3),    // Index 3: Description
                                CreatedOn = reader.GetDateTime(4),
                                ModifiedOn = reader.GetDateTime(5),
                                Inventory = reader.GetInt32(6),
                                StatusId = reader.GetInt32(7),
                                IsPublished = reader.GetBoolean(8),
                                ImageLink = reader.GetString(9)
                            });
                        }

                        // Assuming the stored procedure returns the total product count as the last column in the result set
                        if (reader.NextResult() && reader.Read())
                        {
                            totalCount = reader.GetInt32(0); // The total count of products
                        }
                    }
                }
            }

            return new PaginatedResultDto
            {
                Products = result, // List of products for the current page
                TotalCount = totalCount // Total product count
            };
        }

        public string GetStatusValueByStatusId(int statusID)
        {
            var status = _context.Statuses.FirstOrDefault(s => s.StatusId == statusID);
            return status != null ? status.StatusValue : "pending";
        }

        public List<ProductDTO> GetUserUploadedProducts(int userID)
        {
            // Fetching only published products from the database
            var products = _context.Products
                .Where(u => u.UserId == userID) // Filtering published products
                .ToList();

            // Mapping the filtered products to ProductDTO
            var productDTO = products.Select(product => new ProductDTO
            {
                ProductId = product.ProductId,
                UserId = product.UserId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CreatedOn = product.CreatedOn,
                ModifiedOn = product.ModifiedOn,
                Inventory = product.Inventory,
                StatusId = product.StatusId,
                StatusValue = GetStatusValueByStatusId(product.StatusId),
                IsPublished = product.IsPublished
            }).ToList();

            return productDTO;
        }

        public ProductDetailsViewModel GetProductDetails(int productId)
        {
            var product = _context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDetailsViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CreatedOn = p.CreatedOn,
                    ModifiedOn = p.ModifiedOn,
                    Inventory = p.Inventory,
                    StatusId = p.StatusId,
                    IsPublished = p.IsPublished,
                    Images = p.Images.Select(i => new ImageViewModel
                    {
                        ImagePath = i.Link,
                        IsDefault = (bool)i.IsDefault
                    }).OrderByDescending(i => i.IsDefault).ToList()
                })
                .FirstOrDefault();

            return product;
        }
                
        public bool AddProduct(AddProductDto Dto)
        {
            var product = new AmlaMarketPlace.DAL.Data.Product();
            product.UserId = Dto.UserId;
            product.Name = Dto.ProductName;
            product.Price = Dto.Price;
            product.Description = Dto.Description;
            product.Inventory = Dto.Inventory;

            _context.Products.Add(product);
            _context.SaveChanges();

            Dto.ProductId = product.ProductId;

            var image = new Image
            {
                ProductId = Dto.ProductId,
                Name = Dto.ImageName,
                Link = Dto.ImagePath,
                IsDefault = true
            };

            if (Dto.OptionalImageNames != null && Dto.OptionalImageNames.Any())
            {
                for (int i=0; i<Dto.OptionalImageNames.Count; i++)
                {
                    var optImage = new Image
                    {
                        ProductId = Dto.ProductId,
                        Name = Dto.OptionalImageNames[i],
                        Link = Dto.OptionalImagePaths[i],
                        IsDefault = false
                    };
                    _context.Images.Add(optImage);
                }
            }
            _context.Images.Add(image);
            _context.SaveChanges();
            return true;
        }

        public bool PlaceOrder(int productId, int buyerId)
        {
            var sellerId = _context.Products.Where(p => p.ProductId == productId).Select(p => p.UserId).FirstOrDefault();
            var order = new Order
            {
                BuyerId = buyerId,
                SellerId = sellerId,
                ProductId = productId,
            };
            _context.Orders.Add(order);
            _context.SaveChanges();


            string buyerEmail = _context.Users.FirstOrDefault(u => u.UserId == buyerId).EmailAddress;
            _service.SendMessageOnMail(buyerEmail, "Order Placed", "Seller and buyer are notified successfully");
            return true;
        }

        public bool PublishProductSuccessfully(int productID)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productID);

            if (product != null)
            {
                product.IsPublished = true;
                _context.SaveChanges();

                return true;
            }

            return false;
        }
        public EditProductViewModel GetEditDetails(int id)
        {
            var product = _context.Products
                            .Include(p => p.Images)
                            .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return null; // If the product does not exist, return a 404 error
            }
            var productViewModel = new EditProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Inventory = product.Inventory,
                Images = product.Images.Select(i => new ImageViewModel
                {
                    ImagePath = i.Link,
                    IsDefault = (bool)i.IsDefault
                }).ToList()
            };

            return productViewModel;
        }

        public bool EditProduct(EditProductViewModel model)
        {
            var product = _context.Products
                            .Include(p => p.Images)
                            .FirstOrDefault(p => p.ProductId == model.ProductId);
            product.Name = model.Name;
            product.Price = model.Price;
            string description;
            if (model.Description==null)
            {
                description = "";
            }
            else
            {
                description = model.Description;
            }
            product.Description = description;
            product.Inventory = model.Inventory;

            if (model.Image != null)
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

                var defaultImage = _context.Images
                    .FirstOrDefault(i => i.ProductId == model.ProductId && (bool)i.IsDefault);

                if (defaultImage != null)
                {
                    _context.Images.Remove(defaultImage);
                    var image = new Image
                    {
                        ProductId = model.ProductId,
                        Name = fileName,
                        Link = GetRelativeImagePath(filePath),
                        IsDefault = true
                    };
                    _context.Images.Add(image);
                }
                
            }
            if (model.OptionalImages != null)
            {
                var optionalImages = _context.Images
                .Where(i => i.ProductId == model.ProductId && !(bool)i.IsDefault)
                .ToList();

                if (optionalImages.Any())
                {
                    _context.Images.RemoveRange(optionalImages);
                }

                foreach (var optionalImageFile in model.OptionalImages)
                {
                    string wwwRootPathOpt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string fileNameOpt = Guid.NewGuid().ToString() + Path.GetExtension(optionalImageFile.FileName);
                    string imagesDirectoryOpt = Path.Combine(wwwRootPathOpt, "images");
                    string filePathOpt = Path.Combine(imagesDirectoryOpt, fileNameOpt);
                    using (var stream = new FileStream(filePathOpt, FileMode.Create))
                    {
                        optionalImageFile.CopyTo(stream);
                    }

                    _context.Images.Add(new Image
                    {
                        ProductId = model.ProductId,
                        Name = fileNameOpt,
                        Link = GetRelativeImagePath(filePathOpt),
                        IsDefault = false
                    });
                }
            }
            _context.SaveChanges();

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


        //public bool EditProduct(ProductDetailsViewModel model)
        //{
        //    var existingProduct = _context.Products
        //    .Include(p => p.Images) // Include related images
        //    .FirstOrDefault(p => p.ProductId == model.ProductId);

        //    existingProduct.Name = model.Name;
        //    existingProduct.Price = model.Price;
        //    existingProduct.Description = model.Description;
        //    existingProduct.ModifiedOn = DateTime.Now;
        //    existingProduct.Inventory = model.Inventory;
        //    existingProduct.StatusId = model.StatusId;
        //    existingProduct.IsPublished = model.IsPublished;

        //    _context.SaveChanges();
        //    return true;
        //}




    }
}
