using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
	public class HomeIndexViewModel
	{
		public IEnumerable<Employee> Employees { get; set; }
		public string PageTitle { get; set; }
	}
}
