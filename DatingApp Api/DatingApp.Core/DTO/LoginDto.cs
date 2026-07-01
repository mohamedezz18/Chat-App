using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace DatingApp.Core.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public  string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public  string Password { get; set; }
    }
}
