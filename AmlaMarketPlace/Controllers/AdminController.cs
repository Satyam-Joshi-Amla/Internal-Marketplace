﻿using AmlaMarketPlace.BAL.Agent.Agents.Admin;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmlaMarketPlace.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly AdminAgent _adminAgent;
        public AdminController(AdminAgent adminAgent)
        {
            _adminAgent = adminAgent;
        }

        public IActionResult DashBoard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllUsersList()
        {
            var allUsers = _adminAgent.GetAllUsers();
            return View(allUsers);
        }

        [HttpGet]
        public IActionResult GetAllPublishedProducts()
        {
            var allPublishedProducts = _adminAgent.GetAllPublishedProducts();
            return View(allPublishedProducts);
        }

        //[HttpGet]
        //public IActionResult GetOnlyApprovedProducts()
        //{
        //    var allPublishedProducts = _adminAgent.GetAllPublishedProducts();
        //    return View(allPublishedProducts);
        //}

        [HttpGet]
        public IActionResult ProductsWaitingForApproval()
        {
            var productWaitingForApproval = _adminAgent.ProductsWaitingForApproval();
            return View(productWaitingForApproval);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            bool isApprov = _adminAgent.ApproveProduct(id);
            if (isApprov)
            {
                TempData["ProductApproved"] = "Product Approved.";
            }
            else
            {
                TempData["FailedToApproved"] = "Failed to approve product.";
            }


            return RedirectToAction("ProductsWaitingForApproval", "Admin");
        }

        [HttpPost]
        public IActionResult Reject(int productId, string rejectComment)
        {
            bool isApprov = _adminAgent.RejectProduct(productId, rejectComment);
            if (isApprov)
            {
                TempData["ProductRejected"] = "Product Rejected.";
            }
            else
            {
                TempData["FailedToReject"] = "Failed to reject product.";
            }

            // iss line pe 

            return RedirectToAction("ProductsWaitingForApproval", "Admin");
        }

        [HttpGet]
        public IActionResult GetRejectedProducts()
        {
            var productRejected = _adminAgent.GetRejectedProducts();
            return View(productRejected);
        }
    }
}