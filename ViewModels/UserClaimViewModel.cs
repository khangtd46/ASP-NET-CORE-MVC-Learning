using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
	public class UserClaimViewModel
	{
        public UserClaimViewModel()
        {
            Claims = new List<UserClaim>();
        }
        public string userId { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
