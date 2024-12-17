using AmlaMarketPlace.DAL.Service.Services.Admin;
using AmlaMarketPlace.Models.DTO;

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
        public AdminAgent(AdminService adminService)
        {
            _adminService = adminService;
        }
        public List<UserDTO> GetAllUsers()
        {
            return _adminService.GetAllUsers();
        }
        public List<ProductDTO> GetAllPublishedProducts()
        {
            return _adminService.GetAllPublishedProducts();
        }
        public List<ProductDTO> GetAllApprovedProducts()
        {
            return _adminService.GetAllApprovedProducts();
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
    }
}
