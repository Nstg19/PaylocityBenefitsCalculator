using Api.Dtos.Paycheck;
using Api.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests.IntegrationTests;
public class PaycheckIntegrationTests : IntegrationTest
{
	[Fact]
	public async Task GivenEmployee_ItReturnsAllPaychecksForEmployee()
	{
		var response = await HttpClient.GetAsync($"/api/v1/paychecks/by-employee/1");
		var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<GetPaycheckDto>>>(await response.Content.ReadAsStringAsync());

		Assert.Equal(26, apiResponse.Data?.Count);
	}

	[Fact]
	public async Task GivenNonExistentPaycheck_ItReturns404()
	{
		var response = await HttpClient.GetAsync($"/api/v1/paychecks/-1");

		await response.ShouldReturn(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task GivenNonExistentEmployee_ItReturns404()
	{
		var response = await HttpClient.GetAsync($"/api/v1/paychecks/by-employee/-1");

		await response.ShouldReturn(HttpStatusCode.NotFound);
	}
}
