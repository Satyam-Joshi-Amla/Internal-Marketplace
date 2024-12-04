// Ignore Spelling: Amla

using AmlaMarketPlace.BAL.Agent.Agents.Account;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
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
                            // Create claims
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, _accountAgent.getUserNameByEmail(signInViewModel.EmailAddress)),
                                new Claim(ClaimTypes.Email, signInViewModel.EmailAddress),
                                new Claim("UserId", _accountAgent.getUserIdByEmail(signInViewModel.EmailAddress).ToString()),
                                new Claim(ClaimTypes.Role, "User")
                            };

                            // Create identity and principal
                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            // Sign in synchronously
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                            }).GetAwaiter().GetResult();

                            TempData["Login-Success"] = "Successfully logged in";

                            return RedirectToAction("Index", "Home");
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

                TempData["emailNotRegistered"] = "Email is not registered. Please sign up.";
                return View();
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


        //[HttpPost]
        //public IActionResult ForgotPassword([FromBody] ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Simulate email verification or sending reset link
        //        bool isSuccess = _accountAgent.SendPasswordResetEmail(model.EmailAddress);

        //        if (isSuccess)
        //        {
        //            return Json(new { success = true, message = "Password reset email sent successfully." });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Email address not found." });
        //        }
        //    }

        //    return Json(new { success = false, message = "Invalid email address." });
        //}

    }
}
