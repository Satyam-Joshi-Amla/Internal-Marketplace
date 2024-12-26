
namespace AmlaMarketPlace.Models.ViewModels.Admin
{
    public class AdminDashBoardViewModel
    {
        public int TotalProductsCount { get; set; }
        public int WaitingForApprovalProductsCount { get; set; }
        public int ApprovedProductsCount { get; set; }
        public int RejectedProductsCount { get; set; }
        public int PublishedProductsCount { get; set; }
        public int TotalUsersCount { get; set; }
        public int ActiveUsersCount { get; set; }
        public int InactiveUsersCount { get; set; }
    }
}
