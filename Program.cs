using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using WebApplication1.Models;
using WebApplication1.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDBConnection")));
builder.Services.AddControllersWithViews(
//	option =>
//{
//	var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//	option.Filters.Add(new AuthorizeFilter(policy));
//}
);
//builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolsAndClaimsHandler>();

builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
builder.Services.AddSingleton<DataProtectionPurposeStrings>();
builder.Services.AddLogging(configure => configure.AddNLog());
builder.Services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(5) );
builder.Services.Configure<CustomEmailTokenProviderOption>(o => o.TokenLifespan = TimeSpan.FromDays(3));


builder.Services.AddAuthentication().AddGoogle(options =>
{
	options.ClientId = "203835490796-u37cspq8lbu509ie0n2kflp28tf1css7.apps.googleusercontent.com";
	options.ClientSecret = "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";
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
{
	option.Password.RequiredLength = 6;
	option.SignIn.RequireConfirmedEmail = true;
	option.Tokens.EmailConfirmationTokenProvider = "CustomeEmailTokenProvider";
	option.Lockout.MaxFailedAccessAttempts = 5;
	option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

}).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders().AddTokenProvider<CustomEmailTokenProvider<ApplicationUser>>("CustomeEmailTokenProvider");

builder.Services.AddAuthorization(
	option =>
	{
		option.AddPolicy("DeletePolicy", policy => policy.RequireClaim("Delete Role"));
		//option.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role","True"));

		// đang muốn EditRolePolicy thỏa mãn khi user vừa có (claim edit role = true, vừa là role admin) hoặc là super admin thì viết như dòng dưới ko đc)
		//option.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role", "True").RequireClaim("Admin").RequireClaim("Super Admin"));\
		// phải dùng custom policy: (2 cách ở dưới như nhau)
		//option.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => 
		//																	context.User.IsInRole("Admin") && 
		//																	context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "True") ||
		//																	context.User.IsInRole("Super Admin")));

		//option.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));
		option.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
		
	}
	
	);


var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Error");
	app.UseStatusCodePagesWithReExecute("/Error/{0}");
}
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Company}/{action=Index}/{id?}");
app.Run();



bool AuthorizeAccess(AuthorizationHandlerContext context)
{
	return	context.User.IsInRole("Admin") &&
			context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "True") ||
			context.User.IsInRole("Super Admin");
}