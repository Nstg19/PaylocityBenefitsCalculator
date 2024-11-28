using Api.Dtos.Dependent;

namespace Api.Business.Services.Interfaces;

public interface IDependentService
{
	Task<GetDependentDto> GetDependentByIdAsync(int id);
	Task<List<GetDependentDto>> GetAllDependentsAsync();
}
