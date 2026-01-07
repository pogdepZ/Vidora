using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;
using Vidora.Core.UseCases;
using Vidora.Presentation.Gui.Contracts.ViewModels;

namespace Vidora.Presentation.Gui.ViewModels;

public partial class ManageSubscriptionsViewModel : ObservableRecipient, INavigationAware
{
    private readonly GetSubscriptionPlansUseCase _getPlansUseCase;
    private readonly GetPromosUseCase _getPromosUseCase;
    private readonly CreatePromoUseCase _createPromoUseCase;

    public ManageSubscriptionsViewModel(
        GetSubscriptionPlansUseCase getPlansUseCase,
        GetPromosUseCase getPromosUseCase,
        CreatePromoUseCase createPromoUseCase)
    {
        _getPlansUseCase = getPlansUseCase;
        _getPromosUseCase = getPromosUseCase;
        _createPromoUseCase = createPromoUseCase;

        // Initialize NewPromo with default values
        ResetNewPromo();
    }

    #region Properties - Plans

    [ObservableProperty]
    private ObservableCollection<SubscriptionPlanResult> _plans = new();

    [ObservableProperty]
    private bool _isLoadingPlans;

    #endregion

    #region Properties - Promos

    [ObservableProperty]
    private ObservableCollection<PromoResult> _promos = new();

    [ObservableProperty]
    private PaginationResult _promoPagination = new(1, 10, 0, 1);

    [ObservableProperty]
    private bool _isLoadingPromos;

    #endregion

    #region Properties - Create Promo

    [ObservableProperty]
    private string _newPromoCode = string.Empty;

    [ObservableProperty]
    private string _selectedDiscountType = "fixed_amount";

    [ObservableProperty]
    private decimal _newPromoValue;

    [ObservableProperty]
    private decimal _newPromoMinOrderValue;

    [ObservableProperty]
    private decimal? _newPromoMaxDiscount;

    [ObservableProperty]
    private DateTimeOffset _newPromoStartDate = DateTimeOffset.Now;

    [ObservableProperty]
    private DateTimeOffset _newPromoEndDate = DateTimeOffset.Now.AddMonths(1);

    [ObservableProperty]
    private bool _isCreatingPromo;

    #endregion

    #region Properties - Status

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    #endregion

    #region Computed Properties - Pagination

    public int CurrentPage => PromoPagination.Page;
    public int TotalPages => PromoPagination.TotalPages;
    public bool CanGoPrev => PromoPagination.HasPrev;
    public bool CanGoNext => PromoPagination.HasNext;
    public int ItemsCount => PromoPagination.Count;
    public int TotalPromos => PromoPagination.Total;

    #endregion

    #region Options

    public ObservableCollection<DiscountTypeOption> DiscountTypeOptions { get; } = new()
    {
        new DiscountTypeOption("fixed_amount", "Giảm cố định (VND)"),
        new DiscountTypeOption("percentage", "Phần trăm (%)")
    };

    #endregion

    #region Commands - Load Data

    [RelayCommand]
    private async Task LoadPlansAsync()
    {
        if (IsLoadingPlans) return;
        IsLoadingPlans = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _getPlansUseCase.ExecuteAsync();

            if (result.IsSuccess)
            {
                Plans.Clear();
                foreach (var plan in result.Value)
                {
                    Plans.Add(plan);
                }
            }
            else
            {
                ErrorMessage = result.Error;
                System.Diagnostics.Debug.WriteLine($"[LoadPlansAsync] Error: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"[LoadPlansAsync] Exception: {ex.Message}");
        }
        finally
        {
            IsLoadingPlans = false;
        }
    }

    [RelayCommand]
    private async Task LoadPromosAsync()
    {
        if (IsLoadingPromos) return;
        IsLoadingPromos = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _getPromosUseCase.ExecuteAsync(CurrentPage, 10);

            if (result.IsSuccess)
            {
                Promos.Clear();
                foreach (var promo in result.Value.Promos)
                {
                    Promos.Add(promo);
                }
                PromoPagination = result.Value.Pagination;
            }
            else
            {
                ErrorMessage = result.Error;
                System.Diagnostics.Debug.WriteLine($"[LoadPromosAsync] Error: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"[LoadPromosAsync] Exception: {ex.Message}");
        }
        finally
        {
            IsLoadingPromos = false;
            NotifyPaginationPropertiesChanged();
        }
    }

    [RelayCommand]
    private async Task ChangePageAsync(string direction)
    {
        if (IsLoadingPromos) return;

        int targetPage = CurrentPage;

        if (direction == "Next" && CanGoNext) targetPage++;
        else if (direction == "Prev" && CanGoPrev) targetPage--;

        if (targetPage != CurrentPage)
        {
            PromoPagination = PromoPagination with { Page = targetPage };
            await LoadPromosAsync();
        }
    }

    #endregion

    #region Commands - Create Promo

    [RelayCommand]
    private async Task CreatePromoAsync()
    {
        if (IsCreatingPromo) return;
        IsCreatingPromo = true;
        ErrorMessage = string.Empty;
        IsSuccess = false;

        try
        {
            var command = new CreatePromoCommand(
                Code: NewPromoCode.Trim().ToUpperInvariant(),
                DiscountType: SelectedDiscountType,
                Value: NewPromoValue,
                MinOrderValue: NewPromoMinOrderValue,
                MaxDiscount: NewPromoMaxDiscount,
                StartDate: NewPromoStartDate.UtcDateTime,
                EndDate: NewPromoEndDate.UtcDateTime
            );

            var result = await _createPromoUseCase.ExecuteAsync(command);

            if (result.IsSuccess)
            {
                IsSuccess = true;
                SuccessMessage = $"Tạo mã giảm giá '{result.Value.Code}' thành công!";

                // Reset form and reload list
                ResetNewPromo();
                PromoPagination = PromoPagination with { Page = 1 };
                await LoadPromosAsync();
            }
            else
            {
                ErrorMessage = result.Error;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"[CreatePromoAsync] Exception: {ex.Message}");
        }
        finally
        {
            IsCreatingPromo = false;
        }
    }

    [RelayCommand]
    private void ResetNewPromoForm()
    {
        ResetNewPromo();
        ErrorMessage = string.Empty;
    }

    #endregion

    #region Helper Methods

    private void ResetNewPromo()
    {
        NewPromoCode = string.Empty;
        SelectedDiscountType = "fixed_amount";
        NewPromoValue = 0;
        NewPromoMinOrderValue = 0;
        NewPromoMaxDiscount = null;
        NewPromoStartDate = DateTimeOffset.Now;
        NewPromoEndDate = DateTimeOffset.Now.AddMonths(1);
    }

    private void NotifyPaginationPropertiesChanged()
    {
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanGoPrev));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(ItemsCount));
        OnPropertyChanged(nameof(TotalPromos));
    }

    #endregion

    #region Navigation

    public async Task OnNavigatedToAsync(object parameter)
    {
        await Task.WhenAll(LoadPlansAsync(), LoadPromosAsync());
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }

    #endregion
}

/// <summary>
/// Helper class cho ComboBox DiscountType
/// </summary>
public record DiscountTypeOption(string Value, string DisplayText);