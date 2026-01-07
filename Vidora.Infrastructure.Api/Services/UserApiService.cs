using AutoMapper;
using CSharpFunctionalExtensions;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Interfaces.Api;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Services;

public class UserApiService : IUserApiService
{
    private readonly ApiClient _apiClient;
    private readonly IMapper _mapper;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UserApiService(ApiClient apiClient, IMapper mapper)
    {
        _apiClient = apiClient;
        _mapper = mapper;
    }

    public async Task<Result<UserPaginationResult>> GetUsersAsync(
        string token,
        int page,
        int limit,
        string? search = null,
        string? email = null,
        string? username = null,
        string? role = null,
        string? status = null)
    {
        try
        {
            var query = $"api/users?page={page}&limit={limit}";

            if (!string.IsNullOrWhiteSpace(search))
                query += $"&search={Uri.EscapeDataString(search.Trim())}";

            if (!string.IsNullOrWhiteSpace(email))
                query += $"&email={Uri.EscapeDataString(email.Trim())}";

            if (!string.IsNullOrWhiteSpace(username))
                query += $"&username={Uri.EscapeDataString(username.Trim())}";

            if (!string.IsNullOrWhiteSpace(role))
                query += $"&role={Uri.EscapeDataString(role.Trim().ToLowerInvariant())}";

            if (!string.IsNullOrWhiteSpace(status))
                query += $"&status={Uri.EscapeDataString(status.Trim().ToLowerInvariant())}";

            var response = await _apiClient.GetAsync(query, token);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetUsersAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure<UserPaginationResult>("Không thể tải danh sách người dùng.");

            var responseDto = JsonSerializer.Deserialize<UserPaginationResponseDto>(rawJson, _jsonOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<UserPaginationResult>("Dữ liệu từ server không hợp lệ.");

            var result = _mapper.Map<UserPaginationResult>(responseDto);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetUsersAsync] Error: {ex.Message}");
            return Result.Failure<UserPaginationResult>($"Lỗi: {ex.Message}");
        }
    }

    public async Task<Result<UserDetailResult>> GetUserDetailAsync(string token, int userId)
    {
        try
        {
            var response = await _apiClient.GetAsync($"api/users/{userId}", token);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[GetUserDetailAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure<UserDetailResult>("Không thể tải thông tin chi tiết người dùng.");

            var responseDto = JsonSerializer.Deserialize<UserDetailResponseDto>(rawJson, _jsonOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<UserDetailResult>("Dữ liệu chi tiết người dùng không hợp lệ.");

            var result = _mapper.Map<UserDetailResult>(responseDto.Data);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetUserDetailAsync] Error: {ex.Message}");
            return Result.Failure<UserDetailResult>($"Lỗii: {ex.Message}");
        }
    }

    public async Task<Result<string>> ToggleUserStatusAsync(string token, int userId)
    {
        try
        {
            var response = await _apiClient.PutAsync($"api/users/{userId}/status", null, token);
            var rawJson = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[ToggleUserStatusAsync] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<string>($"Không thể thay đổii trạng thái người dùng: {rawJson}");
            }

            var responseDto = JsonSerializer.Deserialize<UserStatusResponseDto>(rawJson, _jsonOptions);

            if (responseDto == null || !responseDto.Success)
                return Result.Failure<string>("Cập nhật trạng thái thất bại.");

            return Result.Success(responseDto.Data?.Status ?? "UNKNOWN");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ToggleUserStatusAsync] Error: {ex.Message}");
            return Result.Failure<string>($"Lỗi: {ex.Message}");
        }
    }
}
