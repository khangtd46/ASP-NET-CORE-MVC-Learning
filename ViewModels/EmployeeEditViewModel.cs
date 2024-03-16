using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
	public class EmployeeEditViewModel : EmployeeCreateViewModel
	{
		public int Id { get; set; }
		public string? ExistingPhotoPath { get; set; }
	}
}
