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
using Vidora.Core.Interfaces.Api;
using Vidora.Infrastructure.Api.Dtos.Requests;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Services;

public class SubscriptionApiService : ISubscriptionApiService
{
    private readonly ApiClient _apiClient;
    private readonly IMapper _mapper;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SubscriptionApiService(ApiClient apiClient, IMapper mapper)
    {
        _apiClient = apiClient;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<SubscriptionPlanResult>>> GetPlansAsync(string token)
    {
        try
        {
            var response = await _apiClient.GetAsync("api/subscriptions/plans", token);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetPlansAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure<IReadOnlyList<SubscriptionPlanResult>>("Không th? t?i danh sách gói ??ng ký.");

            var responseDto = JsonSerializer.Deserialize<SubscriptionPlansResponseDto>(rawJson, _jsonOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<IReadOnlyList<SubscriptionPlanResult>>("D? li?u t? server không h?p l?.");

            var result = responseDto.Data
                .Select(dto => _mapper.Map<SubscriptionPlanResult>(dto))
                .ToList();

            return Result.Success<IReadOnlyList<SubscriptionPlanResult>>(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetPlansAsync] Error: {ex.Message}");
            return Result.Failure<IReadOnlyList<SubscriptionPlanResult>>($"L?i: {ex.Message}");
        }
    }

    public async Task<Result<PromoPaginationResult>> GetPromosAsync(string token, int page, int limit)
    {
        try
        {
            var query = $"api/promos?page={page}&limit={limit}";

            var response = await _apiClient.GetAsync(query, token);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetPromosAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure<PromoPaginationResult>("Không th? t?i danh sách mã gi?m giá.");

            var responseDto = JsonSerializer.Deserialize<PromoResponseDto>(rawJson, _jsonOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<PromoPaginationResult>("D? li?u t? server không h?p l?.");

            var result = _mapper.Map<PromoPaginationResult>(responseDto);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetPromosAsync] Error: {ex.Message}");
            return Result.Failure<PromoPaginationResult>($"L?i: {ex.Message}");
        }
    }

    public async Task<Result<PromoResult>> CreatePromoAsync(string token, CreatePromoCommand command)
    {
        try
        {
            var request = _mapper.Map<CreatePromoRequestDto>(command);

            var response = await _apiClient.PostCamelCaseAsync("api/promos", request, token);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[CreatePromoAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
            {
                // Try to parse error message from response
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<CreatePromoResponseDto>(rawJson, _jsonOptions);
                    return Result.Failure<PromoResult>(errorResponse?.Message ?? "Không th? t?o mã gi?m giá.");
                }
                catch
                {
                    return Result.Failure<PromoResult>("Không th? t?o mã gi?m giá.");
                }
            }

            var responseDto = JsonSerializer.Deserialize<CreatePromoResponseDto>(rawJson, _jsonOptions);

            if (responseDto == null || !responseDto.Success || responseDto.Data == null)
                return Result.Failure<PromoResult>(responseDto?.Message ?? "T?o mã gi?m giá th?t b?i.");

            var result = _mapper.Map<PromoResult>(responseDto.Data);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CreatePromoAsync] Error: {ex.Message}");
            return Result.Failure<PromoResult>($"L?i: {ex.Message}");
        }
    }

    public async Task<Result<OrderPaginationResult>> GetOrdersAsync(
        string token,
        int page,
        int limit,
        string? search = null,
        string? status = null,
        int? planId = null)
    {
        try
        {
            // Build query string
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

            var response = await _apiClient.GetAsync(query, token);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetOrdersAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure<OrderPaginationResult>("Không th? t?i danh sách ??n hàng.");

            var responseDto = JsonSerializer.Deserialize<OrderPaginationResponseDto>(rawJson, _jsonOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<OrderPaginationResult>("D? li?u t? server không h?p l?.");

            var result = _mapper.Map<OrderPaginationResult>(responseDto);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetOrdersAsync] Error: {ex.Message}");
            return Result.Failure<OrderPaginationResult>($"L?i: {ex.Message}");
        }
    }
}
