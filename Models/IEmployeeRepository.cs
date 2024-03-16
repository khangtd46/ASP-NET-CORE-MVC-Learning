namespace WebApplication1.Models
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int id);
        IEnumerable<Employee> GetAllEmployee();

        Employee addEmployee(Employee employee);

        Employee updateEmployee(Employee employeeChanges);

        Employee deleteEmployee(int id);
    }
}
