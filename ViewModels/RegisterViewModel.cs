using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Ultilities;

namespace WebApplication1.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action:"IsEmailInUse", controller:"Account")] // Khi người dùng nhập vào Field Email này, jquery (từ đâu ra?)
                                                              // sẽ gửi Get Request vào controller account, action isemailinuse ở server
                                                              //jquery này sẽ chờ 1 feedback kiểu json -> xem bên account controller
                                                              // cần có client-side library để chạy: jquery, jquery-validate, jquery-validate-unobtrusive
        [ValidEmailDomain(allowedDomain:"gmail.com",ErrorMessage ="Email must have Domain name = gmail.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        public string City { get; set; }
    }
}
