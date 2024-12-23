using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.Models.ViewModels.Product;
using AmlaMarketPlace.Models.DTO;
using Microsoft.EntityFrameworkCore;
using AmlaMarketPlace.DAL.Service.IServices.IProduct;
using AmlaMarketPlace.ConfigurationManager.UtilityMethods;

namespace AmlaMarketPlace.DAL.Service.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly AmlaMarketPlaceDbContext _context;

        // Initialize the DbContext
        public ProductService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }
        //public PaginatedResultDto GetProductsCache(int userId, int pageNumber = 1, int pageSize = 20)
        //{
        //    string cacheKey = $"ProductList_{userId}";
        //    if (!_cache.TryGetValue(cacheKey, out PaginatedResultDto productList))
        //    {
        //        productList = GetProducts(userId, pageNumber, pageSize);
        //        var cacheOptions = new MemoryCacheEntryOptions
        //        {
        //            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
        //            SlidingExpiration = TimeSpan.FromMinutes(2)
        //        };
        //        _cache.Set(cacheKey, productList, cacheOptions);
        //    }
        //    return productList;
        //}
        public PaginatedResultDto GetProducts(int userId, int pageNumber = 1, int pageSize = 20)
        {
            var result = new List<ProductListViewModel>();
            int totalCount = 0; // This will store the total product count

            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open(); // Open the database connection

                using (var command = connection.CreateCommand())
                {
                    // Modify the stored procedure to take pageNumber and pageSize as parameters
                    command.CommandText = "EXEC GetPaginatedProductsWithDefaultImageExcludeUser @PageNumber, @PageSize, @UserId"; // Name of your updated SP
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

                    var userIdParam = command.CreateParameter();
                    userIdParam.ParameterName = "@UserId";
                    userIdParam.Value = userId;
                    command.Parameters.Add(userIdParam);

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
                IsPublished = product.IsPublished,
                CommentForRejecting = product.StatusId == 3
            ? _context.ProductComments
                .Where(comment => comment.ProductId == product.ProductId)
                .Select(comment => comment.RejectedComments)
                .FirstOrDefault()
            : null
            }).ToList();

            return productDTO;
        }

        public ProductDetailsViewModel GetProductDetails(int productId)
        {
            try
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
                        SellerId = p.UserId,
                        StatusId = p.StatusId,
                        IsPublished = p.IsPublished,
                        Images = p.Images.Select(i => new ImageViewModel
                        {
                            ImagePath = i.Link,
                            IsDefault = (bool)i.IsDefault
                        }).OrderByDescending(i => i.IsDefault).ToList()
                    })
                    .FirstOrDefault();

                if (product != null)
                {
                    product.SellerName = GetUserNameByID(product.SellerId);
                }

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching product details. Please try again later. from service");
            }
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
                for (int i = 0; i < Dto.OptionalImageNames.Count; i++)
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

        public bool PlaceOrder(int productId, int buyerId, int orderQuantity)
        {
            var sellerId = _context.Products.Where(p => p.ProductId == productId).Select(p => p.UserId).FirstOrDefault();
            var order = new Order
            {
                BuyerId = buyerId,
                SellerId = sellerId,
                ProductId = productId,
                Quantity = orderQuantity
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Fetching Product Details
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            string productName = product.Name;

            // Fetching Buyer Details
            string buyerName = GetUserNameByID(buyerId);
            string buyerEmail = _context.Users.FirstOrDefault(u => u.UserId == buyerId).EmailAddress;

            // Fetching Seller Details
            string sellerName = GetUserNameByID(sellerId);
            string sellerEmail = GetUserEmailByID(sellerId);

            // Mail Contents to send to buyer
            string buyerMailSubject = $"Order Placed";
            string buyerMailMessage = $"Hi,\nThank you for Showing interest in {productName}.\n\nHere are the seller Details:\nName: {sellerName}\nEmail: {sellerEmail}\n\nThank you for using our service. ";

            // Mail Contents to send to seller
            string sellerMailSubject = $"Order Received";
            string sellerMailMessage = $"Hi,\nSomeone is interested in purchasing \"{productName}\" from you.\n\nHere are the buyer Details:\nName: {buyerName}\nEmail: {buyerEmail}\n\nThank you for using our service.";

            // Sending Mail to Buyer with Contact Details of Seller
            MailUtility.SendMessageOnMail(buyerEmail, buyerMailSubject, buyerMailMessage);

            // Sending Mail to Seller with Contact Details of Buyer
            MailUtility.SendMessageOnMail(sellerEmail, sellerMailSubject, sellerMailMessage);

            //int? inventory = GetInventory(productId);
            //if (inventory >= orderQuantity)
            //{
            //    UpdateInventory(productId, (int)(inventory - orderQuantity));
            //}
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

        public bool UnpublishProductSuccessfully(int productID)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productID);

            if (product != null)
            {
                product.IsPublished = false;
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
            if (model.Description == null)
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
                string imagesDirectory = Path.Combine(wwwRootPath, "images/ProductImages");
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
                    string imagesDirectoryOpt = Path.Combine(wwwRootPathOpt, "images/ProductImages");
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

        private string GetUserNameByID(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            string name = $"{user.FirstName} {user.LastName}";
            return name;
        }

        private string GetUserEmailByID(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            return user.EmailAddress;
        }

        private string GetProductNameByID(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            return $"{product.Name}";
        }

        public List<OrderDTO> GetOrderHistory(int userId)
        {
            List<Order> orders = _context.Orders
                .Where(s => s.SellerId == userId) // Filtering all orders of specific user
                .ToList();

            List<OrderDTO> orderDTO = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                BuyerId = o.BuyerId,
                BuyerName = GetUserNameByID(o.BuyerId),
                SellerId = o.SellerId,
                SellerName = GetUserNameByID(o.SellerId),
                ProductId = o.ProductId,
                ProductName = GetProductNameByID(o.ProductId),
                OrderTime = o.OrderTime,
                IsApproved = o.IsApproved,
                Quantity = o.Quantity
            }).ToList();

            foreach (OrderDTO orderDetails in orderDTO)
            {
                orderDetails.ActualQuantity = _context.Products.FirstOrDefault(p => p.ProductId == orderDetails.ProductId).Inventory;
            }

            return orderDTO;
        }
        public bool ChangeStatusTO(int statusID, int productID)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productID);
            product.StatusId = statusID;
            return true;
        }

        public int? GetInventory(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            return product?.Inventory;
        }

        public bool UpdateInventory(int productId, int updatedInventory)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                product.Inventory = updatedInventory;
                _context.Products.Update(product);
                _context.SaveChanges();
            }
            return true;

        }

        public List<MyOrdersDto> GetMyRequests(int userId)
        {
            List<Order> orders = _context.Orders
                .Where(s => s.BuyerId == userId) // Filtering all orders of specific user
                .ToList();

            List<MyOrdersDto> myOrders = orders.Select(o => new MyOrdersDto
            {
                OrderId = o.OrderId,
                BuyerId = o.BuyerId,
                BuyerName = GetUserNameByID(o.BuyerId),
                SellerId = o.SellerId,
                SellerName = GetUserNameByID(o.SellerId),
                ProductId = o.ProductId,
                ProductName = GetProductNameByID(o.ProductId),
                OrderTime = o.OrderTime,
                IsApproved = o.IsApproved,
                Quantity = o.Quantity,
                RejectComment = o.RejectComment
            }).ToList();

            return myOrders;
        }

        public bool UpdateOrder(int orderId, int orderStatus, string rejectComment)
        {
            if (orderStatus == 1)
            {
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                if (order != null)
                {
                    var product = _context.Products.FirstOrDefault(p => p.ProductId == order.ProductId);
                    if (product != null)
                    {
                        product.Inventory -= order.Quantity;
                        order.IsApproved = orderStatus;
                        _context.Products.Update(product);
                        _context.Orders.Update(order);
                        _context.SaveChanges();
                    }
                    
                }
            }
            else if (orderStatus == 2)
            {
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.IsApproved = orderStatus;
                    order.RejectComment = rejectComment;
                    _context.Orders.Update(order);
                    _context.SaveChanges();
                }
            }
            return true;
        }

        public int TotalUserUploadedProductsCount(int userId)
        {
            var totalProductCount = _context.Products.Where(u => u.UserId == userId).Count();

            if (totalProductCount != null)
            {
                return totalProductCount;
            }

            return 0;
        }

        public int TotalUserApprovedProductsCount(int userId)
        {
            var totalApprovedProductCount = _context.Products.Where(u => u.UserId == userId && u.StatusId == 2 && u.IsPublished == false).Count();

            if (totalApprovedProductCount != null)
            {
                return totalApprovedProductCount;
            }

            return 0;
        }

        public int TotalUserRejectedProductsCount(int userId)
        {
            var totalRejectedProductCount = _context.Products.Where(u => u.UserId == userId && u.StatusId == 3).Count();

            if (totalRejectedProductCount != null)
            {
                return totalRejectedProductCount;
            }

            return 0;
        }

        public int TotalUserPublishedProductsCount(int userId)
        {
            var totalPublishedProductCount = _context.Products.Where(u => u.UserId == userId && u.IsPublished == true).Count();

            if (totalPublishedProductCount != null)
            {
                return totalPublishedProductCount;
            }

            return 0;
        }

        public int TotalUserWaitingForApprovalProductsCount(int userId)
        {
            var totalWaitingForApprovalProductCount = _context.Products.Where(u => u.UserId == userId && u.StatusId == 1).Count();

            if (totalWaitingForApprovalProductCount != null)
            {
                return totalWaitingForApprovalProductCount;
            }

            return 0;
        }

        public UserDTO GetUserById(int id)
        {
            // Fetching the user from the database based on the email
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);

            if (user != null)
            {
                var userDTO = new UserDTO
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    IsEmailVerified = user.IsEmailVerified,
                    Password = user.Password,
                    MobileNumber = user.MobileNumber,
                    IsmobileNumberVerified = user.IsmobileNumberVerified,
                    UserRoleId = user.UserRoleId,
                    CreatedOn = user.CreatedOn,
                    EditedOn = user.EditedOn,
                    VerificationToken = user.VerificationToken,
                    TokenExpiration = user.TokenExpiration
                };

                var userRoleData = _context.UserRoles.FirstOrDefault(r => r.RoleId == user.UserRoleId);

                userDTO.UserRole = userRoleData != null ? userRoleData.Role : "user";

                return userDTO;
            }

            return null; // Return null if no user is found
        }
    }
}
