using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.UseCases;
using Vidora.Presentation.Gui.Contracts.ViewModels;

namespace Vidora.Presentation.Gui.ViewModels;

public partial class ManageUsersViewModel : ObservableRecipient, INavigationAware
{
    private readonly GetUsersUseCase _getUsersUseCase;
    private readonly GetUserDetailUseCase _getUserDetailUseCase;
    private readonly ToggleUserStatusUseCase _toggleUserStatusUseCase;

    public ManageUsersViewModel(
        GetUsersUseCase getUsersUseCase,
        GetUserDetailUseCase getUserDetailUseCase,
        ToggleUserStatusUseCase toggleUserStatusUseCase)
    {
        _getUsersUseCase = getUsersUseCase;
        _getUserDetailUseCase = getUserDetailUseCase;
        _toggleUserStatusUseCase = toggleUserStatusUseCase;
    }

    #region Properties

    [ObservableProperty]
    private ObservableCollection<AdminUserResult> _users = new();

    [ObservableProperty]
    private PaginationResult _pagination = new(1, 10, 0, 1);

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string? _selectedRole;

    [ObservableProperty]
    private string? _selectedStatus;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess;

    // Detail view properties
    [ObservableProperty]
    private UserDetailResult? _selectedUserDetail;

    [ObservableProperty]
    private bool _isDetailLoading;

    #endregion

    #region Computed Properties

    public int CurrentPage => Pagination.Page;
    public int TotalPages => Pagination.TotalPages;
    public bool CanGoPrev => Pagination.HasPrev;
    public bool CanGoNext => Pagination.HasNext;
    public int ItemsCount => Pagination.Count;
    public int TotalItems => Pagination.Total;

    // Filter options
    public ObservableCollection<string> RoleOptions { get; } = new()
    {
        "Tất cả",
        "ADMIN",
        "USER"
    };

    public ObservableCollection<string> StatusOptions { get; } = new()
    {
        "Tất cả",
        "ACTIVE",
        "LOCKED"
    };

    #endregion

    #region Commands

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        if (IsLoading) return;
        IsLoading = true;

        try
        {
            // Parse filter values
            string? roleFilter = SelectedRole == "Tất cả" ? null : SelectedRole;
            string? statusFilter = SelectedStatus == "Tất cả" ? null : SelectedStatus;

            var result = await _getUsersUseCase.ExecuteAsync(
                page: CurrentPage,
                limit: 10,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                email: null,
                username: null,
                role: roleFilter,
                status: statusFilter);

            if (result.IsSuccess)
            {
                Users.Clear();
                foreach (var user in result.Value.Users)
                {
                    Users.Add(user);
                }
                Pagination = result.Value.Pagination;
            }
            else
            {
                ErrorMessage = result.Error;
                System.Diagnostics.Debug.WriteLine($"[LoadUsersAsync] Error: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"[LoadUsersAsync] Exception: {ex.Message}");
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
        Pagination = Pagination with { Page = 1 };
        await LoadUsersAsync();
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        SearchText = string.Empty;
        SelectedRole = null;
        SelectedStatus = null;
        Pagination = Pagination with { Page = 1 };
        await LoadUsersAsync();
    }

    [RelayCommand]
    private async Task ChangePageAsync(string direction)
    {
        if (IsLoading) return;

        int targetPage = CurrentPage;

        if (direction == "Next" && CanGoNext) targetPage++;
        else if (direction == "Prev" && CanGoPrev) targetPage--;

        if (targetPage != CurrentPage)
        {
            Pagination = Pagination with { Page = targetPage };
            await LoadUsersAsync();
        }
    }

    [RelayCommand]
    private async Task ToggleUserStatusAsync(AdminUserResult user)
    {
        if (user == null) return;

        IsSuccess = false;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _toggleUserStatusUseCase.ExecuteAsync(user.UserId);

            if (result.IsSuccess)
            {
                IsSuccess = true;
                // Update the user's status in the list
                user.Status = result.Value;
                
                // Refresh the list to update UI
                await LoadUsersAsync();
            }
            else
            {
                IsSuccess = false;
                ErrorMessage = result.Error;
            }
        }
        catch (Exception ex)
        {
            IsSuccess = false;
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task ViewUserDetailAsync(AdminUserResult user)
    {
        if (user == null) return;

        IsDetailLoading = true;
        SelectedUserDetail = null;

        try
        {
            var result = await _getUserDetailUseCase.ExecuteAsync(user.UserId);

            if (result.IsSuccess)
            {
                SelectedUserDetail = result.Value;
            }
            else
            {
                ErrorMessage = result.Error;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsDetailLoading = false;
        }
    }

    #endregion

    #region Helper Methods

    private void NotifyPaginationPropertiesChanged()
    {
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanGoPrev));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(ItemsCount));
        OnPropertyChanged(nameof(TotalItems));
    }

    #endregion

    #region Navigation

    public async Task OnNavigatedToAsync(object parameter)
    {
        await LoadUsersAsync();
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }

    #endregion
}
