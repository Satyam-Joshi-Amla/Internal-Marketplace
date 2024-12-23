using AmlaMarketPlace.BAL.Agent.IAgents.IAccount;
using AmlaMarketPlace.BAL.Agent.IAgents.IAdmin;
using AmlaMarketPlace.BAL.Agent.IAgents.IProduct;
using AmlaMarketPlace.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmlaMarketPlace.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IProductAgent _productAgent;
        private readonly IAccountAgent _accountAgent;
        private readonly IAdminAgent _adminAgent;
        public AdminController(IAdminAgent adminAgent, IProductAgent productAgent, IAccountAgent accountAgent)
        {
            _adminAgent = adminAgent;
            _productAgent = productAgent;
            _accountAgent = accountAgent;
        }

        public IActionResult DashBoard()
        {
            var dashBoardNumbers = _adminAgent.GetDashBoardNumbers();
            return View(dashBoardNumbers);
        }

        [HttpGet]
        public IActionResult GetAllUsersList()
        {
            var allUsers = _adminAgent.GetAllUsers();
            return View(allUsers);
        }

        [HttpGet]
        public IActionResult GetActiveUsersList()
        {
            var activeUsers = _adminAgent.GetActiveUsers();
            return View(activeUsers);
        }

        [HttpGet]
        public IActionResult GetInactiveUsersList()
        {
            var inactiveUsers = _adminAgent.GetInactiveUsers();
            return View(inactiveUsers);
        }

        [HttpGet]
        public IActionResult GetAllPublishedProducts()
        {
            var allPublishedProducts = _adminAgent.GetAllPublishedProducts();
            return View(allPublishedProducts);
        }
        
        [HttpGet]
        public IActionResult ProductsWaitingForApproval()
        {
            var productWaitingForApproval = _adminAgent.ProductsWaitingForApproval();
            return View(productWaitingForApproval);
        }

        [HttpGet]
        public IActionResult GetAllApprovedProducts ()
        {
            List<ProductDTO> getAllApprovedProducts = _adminAgent.GetAllApprovedProducts();
            return View(getAllApprovedProducts);
        }

        [HttpGet]
        public IActionResult ProductDetails(int id)
        {
            var productDetails = _productAgent.GetIndividualProduct(id);
            if (productDetails == null)
            {
                return NotFound();
            }

            return View(productDetails);
        }

        [HttpPost]
        public IActionResult PutProductToWaitingForApproval(int id)
        {
            bool isWaitingForApproved = _adminAgent.MakeWaitingForApproval(id);
            if (isWaitingForApproved)
            {
                TempData["WaitingForApprovedSuccess"] = "Product status changes to Pending.";
            }
            else
            {
                TempData["WaitingForApprovedFailed"] = "Failed to change Product status to Pending.";
            }
            return RedirectToAction("GetAllApprovedProducts", "Admin");
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
            bool isRejected = _adminAgent.RejectProduct(productId, rejectComment);
            if (isRejected)
            {
                TempData["ProductRejected"] = "Product Rejected.";
                // Send Mail To Seller with comment.
            }
            else
            {
                TempData["FailedToReject"] = "Failed to reject product.";
            }

            return RedirectToAction("ProductsWaitingForApproval", "Admin");
        }

        [HttpPost]
        public IActionResult Unpublish(int id)
        {
            bool UnpublishedSuccessfully = _productAgent.UnpublishProductSuccessfully(id);

            if (UnpublishedSuccessfully)
            {
                TempData["UnpublishedSuccess"] = "Product is now Unpublished.";
            }
            else
            {
                TempData["unpublishedFailed"] = "Product cannot be unpublished. Please contact admin.";
            }

            // Redirect back to the GetUserUploadedProductsList with the same user id
            int userId = int.Parse(User.FindFirst("UserId")?.Value); // Extract the logged-in user's id
            return RedirectToAction("GetAllPublishedProducts", new { id = userId });
        }

        [HttpGet]
        public IActionResult GetRejectedProducts()
        {
            var productRejected = _adminAgent.GetRejectedProducts();
            return View(productRejected);
        }

        public IActionResult GetUserDetails(int id)
        {
            UserDTO userDetail = _adminAgent.GetUserDetail(id);
            return View(userDetail);
        }

        public IActionResult ResendEmailVerificationLink(string email)
        {
            bool isSent = _accountAgent.SendEmailVerificationLink(email);
            if (isSent)
            {
                TempData["EmailVerificationLinkSentSuccessfully"] = "Verification Link is sent successfully.";
            }
            else
            {
                TempData["EmailVerificationLinkFailedToSend"] = "Failed to send Verification Link. Please contact us.";
            }

            return RedirectToAction("GetInactiveUsersList", "Admin");
        }
    }
}