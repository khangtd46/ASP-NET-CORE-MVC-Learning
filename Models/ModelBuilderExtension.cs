using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace WebApplication1.Models
{
	public static class ModelBuilderExtension
	{
		public static void Seed(this ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Employee>().HasData(
				new Employee()
				{
					Id = 1,
					Name = "Emp1",
					Email = "Emp1@gmail.com",
					Department = Dept.IT,
					PhotoPath = "testttttttt"
				},
				new Employee()
				{
					Id = 2,
					Name = "Emp2",
					Email = "Emp2@gmail.com",
					Department = Dept.HR,
					PhotoPath = "testttttttt"
				}
			 );
		}
	}
}
