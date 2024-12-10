using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.Models.ViewModels.Product;
using AmlaMarketPlace.Models.DTO;
using Microsoft.EntityFrameworkCore;
using AmlaMarketPlace.DAL.Service.Services.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.DAL.Service.Services.Product
{
    public class ProductService
    {
        private readonly AmlaMarketPlaceDbContext _context;
        private readonly AccountService _service;

        // Initialize the DbContext
        public ProductService(AmlaMarketPlaceDbContext context, AccountService service)
        {
            _context = context;
            _service = service;
        }

        public List<ProductListViewModel> GetProducts()
        {
            var result = new List<ProductListViewModel>();
            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open(); // Open the database connection

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "EXEC GetAllProductsWithDefaultImage"; // Name of your SP
                    command.CommandType = System.Data.CommandType.Text; // Can be StoredProcedure or Text

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // Read each row from the result set
                        {
                            result.Add(new ProductListViewModel
                            {
                                ProductId = reader.GetInt32(0),       // Index 0: ProductId
                                Name = reader.GetString(1),   // Index 1: ProductName
                                Price = (float)reader.GetDecimal(2),        // Index 2: Price
                                Description = reader.GetString(3),      // Index 3: ImageLink
                                CreatedOn = reader.GetDateTime(4),
                                ModifiedOn = reader.GetDateTime(5),
                                Inventory = reader.GetInt32(6),
                                StatusId = reader.GetInt32(7),
                                IsPublished = reader.GetBoolean(8),
                                ImageLink = reader.GetString(9)
                            });
                        }
                    }
                }
            }

            return result;
        }

        //public ProductDetailsViewModel GetIndividualProduct(int productId)
        //{
        //    var product = _context.Products.Where(p => p.ProductId == productId)
        //        .Select(p => new ProductDetailsViewModel
        //        {
        //            ProductId = p.ProductId,
        //            Name = p.Name,
        //            Price = p.Price,
        //            Description = p.Description,
        //            CreatedOn = p.CreatedOn,
        //            ModifiedOn = p.ModifiedOn,
        //            Inventory = p.Inventory,
        //            StatusId = p.StatusId,
        //            IsPublished = p.IsPublished
        //        }).FirstOrDefault();

        //    return product;
        //}

        public ProductDetailsViewModel GetProductDetails(int productId)
        {
            var product = _context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDetailsViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CreatedOn = p.CreatedOn,
                    ModifiedOn = p.ModifiedOn,
                    Inventory = p.Inventory,
                    StatusId = p.StatusId,
                    IsPublished = p.IsPublished,
                    Images = p.Images.Select(i => new ImageViewModel
                    {
                        ImagePath = i.Link,
                        IsDefault = (bool)i.IsDefault
                    }).OrderByDescending(i => i.IsDefault).ToList()
                })
                .FirstOrDefault();

            return product;
        }

        public bool AddProduct(AddProductDto Dto)
        {
            var product = new AmlaMarketPlace.DAL.Data.Product();
            product.UserId = Dto.UserId;
            product.Name = Dto.ProductName;
            product.Price = Dto.Price;
            product.Description = Dto.Description;
            product.Inventory = Dto.Inventory;

            _context.Products.Add(product);
            _context.SaveChanges();

            Dto.ProductId = product.ProductId;

            var image = new Image
            {
                ProductId = Dto.ProductId,
                Name = Dto.ImageName,
                Link = Dto.ImagePath,
                IsDefault = true
            };

            if (Dto.OptionalImageNames != null && Dto.OptionalImageNames.Any())
            {
                for (int i=0; i<Dto.OptionalImageNames.Count; i++)
                {
                    var optImage = new Image
                    {
                        ProductId = Dto.ProductId,
                        Name = Dto.OptionalImageNames[i],
                        Link = Dto.OptionalImagePaths[i],
                        IsDefault = false
                    };
                    _context.Images.Add(optImage);
                }
            }
            _context.Images.Add(image);
            _context.SaveChanges();
            return true;
        }

        public bool PlaceOrder(int productId, int buyerId)
        {
            string buyerEmail = _context.Users.FirstOrDefault(u => u.UserId == buyerId).EmailAddress;
            _service.SendMessageOnMail(buyerEmail, "Order Placed", "Seller and buyer are notified successfully");
            return true;
        }

        
    }
}
