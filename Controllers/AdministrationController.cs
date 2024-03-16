using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdministrationController : Controller
	{
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<AdministrationController> logger;

		public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager, ILogger<AdministrationController> logger)
        {
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.logger = logger;
		}
		[HttpGet]
        public IActionResult CreateRole()
		{
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
			if (ModelState.IsValid)
			{
				IdentityRole role = new IdentityRole()
				{
					Name = model.RoleName
				};

			IdentityResult result = await roleManager.CreateAsync(role);

				if (result.Succeeded)
				{
					return RedirectToAction("ListRole", "Administration");
				}
				
			
                    foreach (IdentityError error in result.Errors)
                    {
						ModelState.AddModelError("", error.Description);
                    }
                
            }
            return View();
        }
		[HttpGet]
		public IActionResult ListRole()
		{
			var roles = roleManager.Roles;
			return View(roles);

        }
		[HttpGet]
		public async Task<IActionResult> EditUser(string id)
		{
			var user = await userManager.FindByIdAsync(id);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"The requested user with id:{id} is not found";
				return View("NotFound");
			}
			var userClaims = await userManager.GetClaimsAsync(user);
			var userRoles = await userManager.GetRolesAsync(user);
			var model = new EditUserViewModel()
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				City = user.City,
				Roles = userRoles,
				Claims = userClaims.Select(c => c.Type + ":" + c.Value).ToList(),
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditUser(EditUserViewModel model)
		{
			var user = await userManager.FindByIdAsync(model.Id);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"The requested user with id:{model.Id} is not found";
				return View("NotFound");
			}
			else
			{
				user.UserName = model.UserName;
				user.Email = model.Email;
				user.City = model.City;
			}
			var result = await userManager.UpdateAsync(user);
			if (result.Succeeded)
			{
				return RedirectToAction("ListUser");
				
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}	
			return View(model);
			
		}
		[HttpGet]
		
		public async Task<IActionResult> EditRole(string id)
		{
			var role = await roleManager.FindByIdAsync(id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with id:{id} is not found";
				return View("NotFound");
			}
			else
			{
				EditRoleViewModel model = new EditRoleViewModel()
				{
					Id = role.Id,
					RoleName = role.Name
				};
				var users = userManager.Users.ToList();

				foreach (var user in users)
                {
					if (await userManager.IsInRoleAsync(user, role.Name))
					{
						model.Users.Add(user.UserName);
					}
				}
                return View(model);	
            }
		}
		[HttpPost]
		
		public async Task<IActionResult> EditRole(EditRoleViewModel model)
		{
			var role = await roleManager.FindByIdAsync(model.Id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with id:{model.Id} is not found";
				return View("NotFound");
			}
			else
			{
				role.Name = model.RoleName;
				var result = await roleManager.UpdateAsync(role);
				if (result.Succeeded)
				{
					return RedirectToAction("ListRole","Administration");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}

					//đoạn foreach ở dưới đây dùng để load lại user list, tại vì trong httppost, không có gửi user list vào model, nên bị mất, có lỗi
					//load lại View sẽ mất user list
					//foreach (var user in userManager.Users)
					//{
					//	//              if (await userManager.IsInRoleAsync(user,role.Name))
					//	//{
					//	model.Users.Add(user.UserName);
					//	//	}
					//}

					return View(model);
				}	
			}
		}

		[HttpGet]
		public async Task<IActionResult> EditUsersInRole(string RoleId)
		{
			var role = await roleManager.FindByIdAsync(RoleId);
			ViewBag.RoleID = RoleId;
			if (role == null)
			{
				ViewBag.ErrorMessage = $"The Role With id:{RoleId} Cannot Be Found";
				return View("NotFound");
			}

			List<UserRoleViewModel> model = new List<UserRoleViewModel>();
			var users = userManager.Users.ToList();

			foreach (var user in users)	
            {
				var userRoleViewModel = new UserRoleViewModel()
				{
					UserID = user.Id,
					UserName = user.UserName,
				};
				if (await userManager.IsInRoleAsync(user,role.Name))
				{
					userRoleViewModel.IsSelected = true;
				}
				else 
				{ 
					userRoleViewModel.IsSelected = false;
				}
				model.Add(userRoleViewModel);
			}
			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model,string RoleId)
		{
			var role = await roleManager.FindByIdAsync(RoleId);
			IdentityResult result = null;

			if (role == null)
			{
				ViewBag.ErrorMessage = $"The Role With id:{RoleId} Cannot Be Found";
				return View("NotFound");
			}
            for (int i = 0; i < model.Count; i++)
            {
				var user = await userManager.FindByIdAsync(model[i].UserID);

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user,role.Name)))
				{
					result = await userManager.AddToRoleAsync(user, role.Name);
				}
				else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user,role.Name))
				{
					result = await userManager.RemoveFromRoleAsync(user, role.Name);
				}
				else
				{
					continue;
				}
					if (result.Succeeded)
				{
					if (i <model.Count -1 )
					{
						continue;
					}
					else { return RedirectToAction("EditRole",new {id = RoleId }); }

				}
					else
				{
                    foreach (var error in result.Errors)
                    {
						ModelState.AddModelError("", error.Description);
						return View(model);
                    }
                }
            }
			return RedirectToAction("EditRole", new { id = RoleId });
		}


        [HttpGet]
        public IActionResult ListUser()
        {
            var users = userManager.Users.ToList();

            return View(users);

        }

		[HttpPost]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await userManager.FindByIdAsync(id);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"The User With id:{id} Cannot Be Found";
				return View("NotFound");
			}
			else
			{
				var result = await userManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					return RedirectToAction("ListUser");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View("ListUser");
			}

		}


		[HttpPost]
		//[Authorize(Policy = "DeletePolicy")]
		public async Task<IActionResult> DeleteRole(string id)
		{
			var role = await roleManager.FindByIdAsync(id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"The Role With id:{id} Cannot Be Found";
				return View("NotFound");
			}
			else
			{
				try
				{
					var result = await roleManager.DeleteAsync(role);

					if (result.Succeeded)
					{
						return RedirectToAction("ListRole");
					}
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return View("ListRole");
				}
				catch (DbUpdateException ex)
				{
					logger.LogError($"Exception occured: {ex}");
						ViewBag.ErrorTitle = $"The role {role.Name} is in use";
					ViewBag.ErrorMessage = $"There are users in this {role.Name} => cannot delete role";
					return View("Error");
				}
			}

		}


		[HttpGet]
		[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> ManageUserRole(string userId)
		{
			ViewBag.userId = userId;
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"The Role With id:{userId} Cannot Be Found";
				return View("NotFound");
			}
			List<ManageUserRoleViewModel> listModels = new List<ManageUserRoleViewModel>();
			var roles = roleManager.Roles.ToList();

			foreach (var role in roles)
			{
				ManageUserRoleViewModel model = new ManageUserRoleViewModel();
				if (await userManager.IsInRoleAsync(user,role.Name))
				{
					model.IsSelected = true;
				}
				else { model.IsSelected = false; }
				model.RoleName = role.Name;
				model.userID = userId;
				listModels.Add(model);
			}
			return View(listModels);

		}
		[HttpPost]
		[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> ManageUserRole(List<ManageUserRoleViewModel> listModels, string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"The Role With id:{userId} Cannot Be Found";
				return View("NotFound");
			}
			var roles = await userManager.GetRolesAsync(user);
			var result = await userManager.RemoveFromRolesAsync(user, roles);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "cannot remove existing roles");
				return View(listModels);
			}

			result = await userManager.AddToRolesAsync(user, listModels.Where(c => c.IsSelected).Select(c => c.RoleName));

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "cannot add role to user");
				return View(listModels);
			}
			return RedirectToAction("EditUser", new { id = userId });









			//IdentityResult result = null;
			//for (int i = 0; i < listModels.Count; i++)
			//{
			//	var role = await roleManager.FindByNameAsync(listModels[i].RoleName);

			//	if (listModels[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
			//	{
			//		result = await userManager.AddToRoleAsync(user, role.Name);
			//	}
			//	else if (!listModels[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
			//	{
			//		result = await userManager.RemoveFromRoleAsync(user, role.Name);
			//	}
			//	else
			//	{
			//		continue;
			//	}
			//	if (result.Succeeded)
			//	{
			//		if (i < listModels.Count - 1)
			//		{
			//			continue;
			//		}
			//		else { return RedirectToAction("EditUser", new { id = userId }); }

			//	}
			//	else
			//	{
			//		foreach (var error in result.Errors)
			//		{
			//			ModelState.AddModelError("", error.Description);
			//			return View(listModels);
			//		}
			//	}
			//}
			//return RedirectToAction("EditUser", new { id = userId });

		}
		[HttpGet]
		public async Task<IActionResult> ManageUserClaim(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"The User With id:{userId} Cannot Be Found";
				return View("NotFound");
			}
			var existingUserClaims = await userManager.GetClaimsAsync(user);

			UserClaimViewModel model = new UserClaimViewModel();
			model.userId = userId;
            foreach (Claim claim in ClaimStore.AllClaim)
            {
				UserClaim userClaim = new UserClaim();
				userClaim.ClaimType = claim.Type;

				if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "True"))
				{
					userClaim.isSelected = true;
				}
				else
				{
					userClaim.isSelected = false;
				}
				model.Claims.Add(userClaim);
			}
			return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> ManageUserClaim(UserClaimViewModel listModels)
		{
			var user = await userManager.FindByIdAsync(listModels.userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"The User With id:{listModels.userId} Cannot Be Found";
				return View("NotFound");
			}

			var claims = await userManager.GetClaimsAsync(user);
			var result = await userManager.RemoveClaimsAsync(user, claims);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "cannot remove existing claims");
				return View(listModels);
			}

			result = await userManager.AddClaimsAsync(user, listModels.Claims.Select(c => new Claim(c.ClaimType,c.isSelected.ToString())));

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "cannot add claim to user");
				return View(listModels);
			}
			return RedirectToAction("EditUser", new { id = listModels.userId });
			
		}


	}
}
