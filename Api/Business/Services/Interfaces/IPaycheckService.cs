using Api.Dtos.Employee;
using Api.Dtos.Paycheck;

namespace Api.Business.Services.Interfaces;

public interface IPaycheckService
{
	Task<GetPaycheckDto> GetPaycheckByIdAsync(int id);
	Task<List<GetPaycheckDto>> GetAllPaychecksByEmployeeIdAsync(int employeeId);
	List<GetPaycheckDto> CalculatePaychecksForEmployee(GetEmployeeDto employee);
	decimal CalculateAnnualDeductionsForEmployee(GetEmployeeDto employee);
}
