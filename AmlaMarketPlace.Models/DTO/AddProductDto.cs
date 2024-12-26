namespace AmlaMarketPlace.Models.DTO
{
    public class AddProductDto
    {
        public int UserId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Inventory { get; set; }
        public int ProductId { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public List<string>? OptionalImageNames { get; set; }
        public List<string>? OptionalImagePaths { get; set; }
    }
}
