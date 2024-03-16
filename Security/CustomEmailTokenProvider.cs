using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WebApplication1.Security
{
	public class CustomEmailTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
	{
        public CustomEmailTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<CustomEmailTokenProviderOption> options, ILogger<CustomEmailTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
            
        }
    }
}
