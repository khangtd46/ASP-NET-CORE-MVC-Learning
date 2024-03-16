using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class AddPasswordViewModel
    {
        [Required]
        [Display(Name = "Password")]
       // [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
     //   [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string confirmPassword { get; set; }
    }
}
