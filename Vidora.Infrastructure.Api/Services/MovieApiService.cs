using AutoMapper;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Entities;
using Vidora.Core.Interfaces.Api;
using Vidora.Infrastructure.Api.Dtos.Responses;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Services
{
    public class MovieApiService : IMovieApiService
    {
        private readonly ApiClient _apiClient;
        private readonly IMapper _mapper; // Thêm Mapper

        public MovieApiService(ApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        public async Task<Result<MoviePaginationResult>> GetAdminMoviesAsync(
            string token,
            int page,
            int limit,
            string? title = null,
            int? genreId = null,
            int? releaseYear = null)
        {
            try
            {
                var query = $"api/movies?page={page}&limit={limit}";

                if (!string.IsNullOrWhiteSpace(title))
                    query += $"&title={Uri.EscapeDataString(title)}";

                if (genreId.HasValue)
                    query += $"&genreId={genreId.Value}";

                if (releaseYear.HasValue)
                    query += $"&releaseYear={releaseYear.Value}";

                var response = await _apiClient.GetAsync(query, token);

                var rawJson = await response.Content.ReadAsStringAsync();

                // Đặt breakpoint ở dòng này
                System.Diagnostics.Debug.WriteLine(rawJson);

                if (!response.IsSuccessStatusCode)
                    return Result.Failure<MoviePaginationResult>("Không thể tải danh sách phim.");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // 1. Đọc dữ liệu dưới dạng DTO Response (để tránh lỗi NullReference từ JSON)
                var responseDto = await response.Content.ReadFromJsonAsync<MoviePaginationResponseDto>(options);

                if (responseDto == null)
                    return Result.Failure<MoviePaginationResult>("Dữ liệu từ server trống.");

                // 2. Dùng AutoMapper để chuyển sang MoviePaginationResult của Core
                var result = _mapper.Map<MoviePaginationResult>(responseDto);

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<MoviePaginationResult>($"Lỗi: {ex.Message}");
            }
        }

        public async Task<Result<MovieDetailResult>> GetMovieDetailAsync(string token, int movieId)
        {
            try
            {
                var response = await _apiClient.GetAsync($"api/movies/{movieId}", token);
                var jsonString = await response.Content.ReadAsStringAsync();

                // Log ra để xem API thực tế trả về gì
                System.Diagnostics.Debug.WriteLine($"JSON THÔ: {jsonString}");

                // MovieApiService.cs
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    // THÊM DÒNG NÀY: Cho phép đọc số từ chuỗi và ngược lại
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                var resultDto = JsonSerializer.Deserialize<MovieDetailResponseDto>(jsonString, options);

                if (resultDto?.Success == true && resultDto.Data != null)
                {
                    return Result.Success(_mapper.Map<MovieDetailResult>(resultDto.Data));
                }
                return Result.Failure<MovieDetailResult>("Dữ liệu không hợp lệ");
            }
            catch (JsonException ex)
            {
                return Result.Failure<MovieDetailResult>($"Lỗi JSON: {ex.Message} tại {ex.Path}");
            }
            }

        public async Task<Result<bool>> ToggleDeleteMovieAsync(string token, int movieId)
        {
            // API thường dùng POST hoặc PATCH cho tính năng toggle
            var response = await _apiClient.PatchAsync($"api/movies/{movieId}/toggle-delete", null, token);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success(true);
            }

            var error = await response.Content.ReadAsStringAsync();
            return Result.Failure<bool>($"Không thể thay đổi trạng thái phim: {error}");
        }

        public async Task<Result<List<GenreResult>>> GetGenresAsync(string token)
        {
            var response = await _apiClient.GetAsync("api/movies/genres", token);

            if (!response.IsSuccessStatusCode)
                return Result.Failure<List<GenreResult>>("Không thể lấy danh sách thể loại từ máy chủ");

            try
            {
                // DEBUG: In ra JSON thô
                var rawJson = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[GetGenresAsync] Raw JSON: {rawJson}");

                // Reset stream position
                var resultDto = System.Text.Json.JsonSerializer.Deserialize<GenreResponseDto>(rawJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultDto != null && resultDto.Success)
                {
                    var genres = resultDto.Data.Select(d => new GenreResult
                    {
                        Id = d.Id,
                        Name = d.Name
                    }).ToList();

                    System.Diagnostics.Debug.WriteLine($"[GetGenresAsync] Parsed {genres.Count} genres");
                    foreach (var g in genres)
                    {
                        System.Diagnostics.Debug.WriteLine($"  -> Genre Id: {g.Id}, Name: {g.Name}");
                    }

                    return Result.Success(genres);
                }

                return Result.Failure<List<GenreResult>>("API trả về trạng thái không thành công");
            }
            catch (Exception ex)
            {
                return Result.Failure<List<GenreResult>>($"Lỗi xử lý dữ liệu: {ex.Message}");
            }
        }

        public async Task<Result<GenreResult>> CreateGenreAsync(string token, string name)
        {
            // Node.js thường dùng body { name: "..." }
            var response = await _apiClient.PostAsync("api/movies/genres", new { name }, token);
            if (!response.IsSuccessStatusCode) return Result.Failure<GenreResult>("Lỗi tạo thể loại mới");

            var result = await response.Content.ReadFromJsonAsync<GenreResult>();
            return Result.Success(result!);
        }

        // --- ACTORS ---

        public async Task<Result<MemberResult>> CreateActorAsync(string token, string name)
        {
            var response = await _apiClient.PostAsync("api/movies/members", new { name }, token);
            if (!response.IsSuccessStatusCode) return Result.Failure<MemberResult>("Lỗi tạo diễn viên mới");

            var result = await response.Content.ReadFromJsonAsync<MemberResult>();
            return Result.Success(result!);
        }

        public async Task<Result<List<MemberResult>>> GetMembersAsync(string token)
        {
            var response = await _apiClient.GetAsync("api/movies/members", token);
            if (!response.IsSuccessStatusCode) return Result.Failure<List<MemberResult>>("Lỗi lấy thành viên");

            // DEBUG: In ra JSON thô
            var rawJson = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[GetMembersAsync] Raw JSON: {rawJson}");

            var resultDto = System.Text.Json.JsonSerializer.Deserialize<MemberResponseDto>(rawJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (resultDto?.Success == true)
            {
                var members = resultDto.Data.Select(d => new MemberResult(d.Id, d.Name)).ToList();

                System.Diagnostics.Debug.WriteLine($"[GetMembersAsync] Parsed {members.Count} members");
                foreach (var m in members)
                {
                    System.Diagnostics.Debug.WriteLine($"  -> Member Id: {m.Id}, Name: {m.Name}");
                }

                return Result.Success(members);
            }
            return Result.Failure<List<MemberResult>>("Dữ liệu thành viên không hợp lệ");
        }

        // --- MOVIES ---

        public async Task<Result<bool>> CreateMovieAsync(string token, object movieData)
        {
            // --- ĐOẠN CODE DEBUG ---
            var jsonDebug = JsonSerializer.Serialize(movieData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            System.Diagnostics.Debug.WriteLine("================= DATA GỬI ĐI =================");
            System.Diagnostics.Debug.WriteLine(jsonDebug);
            System.Diagnostics.Debug.WriteLine("===============================================");

            var response = await _apiClient.PostAsync("api/movies", movieData, token);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success(true);
            }

            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"❌ API ERROR: {error}");

            return Result.Failure<bool>($"Lỗi tạo phim: {error}");
        }

        public async Task<Result<bool>> UpdateMovieAsync(string token, int movieId, object movieData)
        {
            // --- ĐOẠN CODE DEBUG ---
            var jsonDebug = JsonSerializer.Serialize(movieData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            System.Diagnostics.Debug.WriteLine("================= UPDATE MOVIE DATA =================");
            System.Diagnostics.Debug.WriteLine($"Movie ID: {movieId}");
            System.Diagnostics.Debug.WriteLine(jsonDebug);
            System.Diagnostics.Debug.WriteLine("=====================================================");

            var response = await _apiClient.PutAsync($"api/movies/{movieId}", movieData, token);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success(true);
            }

            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"❌ API UPDATE ERROR: {error}");

            return Result.Failure<bool>($"Lỗi cập nhật phim: {error}");
        }
    }
}