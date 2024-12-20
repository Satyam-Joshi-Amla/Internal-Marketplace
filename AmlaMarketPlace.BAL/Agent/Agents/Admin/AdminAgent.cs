using AmlaMarketPlace.DAL.Service.Services.Admin;
using AmlaMarketPlace.DAL.Service.Services.Profile;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.BAL.Agent.Agents.Admin
{
    public class AdminAgent
    {
        private readonly AdminService _adminService;
        private readonly ProfileService _profileService;
        public AdminAgent(AdminService adminService, ProfileService profileService)
        {
            _adminService = adminService;
            _profileService = profileService;
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
        public List<ProductDTO> GetAllPublishedProducts()
        {
            return _adminService.GetAllPublishedProducts();
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
            };

            return adminDashBoardViewModel;
        }
        public UserDTO GetUserDetail(int UserID)
        {
            return _profileService.GetUser(UserID);
        }
    }
}
