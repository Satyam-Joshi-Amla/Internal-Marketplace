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
    }
}
