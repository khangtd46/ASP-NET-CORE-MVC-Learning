namespace WebApplication1.Models
{
	public class SQLEmployeeRepository : IEmployeeRepository
	{
		private readonly AppDBContext context;

		public SQLEmployeeRepository(AppDBContext context)
        {
			this.context = context;
		}

        public Employee addEmployee(Employee employee)
		{
			context.Employees.Add(employee);
			context.SaveChanges();
			return employee;
		}

		public Employee deleteEmployee(int id)
		{
			Employee employee = context.Employees.Find(id);
			if (employee != null) 
			{
				context.Employees.Remove(employee);
				context.SaveChanges();
			}
			return employee;
		}

		public IEnumerable<Employee> GetAllEmployee()
		{
			return context.Employees;
		}

		public Employee GetEmployee(int id)
		{
			return context.Employees.Find(id);
		}

		public Employee updateEmployee(Employee employeeChanges)
		{
			var employee = context.Employees.Attach(employeeChanges);
			employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			context.SaveChanges();
			return employeeChanges;
		}
	}
}
