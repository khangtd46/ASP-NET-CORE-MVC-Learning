using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
	public class EmployeeCreateViewModel
	{
		
		[Required(ErrorMessage = "Nhap Ten Ban oi")]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Office Email")]
		public string Email { get; set; }
		[Required]
		public Dept? Department { get; set; }

		public IFormFile? Photo { get; set; }
	}
}
