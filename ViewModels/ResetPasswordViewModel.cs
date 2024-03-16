using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

		[Display(Name = "Confirm Password")]
		[DataType(DataType.Password)]
		[Compare("Password",ErrorMessage = "Password and Confirm Password must match")]
		public string ConfirmPassword { get; set; }

        public string token { get; set; }
    }
}
