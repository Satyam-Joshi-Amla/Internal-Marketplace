using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.ViewModels.Admin
{
    public class AdminDashBoardViewModel
    {
        public int TotalProductsCount { get; set; }
        public int WaitingForApprovalProductsCount { get; set; }
        public int ApprovedProductsCount { get; set; }
        public int RejectedProductsCount { get; set; }
        public int PublishedProductsCount { get; set; }
    }
}
