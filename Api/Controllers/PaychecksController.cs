using Api.Business.Services.Interfaces;
using Api.Data.Models;
using Api.Dtos.Paycheck;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaychecksController : ControllerBase
{
	private readonly IPaycheckService _paycheckService;

	public PaychecksController(IPaycheckService paycheckService)
	{
		_paycheckService = paycheckService;
	}

	[SwaggerOperation(Summary = "Get paycheck by id")]
	[HttpGet("{id}")]
	public async Task<ActionResult<ApiResponse<GetPaycheckDto>>> Get(int id)
	{
		var paycheck = await _paycheckService.GetPaycheckByIdAsync(id);

		if (paycheck == null)
		{
			return NotFound();
		}

		var result = new ApiResponse<GetPaycheckDto>
		{
			Data = paycheck,
			Success = true
		};

		return result;
	}

	[SwaggerOperation(Summary = "Get all paychecks by employeeId")]
	[HttpGet("by-employee/{employeeId}")]
	public async Task<ActionResult<ApiResponse<List<GetPaycheckDto>>>> GetPaychecksByEmployeeId(int employeeId)
	{
		var paychecks = await _paycheckService.GetAllPaychecksByEmployeeIdAsync(employeeId);

		if (!paychecks.Any())
		{
			return NotFound();
		}

		var result = new ApiResponse<List<GetPaycheckDto>>
		{
			Data = paychecks,
			Success = true
		};

		return result;
	}
}
