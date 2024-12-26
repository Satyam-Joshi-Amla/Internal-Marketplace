namespace AmlaMarketPlace.Models.ViewModels.Product
{
    public class SellerDashBoardViewModel
    {
        public int SellerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalProductsCount { get; set; }
        public int RejectedProductsCount { get; set; }
        public int ApprovedProductsCount { get; set; }
        public int PublishedProductsCount { get; set; }
        public int WaitingForApprovalProductsCount { get; set; }

    }
}
