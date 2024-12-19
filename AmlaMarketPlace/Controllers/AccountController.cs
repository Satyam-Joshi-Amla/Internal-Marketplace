// Ignore Spelling: Amla

using AmlaMarketPlace.BAL.Agent.Agents.Account;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace AmlaMarketPlace.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountAgent _accountAgent;
        public AccountController(AccountAgent accountAgent)
        {
            _accountAgent = accountAgent;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            ViewData["Title"] = "Sign up";
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(SignUpViewModel signUpViewModel)
        {
            if (ModelState.IsValid)
            {
                bool canBeSignUp = _accountAgent.SignUp(signUpViewModel);

                if (canBeSignUp)
                {
                    TempData["VerificationLink"] = "a verification link is sent on your mail id";
                    return RedirectToAction("SignIn");
                }
                else
                {
                    TempData["SignUpMessage"] = "Email is already registered. Please sign in";
                    return View(signUpViewModel);
                }
            }

            // if something goes wrong
            return View(signUpViewModel);
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            ViewData["Title"] = "Sign in";
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(SignInViewModel signInViewModel)
        {
            if (ModelState.IsValid)
            {
                if (_accountAgent.isEmailRegistered(signInViewModel.EmailAddress))
                {
                    var isEmailVerified = _accountAgent.isEmailAlreadyVerified(signInViewModel.EmailAddress);

                    if (isEmailVerified)
                    {
                        if (_accountAgent.isValidCredentials(signInViewModel))
                        {
                            string userRole = _accountAgent.getUserRoleByEmail(signInViewModel.EmailAddress);
                            // Create claims
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, _accountAgent.getUserNameByEmail(signInViewModel.EmailAddress)),
                                new Claim(ClaimTypes.Email, signInViewModel.EmailAddress),
                                new Claim("UserId", _accountAgent.getUserIdByEmail(signInViewModel.EmailAddress).ToString()),
                                new Claim(ClaimTypes.Role, userRole)
                            };

                            // Create identity and principal
                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            // Sign in synchronously
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                            {
                                IsPersistent = signInViewModel.RememberMe,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                            }).GetAwaiter().GetResult();

                            TempData["Login-Success"] = "Successfully logged in";

                            if (userRole == "user")
                            {
                                return RedirectToAction("ProductListing", "Product");
                            }
                            else if (userRole == "admin")
                            {
                                return RedirectToAction("DashBoard", "Admin");
                            }
                        }
                        else
                        {
                            TempData["invalidCredentials"] = "Please input the correct password.";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["emailNotVerified"] = "Email is not verified. Please check your email and click on the verification link.";
                        return View();
                    }
                }
                else
                {
                    TempData["emailNotRegistered"] = "Email is not registered. Please sign up.";
                    return View();
                }
            }

            TempData["UnexpectedInput"] = "Unexpected Input.";
            return View();
        }

        [HttpPost]
        public IActionResult SignOut()
        {
            // SignOut the user
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).GetAwaiter().GetResult();
            TempData["SignOutMessage"] = "You have successfully signed out.";
            return RedirectToAction("SignIn", "Account");
        }

        public IActionResult VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid verification link.";
                return RedirectToAction("SignIn");
            }

            // Specifically for verifying token and setting up isEmailVerified to true
            var isVerified = _accountAgent.VerifyEmail(token);

            if (isVerified)
            {
                TempData["SuccessMessage"] = "Your email has been verified successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "The verification link is invalid or expired.";
            }

            return RedirectToAction("SignIn");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPasswordVerifyEmail([FromForm] string email)
        {
            try
            {
                // Validate the email input
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest("Email address is required.");
                }

                // Simulate checking if the email exists in your system
                bool emailExists = _accountAgent.isEmailRegistered(email);

                if (!emailExists)
                {
                    return BadRequest("The provided email address is not registered.");
                }

                // Send verification link
                bool linkSentSuccessfully = _accountAgent.SendResetPasswordVerificationLink(email);

                if (linkSentSuccessfully)
                {
                    TempData["ResetPasswordLinkMessage"] = "Reset link send successfully";
                }
                else
                {
                    TempData["ResetPasswordLinkMessage"] = "There was an error to send link. Please try again later or contact us.";
                }

                return RedirectToAction("SignIn", "Account");
            }
            catch (Exception ex)
            {
                // Return a generic error message
                return BadRequest("An error occurred while processing your request. Please try again later.");
            }
        }

        public IActionResult VerifyAndRedirectToResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid verification link.";
                return RedirectToAction("SignIn");
            }

            var isVerified = _accountAgent.VerifyTokenWithTime(token);

            if (isVerified)
            {
                string userEmail = _accountAgent.GetUserEmailFromToken(token);
                return RedirectToAction("ResetPassword", "Account", new { email = userEmail });
            }
            else
            {
                TempData["ErrorMessage"] = "The verification link is invalid or expired.";
            }

            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel { Email = email };

            return View(resetPasswordViewModel);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool passwordResetSuccessfull = _accountAgent.UpdatePassword(resetPasswordViewModel.Email, resetPasswordViewModel);

                    if (passwordResetSuccessfull)
                    {
                        TempData["PasswordResetSuccess"] = "Password is successfully updated.";
                        return RedirectToAction("SignIn", "Account");
                    }
                    else
                    {
                        TempData["PasswordResetFailed"] = "We are unable to update your password. Please contact us.";
                        return RedirectToAction("SignIn", "Account");
                    }
                }
                else
                {
                    return View(resetPasswordViewModel);
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "We are facing some issues in reseting password. Sorry for the inconvenience. Our services will be back soon.";
                return RedirectToAction("Error");
            }
        }
    }
}
