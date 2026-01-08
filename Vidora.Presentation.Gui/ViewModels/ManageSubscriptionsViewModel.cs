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
    private readonly GetOrdersUseCase _getOrdersUseCase;

    public ManageSubscriptionsViewModel(
        GetSubscriptionPlansUseCase getPlansUseCase,
        GetPromosUseCase getPromosUseCase,
        CreatePromoUseCase createPromoUseCase,
        GetOrdersUseCase getOrdersUseCase)
    {
        _getPlansUseCase = getPlansUseCase;
        _getPromosUseCase = getPromosUseCase;
        _createPromoUseCase = createPromoUseCase;
        _getOrdersUseCase = getOrdersUseCase;

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

    #region Properties - Orders

    [ObservableProperty]
    private ObservableCollection<OrderResult> _orders = new();

    [ObservableProperty]
    private PaginationResult _orderPagination = new(1, 10, 0, 1);

    [ObservableProperty]
    private bool _isLoadingOrders;

    [ObservableProperty]
    private string _orderSearchText = string.Empty;

    [ObservableProperty]
    private string? _selectedOrderStatus;

    [ObservableProperty]
    private int? _selectedOrderPlanId;

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

    #region Computed Properties - Promo Pagination

    public int CurrentPage => PromoPagination.Page;
    public int TotalPages => PromoPagination.TotalPages;
    public bool CanGoPrev => PromoPagination.HasPrev;
    public bool CanGoNext => PromoPagination.HasNext;
    public int ItemsCount => PromoPagination.Count;
    public int TotalPromos => PromoPagination.Total;

    #endregion

    #region Computed Properties - Order Pagination

    public int OrderCurrentPage => OrderPagination.Page;
    public int OrderTotalPages => OrderPagination.TotalPages;
    public bool CanGoOrderPrev => OrderPagination.HasPrev;
    public bool CanGoOrderNext => OrderPagination.HasNext;
    public int OrderItemsCount => OrderPagination.Count;
    public int TotalOrders => OrderPagination.Total;

    #endregion

    #region Options

    public ObservableCollection<DiscountTypeOption> DiscountTypeOptions { get; } = new()
    {
        new DiscountTypeOption("fixed_amount", "Fixed Amount (VND)"),
        new DiscountTypeOption("percentage", "Percentage (%)")
    };

    public ObservableCollection<OrderStatusOption> OrderStatusOptions { get; } = new()
    {
        new OrderStatusOption(null, "All"),
        new OrderStatusOption("COMPLETED", "Completed"),
        new OrderStatusOption("PENDING", "Pending"),
        new OrderStatusOption("FAILED", "Failed")
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
    private async Task LoadOrdersAsync()
    {
        if (IsLoadingOrders) return;
        IsLoadingOrders = true;
        ErrorMessage = string.Empty;

        try
        {
            var search = string.IsNullOrWhiteSpace(OrderSearchText) ? null : OrderSearchText.Trim();
            var result = await _getOrdersUseCase.ExecuteAsync(
                OrderCurrentPage,
                10,
                search,
                SelectedOrderStatus,
                SelectedOrderPlanId);

            if (result.IsSuccess)
            {
                Orders.Clear();
                foreach (var order in result.Value.Orders)
                {
                    Orders.Add(order);
                }
                OrderPagination = result.Value.Pagination;
            }
            else
            {
                ErrorMessage = result.Error;
                System.Diagnostics.Debug.WriteLine($"[LoadOrdersAsync] Error: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"[LoadOrdersAsync] Exception: {ex.Message}");
        }
        finally
        {
            IsLoadingOrders = false;
            NotifyOrderPaginationPropertiesChanged();
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

    [RelayCommand]
    private async Task ChangeOrderPageAsync(string direction)
    {
        if (IsLoadingOrders) return;

        int targetPage = OrderCurrentPage;

        if (direction == "Next" && CanGoOrderNext) targetPage++;
        else if (direction == "Prev" && CanGoOrderPrev) targetPage--;

        if (targetPage != OrderCurrentPage)
        {
            OrderPagination = OrderPagination with { Page = targetPage };
            await LoadOrdersAsync();
        }
    }

    [RelayCommand]
    private async Task SearchOrdersAsync()
    {
        // Reset to page 1 when searching
        OrderPagination = OrderPagination with { Page = 1 };
        await LoadOrdersAsync();
    }

    [RelayCommand]
    private async Task FilterOrdersByStatusAsync(string? status)
    {
        SelectedOrderStatus = status;
        // Reset to page 1 when filtering
        OrderPagination = OrderPagination with { Page = 1 };
        await LoadOrdersAsync();
    }

    [RelayCommand]
    private async Task FilterOrdersByPlanAsync(int? planId)
    {
        SelectedOrderPlanId = planId;
        // Reset to page 1 when filtering
        OrderPagination = OrderPagination with { Page = 1 };
        await LoadOrdersAsync();
    }

    [RelayCommand]
    private async Task ClearOrderFiltersAsync()
    {
        OrderSearchText = string.Empty;
        SelectedOrderStatus = null;
        SelectedOrderPlanId = null;
        OrderPagination = OrderPagination with { Page = 1 };
        await LoadOrdersAsync();
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
                SuccessMessage = $"Promo code created successfully!";

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

    private void NotifyOrderPaginationPropertiesChanged()
    {
        OnPropertyChanged(nameof(OrderCurrentPage));
        OnPropertyChanged(nameof(OrderTotalPages));
        OnPropertyChanged(nameof(CanGoOrderPrev));
        OnPropertyChanged(nameof(CanGoOrderNext));
        OnPropertyChanged(nameof(OrderItemsCount));
        OnPropertyChanged(nameof(TotalOrders));
    }

    #endregion

    #region Navigation

    public async Task OnNavigatedToAsync(object parameter)
    {
        await Task.WhenAll(LoadPlansAsync(), LoadPromosAsync(), LoadOrdersAsync());
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

/// <summary>
/// Helper class cho ComboBox OrderStatus
/// </summary>
public record OrderStatusOption(string? Value, string DisplayText);