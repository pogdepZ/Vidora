using AutoMapper;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Helpers;
using Vidora.Core.Interfaces.Api;
using Vidora.Infrastructure.Api.Clients;
using Vidora.Infrastructure.Api.Dtos.Responses;

namespace Vidora.Infrastructure.Api.Services;

public class StatsApiService : IStatsApiService
{
    private readonly ApiClient _apiClient;
    private readonly IMapper _mapper;

    public StatsApiService(ApiClient apiClient, IMapper mapper)
    {
        _apiClient = apiClient;
        _mapper = mapper;
    }

    public async Task<Result<AdminDashboardResult>> GetDashboardStatsAsync()
    {
        var response = await _apiClient.GetAsync("api/stats/dashboard");

        if (!response.IsSuccessStatusCode)
            return Result.Failure<AdminDashboardResult>("Không thể tải dữ liệu thống kê.");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var dto = await response.Content.ReadFromJsonAsync<DashboardResponse>(options);

        var result = _mapper.Map<AdminDashboardResult>(dto);
        return Result.Success(result);
    }
}
