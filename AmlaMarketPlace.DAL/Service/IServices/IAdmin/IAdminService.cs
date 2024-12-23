using AmlaMarketPlace.Models.DTO;

namespace AmlaMarketPlace.DAL.Service.IServices.IAdmin
{
    public interface IAdminService
    {
        string GetUserRoleById(int userRoleId);
        List<UserDTO> GetAllUsers();
        List<UserDTO> GetActiveUsers();
        List<UserDTO> GetInactiveUsers();
        List<ProductDTO> GetAllPublishedProducts();
        List<ProductDTO> ProductsWaitingForApproval();
        List<ProductDTO> GetAllApprovedProducts();
        List<ProductDTO> GetRejectedProducts();
        bool MakeWaitingForApproval(int productID);
        bool ApproveProduct(int productID);
        UserDTO GetUserById(int id);
        bool RejectProduct(int productId, string rejectComment);
        int TotalProductsCount();
        int PendingProductsCount();
        int ApprovedProductsCount();
        int RejectedProductsCount();
        int PublishedProductsCount();
        int TotalUsersCount();
        int ActiveUserCount();
        int InactiveUserCount();
    }
}
