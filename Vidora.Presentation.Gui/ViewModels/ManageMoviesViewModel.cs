using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Entities;
using Vidora.Core.Interfaces.Api;
using Vidora.Core.Services;
using Vidora.Core.UseCases;
using Vidora.Infrastructure.Api.Dtos.Responses;
using Vidora.Infrastructure.Api.Services;
using Vidora.Presentation.Gui.Contracts.ViewModels;

namespace Vidora.Presentation.Gui.ViewModels;

public partial class ManageMoviesViewModel : ObservableRecipient, INavigationAware
{
    private readonly GetMoviesUseCase _getMoviesUseCase;
    private readonly GetMovieDetailUseCase _getMovieDetailUseCase;
    private readonly DeleteMovieUseCase _deleteMovieUseCase;
    private readonly CreateMovieUseCase _createMovieUseCase;
    private readonly UpdateMovieUseCase _updateMovieUseCase;

    private readonly IMovieApiService _movieApiService;
    private readonly ISessionStateService _sessionService;

    // Property to hold specific error message from API
    [ObservableProperty]
    private string _errorMessage;

    // Property to check success/failure status
    [ObservableProperty]
    private bool _isSuccess;

    [ObservableProperty]
    private ObservableCollection<AdminMovie> _movies = new();

    [ObservableProperty]
    private PaginationResult _pagination = new(1, 5, 0, 1);

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _searchText = string.Empty;

    // Filter properties
    [ObservableProperty]
    private ObservableCollection<GenreResult> _filterGenres = new();

    [ObservableProperty]
    private GenreResult? _selectedFilterGenre;

    [ObservableProperty]
    private int? _filterReleaseYear;

    [ObservableProperty]
    private string _filterReleaseYearText = string.Empty;

    // Edit movie properties
    [ObservableProperty] private int _editMovieId;
    [ObservableProperty] private string _editMovieTitle = string.Empty;
    [ObservableProperty] private string _editMovieDescription = string.Empty;
    [ObservableProperty] private string _editMoviePosterUrl = string.Empty;
    [ObservableProperty] private string _editMovieBannerUrl = string.Empty;
    [ObservableProperty] private string _editMovieTrailerUrl = string.Empty;
    [ObservableProperty] private string _editMovieUrl = string.Empty;
    [ObservableProperty] private int _editMovieReleaseYear = DateTime.Now.Year;

    public List<Genre> EditSelectedGenres { get; } = new();
    public ObservableCollection<(MemberResult Member, string Role)> EditSelectedCast { get; } = new();

    public ManageMoviesViewModel(
        GetMoviesUseCase getMoviesUseCase,
        GetMovieDetailUseCase getMovieDetailUseCase,
        DeleteMovieUseCase deleteMovieUseCase,
        CreateMovieUseCase createMovieUseCase,
        UpdateMovieUseCase updateMovieUseCase,
        IMovieApiService movieApiService,
        ISessionStateService sessionService)
    {
        _getMoviesUseCase = getMoviesUseCase;
        _getMovieDetailUseCase = getMovieDetailUseCase;
        _deleteMovieUseCase = deleteMovieUseCase;
        _createMovieUseCase = createMovieUseCase;
        _updateMovieUseCase = updateMovieUseCase;
        _movieApiService = movieApiService;
        _sessionService = sessionService;
    }

    [ObservableProperty] private string _newMovieTitle = string.Empty;
    [ObservableProperty] private string _newMovieDescription = string.Empty;
    [ObservableProperty] private string _newMoviePosterUrl = string.Empty;
    [ObservableProperty] private string _newMovieBannerUrl = string.Empty;
    [ObservableProperty] private string _newMovieTrailerUrl = string.Empty;
    [ObservableProperty] private string _newMovieUrl = string.Empty;
    [ObservableProperty] private int _newMovieReleaseYear = DateTime.Now.Year;

    [ObservableProperty] private string _newMovieDirector = string.Empty;
    [ObservableProperty] private string _newMovieGenresRaw = string.Empty;
    [ObservableProperty] private string _newMovieActorsRaw = string.Empty;

    #region Properties for UI Binding
    public int CurrentPage => Pagination.Page;
    public int TotalPages => Pagination.TotalPages;
    public bool CanGoPrev => Pagination.HasPrev;
    public bool CanGoNext => Pagination.HasNext;
    public int ItemsCount => Pagination.Count;
    #endregion

    [RelayCommand]
    private async Task LoadMoviesAsync()
    {
        if (IsLoading) return;
        IsLoading = true;

        try
        {
            // Parse release year from text
            int? releaseYear = null;
            if (int.TryParse(FilterReleaseYearText, out var year) && year > 1900 && year <= DateTime.Now.Year + 5)
            {
                releaseYear = year;
            }

            // Get genreId from selected filter (Id = 0 means "All genres")
            int? genreId = SelectedFilterGenre?.Id > 0 ? SelectedFilterGenre.Id : null;

            // 1. Lấy danh sách phim từ trang hiện tại với filter
            var result = await _getMoviesUseCase.ExecuteAsync(
                CurrentPage,
                5,
                string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                genreId,
                releaseYear);

            if (result.IsSuccess)
            {
                var fetchedMovies = result.Value.Movies.ToList();

                // 2. Chạy lấy chi tiết cho TẤT CẢ các phim cùng một lúc (Parallel)
                // Chúng ta await ở đây để gom đủ thông tin rồi mới hiện lên UI
                var detailTasks = fetchedMovies.Select(async movie =>
                {
                    System.Diagnostics.Debug.WriteLine($"---> Đang kiểm tra phim: {movie.Title} | ID: {movie.Id}");
                    var detailResult = await _getMovieDetailUseCase.ExecuteAsync(movie.Id);
                    //if (detailResult.IsSuccess)
                    //{
                    //    // Lấy thẳng DirectorName đã được Record tính toán sẵn
                    //    movie.DirectorName = detailResult.Value.DirectorName;
                    //}
                    //else
                    //{
                    //    movie.DirectorName = "N/A";
                    //}
                    if (detailResult.IsSuccess)
                    {
                        var detail = detailResult.Value;

                        // --- ĐOẠN DEBUG BẮT ĐẦU ---
                        System.Diagnostics.Debug.WriteLine($"==== DEBUG PHIM: {detail.Title} (ID: {detail.MovieId}) ====");

                        if (detail.Actors == null)
                        {
                            System.Diagnostics.Debug.WriteLine("LỖI: Mảng Actors bị NULL (Có thể do Mapping DTO)");
                        }
                        else if (detail.Actors.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("LỖI: Mảng Actors RỖNG (API không trả về actor hoặc sai tên key JSON)");
                        }
                        else
                        {
                            foreach (var actor in detail.Actors)
                            {
                                System.Diagnostics.Debug.WriteLine($"Actor: {actor.Name} | Role: '{actor.Role}'");
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"=> Kết quả DirectorName: {detail.DirectorName}");
                        System.Diagnostics.Debug.WriteLine("================================================");
                        // --- ĐOẠN DEBUG KẾT THÚC ---

                        movie.DirectorName = detail.DirectorName;
                    }
                });

                await Task.WhenAll(detailTasks); // Đợi tất cả API chi tiết xong

                // 3. Sau khi đã có đủ Director, mới đổ vào ObservableCollection
                Movies.Clear();
                foreach (var movie in fetchedMovies)
                {
                    Movies.Add(movie);
                }

                Pagination = result.Value.Pagination;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Lỗi hệ thống: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            NotifyPaginationPropertiesChanged();
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        // Reset về trang 1 khi search/filter
        Pagination = Pagination with { Page = 1 };
        await LoadMoviesAsync();
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        SearchText = string.Empty;
        SelectedFilterGenre = null;
        FilterReleaseYearText = string.Empty;
        FilterReleaseYear = null;
        Pagination = Pagination with { Page = 1 };
        await LoadMoviesAsync();
    }

    public async Task LoadFilterGenresAsync()
    {
        var token = _sessionService.CurrentSession?.AccessToken?.Token?.Trim('"');
        var apiResult = await _movieApiService.GetGenresAsync();

        if (apiResult.IsSuccess)
        {
            FilterGenres.Clear();
            // Add empty option for "All genres"
            FilterGenres.Add(new GenreResult { Id = 0, Name = "All Genres" });
            foreach (var item in apiResult.Value)
            {
                FilterGenres.Add(item);
            }
        }
    }

    public async Task<MovieDetailResult?> GetMovieDetailForEditAsync(int movieId)
    {
        var result = await _getMovieDetailUseCase.ExecuteAsync(movieId);
        if (result.IsSuccess)
        {
            var detail = result.Value;

            // Populate edit properties
            EditMovieId = detail.MovieId;
            EditMovieTitle = detail.Title;
            EditMovieDescription = detail.Description;
            EditMoviePosterUrl = detail.PosterUrl ?? string.Empty;
            EditMovieBannerUrl = detail.BannerUrl ?? string.Empty;
            EditMovieTrailerUrl = detail.TrailerUrl ?? string.Empty;
            EditMovieUrl = detail.MovieUrl ?? string.Empty;
            EditMovieReleaseYear = detail.ReleaseYear;

            return detail;
        }
        return null;
    }

    [RelayCommand]
    private async Task UpdateMovieAsync()
    {
        if (string.IsNullOrWhiteSpace(EditMovieTitle)) return;
        IsSuccess = false; // Reset trạng thái trước khi gọi API
        ErrorMessage = string.Empty;

        IsLoading = true;
        try
        {
            var genreIds = EditSelectedGenres.Select(g => g.Id).ToList();
            var castAndCrew = EditSelectedCast.Select(c => (c.Member.Id, c.Role)).ToList();

            var command = new UpdateMovieCommand(
                movieId: EditMovieId,
                title: EditMovieTitle,
                description: EditMovieDescription,
                posterUrl: EditMoviePosterUrl,
                bannerUrl: EditMovieBannerUrl,
                trailerUrl: EditMovieTrailerUrl,
                movieUrl: EditMovieUrl,
                releaseYear: EditMovieReleaseYear,
                genreIds: genreIds,
                castAndCrew: castAndCrew
            );

            var result = await _updateMovieUseCase.ExecuteAsync(command);
            IsSuccess = result.IsSuccess;

            if (result.IsSuccess)
            {
                ResetEditForm();
                await LoadMoviesAsync();
                System.Diagnostics.Debug.WriteLine("Cập nhật phim thành công!");
            }
            else
            {
                ErrorMessage = result.Error ?? "Lỗi cập nhật phim.";
                System.Diagnostics.Debug.WriteLine($"Lỗi cập nhật phim: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            IsSuccess = false;
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"Lỗi hệ thống khi cập nhật: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ResetEditForm()
    {
        EditMovieId = 0;
        EditMovieTitle = string.Empty;
        EditMovieDescription = string.Empty;
        EditMoviePosterUrl = string.Empty;
        EditMovieBannerUrl = string.Empty;
        EditMovieTrailerUrl = string.Empty;
        EditMovieUrl = string.Empty;
        EditMovieReleaseYear = DateTime.Now.Year;
        EditSelectedGenres.Clear();
        EditSelectedCast.Clear();
    }

    [RelayCommand]
    private async Task ToggleDeleteAsync(AdminMovie movie)
    {
        if (movie == null) return;

        IsSuccess = false;
        ErrorMessage = string.Empty;

        // Gọi UseCase - Token đã được UseCase tự lấy từ SessionService
        var result = await _deleteMovieUseCase.ExecuteAsync(movie.Id);
        IsSuccess = result.IsSuccess;

        if (result.IsSuccess)
        {
            // Xóa phim khỏi UI
            Movies.Remove(movie);

            // Cập nhật lại số lượng hiển thị trên phân trang
            Pagination = Pagination with { Total = Math.Max(0, Pagination.Total - 1) };
            NotifyPaginationPropertiesChanged();
        }
        else
        {
            ErrorMessage = result.Error ?? "Lỗi khi xóa phim.";
            // Log lỗi hoặc hiển thị Dialog báo lỗi cho người dùng
            System.Diagnostics.Debug.WriteLine($"[ToggleDelete Error]: {result.Error}");
        }
    }

    [RelayCommand]
    private async Task ChangePageAsync(string direction)
    {
        if (IsLoading) return;

        int targetPage = CurrentPage;

        // Kiểm tra xem CanGoNext có bằng True không?
        // Nếu TotalItems map đúng là 24, CanGoNext sẽ tự động là True.
        if (direction == "Next" && CanGoNext) targetPage++;
        else if (direction == "Prev" && CanGoPrev) targetPage--;

        if (targetPage != CurrentPage)
        {
            Pagination = Pagination with { Page = targetPage };
            await LoadMoviesAsync(); // Nó sẽ gọi lại UseCase với page mới (ví dụ trang 2)
        }
    }

    [RelayCommand]
    private async Task AddMovieAsync()
    {
        if (string.IsNullOrWhiteSpace(NewMovieTitle)) return;

        IsSuccess = false; // Reset trạng thái trước khi gọi API
        ErrorMessage = string.Empty;

        IsLoading = true;
        try
        {
            // 1. Lấy danh sách GenreIds từ SelectedGenres
            var genreIds = SelectedGenres.Select(g => g.Id).ToList();

            // 2. Lấy danh sách CastAndCrew từ SelectedCast
            var castAndCrew = SelectedCast.Select(c => (c.Member.Id, c.Role)).ToList();

            var command = new CreateMovieCommand(
                title: NewMovieTitle,
                description: NewMovieDescription,
                posterUrl: NewMoviePosterUrl,
                bannerUrl: NewMovieBannerUrl,
                trailerUrl: NewMovieTrailerUrl,
                movieUrl: NewMovieUrl,
                releaseYear: NewMovieReleaseYear,
                genreIds: genreIds,
                castAndCrew: castAndCrew
            );

            // 3. Gọi UseCase
            var result = await _createMovieUseCase.ExecuteAsync(command);
            IsSuccess = result.IsSuccess;

            if (result.IsSuccess)
            {
                // 4. Làm sạch Form sau khi thêm thành công
                ResetAddForm();

                // 5. Load lại danh sách để thấy phim mới
                await LoadMoviesAsync();

                System.Diagnostics.Debug.WriteLine("Thêm phim thành công!");
            }
            else
            {
                ErrorMessage = result.Error ?? "Không thể tạo phim.";
                System.Diagnostics.Debug.WriteLine($"Lỗi thêm phim: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            IsSuccess = false;
            ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Lỗi hệ thống khi thêm: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Danh sách tất cả thể loại tải từ DB
    [ObservableProperty]
    private ObservableCollection<Genre> _availableGenres = new();

    // Danh sách các thể loại được người dùng tích chọn
    public List<Genre> SelectedGenres { get; } = new();

    // Hàm tải danh sách thể loại (Gọi khi mở Popup hoặc khi vào trang)
    public async Task LoadAvailableGenresAsync()
    {
        var token = _sessionService.CurrentSession?.AccessToken?.Token?.Trim('"');
        var apiResult = await _movieApiService.GetGenresAsync();

        if (apiResult.IsSuccess)
        {
            AvailableGenres.Clear();
            foreach (var item in apiResult.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[Genre] Id: {item.Id}, Name: {item.Name}");
                AvailableGenres.Add(new Vidora.Core.Entities.Genre
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }
        }
    }

    // Trong ViewModel
    // Sử dụng Tuple để giữ MemberResult và chuỗi Role cố định
    public ObservableCollection<(MemberResult Member, string Role)> SelectedCast { get; } = new();

    // Đảm bảo có hàm load Member
    [ObservableProperty]
    private ObservableCollection<MemberResult> _availableMembers = new();

    public async Task LoadAvailableMembersAsync()
    {
        var token = _sessionService?.CurrentSession?.AccessToken?.Token?.Trim('"');
        var result = await _movieApiService.GetMembersAsync();
        if (result.IsSuccess)
        {
            AvailableMembers.Clear();
            foreach (var m in result.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[Member] Id: {m.Id}, Name: {m.Name}");
                AvailableMembers.Add(m);
            }
        }
    }

    private void ResetAddForm()
    {
        NewMovieTitle = string.Empty;
        NewMovieDescription = string.Empty;
        NewMoviePosterUrl = string.Empty;
        NewMovieBannerUrl = string.Empty;
        NewMovieTrailerUrl = string.Empty;
        NewMovieUrl = string.Empty;
        NewMovieReleaseYear = DateTime.Now.Year;
        NewMovieDirector = string.Empty;
        NewMovieGenresRaw = string.Empty;
        NewMovieActorsRaw = string.Empty;
        SelectedGenres.Clear();
        SelectedCast.Clear();
    }

    private void NotifyPaginationPropertiesChanged()
    {
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanGoPrev));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(ItemsCount));
    }

    public async Task OnNavigatedToAsync(object parameter)
    {
        await LoadFilterGenresAsync();
        await LoadMoviesAsync();
    }

    public Task OnNavigatedFromAsync() => Task.CompletedTask;

    #region Import Excel Properties

    [ObservableProperty]
    private bool _isImporting;

    [ObservableProperty]
    private int _importTotalCount;

    [ObservableProperty]
    private int _importCurrentIndex;

    [ObservableProperty]
    private int _importSuccessCount;

    [ObservableProperty]
    private int _importFailedCount;

    [ObservableProperty]
    private string _importStatusMessage = string.Empty;

    [ObservableProperty]
    private double _importProgress;

    #endregion

    #region Import Excel Methods

    /// <summary>
    /// Read Excel file and return list of CreateMovieCommand
    /// Columns: Title | Description | ReleaseYear | PosterUrl | TrailerUrl | MovieUrl | BannerUrl
    /// </summary>
    public List<CreateMovieCommand> ReadMoviesFromExcel(string filePath)
    {
        var movies = new List<CreateMovieCommand>();

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1); // Skip header row

        if (rows == null) return movies;

        foreach (var row in rows)
        {
            try
            {
                var title = row.Cell(1).GetString().Trim();
                if (string.IsNullOrWhiteSpace(title)) continue;

                var description = row.Cell(2).GetString().Trim();
                var releaseYearStr = row.Cell(3).GetString().Trim();
                var posterUrl = row.Cell(4).GetString().Trim();
                var trailerUrl = row.Cell(5).GetString().Trim();
                var movieUrl = row.Cell(6).GetString().Trim();
                var bannerUrl = row.Cell(7).GetString().Trim();

                int.TryParse(releaseYearStr, out var releaseYear);
                if (releaseYear < 1900 || releaseYear > DateTime.Now.Year + 5)
                    releaseYear = DateTime.Now.Year;

                var command = new CreateMovieCommand(
                    title: title,
                    description: description,
                    posterUrl: posterUrl,
                    bannerUrl: bannerUrl,
                    trailerUrl: trailerUrl,
                    movieUrl: movieUrl,
                    releaseYear: releaseYear,
                    genreIds: new List<int>(),
                    castAndCrew: new List<(int, string)>()
                );

                movies.Add(command);
            }
            catch
            {
                // Skip invalid rows
            }
        }

        return movies;
    }

    /// <summary>
    /// Import movies from CreateMovieCommand list (call API sequentially)
    /// </summary>
    public async Task ImportMoviesAsync(List<CreateMovieCommand> movies)
    {
        if (movies == null || movies.Count == 0) return;

        IsImporting = true;
        ImportTotalCount = movies.Count;
        ImportCurrentIndex = 0;
        ImportSuccessCount = 0;
        ImportFailedCount = 0;
        ImportProgress = 0;
        ImportStatusMessage = "Importing...";

        try
        {
            for (int i = 0; i < movies.Count; i++)
            {
                ImportCurrentIndex = i + 1;
                ImportStatusMessage = $"Importing movie {ImportCurrentIndex}/{ImportTotalCount}: {movies[i].Title}";
                ImportProgress = (double)ImportCurrentIndex / ImportTotalCount * 100;

                var result = await _createMovieUseCase.ExecuteAsync(movies[i]);

                if (result.IsSuccess)
                {
                    ImportSuccessCount++;
                }
                else
                {
                    ImportFailedCount++;
                    System.Diagnostics.Debug.WriteLine($"[Import Failed] {movies[i].Title}: {result.Error}");
                }
            }

            ImportStatusMessage = $"Completed! Success: {ImportSuccessCount}, Failed: {ImportFailedCount}";
        }
        catch (Exception ex)
        {
            ImportStatusMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[Import Error] {ex.Message}");
        }
        finally
        {
            IsImporting = false;
            await LoadMoviesAsync(); // Reload list after import
        }
    }

    /// <summary>
    /// Reset import state
    /// </summary>
    public void ResetImportState()
    {
        IsImporting = false;
        ImportTotalCount = 0;
        ImportCurrentIndex = 0;
        ImportSuccessCount = 0;
        ImportFailedCount = 0;
        ImportProgress = 0;
        ImportStatusMessage = string.Empty;
    }

    #endregion
}
