using AmlaMarketPlace.BAL.Agent.IAgents.IProduct;
using AmlaMarketPlace.DAL.Service.IServices.IProduct;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Product;

namespace AmlaMarketPlace.BAL.Agent.Agents.Product
{
    public class ProductAgent : IProductAgent
    {
        #region Dependency Injection : Service Fields

        private readonly IProductService _productService;

        #endregion

        #region Constructor
        public ProductAgent(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a paginated list of products for a specific user. It calls _productService to fetch the products based on the userId, pageNumber, and pageSize.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of products per page.</param>
        /// <param name="userId">The ID of the user for whom the products are being fetched.</param>
        /// <returns>
        /// A PaginatedResultDto containing the paginated list of products for the specified user.
        /// </returns>
        public PaginatedResultDto GetProducts(int pageNumber, int pageSize, int userId)
        {
            return _productService.GetProducts(userId, pageNumber, pageSize);
        }

        /// <summary>
        /// Retrieves a list of products uploaded by a specific user. It calls _productService to fetch the products based on the userID.
        /// </summary>
        /// <param name="userID">The ID of the user whose uploaded products are to be fetched.</param>
        /// <returns>
        /// A list of ProductDTO objects representing the products uploaded by the specified user.
        /// </returns>
        public List<ProductDTO> GetUserUploadedProducts(int userID)
        {
            return _productService.GetUserUploadedProducts(userID);
        }

        /// <summary>
        /// Retrieves the details of an individual product based on the provided productId. It calls _productService to fetch the product details.
        /// If an error occurs, it throws a new exception with a custom error message.
        /// </summary>
        /// <param name="productId">The ID of the product whose details are to be fetched.</param>
        /// <returns>
        /// A ProductDetailsViewModel containing the details of the specified product.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error occurs while fetching the product details.
        /// </exception>
        public ProductDetailsViewModel GetIndividualProduct(int productId)
        {
            try
            {
                return _productService.GetProductDetails(productId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching product details. Please try again later. from agent");
            }
        }

        /// <summary>
        /// Adds a new product along with its main and optional images. It saves the images to the "wwwroot/images/ProductImages" directory, generates a unique file name for each image, and then stores the product details in the database.
        /// </summary>
        /// <param name="model">The model containing the product details and images to be added.</param>
        /// <returns>
        /// A boolean indicating whether the product was successfully added. Always returns true, but may suppress errors in case of failure.
        /// </returns>
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

        /// <summary>
        /// Converts a full file path to a relative web path by extracting the portion after "wwwroot" and replacing backslashes with forward slashes for web compatibility.
        /// </summary>
        /// <param name="fullPath">The full file path to the image.</param>
        /// <returns>
        /// A string representing the relative path of the image, or an empty string if "wwwroot" is not found in the full path.
        /// </returns>
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
        /// Places an order for a specific product on behalf of the buyer. It calls the _productService to process the order with the given productId, buyerId, and orderQuantity.
        /// </summary>
        /// <param name="productId">The ID of the product being ordered.</param>
        /// <param name="buyerId">The ID of the buyer placing the order.</param>
        /// <param name="orderQuantity">The quantity of the product being ordered.</param>
        /// <returns>
        /// A boolean indicating that the order was successfully placed. Always returns true.
        /// </returns>
        public bool PlaceOrder(int productId, int buyerId, int orderQuantity)
        {
            _productService.PlaceOrder(productId, buyerId, orderQuantity);
            return true;
        }

        /// <summary>
        /// Publishes a product successfully based on the provided productID. It calls the _productService to update the product's status to published.
        /// </summary>
        /// <param name="productID">The ID of the product to be published.</param>
        /// <returns>
        /// A boolean indicating whether the product was successfully published.
        /// </returns>
        public bool PublishProductSuccessfully(int productID)
        {
            return _productService.PublishProductSuccessfully(productID);
        }

        /// <summary>
        /// Unpublishes a product based on the provided productID. It calls the _productService to update the product's status to unpublished.
        /// </summary>
        /// <param name="productID">The ID of the product to be unpublished.</param>
        /// <returns>
        /// A boolean indicating whether the product was successfully unpublished.
        /// </returns>
        public bool UnpublishProductSuccessfully(int productID)
        {
            return _productService.UnpublishProductSuccessfully(productID);
        }

        /// <summary>
        /// Retrieves the details of a product for editing based on the provided product ID. It calls the _productService to fetch the product's editable information.
        /// </summary>
        /// <param name="id">The ID of the product whose details are to be fetched for editing.</param>
        /// <returns>
        /// An EditProductViewModel containing the details of the product to be edited.
        /// </returns>
        public EditProductViewModel GetEditDetails(int id)
        {
            return _productService.GetEditDetails(id);
        }

        /// <summary>
        /// Edits an existing product using the provided details. It calls the _productService to update the product information based on the given EditProductViewModel.
        /// </summary>
        /// <param name="model">The model containing the updated product details.</param>
        /// <returns>
        /// A boolean indicating whether the product was successfully edited.
        /// </returns>
        public bool EditProduct(EditProductViewModel model)
        {
            return _productService.EditProduct(model);
        }

        /// <summary>
        /// Retrieves the order history for a specific user. It calls the _productService to fetch all orders associated with the given userId.
        /// </summary>
        /// <param name="userId">The ID of the user whose order history is to be fetched.</param>
        /// <returns>
        /// A list of OrderDTO objects representing the user's past orders.
        /// </returns>
        public List<OrderDTO> GetOrderHistory(int userId)
        {
            return _productService.GetOrderHistory(userId);
        }

        /// <summary>
        /// Changes the status of a product to "Pending" based on the provided productId. It calls the _productService to update the product's status to "Pending" (represented by status code 1).
        /// </summary>
        /// <param name="productId">The ID of the product whose status is to be changed to pending.</param>
        /// <returns>
        /// A boolean indicating whether the product's status was successfully changed to pending.
        /// </returns>
        public bool ChangeStatusTOPending(int productId)
        {
            return _productService.ChangeStatusTO(1, productId);
        }

        /// <summary>
        /// Changes the status of a product to "Approved" based on the provided productId. It calls the _productService to update the product's status to "Approved" (represented by status code 2).
        /// </summary>
        /// <param name="productId">The ID of the product whose status is to be changed to approved.</param>
        /// <returns>
        /// A boolean indicating whether the product's status was successfully changed to approved.
        /// </returns>
        public bool ChangeStatusTOApproved(int productId)
        {
            return _productService.ChangeStatusTO(2, productId);
        }

        /// <summary>
        /// Changes the status of a product to "Rejected" based on the provided productId. It calls the _productService to update the product's status to "Rejected" (represented by status code 3).
        /// </summary>
        /// <param name="productId">The ID of the product whose status is to be changed to rejected.</param>
        /// <returns>
        /// A boolean indicating whether the product's status was successfully changed to rejected.
        /// </returns>
        public bool ChangeStatusTORejected(int productId)
        {
            return _productService.ChangeStatusTO(3, productId);
        }

        /// <summary>
        /// Retrieves a list of requests made by a specific user. It calls the _productService to fetch all requests associated with the given userId.
        /// </summary>
        /// <param name="userId">The ID of the user whose requests are to be fetched.</param>
        /// <returns>
        /// A list of MyOrdersDto objects representing the user's requests.
        /// </returns>
        public List<MyOrdersDto> GetMyRequests(int userId)
        {
            return _productService.GetMyRequests(userId);
        }

        /// <summary>
        /// Updates the status of an order based on the provided orderId, orderStatus, and an optional reject comment. It calls the _productService to update the order details.
        /// </summary>
        /// <param name="orderId">The ID of the order to be updated.</param>
        /// <param name="orderStatus">The new status of the order (e.g., approved, rejected, etc.).</param>
        /// <param name="rejectComment">An optional comment to be provided if the order is rejected.</param>
        /// <returns>
        /// A boolean indicating whether the order was successfully updated. Always returns true.
        /// </returns>
        public bool UpdateOrder(int orderId, int orderStatus, string rejectComment)
        {
            _productService.UpdateOrder(orderId, orderStatus, rejectComment);
            return true;
        }

        /// <summary>
        /// Retrieves the dashboard data for a seller based on the provided userId. It fetches the seller's personal details and product statistics such as total products, approved, rejected, published, and those waiting for approval.
        /// </summary>
        /// <param name="userId">The ID of the seller whose dashboard data is to be fetched.</param>
        /// <returns>
        /// A SellerDashBoardViewModel containing the seller's personal information and product statistics.
        /// </returns>
        public SellerDashBoardViewModel GetSellerDashBoardData(int userId)
        {
            UserDTO user = _productService.GetUserById(userId);

            SellerDashBoardViewModel sellerDashBoardViewModel = new SellerDashBoardViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.EmailAddress,
                PhoneNumber = user.MobileNumber,
                TotalProductsCount = _productService.TotalUserUploadedProductsCount(userId),
                RejectedProductsCount = _productService.TotalUserRejectedProductsCount(userId),
                ApprovedProductsCount = _productService.TotalUserApprovedProductsCount(userId),
                PublishedProductsCount = _productService.TotalUserPublishedProductsCount(userId),
                WaitingForApprovalProductsCount = _productService.TotalUserWaitingForApprovalProductsCount(userId),
            };

            return sellerDashBoardViewModel;
        }

        #endregion
    }
}
