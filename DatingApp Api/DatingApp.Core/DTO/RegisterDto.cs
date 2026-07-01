using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatingApp.Core.DTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Display Name is required")] 
        public  string DisplayName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public  string  Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public  string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;
        [Required] public string Country { get; set; } = string.Empty;
        [Required] public DateOnly DateOfBirth { get; set; }
    }
}
