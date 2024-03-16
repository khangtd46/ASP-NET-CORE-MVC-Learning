using System.Runtime.InteropServices;
using System.Security.Claims;

namespace WebApplication1.Models
{
	public static class ClaimStore
	{
		public static List<Claim> AllClaim {  get; set; } = new List<Claim>()
		{
			new Claim("Create Role","Create Role"),
			new Claim("Edit Role","Edit Role"),
			new Claim("Delete Role","Delete Role"),

		};
	}
}
