using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Vidora.Core.Helpers;
using Vidora.Infrastructure.Api.Options;

namespace Vidora.Infrastructure.Api.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    // CamelCase options cho các API yêu cầu camelCase (như movies API)
    public static readonly JsonSerializerOptions CamelCaseOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public ApiClient(IOptions<ApiOptions> options)
    {
        var apiOptions = options.Value;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiOptions.BaseUrl),
            Timeout = TimeSpan.FromSeconds(apiOptions.Timeout)
        };
    }

    public Task<HttpResponseMessage> GetAsync(
        string url,
        string? token = null,
        Action<HttpRequestHeaders>? headers = null,
        CancellationToken ct = default)
        => SendAsync(HttpMethod.Get, url, null, token, headers, ct);

    public Task<HttpResponseMessage> PostAsync(
        string url,
        object? body,
        string? token = null,
        Action<HttpRequestHeaders>? headers = null,
        CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, url, body, token, headers, ct);

    // POST với CamelCase
    public Task<HttpResponseMessage> PostCamelCaseAsync(
        string url,
        object? body,
        string? token = null,
        Action<HttpRequestHeaders>? headers = null,
        CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, url, body, token, headers, ct, CamelCaseOptions);

    public Task<HttpResponseMessage> PutAsync(
        string url,
        object? body,
        string? token = null,
        Action<HttpRequestHeaders>? headers = null,
        CancellationToken ct = default)
        => SendAsync(HttpMethod.Put, url, body, token, headers, ct);

    // PUT với CamelCase
    public Task<HttpResponseMessage> PutCamelCaseAsync(
        string url,
        object? body,
        string? token = null,
        Action<HttpRequestHeaders>? headers = null,
        CancellationToken ct = default)
        => SendAsync(HttpMethod.Put, url, body, token, headers, ct, CamelCaseOptions);

    public Task<HttpResponseMessage> DeleteAsync(
        string url,
        string? token = null,
        Action<HttpRequestHeaders>? headers = null,
        CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, url, null, token, headers, ct);

    public Task<HttpResponseMessage> PatchAsync(
        string url,
        object? body,
        string? token = null,
        Action<HttpRequestHeaders>? headers = null,
        CancellationToken ct = default)
        => SendAsync(HttpMethod.Patch, url, body, token, headers, ct);

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        object? body,
        string? token,
        Action<HttpRequestHeaders>? headers,
        CancellationToken ct,
        JsonSerializerOptions? jsonOptions = null)
    {
        using var request = new HttpRequestMessage(method, url);

        // JSON body - sử dụng options được cung cấp hoặc mặc định là SnakeCaseOptions
        if (body != null)
            request.Content = JsonContent.Create(body, options: jsonOptions ?? JsonHelper.SnakeCaseOptions);

        // Bearer token
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Extra headers
        headers?.Invoke(request.Headers);

        // Send request
        var response = await _httpClient.SendAsync(request, ct);

        return response;
    }
}