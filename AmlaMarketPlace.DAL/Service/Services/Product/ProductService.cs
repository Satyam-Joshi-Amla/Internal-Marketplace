﻿using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.Models.ViewModels.Product;
using AmlaMarketPlace.Models.DTO;
using Microsoft.EntityFrameworkCore;

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

        // Initialize the DbContext
        public ProductService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }

        public List<ProductListViewModel> GetProducts()
        {
            var result = new List<ProductListViewModel>();
            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open(); // Open the database connection

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "EXEC GetAllProductsWithImage"; // Name of your SP
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
                Link = Dto.ImagePath
            };
            _context.Images.Add(image);
            _context.SaveChanges();
            return true;
        }
    }
}
