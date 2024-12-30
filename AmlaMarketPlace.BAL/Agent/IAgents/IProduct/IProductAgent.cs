using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Product;

namespace AmlaMarketPlace.BAL.Agent.IAgents.IProduct
{
    public interface IProductAgent
    {
        PaginatedResultDto GetProducts(int pageNumber, int pageSize, int userId); // Retrieves paginated products for a specific user
        PaginatedResultDto SearchProducts(string searchTerm, int userId, int pageNumber = 1, int pageSize = 20);
        List<ProductDTO> GetUserUploadedProducts(int userID); // Retrieves the list of products uploaded by a user
        ProductDetailsViewModel GetIndividualProduct(int productId); // Retrieves details of a specific product
        bool AddProduct(AddProductViewModel model); // Adds a new product
        string GetRelativeImagePath(string fullPath); // Gets relative path from a given full image path
        bool PlaceOrder(int productId, int buyerId, int orderQuantity); // Places an order for a product
        bool PublishProductSuccessfully(int productID); // Publishes a product successfully
        bool UnpublishProductSuccessfully(int productID); // Unpublishes a product successfully
        EditProductViewModel GetEditDetails(int id); // Retrieves details for editing a product
        bool EditProduct(EditProductViewModel model); // Edits a product's details
        List<OrderDTO> GetOrderHistory(int userId); // Retrieves order history for a user
        bool ChangeStatusTOPending(int productId); // Changes product status to pending
        bool ChangeStatusTOApproved(int productId); // Changes product status to approved
        bool ChangeStatusTORejected(int productId); // Changes product status to rejected
        List<MyOrdersDto> GetMyRequests(int userId);
        bool UpdateOrder(int orderId, int orderStatus, string rejectComment);
        SellerDashBoardViewModel GetSellerDashBoardData(int userId); // Retrieves seller dashboard data
    }
}
