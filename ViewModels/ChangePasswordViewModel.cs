using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace WebApplication1.ViewModels
{
	public class ChangePasswordViewModel
	{
		[Required]
		[Display(Name = "Old Password")]
		[DataType(DataType.Password)]
		public string oldPassword { get; set; }
		[Required]
		[Display(Name = "New Password")]
		[DataType(DataType.Password)]
		public string newPassword { get; set; }
		[Display(Name = "Confirm New Password")]
		[DataType(DataType.Password)]
		[Compare("newPassword", ErrorMessage = "Password and Confirm Password must match")]
		public string confirmNewPassword { get; set; }

	}
}
