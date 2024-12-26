using AmlaMarketPlace.BAL.Agent.IAgents.IAdmin;
using AmlaMarketPlace.DAL.Service.IServices.IAdmin;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Admin;

namespace AmlaMarketPlace.BAL.Agent.Agents.Admin
{
    public class AdminAgent : IAdminAgent
    {
        #region Dependency Injection : Service Fields

        private readonly IAdminService _adminService;

        #endregion

        #region Constructor
        public AdminAgent(IAdminService adminService)
        {
            _adminService = adminService;
        }
        #endregion

        #region Methods

        /// <summary>  
        /// Retrieves a list of all users.  
        /// </summary>  
        /// <returns>Returns a list of `UserDTO` objects containing user information.</returns>  
        public List<UserDTO> GetAllUsers()
        {
            return _adminService.GetAllUsers();
        }

        /// <summary>  
        /// Retrieves a list of users who have verified their email addresses.  
        /// </summary>  
        /// <returns>Returns a list of `UserDTO` objects representing active users (with verified emails).</returns>  
        public List<UserDTO> GetActiveUsers()
        {
            return _adminService.GetActiveUsers();
        }

        /// <summary>  
        /// Retrieves a list of users who have not verified their email addresses.  
        /// </summary>  
        /// <returns>Returns a list of `UserDTO` objects representing inactive users.</returns>  
        public List<UserDTO> GetInactiveUsers()
        {
            return _adminService.GetInactiveUsers();
        }

        /// <summary>  
        /// Retrieves a list of published products along with their associated user details.  
        /// </summary>  
        /// <returns>Returns a list of `PublishedProductsViewModel` objects containing product details and user names.</returns>  
        public List<PublishedProductsViewModel> GetAllPublishedProducts()
        {
            List<ProductDTO> listOfPublishedProducts = _adminService.GetAllPublishedProducts();

            List<PublishedProductsViewModel> listOfPublishedProductsViewModel = listOfPublishedProducts
            .Select(product =>
            {
                var userDetail = GetUserDetail(product.UserId);
                return new PublishedProductsViewModel
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
                    StatusValue = product.StatusValue,
                    IsPublished = product.IsPublished,
                    CommentForRejecting = product.CommentForRejecting,
                    UserName = $"{userDetail.FirstName} {userDetail.LastName}",
                };
            }).ToList();

            return listOfPublishedProductsViewModel;

        }

        /// <summary>  
        /// Retrieves a list of approved products.  
        /// </summary>  
        /// <returns>Returns a list of `ProductDTO` objects representing approved products.</returns>  
        public List<ProductDTO> GetAllApprovedProducts()
        {
            return _adminService.GetAllApprovedProducts();
        }

        /// <summary>  
        /// Marks a product as waiting for approval based on the provided product ID.  
        /// </summary>  
        /// <param name="ProductID">The ID of the product to update.</param>  
        /// <returns>Returns `true` if the operation is successful, otherwise `false`.</returns>  
        public bool MakeWaitingForApproval(int ProductID)
        {
            return _adminService.MakeWaitingForApproval(ProductID);
        }

        /// <summary>  
        /// Approves a product based on the provided product ID.  
        /// </summary>  
        /// <param name="ProductID">The ID of the product to approve.</param>  
        /// <returns>Returns `true` if the product is approved successfully, otherwise `false`.</returns>  
        public bool ApproveProduct(int ProductID)
        {
            return _adminService.ApproveProduct(ProductID);
        }

        /// <summary>  
        /// Rejects a product based on the provided product ID and reason.  
        /// </summary>  
        /// <param name="ProductID">The ID of the product to reject.</param>  
        /// <param name="rejectComment">The reason for rejecting the product.</param>  
        /// <returns>Returns `true` if the product is rejected successfully, otherwise `false`.</returns>  
        public bool RejectProduct(int ProductID, string rejectComment)
        {
            return _adminService.RejectProduct(ProductID, rejectComment);
        }

        /// <summary>
        /// Retrieves a list of products that are waiting for approval.
        /// It calls the _adminService to fetch the products that are currently in the approval queue.
        /// </summary>
        /// <returns> A list of ProductDTO objects representing the products awaiting approval.   </returns>
        public List<ProductDTO> ProductsWaitingForApproval()
        {
            return _adminService.ProductsWaitingForApproval();
        }

        /// <summary>
        /// Retrieves a list of products that have been rejected. It calls the _adminService to fetch the rejected products.
        /// </summary>
        /// <returns>
        /// A list of ProductDTO objects representing the rejected products.
        /// </returns>
        public List<ProductDTO> GetRejectedProducts()
        {
            return _adminService.GetRejectedProducts();
        }

        /// <summary>
        /// Retrieves various dashboard statistics including product and user counts. It calls multiple methods from _adminService to fetch totals for products (approved, pending, rejected, published) and users (active, inactive, total).
        /// </summary>
        /// <returns>
        /// An AdminDashBoardViewModel containing all the relevant counts for products and users.
        /// </returns>
        public AdminDashBoardViewModel GetDashBoardNumbers()
        {
            AdminDashBoardViewModel adminDashBoardViewModel = new AdminDashBoardViewModel()
            {
                TotalProductsCount = _adminService.TotalProductsCount(),
                WaitingForApprovalProductsCount = _adminService.PendingProductsCount(),
                ApprovedProductsCount = _adminService.ApprovedProductsCount(),
                RejectedProductsCount = _adminService.RejectedProductsCount(),
                PublishedProductsCount = _adminService.PublishedProductsCount(),
                TotalUsersCount = _adminService.TotalUsersCount(),
                ActiveUsersCount = _adminService.ActiveUserCount(),
                InactiveUsersCount = _adminService.InactiveUserCount()
            };

            return adminDashBoardViewModel;
        }

        /// <summary>
        /// Retrieves details of a user based on the provided UserID. It calls _adminService to fetch the user information.
        /// </summary>
        /// <param name="UserID">The ID of the user whose details are to be fetched.</param>
        /// <returns>
        /// A UserDTO object containing the details of the specified user.
        /// </returns>
        public UserDTO GetUserDetail(int UserID)
        {
            return _adminService.GetUserById(UserID);
        }


        #endregion
    }
}
