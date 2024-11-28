using Api.Business.Services.Interfaces;
using Api.Data.Models;
using Api.Data.Repositories;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Microsoft.EntityFrameworkCore;

namespace Api.Business.Services;

public class EmployeeService : IEmployeeService
{
	private readonly IRepository<Employee> _employeeRepository;

	public EmployeeService(IRepository<Employee> employeeRepository)
	{
		_employeeRepository = employeeRepository;
	}

	public async Task<GetEmployeeDto> GetEmployeeByIdAsync(int id)
	{
		var employee = await _employeeRepository.GetAll(e => e.Id == id)
			.Include(e => e.Dependents)
			.Select(e => new GetEmployeeDto
			{
				Id = e.Id,
				FirstName = e.FirstName,
				LastName = e.LastName,
				Salary = e.Salary,
				DateOfBirth = e.DateOfBirth,
				Dependents = e.Dependents.Select(d => new GetDependentDto
				{
					Id = d.Id,
					FirstName = d.FirstName,
					LastName = d.LastName,
					Relationship = d.Relationship,
					DateOfBirth = d.DateOfBirth
				}).ToList()
			})
			.FirstOrDefaultAsync();

		return employee;
	}

	public async Task<List<GetEmployeeDto>> GetAllEmployeesAsync()
	{
		var employees = await _employeeRepository.GetAll()
			.Include(e => e.Dependents)
			.Select(e => new GetEmployeeDto
			{
				Id = e.Id,
				FirstName = e.FirstName,
				LastName = e.LastName,
				Salary = e.Salary,
				DateOfBirth = e.DateOfBirth,
				Dependents = e.Dependents.Select(d => new GetDependentDto
				{
					Id = d.Id,
					FirstName = d.FirstName,
					LastName = d.LastName,
					Relationship = d.Relationship,
					DateOfBirth = d.DateOfBirth
				}).ToList()
			})
			.ToListAsync();

		return employees;
	}
}
