using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
	public class CreateRoleViewModel
	{

      
        [Required]
        public string RoleName { get; set; }

    }
}
