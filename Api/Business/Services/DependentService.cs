using Api.Business.Services.Interfaces;
using Api.Data.Models;
using Api.Data.Repositories;
using Api.Dtos.Dependent;
using Microsoft.EntityFrameworkCore;

namespace Api.Business.Services;

public class DependentService : IDependentService
{
	private readonly IRepository<Dependent> _dependentRepository;

	public DependentService(IRepository<Dependent> dependentRepository)
	{
		_dependentRepository = dependentRepository;
	}

	public async Task<GetDependentDto> GetDependentByIdAsync(int id)
	{
		var dependent = await _dependentRepository.GetAll(d => d.Id == id)
			.Select(d => new GetDependentDto
			{
				Id = d.Id,
				FirstName = d.FirstName,
				LastName = d.LastName,
				Relationship = d.Relationship,
				DateOfBirth = d.DateOfBirth
			})
			.FirstOrDefaultAsync();

		return dependent;
	}

	public async Task<List<GetDependentDto>> GetAllDependentsAsync()
	{
		var dependents = await _dependentRepository.GetAll()
			.Select(d => new GetDependentDto
			{
				Id = d.Id,
				FirstName = d.FirstName,
				LastName = d.LastName,
				Relationship = d.Relationship,
				DateOfBirth = d.DateOfBirth
			})
			.ToListAsync();

		return dependents;
	}
}
