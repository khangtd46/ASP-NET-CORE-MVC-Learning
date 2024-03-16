using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace WebApplication1.Security
{
	public class CanEditOnlyOtherAdminRolsAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
		{
			var authFilterContext = context.Resource as Microsoft.AspNetCore.Http.DefaultHttpContext;
			if (authFilterContext == null)
			{
				return Task.CompletedTask;
			}
			string loggedInAdmin = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;  //quan trong: tim ra ID cua nguoi dang thao tac chinh sua
			string adminBeingEdited = authFilterContext.HttpContext.Request.Query["userId"]; //quan trong: tim ra ID cua nguoi dang bi edit

			if (context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "True") && loggedInAdmin.ToLower() != adminBeingEdited.ToLower())
			{
				context.Succeed(requirement);
			}
			return Task.CompletedTask;

		}
	}
}
