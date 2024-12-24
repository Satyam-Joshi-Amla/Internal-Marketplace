using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Product;

namespace AmlaMarketPlace.DAL.Service.IServices.IProduct
{
    public interface IProductService
    {
        PaginatedResultDto GetProducts(int userId, int pageNumber = 1, int pageSize = 20);
        string GetStatusValueByStatusId(int statusID);
        List<ProductDTO> GetUserUploadedProducts(int userID);
        ProductDetailsViewModel GetProductDetails(int productId);
        bool AddProduct(AddProductDto Dto);
        bool PlaceOrder(int productId, int buyerId, int orderQuantity);
        bool PublishProductSuccessfully(int productID);
        bool UnpublishProductSuccessfully(int productID);
        EditProductViewModel GetEditDetails(int id);
        bool EditProduct(EditProductViewModel model);
        string GetRelativeImagePath(string fullPath);
        List<OrderDTO> GetOrderHistory(int userId);
        bool ChangeStatusTO(int statusID, int productID);
        int? GetInventory(int productId);
        bool UpdateInventory(int productId, int updatedInventory);
        List<MyOrdersDto> GetMyRequests(int userId);
        bool UpdateOrder(int orderId, int orderStatus, string rejectComment);
        int TotalUserUploadedProductsCount(int userId);
        int TotalUserApprovedProductsCount(int userId);
        int TotalUserRejectedProductsCount(int userId);
        int TotalUserPublishedProductsCount(int userId);
        int TotalUserWaitingForApprovalProductsCount(int userId);
        UserDTO GetUserById(int id);
    }
}
