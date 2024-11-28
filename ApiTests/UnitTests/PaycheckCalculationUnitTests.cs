using Api.Business.Services;
using Api.Data.Models;
using Api.Data.Repositories;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ApiTests.UnitTests;
public class PaycheckCalculationUnitTests
{
	private readonly PaycheckService _paycheckService;

	public PaycheckCalculationUnitTests()
	{
		var employeeRepoMock = new Mock<IRepository<Employee>>();
		var paycheckRepoMock = new Mock<IRepository<Paycheck>>();
		var employeeService = new EmployeeService(employeeRepoMock.Object);
		_paycheckService = new PaycheckService(paycheckRepoMock.Object, employeeService);
	}

	[Fact]
	public void GivenNoDependents_ItCalculatesAnnualDeductionsForEmployee()
	{
		//Arrange
		var employeeDto = new GetEmployeeDto
		{
			Salary = 75420.99m
		};
		var expected = 12000m;

		//Act
		var result = _paycheckService.CalculateAnnualDeductionsForEmployee(employeeDto);

		//Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void GivenDependents_ItCalculatesAnnualDeductionsForEmployee()
	{
		//Arrange
		var employeeDto = new GetEmployeeDto
		{
			Salary = 92365.22m,
			Dependents = new List<GetDependentDto>
			{
				new()
				{
					DateOfBirth = new DateTime(1998, 3, 3)
				},
				new()
				{
					DateOfBirth = new DateTime(2020, 6, 23)
				},
				new()
				{
					DateOfBirth = new DateTime(2021, 5, 18)
				}
			}
		};
		var expected = 35447.3044m;

		//Act
		var result = _paycheckService.CalculateAnnualDeductionsForEmployee(employeeDto);

		//Assert
		Assert.Equal(expected, result, 2);
	}

	[Fact]
	public void GivenDependentOver50_ItCalculatesAnnualDeductionsForEmployee()
	{
		//Arrange
		var employeeDto = new GetEmployeeDto
		{
			Salary = 143211.12m,
			Dependents = new List<GetDependentDto>
			{
				new()
				{
					DateOfBirth = new DateTime(1973, 1, 2)
				}
			}
		};
		var expected = 24464.224m;

		//Act
		var result = _paycheckService.CalculateAnnualDeductionsForEmployee(employeeDto);

		//Assert
		Assert.Equal(expected, result, 2);
	}

	[Fact]
	public void GivenEmployee_ItCalculatesAllPaychecksForEmployee()
	{
		//Arrange
		var employeeDto = new GetEmployeeDto
		{
			Salary = 75420.99m
		};
		var expectedGrossPay = 2900.807m;
		var expectedDeductions = 461.538m;

		//Act
		var result = _paycheckService.CalculatePaychecksForEmployee(employeeDto);
		var firstPaycheck = result.FirstOrDefault();

		//Assert
		Assert.Equal(26, result.Count());
		Assert.NotNull(firstPaycheck);
		Assert.Equal(expectedGrossPay, firstPaycheck.GrossPay, 2);
		Assert.Equal(expectedDeductions, firstPaycheck.Deductions, 2);
	}

	[Fact]
	public void GivenDeductionsExceedSalary_ItThrowsException()
	{
		//Arrange
		var employeeDto = new GetEmployeeDto
		{
			Salary = 25000m,
			Dependents = new List<GetDependentDto>
			{
				new()
				{
					DateOfBirth = new DateTime(1970, 3, 3)
				},
				new()
				{
					DateOfBirth = new DateTime(2000, 5, 18)
				}
			}
		};

		//Act & Assert
		Assert.Throws<InvalidOperationException>(() => _paycheckService.CalculateAnnualDeductionsForEmployee(employeeDto));
	}

	[Fact]
	public void GivenZeroSalary_ItThrowsException()
	{
		//Arrange
		var employeeDto = new GetEmployeeDto
		{
			Salary = 0m
		};

		//Act & Assert
		Assert.Throws<InvalidOperationException>(() => _paycheckService.CalculateAnnualDeductionsForEmployee(employeeDto));
	}

	[Fact]
	public void GivenDeductionsEqualSalary_ItReturnsZeroForNetPay()
	{
		//Arrange
		var employeeDto = new GetEmployeeDto
		{
			Salary = 26400m,
			Dependents = new List<GetDependentDto>
			{
				new()
				{
					DateOfBirth = new DateTime(2005, 6, 6)
				},
				new()
				{
					DateOfBirth = new DateTime(2012, 8, 9)
				}
			}
		};
		var expectedGrossPay = 1015.384m;
		var expectedDeductions = 1015.384m;

		//Act
		var result = _paycheckService.CalculatePaychecksForEmployee(employeeDto);
		var firstPaycheck = result.FirstOrDefault();

		//Assert
		Assert.Equal(26, result.Count());
		Assert.Equal(expectedGrossPay, firstPaycheck.GrossPay, 2);
		Assert.Equal(expectedDeductions, firstPaycheck.Deductions, 2);
		Assert.All(result, r => Assert.Equal(0, r.NetPay));
	}
}
