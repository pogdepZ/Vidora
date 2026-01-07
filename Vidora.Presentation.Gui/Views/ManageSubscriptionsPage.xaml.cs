using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;
using Vidora.Presentation.Gui.ViewModels;

namespace Vidora.Presentation.Gui.Views;

public sealed partial class ManageSubscriptionsPage : Page
{
    public ManageSubscriptionsViewModel ViewModel { get; } = App.GetService<ManageSubscriptionsViewModel>();

    public ManageSubscriptionsPage()
    {
        InitializeComponent();

        // Set default selection for status filter
        OrderStatusComboBox.SelectedIndex = 0;
    }

    private int _notificationId = 0;

    /// <summary>
    /// Hi?n th? thông báo InfoBar
    /// </summary>
    private async void ShowNotification(string title, string message, InfoBarSeverity severity = InfoBarSeverity.Success, int autoHideSeconds = 3)
    {
        NotificationInfoBar.Title = title;
        NotificationInfoBar.Message = message;
        NotificationInfoBar.Severity = severity;
        NotificationInfoBar.IsOpen = true;

        int currentId = ++_notificationId;

        if (autoHideSeconds > 0)
        {
            await Task.Delay(autoHideSeconds * 1000);
            if (currentId == _notificationId)
            {
                NotificationInfoBar.IsOpen = false;
            }
        }
    }

    #region Order Search and Filter Handlers

    private async void OnOrderSearchKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            await ViewModel.SearchOrdersCommand.ExecuteAsync(null);
        }
    }

    private async void OnOrderStatusSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is OrderStatusOption option)
        {
            await ViewModel.FilterOrdersByStatusCommand.ExecuteAsync(option.Value);
        }
    }

    private async void OnOrderPlanSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            if (comboBox.SelectedItem is SubscriptionPlanResult plan)
            {
                await ViewModel.FilterOrdersByPlanCommand.ExecuteAsync(plan.PlanId);
            }
            else
            {
                // Clear plan filter when nothing selected
                await ViewModel.FilterOrdersByPlanCommand.ExecuteAsync(null);
            }
        }
    }

    #endregion

    private async void OnAddPromoClick(object sender, RoutedEventArgs e)
    {
        // Build dialog content
        var mainStack = new StackPanel { Width = 450, Spacing = 16 };

        // Code
        var codeBox = new TextBox
        {
            Header = "Mã gi?m giá *",
            PlaceholderText = "VD: WELCOME2025",
            Text = ViewModel.NewPromoCode
        };
        mainStack.Children.Add(codeBox);

        // Discount Type
        var typeCombo = new ComboBox
        {
            Header = "Lo?i gi?m giá *",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemsSource = ViewModel.DiscountTypeOptions,
            DisplayMemberPath = "DisplayText",
            SelectedValuePath = "Value",
            SelectedValue = ViewModel.SelectedDiscountType
        };
        mainStack.Children.Add(typeCombo);

        // Value
        var valueBox = new NumberBox
        {
            Header = "Giá tr? gi?m *",
            PlaceholderText = "0",
            Value = (double)ViewModel.NewPromoValue,
            Minimum = 0,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact
        };
        mainStack.Children.Add(valueBox);

        // Min Order Value
        var minOrderBox = new NumberBox
        {
            Header = "Giá tr? ??n hàng t?i thi?u",
            PlaceholderText = "0",
            Value = (double)ViewModel.NewPromoMinOrderValue,
            Minimum = 0,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact
        };
        mainStack.Children.Add(minOrderBox);

        // Max Discount
        var maxDiscountBox = new NumberBox
        {
            Header = "Gi?m t?i ?a (?? tr?ng n?u không gi?i h?n)",
            PlaceholderText = "Không gi?i h?n",
            Value = ViewModel.NewPromoMaxDiscount.HasValue ? (double)ViewModel.NewPromoMaxDiscount.Value : double.NaN,
            Minimum = 0,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact
        };
        mainStack.Children.Add(maxDiscountBox);

        // Date Range
        var dateStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16 };
        
        var startDatePicker = new DatePicker
        {
            Header = "Ngày b?t ??u *",
            Date = ViewModel.NewPromoStartDate
        };
        dateStack.Children.Add(startDatePicker);

        var endDatePicker = new DatePicker
        {
            Header = "Ngày k?t thúc *",
            Date = ViewModel.NewPromoEndDate
        };
        dateStack.Children.Add(endDatePicker);
        
        mainStack.Children.Add(dateStack);

        // Error message
        var errorText = new TextBlock
        {
            Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
            TextWrapping = TextWrapping.Wrap,
            Visibility = Visibility.Collapsed
        };
        mainStack.Children.Add(errorText);

        var scrollViewer = new ScrollViewer
        {
            Content = mainStack,
            MaxHeight = 500,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        ContentDialog dialog = new ContentDialog
        {
            Title = "Thêm mã gi?m giá m?i",
            Content = scrollViewer,
            PrimaryButtonText = "T?o",
            CloseButtonText = "H?y",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Validate
            var code = codeBox.Text?.Trim().ToUpperInvariant() ?? string.Empty;
            var discountType = typeCombo.SelectedValue?.ToString() ?? "fixed_amount";
            var value = (decimal)valueBox.Value;
            var minOrderValue = (decimal)minOrderBox.Value;
            var maxDiscount = double.IsNaN(maxDiscountBox.Value) ? (decimal?)null : (decimal)maxDiscountBox.Value;
            var startDate = startDatePicker.Date.UtcDateTime;
            var endDate = endDatePicker.Date.UtcDateTime;

            // Client-side validation
            if (string.IsNullOrWhiteSpace(code))
            {
                ShowNotification("L?i", "Mã gi?m giá không ???c ?? tr?ng.", InfoBarSeverity.Error);
                return;
            }

            if (value <= 0)
            {
                ShowNotification("L?i", "Giá tr? gi?m ph?i l?n h?n 0.", InfoBarSeverity.Error);
                return;
            }

            if (discountType == "percentage" && value > 100)
            {
                ShowNotification("L?i", "Ph?n tr?m gi?m giá không ???c v??t quá 100%.", InfoBarSeverity.Error);
                return;
            }

            if (startDate >= endDate)
            {
                ShowNotification("L?i", "Ngày b?t ??u ph?i tr??c ngày k?t thúc.", InfoBarSeverity.Error);
                return;
            }

            // Update ViewModel and execute
            ViewModel.NewPromoCode = code;
            ViewModel.SelectedDiscountType = discountType;
            ViewModel.NewPromoValue = value;
            ViewModel.NewPromoMinOrderValue = minOrderValue;
            ViewModel.NewPromoMaxDiscount = maxDiscount;
            ViewModel.NewPromoStartDate = startDatePicker.Date;
            ViewModel.NewPromoEndDate = endDatePicker.Date;

            await ViewModel.CreatePromoCommand.ExecuteAsync(null);

            if (ViewModel.IsSuccess)
            {
                ShowNotification("Thành công", ViewModel.SuccessMessage, InfoBarSeverity.Success);
            }
            else
            {
                ShowNotification("L?i", ViewModel.ErrorMessage ?? "Không th? t?o mã gi?m giá.", InfoBarSeverity.Error);
            }
        }
    }

    #region Order Details Handler

    private async void OnViewOrderDetailsClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is OrderResult order)
        {
            await ShowOrderDetailsDialogAsync(order);
        }
    }

    private async Task ShowOrderDetailsDialogAsync(OrderResult order)
    {
        // Build dialog content
        var mainStack = new StackPanel { Width = 450, Spacing = 16 };

        // Order Info Section
        var orderInfoBorder = new Border
        {
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorSecondaryBrush"],
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16)
        };

        var orderInfoStack = new StackPanel { Spacing = 12 };

        // Header
        orderInfoStack.Children.Add(new TextBlock
        {
            Text = "Order Information",
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 16
        });

        // Order Code
        orderInfoStack.Children.Add(CreateInfoRow("Order Code:", order.OrderCode));

        // Order Type
        orderInfoStack.Children.Add(CreateInfoRow("Order Type:", order.OrderTypeDisplay));

        // Created At
        orderInfoStack.Children.Add(CreateInfoRow("Created At:", order.CreatedAt.ToString("MM/dd/yyyy HH:mm")));

        // Status
        var statusRow = new Grid();
        statusRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(130) });
        statusRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        statusRow.Children.Add(new TextBlock
        {
            Text = "Status:",
            Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });

        var statusBorder = new Border
        {
            Padding = new Thickness(10, 4, 10, 4),
            CornerRadius = new CornerRadius(4),
            HorizontalAlignment = HorizontalAlignment.Left
        };

        // Set status color based on status
        statusBorder.Background = order.StatusColor switch
        {
            "Success" => new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green),
            "Warning" => new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange),
            "Error" => new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
            _ => new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray)
        };

        statusBorder.Child = new TextBlock
        {
            Text = order.StatusDisplay,
            FontSize = 12,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
        };
        Grid.SetColumn(statusBorder, 1);
        statusRow.Children.Add(statusBorder);
        orderInfoStack.Children.Add(statusRow);

        orderInfoBorder.Child = orderInfoStack;
        mainStack.Children.Add(orderInfoBorder);

        // Customer Info Section
        var customerInfoBorder = new Border
        {
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorSecondaryBrush"],
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16)
        };

        var customerInfoStack = new StackPanel { Spacing = 12 };

        customerInfoStack.Children.Add(new TextBlock
        {
            Text = "Customer Information",
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 16
        });

        customerInfoStack.Children.Add(CreateInfoRow("Full Name:", order.UserFullName));
        customerInfoStack.Children.Add(CreateInfoRow("Email:", order.UserEmail));

        customerInfoBorder.Child = customerInfoStack;
        mainStack.Children.Add(customerInfoBorder);

        // Plan Info Section
        var planInfoBorder = new Border
        {
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorSecondaryBrush"],
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16)
        };

        var planInfoStack = new StackPanel { Spacing = 12 };

        planInfoStack.Children.Add(new TextBlock
        {
            Text = "Subscription Plan",
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 16
        });

        planInfoStack.Children.Add(CreateInfoRow("Plan Name:", order.PlanName));
        planInfoStack.Children.Add(CreateInfoRow("Plan Price:", $"{order.PlanPrice:N0} VND"));
        planInfoStack.Children.Add(CreateInfoRow("Duration:", $"{order.Durations} month(s)"));

        if (!string.IsNullOrEmpty(order.Description))
        {
            planInfoStack.Children.Add(CreateInfoRow("Description:", order.Description));
        }

        planInfoBorder.Child = planInfoStack;
        mainStack.Children.Add(planInfoBorder);

        // Payment Info Section
        var paymentInfoBorder = new Border
        {
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorSecondaryBrush"],
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16)
        };

        var paymentInfoStack = new StackPanel { Spacing = 12 };

        paymentInfoStack.Children.Add(new TextBlock
        {
            Text = "Payment Details",
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 16
        });

        paymentInfoStack.Children.Add(CreateInfoRow("Original Price:", order.AmountDisplay));
        paymentInfoStack.Children.Add(CreateInfoRow("Discount:", order.DiscountAmountDisplay));

        // Final Amount with accent color
        var finalAmountRow = new Grid();
        finalAmountRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(130) });
        finalAmountRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        finalAmountRow.Children.Add(new TextBlock
        {
            Text = "Total:",
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        });

        var finalAmountText = new TextBlock
        {
            Text = order.FinalAmountDisplay,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            FontSize = 18,
            Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AccentTextFillColorPrimaryBrush"]
        };
        Grid.SetColumn(finalAmountText, 1);
        finalAmountRow.Children.Add(finalAmountText);
        paymentInfoStack.Children.Add(finalAmountRow);

        paymentInfoStack.Children.Add(CreateInfoRow("Paid At:", order.PaidAtDisplay));

        paymentInfoBorder.Child = paymentInfoStack;
        mainStack.Children.Add(paymentInfoBorder);

        var scrollViewer = new ScrollViewer
        {
            Content = mainStack,
            MaxHeight = 500,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        ContentDialog dialog = new ContentDialog
        {
            Title = $"Order Details #{order.OrderCode}",
            Content = scrollViewer,
            CloseButtonText = "Close",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.Content.XamlRoot
        };

        await dialog.ShowAsync();
    }

    private Grid CreateInfoRow(string label, string value)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(130) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        grid.Children.Add(new TextBlock
        {
            Text = label,
            Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });

        var valueText = new TextBlock
        {
            Text = value,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(valueText, 1);
        grid.Children.Add(valueText);

        return grid;
    }

    #endregion
}
