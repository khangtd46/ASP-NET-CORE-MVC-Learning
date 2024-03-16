namespace WebApplication1.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employees;

        public MockEmployeeRepository() 
        {
            _employees = new List<Employee>()
            {
                new Employee() {Id = 1,Name = "Emp1", Department = Dept.HR, Email = "Emp1@gmail.com" },
                new Employee() {Id = 2,Name = "Emp2", Department = Dept.IT, Email = "Emp2@gmail.com" },
                new Employee() {Id = 3,Name = "Emp3", Department = Dept.QC, Email = "Emp3@gmail.com" },
            };
        }

		public Employee addEmployee(Employee employee)
		{
            employee.Id = _employees.Max(e => e.Id) + 1; 
			_employees.Add(employee);
            return employee;
		}

		public Employee deleteEmployee(int id)
		{
			Employee employee = _employees.FirstOrDefault(e => e.Id == id);
			if (employee != null)
			{
				_employees.Remove(employee);				
			}
			return employee;
		}

		public IEnumerable<Employee> GetAllEmployee()
		{
			return _employees;
		}

		public Employee GetEmployee(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }

		public Employee updateEmployee(Employee employeeChanges)
		{
			Employee employee = _employees.FirstOrDefault(e => e.Id == employeeChanges.Id);
			if (employee != null)
			{
				employee.Name = employeeChanges.Name;
				employee.Department = employeeChanges.Department;
				employee.Email = employeeChanges.Email;
			}
			return employee;
		}
	}
}
