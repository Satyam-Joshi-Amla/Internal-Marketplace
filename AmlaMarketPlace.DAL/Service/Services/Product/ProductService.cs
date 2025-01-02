using AmlaMarketPlace.ConfigurationManager.UtilityMethods;
using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.DAL.Service.IServices.IProduct;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Product;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AmlaMarketPlace.DAL.Service.Services.Product
{
    public class ProductService : IProductService
    {
        #region Dependency Injection - Database Context

        private readonly AmlaMarketPlaceDbContext _context;

        #endregion

        #region Constructor

        // Initialize the DbContext
        public ProductService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        #region Product Services

        /// <summary>
        /// Retrieves a paginated list of products for a specific user, excluding their own products.
        /// This method leverages a stored procedure to efficiently fetch product details and total count.
        /// </summary>
        /// <param name="userId">The ID of the user making the request, used to exclude their own products from the results.</param>
        /// <param name="pageNumber">The page number for pagination. Defaults to 1 if not specified.</param>
        /// <param name="pageSize">The number of products to include per page. Defaults to 20 if not specified.</param>
        /// <returns>A `PaginatedResultDto` containing the list of products for the specified page and the total product count.</returns>
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
                connection.Close();
            }

            return new PaginatedResultDto
            {
                Products = result, // List of products for the current page
                TotalCount = totalCount // Total product count
            };
        }

        /// <summary>
        /// Retrieves a paginated list of products matching the search criteria for a specific user, excluding their own products.
        /// This method leverages a stored procedure to efficiently fetch product details and total count based on search terms.
        /// </summary>
        /// <param name="searchTerm">The term to search in product name or description.</param>
        /// <param name="userId">The ID of the user making the request, used to exclude their own products from the results.</param>
        /// <param name="pageNumber">The page number for pagination. Defaults to 1 if not specified.</param>
        /// <param name="pageSize">The number of products to include per page. Defaults to 20 if not specified.</param>
        /// <returns>A `PaginatedResultDto` containing the list of products matching the search term, page details, and total product count.</returns>
        public PaginatedResultDto SearchProducts(string searchTerm, int userId, int pageNumber = 1, int pageSize = 20)
        {
            var result = new List<ProductListViewModel>();
            int totalCount = 0; // This will store the total product count

            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open(); // Open the database connection

                using (var command = connection.CreateCommand())
                {
                    // Call the stored procedure for searching products
                    command.CommandText = "EXEC SearchProductsWithDynamicFilter @SearchTerm, @PageNumber, @PageSize, @UserId"; // Name of your SP
                    command.CommandType = System.Data.CommandType.Text;

                    // Add the parameters for search and pagination
                    var searchTermParam = command.CreateParameter();
                    searchTermParam.ParameterName = "@SearchTerm";
                    searchTermParam.Value = searchTerm ?? string.Empty;
                    command.Parameters.Add(searchTermParam);

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
                        // Read the products for the current page based on the search
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
                            totalCount = reader.GetInt32(0); // The total count of matching products
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


        /// <summary>
        /// Retrieves the descriptive status value associated with a given status ID.
        /// If no matching status is found, it defaults to "pending."
        /// </summary>
        /// <param name="statusID">The unique identifier of the status to look up.</param>
        /// <returns>A string representing the status value, or "pending" if the status ID does not exist.</returns>
        public string GetStatusValueByStatusId(int statusID)
        {
            var status = _context.Statuses.FirstOrDefault(s => s.StatusId == statusID);
            return status != null ? status.StatusValue : "pending";
        }

        /// <summary>
        /// Gets a list of products uploaded by a specific user. It includes details like status, and rejection comments if the product was rejected.
        /// </summary>
        /// <param name="userID">The ID of the user whose products you want to get.</param>
        /// <returns>A list of products with all their details.</returns>
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
                .OrderBy(comment => comment.Date)
                .Select(comment => comment.RejectedComments)
                .LastOrDefault()
            : null
            }).ToList();

            return productDTO;
        }

        /// <summary>
        /// Retrieves detailed information about a specific product, including images and seller information.
        /// </summary>
        /// <param name="productId">The ID of the product to fetch details for.</param>
        /// <returns>A detailed view model containing product and seller information.</returns>
        /// <exception cref="Exception">Throws an exception if something goes wrong while retrieving product details.</exception>
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

        /// <summary>
        /// Adds a new product to the database along with its associated images.
        /// </summary>
        /// <param name="Dto">The product details and image information to be added.</param>
        /// <returns>Returns true if the product is successfully added.</returns>
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

        /// <summary>
        /// Places an order for a product, sends email notifications to both the buyer and the seller with their respective contact details, 
        /// and updates the database with the order information.
        /// </summary>
        /// <param name="productId">The ID of the product being ordered.</param>
        /// <param name="buyerId">The ID of the buyer placing the order.</param>
        /// <param name="orderQuantity">The quantity of the product to be ordered.</param>
        /// <returns>Returns true if the order is successfully placed.</returns>
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

        /// <summary>
        /// Publishes a product by setting its isPublished status to true in the database.
        /// </summary>
        /// <param name="productID">The ID of the product to be published.</param>
        /// <returns>Returns true if the product is successfully published; otherwise, false.</returns>
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

        /// <summary>
        /// Unpublishes a product by setting its published status to false in the database.
        /// </summary>
        /// <param name="productID">The ID of the product to be unpublished.</param>
        /// <returns>Returns true if the product is successfully unpublished; otherwise, false.</returns>
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

        /// <summary>
        /// Retrieves the details of a product for editing, including its images.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>An EditProductViewModel containing the product details and images, or null if the product does not exist.</returns>
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

        /// <summary>
        /// Updates the details of a product and its associated images in the database.
        /// </summary>
        /// <param name="model">The model containing updated product information.</param>
        /// <returns>Returns true if the update was successful; otherwise, false.</returns>
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

        /// <summary>
        /// Converts a full file path to a relative path by removing the "wwwroot" directory.
        /// </summary>
        /// <param name="fullPath">The full path of the file.</param>
        /// <returns>The relative path from "wwwroot" onwards, with forward slashes.</returns>
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

        /// <summary>
        /// Retrieves the name of a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The name of the product, or an empty string if not found.</returns>
        private string GetProductNameByID(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            return $"{product.Name}";
        }

        /// <summary>
        /// Retrieves the order history for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose order history is being fetched.</param>
        /// <returns>A list of order details related to the user.</returns>
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

        /// <summary>
        /// Changes the status of a product to the specified status ID.
        /// </summary>
        /// <param name="statusID">The ID of the new status to apply to the product.</param>
        /// <param name="productID">The ID of the product whose status is being updated.</param>
        /// <returns>Returns true if the status was successfully updated.</returns>
        public bool ChangeStatusTO(int statusID, int productID)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productID);
            product.StatusId = statusID;
            return true;
        }

        /// <summary>
        /// Retrieves the inventory count of a specified product.
        /// </summary>
        /// <param name="productId">The ID of the product whose inventory count is to be retrieved.</param>
        /// <returns>The inventory count if the product exists; otherwise, null.</returns>
        public int? GetInventory(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            return product?.Inventory;
        }

        /// <summary>
        /// Updates the inventory count of a specified product.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="updatedInventory">The new inventory count.</param>
        /// <returns>A boolean indicating if the update was successful.</returns>
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

        /// <summary>
        /// Retrieves the list of order requests made by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose order requests are to be fetched.</param>
        /// <returns>A list of order details for the specified user.</returns>
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

        /// <summary>
        /// Updates the status and details of an order based on the given order ID and status.
        /// </summary>
        /// <param name="orderId">The ID of the order to be updated.</param>
        /// <param name="orderStatus">The status to update the order to (e.g., approved or rejected).</param>
        /// <param name="rejectComment">The rejection comment, if the order is rejected.</param>
        /// <returns>A boolean indicating whether the update was successful.</returns>
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

        #endregion

        #region Seller DashBoard Counts

        /// <summary>
        /// Gets the total count of products uploaded by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose product count is to be retrieved.</param>
        /// <returns>The total number of products uploaded by the specified user, or 0 if none found.</returns>
        public int TotalUserUploadedProductsCount(int userId)
        {
            var totalProductCount = _context.Products.Where(u => u.UserId == userId).Count();

            if (totalProductCount != null)
            {
                return totalProductCount;
            }

            return 0;
        }

        /// <summary>
        /// Gets the total count of products approved by a specific user that are not published yet.
        /// </summary>
        /// <param name="userId">The ID of the user whose approved product count is to be retrieved.</param>
        /// <returns>The total number of approved, non-published products uploaded by the specified user, or 0 if none found.</returns>
        public int TotalUserApprovedProductsCount(int userId)
        {
            var totalApprovedProductCount = _context.Products.Where(u => u.UserId == userId && u.StatusId == 2 && u.IsPublished == false).Count();

            if (totalApprovedProductCount != null)
            {
                return totalApprovedProductCount;
            }

            return 0;
        }

        /// <summary>
        /// Gets the total count of products rejected by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose rejected product count is to be retrieved.</param>
        /// <returns>The total number of rejected products uploaded by the specified user, or 0 if none found.</returns>
        public int TotalUserRejectedProductsCount(int userId)
        {
            var totalRejectedProductCount = _context.Products.Where(u => u.UserId == userId && u.StatusId == 3).Count();

            if (totalRejectedProductCount != null)
            {
                return totalRejectedProductCount;
            }

            return 0;
        }

        /// <summary>
        /// Gets the total count of products published by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose published product count is to be retrieved.</param>
        /// <returns>The total number of published products uploaded by the specified user, or 0 if none found.</returns>
        public int TotalUserPublishedProductsCount(int userId)
        {
            var totalPublishedProductCount = _context.Products.Where(u => u.UserId == userId && u.IsPublished == true).Count();

            if (totalPublishedProductCount != null)
            {
                return totalPublishedProductCount;
            }

            return 0;
        }

        /// <summary>
        /// Gets the total count of products uploaded by a specific user that are waiting for approval.
        /// </summary>
        /// <param name="userId">The ID of the user whose waiting-for-approval product count is to be retrieved.</param>
        /// <returns>The total number of products uploaded by the user that are waiting for approval, or 0 if none found.</returns>
        public int TotalUserWaitingForApprovalProductsCount(int userId)
        {
            var totalWaitingForApprovalProductCount = _context.Products.Where(u => u.UserId == userId && u.StatusId == 1).Count();

            if (totalWaitingForApprovalProductCount != null)
            {
                return totalWaitingForApprovalProductCount;
            }

            return 0;
        }

        #endregion

        #region User Related

        /// <summary>
        /// Retrieves user details based on the provided user ID and maps the result to a UserDTO.
        /// </summary>
        /// <param name="id">The ID of the user to fetch details for.</param>
        /// <returns>A UserDTO object containing user information, or null if no user is found.</returns>
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

        /// <summary>
        /// Retrieves the email address of a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The email address of the user, or an empty string if not found.</returns>
        private string GetUserEmailByID(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            return user.EmailAddress;
        }

        /// <summary>
        /// Retrieves the full name of a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The full name of the user, or an empty string if not found.</returns>
        private string GetUserNameByID(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            string name = $"{user.FirstName} {user.LastName}";
            return name;
        }

        #endregion

        #endregion

    }
}
