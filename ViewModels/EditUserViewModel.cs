using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebApplication1.ViewModels
{
	public class EditUserViewModel
	{
		public EditUserViewModel()
		{
			IList<String> Roles = new List<String>();
			List<string> Claims = new List<string>();
		}
		public string Id { get; set;}
		[Required]
		public string UserName { get; set;}
		[Required]
		[EmailAddress]
		[Remote(action: "IsEmailInUse", controller: "Account")]
		public string Email { get; set;}
		public string City { get; set;}
		public List<string> Claims { get; set;}
		public IList<String> Roles { get; set; }
	}
}
