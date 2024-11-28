using Api.Dtos.Employee;

namespace Api.Business.Services.Interfaces;

public interface IEmployeeService
{
	Task<GetEmployeeDto> GetEmployeeByIdAsync(int id);
	Task<List<GetEmployeeDto>> GetAllEmployeesAsync();
}
