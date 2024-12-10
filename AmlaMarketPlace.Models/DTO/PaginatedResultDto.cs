using AmlaMarketPlace.Models.ViewModels.Product;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.DTO
{
    public class PaginatedResultDto
    {
        public List<ProductListViewModel> Products { get; set; }
        public int TotalCount { get; set; }
    }
}
