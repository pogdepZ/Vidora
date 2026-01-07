using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Presentation.Gui.ViewModels;

namespace Vidora.Presentation.Gui.Views;

public sealed partial class ManageSubscriptionsPage : Page
{
    public ManageSubscriptionsViewModel ViewModel { get; } = App.GetService<ManageSubscriptionsViewModel>();

    public ManageSubscriptionsPage()
    {
        InitializeComponent();
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
}
