using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.DAL.Service.IServices.IAdmin;
using AmlaMarketPlace.ConfigurationManager.UtilityMethods;

namespace AmlaMarketPlace.DAL.Service.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly AmlaMarketPlaceDbContext _context;
        public AdminService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }


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
        private string GetStatusValueByStatusId(int statusID)
        {
            var status = _context.Statuses.FirstOrDefault(s => s.StatusId == statusID);
            return status != null ? status.StatusValue : "pending";
        }
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
        public int TotalProductsCount()
        {
            return _context.Products.Count();
        }
        public int PendingProductsCount()
        {
            return _context.Products.Where(p => p.StatusId == 1).Count();
        }
        public int ApprovedProductsCount()
        {
            return _context.Products.Where(p => p.StatusId == 2 && p.IsPublished == false).Count();
        }
        public int RejectedProductsCount()
        {
            return _context.Products.Where(p => p.StatusId == 3).Count();
        }
        public int PublishedProductsCount()
        {
            return _context.Products.Where(p => p.IsPublished == true).Count();
        }
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
    }
}