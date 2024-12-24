using AmlaMarketPlace.ConfigurationManager.UtilityMethods;
using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.DAL.Service.IServices.IAdmin;
using AmlaMarketPlace.Models.DTO;

namespace AmlaMarketPlace.DAL.Service.Services.Admin
{
    public class AdminService : IAdminService
    {
        #region Dependency Injection : Database Context
        private readonly AmlaMarketPlaceDbContext _context;
        #endregion

        #region Constructor
        public AdminService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods

        #region User Services

        /// <summary>
        /// Fetches role of the user by user's role id
        /// </summary>
        /// <param name="userRoleId">Role id of the user</param>
        /// <returns>Returns "user" if user else returns admin</returns>
        public string GetUserRoleById(int userRoleId)
        {
            var userRole = _context.UserRoles.FirstOrDefault(r => r.RoleId == userRoleId);

            return userRole != null ? userRole.Role : "user";
        }


        /// <summary>
        /// Fetches list of all users
        /// </summary>
        /// <returns>Returns List of UserDTO type user details</returns>
        public List<UserDTO> GetAllUsers()
        {
            var users = _context.Users.ToList();
            var userDTOs = users.Select(user => new UserDTO
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
                UserRole = GetUserRoleById(user.UserRoleId), // Retrieve the user role
                CreatedOn = user.CreatedOn,
                EditedOn = user.EditedOn,
                VerificationToken = user.VerificationToken,
                TokenExpiration = user.TokenExpiration
            }).ToList();

            return userDTOs;
        }


        /// <summary>
        /// Fetches list of all users whose email id is verified
        /// </summary>
        /// <returns>Returns list of UserDTO type user details</returns>
        public List<UserDTO> GetActiveUsers()
        {
            var users = _context.Users.Where(e => e.IsEmailVerified == true).ToList();
            var userDTOs = users.Select(user => new UserDTO
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
                UserRole = GetUserRoleById(user.UserRoleId), // Retrieve the user role
                CreatedOn = user.CreatedOn,
                EditedOn = user.EditedOn,
                VerificationToken = user.VerificationToken,
                TokenExpiration = user.TokenExpiration
            }).ToList();

            return userDTOs;
        }


        /// <summary>
        /// Fetches list of users whose email id is unverified
        /// </summary>
        /// <returns>Returns list of UserDTO type user details</returns>
        public List<UserDTO> GetInactiveUsers()
        {
            var users = _context.Users.Where(e => e.IsEmailVerified == false).ToList();
            var userDTOs = users.Select(user => new UserDTO
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
                UserRole = GetUserRoleById(user.UserRoleId), // Retrieve the user role
                CreatedOn = user.CreatedOn,
                EditedOn = user.EditedOn,
                VerificationToken = user.VerificationToken,
                TokenExpiration = user.TokenExpiration
            }).ToList();

            return userDTOs;
        }

        /// <summary>
        /// Fetches user complete data in UserDTO Model
        /// </summary>
        /// <param name="id">Requires user id</param>
        /// <returns>Returns UserDTO Model</returns>
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

        #endregion

        #region Product Services

        /// <summary>
        /// Fetches list of all published products
        /// </summary>
        /// <returns>Returns list of ProductDTO type product details</returns>
        public List<ProductDTO> GetAllPublishedProducts()
        {
            var products = _context.Products
                .Where(product => product.IsPublished)
                .ToList();
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


        /// <summary>
        /// Fetches list of products whose approval status is pending
        /// </summary>
        /// <returns>Returns list of ProductDTO type product details</returns>
        public List<ProductDTO> ProductsWaitingForApproval()
        {
            var products = _context.Products
                .Where(product => product.StatusId == 1)
                .ToList();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ProductDTO> GetAllApprovedProducts()
        {
            // Fetching only approved products from the database
            var products = _context.Products
                .Where(product => product.StatusId == 2 && product.IsPublished == false)
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

        /// <summary>
        /// Retrieves the status value based on the provided status ID.
        /// </summary>
        /// <param name="statusID">The ID of the status to fetch.</param>
        /// <returns>The status value corresponding to the ID, or "pending" if no match is found.</returns>
        private string GetStatusValueByStatusId(int statusID)
        {
            var status = _context.Statuses.FirstOrDefault(s => s.StatusId == statusID);
            return status != null ? status.StatusValue : "pending";
        }

        /// <summary>
        /// Sets the status of a product to "waiting for approval" (Status ID = 1).
        /// </summary>
        /// <param name="productID">The ID of the product to update.</param>
        /// <returns>Returns true if the product status is updated successfully; otherwise, false if the product is not found.</returns>
        public bool MakeWaitingForApproval(int productID)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productID);

            //Status ID = 1 means Pending
            //Status ID = 2 means Approved
            //Status ID = 3 means Rejected

            if (product != null)
            {
                product.StatusId = 1;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Approves a product by setting its status to "approved" (Status ID = 2).
        /// </summary>
        /// <param name="productID">The ID of the product to approve.</param>
        /// <returns>Returns true if the product status is updated successfully; otherwise, false if the product is not found.</returns>
        public bool ApproveProduct(int productID)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productID);

            //Status ID = 1 means Pending
            //Status ID = 2 means Approved
            //Status ID = 3 means Rejected

            if (product != null)
            {
                product.StatusId = 2;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Rejects a product by setting its status to "rejected" (Status ID = 3) and adds a rejection comment.
        /// Sends an email notification to the seller with the rejection details.
        /// </summary>
        /// <param name="productId">The ID of the product to reject.</param>
        /// <param name="rejectComment">The reason for rejecting the product.</param>
        /// <returns>Returns true if the product is rejected and email notification is sent; otherwise, false if the product is not found.</returns>
        public bool RejectProduct(int productId, string rejectComment)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);

            //Status ID = 1 means Pending
            //Status ID = 2 means Approved
            //Status ID = 3 means Rejected

            if (product != null)
            {
                var commentEntry = new AmlaMarketPlace.DAL.Data.ProductComment();
                commentEntry.ProductId = productId;
                commentEntry.RejectedComments = rejectComment;
                _context.ProductComments.Add(commentEntry);

                product.StatusId = 3;
                _context.SaveChanges();

                // Send Product Rejected Mail to Seller with Comment.                
                string subject = "Product Rejected";
                var user = GetUserById(product.UserId);
                string sellerEmail = user.EmailAddress;
                string mailMessage = $@"
Hi {user.FirstName} {user.LastName},    
We regret to inform you that your product has been rejected.

Here are the Product details:
Name: {product.Name}
Price: {product.Price}

Reason of rejection: 
{rejectComment}

Please edit the product according to the comment and resend for approval.
    
Please contact support if you have any questions.
Best Regards
Amla Marketplace";

                MailUtility.SendMessageOnMail(sellerEmail, subject, mailMessage);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves a list of rejected products along with their rejection comments.
        /// </summary>
        /// <returns>A list of ProductDTO objects representing rejected products.</returns>
        public List<ProductDTO> GetRejectedProducts()
        {
            // Fetching only rejected products from the database
            var products = _context.Products
                .Where(product => product.StatusId == 3) // Filtering published products
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
                CommentForRejecting = _context.ProductComments
                .Where(comment => comment.ProductId == product.ProductId)
                .Select(comment => comment.RejectedComments)
                .FirstOrDefault()
            }).ToList();

            return productDTO;
        }

        #endregion

        #region Admin Dashboard Count Services

        /// <summary>
        /// Returns the total count of products in the database.
        /// </summary>
        /// <returns>The total number of products.</returns>
        public int TotalProductsCount()
        {
            return _context.Products.Count();
        }

        /// <summary>
        /// Returns the count of products that are pending approval.
        /// </summary>
        /// <returns>The number of pending products.</returns>
        public int PendingProductsCount()
        {
            return _context.Products.Where(p => p.StatusId == 1).Count();
        }

        /// <summary>
        /// Returns the count of products that are approved but not yet published.
        /// </summary>
        /// <returns>The number of approved but unpublished products.</returns>
        public int ApprovedProductsCount()
        {
            return _context.Products.Where(p => p.StatusId == 2 && p.IsPublished == false).Count();
        }

        /// <summary>
        /// Returns the count of products that have been rejected.
        /// </summary>
        /// <returns>The number of rejected products.</returns>
        public int RejectedProductsCount()
        {
            return _context.Products.Where(p => p.StatusId == 3).Count();
        }

        /// <summary>
        /// Returns the count of products that have been published.
        /// </summary>
        /// <returns>The number of published products.</returns>
        public int PublishedProductsCount()
        {
            return _context.Products.Where(p => p.IsPublished == true).Count();
        }

        /// <summary>
        /// Retrieves the total number of users.
        /// </summary>
        /// <returns>The count of users.</returns>
        /// <exception cref="Exception">Thrown when there is an error retrieving the user count from the database.</exception>
        public int TotalUsersCount()
        {
            try
            {
                return _context.Users.Count();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the total number of users.", ex);
            }
        }

        /// <summary>
        /// Retrieves the count of active users who have verified their email.
        /// </summary>
        /// <returns>The count of active users.</returns>
        /// <exception cref="Exception">Thrown when there is an error retrieving the active user count from the database.</exception>
        public int ActiveUserCount()
        {
            try
            {
                return _context.Users.Where(u => u.IsEmailVerified == true).Count();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the total number of users.", ex);
            }
        }

        /// <summary>
        /// Retrieves the count of inactive users who have not verified their email.
        /// </summary>
        /// <returns>The count of inactive users.</returns>
        /// <exception cref="Exception">Thrown when there is an error retrieving the inactive user count from the database.</exception>
        public int InactiveUserCount()
        {
            try
            {
                return _context.Users.Where(u => u.IsEmailVerified == false).Count();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the total number of users.", ex);
            }
        }

        #endregion

        #endregion
    }
}
