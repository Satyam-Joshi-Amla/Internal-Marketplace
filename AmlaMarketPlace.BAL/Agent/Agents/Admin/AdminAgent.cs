using AmlaMarketPlace.BAL.Agent.IAgents.IAdmin;
using AmlaMarketPlace.DAL.Service.IServices.IAdmin;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Admin;

namespace AmlaMarketPlace.BAL.Agent.Agents.Admin
{
    public class AdminAgent : IAdminAgent
    {
        private readonly IAdminService _adminService;
        public AdminAgent(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public List<UserDTO> GetAllUsers()
        {
            return _adminService.GetAllUsers();
        }
        public List<UserDTO> GetActiveUsers()
        {
            return _adminService.GetActiveUsers();
        }
        public List<UserDTO> GetInactiveUsers()
        {
            return _adminService.GetInactiveUsers();
        }
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
        public List<ProductDTO> GetAllApprovedProducts()
        {
            return _adminService.GetAllApprovedProducts();
        }
        public bool MakeWaitingForApproval(int ProductID)
        {
            return _adminService.MakeWaitingForApproval(ProductID);
        }
        public bool ApproveProduct(int ProductID)
        {
            return _adminService.ApproveProduct(ProductID);
        }
        public bool RejectProduct(int ProductID, string rejectComment)
        {
            return _adminService.RejectProduct(ProductID, rejectComment);
        }
        public List<ProductDTO> ProductsWaitingForApproval()
        {
            return _adminService.ProductsWaitingForApproval();
        }
        public List<ProductDTO> GetRejectedProducts()
        {
            return _adminService.GetRejectedProducts();
        }
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
        public UserDTO GetUserDetail(int UserID)
        {
            return _adminService.GetUserById(UserID);
        }
    }
}
