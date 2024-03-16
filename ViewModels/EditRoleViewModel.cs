using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
	public class EditRoleViewModel
	{
        public EditRoleViewModel()
        {
            Users = new List<String>();
		}
        public string Id { get; set; }

		[Required]
		public string RoleName { get; set; }

		public List<String> Users { get; set; }

	}
}
