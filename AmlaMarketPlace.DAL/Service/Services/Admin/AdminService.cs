﻿using System;
using System.Collections.Generic;
using System.Linq;

using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.Models.DTO;

namespace AmlaMarketPlace.DAL.Service.Services.Admin
{
    public class AdminService
    {
        private readonly AmlaMarketPlaceDbContext _context;

        public AdminService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }

        // Utility method to retrieve the user role by ID
        private string GetUserRoleById(int userRoleId)
        {
            //var userRole = _context.UserRoles.FirstOrDefault(r => r.RoleId == userRoleId);
            //return userRole != null ? userRole.Role : "user"; // Default to "user" if role not found
            return "user";
        }

        public List<UserDTO> GetAllUsers()
        {
            // Fetching all users from the database
            var users = _context.Users.ToList();

            // Mapping the database user records to UserDTO
            var userDTOs = users.Select(user => new UserDTO
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                IsEmailVerified = user.IsEmailVerified,
                Password = user.Password,
                MobileNumber = user.MobileNumber,
                IsmobileNumberVerified = user.IsmobileNumberVerified,
                UserRoleId = user.UserRoleId,
                UserRole = GetUserRoleById(user.UserRoleId), // Retrieve the user role
                CreatedOn = user.CreatedOn,
                EditedOn = user.EditedOn,
                VerificationToken = user.VerificationToken,
                TokenExpiration = user.TokenExpiration
            }).ToList();

            return userDTOs;
        }

        public List<ProductDTO> GetAllPublishedProducts()
        {
            // Fetching only published products from the database
            var products = _context.Products
                .Where(product => product.IsPublished) // Filtering published products
                .ToList();

            // Mapping the filtered products to ProductDTO
            var productDTO = products.Select(product => new ProductDTO
            {
                ProductId = product.ProductId,
                UserId = product.UserId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CreatedOn = product.CreatedOn,
                ModifiedOn = product.ModifiedOn,
                Inventory = product.Inventory,
                StatusId = product.StatusId,
                IsPublished = product.IsPublished
            }).ToList();

            return productDTO;
        }


    }
}



