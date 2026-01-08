using AutoMapper;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Helpers;
using Vidora.Core.Interfaces.Api;
using Vidora.Infrastructure.Api.Clients;
using Vidora.Infrastructure.Api.Dtos.Requests;
using Vidora.Infrastructure.Api.Dtos.Responses;

namespace Vidora.Infrastructure.Api.Services;

public class SubscriptionApiService : ISubscriptionApiService
{
    private readonly ApiClient _apiClient;
    private readonly IMapper _mapper;

    public SubscriptionApiService(ApiClient apiClient, IMapper mapper)
    {
        _apiClient = apiClient;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<SubscriptionPlanResult>>> GetPlansAsync()
    {
        try
        {
            var response = await _apiClient.GetAsync("api/subscriptions/plans");
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetPlansAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure<IReadOnlyList<SubscriptionPlanResult>>("Không thể tải danh sách gói đăng ký.");

            var responseDto = JsonSerializer.Deserialize<SubscriptionPlansResponseDto>(rawJson, JsonHelper.CamelCaseOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<IReadOnlyList<SubscriptionPlanResult>>("Dữ liệu từ server không hợp lệ.");

            var result = responseDto.Data
                .Select(dto => _mapper.Map<SubscriptionPlanResult>(dto))
                .ToList();

            return Result.Success<IReadOnlyList<SubscriptionPlanResult>>(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetPlansAsync] Error: {ex.Message}");
            return Result.Failure<IReadOnlyList<SubscriptionPlanResult>>($"Lỗi: {ex.Message}");
        }
    }

    public async Task<Result<PromoPaginationResult>> GetPromosAsync(int page, int limit)
    {
        try
        {
            var query = $"api/promos?page={page}&limit={limit}";

            var response = await _apiClient.GetAsync(query);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetPromosAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure<PromoPaginationResult>("Không thể tải danh sách mã giảm giá.");

            var responseDto = JsonSerializer.Deserialize<PromoResponse>(rawJson, JsonHelper.CamelCaseOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<PromoPaginationResult>("Dữ liệu từ server không hợp lệ.");

            var result = _mapper.Map<PromoPaginationResult>(responseDto);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetPromosAsync] Error: {ex.Message}");
            return Result.Failure<PromoPaginationResult>($"Lỗi: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CreatePromoAsync(CreatePromoCommand command)
    {
        var request = _mapper.Map<CreatePromoRequestDto>(command);

        var response = await _apiClient.PostAsync("api/promos", request);
        var rawJson = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"[CreatePromoAsync] Raw JSON: {rawJson}");

        if (!response.IsSuccessStatusCode) return Result.Failure<bool>("Lỗi tạo phiếu giảm giá mới");
        return Result.Success(true);
    }

    public async Task<Result<OrderPaginationResult>> GetOrdersAsync(
        int page,
        int limit,
        string? search = null,
        string? status = null,
        int? planId = null)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"limit={limit}"
            };

            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"search={HttpUtility.UrlEncode(search)}");

            if (!string.IsNullOrWhiteSpace(status))
                queryParams.Add($"status={status}");

            if (planId.HasValue)
                queryParams.Add($"planId={planId.Value}");

            var query = $"api/orders/all?{string.Join("&", queryParams)}";

            var response = await _apiClient.GetAsync(query);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetOrdersAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure<OrderPaginationResult>("Không thể tải danh sách đơn hàng.");

            var responseDto = JsonSerializer.Deserialize<OrderPaginationResponseDto>(rawJson, JsonHelper.CamelCaseOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<OrderPaginationResult>("Dữ liệu từ server không hợp lệ.");

            var result = _mapper.Map<OrderPaginationResult>(responseDto);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetOrdersAsync] Error: {ex.Message}");
            return Result.Failure<OrderPaginationResult>($"Lỗi: {ex.Message}");
        }
    }
}
