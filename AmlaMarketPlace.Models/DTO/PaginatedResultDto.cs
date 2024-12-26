using AmlaMarketPlace.Models.ViewModels.Product;

namespace AmlaMarketPlace.Models.DTO
{
    public class PaginatedResultDto
    {
        public List<ProductListViewModel> Products { get; set; }
        public int TotalCount { get; set; }
    }
}
