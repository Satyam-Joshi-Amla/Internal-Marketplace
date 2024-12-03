﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.ViewModels.Account
{
    public class SignInViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email:")]
        public string EmailAddress { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}
