using Api.Business.Services.Interfaces;
using Api.Data.Models;
using Api.Data.Repositories;
using Api.Dtos.Employee;
using Api.Dtos.Paycheck;
using Microsoft.EntityFrameworkCore;

namespace Api.Business.Services;

public class PaycheckService : IPaycheckService
{
	private readonly IRepository<Paycheck> _paycheckRepository;

	private readonly IEmployeeService _employeeService;

	public PaycheckService(IRepository<Paycheck> paycheckRepository, IEmployeeService employeeService)
	{
		_paycheckRepository = paycheckRepository;
		_employeeService = employeeService;
	}

	public async Task<GetPaycheckDto> GetPaycheckByIdAsync(int id)
	{
		var paycheck = await _paycheckRepository.GetAll(p => p.Id == id)
			.Select(p => new GetPaycheckDto
			{
				Id = p.Id,
				EmployeeId = p.EmployeeId,
				GrossPay = p.GrossPay,
				Deductions = p.Deductions,
				NetPay = p.NetPay,
				YearToDate = p.YearToDate,
				HoursWorked = p.HoursWorked,
				StartDate = p.StartDate,
				EndDate = p.EndDate,
				PayDate = p.PayDate
			})
			.FirstOrDefaultAsync();

		return paycheck;
	}

	/// <summary>
	/// Get all paychecks from database for an employee.
	/// If no paychecks exist, then calculate paychecks.
	/// Store calculated paychecks in the database for future retrieval.
	/// </summary>
	/// <param name="employeeId">Employee Id</param>
	/// <returns>List of paychecks with detailed info.</returns>
	public async Task<List<GetPaycheckDto>> GetAllPaychecksByEmployeeIdAsync(int employeeId)
	{
		var paychecks = await _paycheckRepository.GetAll(p => p.EmployeeId == employeeId)
			.Select(p => new GetPaycheckDto
			{
				Id = p.Id,
				EmployeeId = p.EmployeeId,
				GrossPay = p.GrossPay,
				Deductions = p.Deductions,
				NetPay = p.NetPay,
				YearToDate = p.YearToDate,
				HoursWorked = p.HoursWorked,
				StartDate = p.StartDate,
				EndDate = p.EndDate,
				PayDate = p.PayDate
			})
			.ToListAsync();

		if (!paychecks.Any())
		{
			var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);

			if (employee != null)
			{
				try
				{
					paychecks = CalculatePaychecksForEmployee(employee);

					await SavePaychecksAsync(paychecks);
				}
				catch (InvalidOperationException)
				{
					//Log message
				}
			}
		}

		return paychecks;
	}

	/// <summary>
	/// Calculate 26 paychecks with deductions spread out evenly.
	/// Paychecks are calculated for the year 2024.
	/// Start date for the pay period is set to 12/23/2023. This would vary depending on the employer.
	/// </summary>
	/// <param name="employee">Employee and dependent details.</param>
	/// <returns>List of paychecks.</returns>
	public List<GetPaycheckDto> CalculatePaychecksForEmployee(GetEmployeeDto employee)
	{
		var paychecks = new List<GetPaycheckDto>();
		var annualDeductions = CalculateAnnualDeductionsForEmployee(employee);

		var grossPayPerPaycheck = employee.Salary / Constants.NoOfPaychecks;
		var deductionsPerPaycheck = annualDeductions / Constants.NoOfPaychecks;
		var netPayPerPaycheck = grossPayPerPaycheck - deductionsPerPaycheck;
		var startDate = new DateTime(2023, 12, 23);

		for (int i = 1; i <= Constants.NoOfPaychecks; i++)
		{
			var paycheck = new GetPaycheckDto
			{
				EmployeeId = employee.Id,
				GrossPay = grossPayPerPaycheck,
				Deductions = deductionsPerPaycheck,
				NetPay = netPayPerPaycheck,
				YearToDate = grossPayPerPaycheck * i,
				HoursWorked = Constants.HoursWorked,
				StartDate = startDate,
				EndDate = startDate.AddDays(13),
				PayDate = startDate.AddDays(13)
			};

			startDate = startDate.AddDays(14);
			paychecks.Add(paycheck);
		}

		return paychecks;
	}

	/// <summary>
	/// Calculate total annual deductions for an employee.
	/// Deductions include base cost, dependents cost and additional cost based on salary.
	/// </summary>
	/// <param name="employee">Employee and dependent details.</param>
	/// <returns>TotalDeductions</returns>
	/// <exception cref="InvalidOperationException">Thrown when deductions exceed salary.</exception>
	public decimal CalculateAnnualDeductionsForEmployee(GetEmployeeDto employee)
	{
		var baseDeductions = Constants.BaseCost * 12;
		var dependentDeductions = employee.Dependents.Count() * Constants.DependentCost * 12;
		var dependentsOver50 = employee.Dependents.Where(d => d.DateOfBirth.GetAge() > 50).Count();
		var dependentsOver50Deductions = dependentsOver50 * Constants.DependentOver50Cost * 12;

		var additionalCost = 0m;
		if (employee.Salary > Constants.IncomeThreshold)
		{
			additionalCost = (Constants.IncomePercent / 100) * employee.Salary;
		}

		var totalDeductions = baseDeductions + dependentDeductions + dependentsOver50Deductions + additionalCost;

		if (totalDeductions > employee.Salary)
		{
			throw new InvalidOperationException("Total deductions cannot exceeed salary.");
		}

		return totalDeductions;
	}


	private async Task SavePaychecksAsync(List<GetPaycheckDto> paychecks)
	{
		foreach (var paycheck in paychecks)
		{
			var dbModel = new Paycheck
			{
				EmployeeId = paycheck.EmployeeId,
				GrossPay = paycheck.GrossPay,
				Deductions = paycheck.Deductions,
				NetPay = paycheck.NetPay,
				YearToDate = paycheck.YearToDate,
				HoursWorked = paycheck.HoursWorked,
				StartDate = paycheck.StartDate,
				EndDate = paycheck.EndDate,
				PayDate = paycheck.PayDate
			};

			var entity = await _paycheckRepository.CreateAsync(dbModel);
			paycheck.Id = entity.Id;
		}
	}
}
