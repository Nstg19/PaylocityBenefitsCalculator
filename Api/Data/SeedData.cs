using Api.Data.Models;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;

namespace Api.Data;

public class SeedData : IHostedService
{
	private readonly IServiceProvider _serviceProvider;

	public SeedData(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	/// <summary>
	/// Start a background task.
	/// Create a sqlite database.
	/// Seed data in the database.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();

		var employeeDbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();

		await employeeDbContext.Database.EnsureCreatedAsync(cancellationToken);

		if (!employeeDbContext.Employees.Any())
		{
			await SeedDataAsync(employeeDbContext);
		}
	}

	/// <summary>
	/// Add employee and dependents in the database.
	/// </summary>
	/// <param name="employeeDbContext"></param>
	/// <returns></returns>
	public async Task SeedDataAsync(EmployeeDbContext employeeDbContext)
	{
		var employeeDtos = GetEmployeeDtos();

		foreach (var employeeDto in employeeDtos)
		{
			var employee = new Employee
			{
				Id = employeeDto.Id,
				FirstName = employeeDto.FirstName,
				LastName = employeeDto.LastName,
				DateOfBirth = employeeDto.DateOfBirth,
				Salary = employeeDto.Salary
			};

			var employeeEntity = await employeeDbContext.Employees.AddAsync(employee);

			//Validate employee having multiple spouse/partner.
			var spousePartner = employeeDto.Dependents.Where(d => d.Relationship == Relationship.DomesticPartner || d.Relationship == Relationship.Spouse).ToList();
			if (spousePartner.Count() > 1)
			{
				foreach (var sp in spousePartner.Skip(1))
				{
					employeeDto.Dependents.Remove(sp);
				}
			}

			foreach (var dependentDto in employeeDto.Dependents)
			{
				var dependent = new Dependent
				{
					Id = dependentDto.Id,
					FirstName = dependentDto.FirstName,
					LastName = dependentDto.LastName,
					DateOfBirth = dependentDto.DateOfBirth,
					Relationship = dependentDto.Relationship,
					EmployeeId = employeeEntity.Entity.Id
				};

				await employeeDbContext.Dependents.AddAsync(dependent);
			}
		}

		await employeeDbContext.SaveChangesAsync();
	}

	/// <summary>
	/// Background task completed.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private List<GetEmployeeDto> GetEmployeeDtos()
	{
		return new List<GetEmployeeDto>
		{
			new()
			{
				Id = 1,
				FirstName = "LeBron",
				LastName = "James",
				Salary = 75420.99m,
				DateOfBirth = new DateTime(1984, 12, 30)
			},
			new()
			{
				Id = 2,
				FirstName = "Ja",
				LastName = "Morant",
				Salary = 92365.22m,
				DateOfBirth = new DateTime(1999, 8, 10),
				Dependents = new List<GetDependentDto>
				{
					new()
					{
						Id = 1,
						FirstName = "Spouse",
						LastName = "Morant",
						Relationship = Relationship.Spouse,
						DateOfBirth = new DateTime(1998, 3, 3)
					},
					new()
					{
						Id = 2,
						FirstName = "Child1",
						LastName = "Morant",
						Relationship = Relationship.Child,
						DateOfBirth = new DateTime(2020, 6, 23)
					},
					new()
					{
						Id = 3,
						FirstName = "Child2",
						LastName = "Morant",
						Relationship = Relationship.Child,
						DateOfBirth = new DateTime(2021, 5, 18)
					}
				}
			},
			new()
			{
				Id = 3,
				FirstName = "Michael",
				LastName = "Jordan",
				Salary = 143211.12m,
				DateOfBirth = new DateTime(1963, 2, 17),
				Dependents = new List<GetDependentDto>
				{
					new()
					{
						Id = 4,
						FirstName = "DP",
						LastName = "Jordan",
						Relationship = Relationship.DomesticPartner,
						DateOfBirth = new DateTime(1974, 1, 2)
					}
				}
			}
		};
	}
}
