using Api.Business.Services.Interfaces;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
	private readonly IEmployeeService _employeeService;

	public EmployeesController(IEmployeeService employeeService)
	{
		_employeeService = employeeService;
	}

	[SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
		var employee = await _employeeService.GetEmployeeByIdAsync(id);

		if (employee == null)
		{
			return NotFound();
		}

		var result = new ApiResponse<GetEmployeeDto>
		{
			Data = employee,
			Success = true
		};

		return result;
	}

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
		var employees = await _employeeService.GetAllEmployeesAsync();

		var result = new ApiResponse<List<GetEmployeeDto>>
		{
			Data = employees,
			Success = true
		};

		return result;
	}
}
