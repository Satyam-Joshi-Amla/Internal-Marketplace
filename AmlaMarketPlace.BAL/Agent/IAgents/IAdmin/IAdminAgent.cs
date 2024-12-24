using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Admin;
using System.Collections.Generic;

namespace AmlaMarketPlace.BAL.Agent.IAgents.IAdmin
{
    public interface IAdminAgent
    {
        List<UserDTO> GetAllUsers(); // Retrieves all users
        List<UserDTO> GetActiveUsers(); // Retrieves active users
        List<UserDTO> GetInactiveUsers(); // Retrieves inactive users
        List<PublishedProductsViewModel> GetAllPublishedProducts(); // Retrieves all published products
        List<ProductDTO> GetAllApprovedProducts(); // Retrieves approved products
        bool MakeWaitingForApproval(int ProductID); // Marks a product as waiting for approval
        bool ApproveProduct(int ProductID); // Approves a product
        bool RejectProduct(int ProductID, string rejectComment); // Rejects a product with a comment
        List<ProductDTO> ProductsWaitingForApproval(); // Retrieves products waiting for approval
        List<ProductDTO> GetRejectedProducts(); // Retrieves rejected products
        AdminDashBoardViewModel GetDashBoardNumbers(); // Retrieves dashboard statistics
        UserDTO GetUserDetail(int UserID); // Retrieves user details by ID
    }
}
