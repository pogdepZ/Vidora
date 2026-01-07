using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Presentation.Gui.ViewModels;

namespace Vidora.Presentation.Gui.Views;

public sealed partial class ManageUsersPage : Page
{
    public ManageUsersViewModel ViewModel { get; } = App.GetService<ManageUsersViewModel>();

    public ManageUsersPage()
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

    private async void OnSearchQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (ViewModel.SearchCommand.CanExecute(null))
        {
            await ViewModel.SearchCommand.ExecuteAsync(null);
        }
    }

    private async void OnViewDetailClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.DataContext is not AdminUserResult user)
            return;

        // Load user detail
        await ViewModel.ViewUserDetailCommand.ExecuteAsync(user);

        if (ViewModel.SelectedUserDetail == null)
        {
            ShowNotification("Lỗi", "Không thể tải thông tin chi tiết người dùng.", InfoBarSeverity.Error);
            return;
        }

        var detail = ViewModel.SelectedUserDetail;

        // Build dialog content
        var stackPanel = new StackPanel { Width = 500, Padding = new Thickness(0, 0, 15, 0), Spacing = 16 };

        // User Info Section
        var userInfoSection = new StackPanel { Spacing = 8 };
        userInfoSection.Children.Add(new TextBlock 
        { 
            Text = "Thông tin người dùng", 
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 8)
        });

        var userInfoGrid = new Grid { ColumnSpacing = 12, RowSpacing = 8 };
        userInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        userInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        AddInfoRow(userInfoGrid, 0, "Họ tên:", detail.User.FullName);
        AddInfoRow(userInfoGrid, 1, "Username:", detail.User.Username);
        AddInfoRow(userInfoGrid, 2, "Email:", detail.User.Email);
        AddInfoRow(userInfoGrid, 3, "Vai trò:", detail.User.RoleDisplayText);
        AddInfoRow(userInfoGrid, 4, "Trạng thái:", detail.User.StatusDisplayText);
        AddInfoRow(userInfoGrid, 5, "Ngày tạo:", detail.User.CreatedAt.ToString("dd/MM/yyyy HH:mm"));
        AddInfoRow(userInfoGrid, 6, "Giới tính:", detail.User.Gender ?? "Chưa cập nhật");
        AddInfoRow(userInfoGrid, 7, "Ngày sinh:", detail.User.Birthday?.ToString("dd/MM/yyyy") ?? "Chưa nhập nhật");

        userInfoSection.Children.Add(userInfoGrid);
        stackPanel.Children.Add(userInfoSection);

        // Subscriptions Section
        if (detail.Subscriptions.Count > 0)
        {
            var subSection = new StackPanel { Spacing = 8 };
            subSection.Children.Add(new TextBlock 
            { 
                Text = $"Gói đăng ký ({detail.Subscriptions.Count})", 
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                FontSize = 16,
                Margin = new Thickness(0, 8, 0, 8)
            });

            foreach (var sub in detail.Subscriptions)
            {
                var subBorder = new Border
                {
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var subStack = new StackPanel { Spacing = 4 };
                subStack.Children.Add(new TextBlock { Text = sub.PlanName, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });
                subStack.Children.Add(new TextBlock { Text = $"Từ: {sub.StartDate:dd/MM/yyyy} - ??n: {sub.EndDate:dd/MM/yyyy}", FontSize = 12, Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray) });
                subStack.Children.Add(new TextBlock { Text = $"Trạng thái: {sub.StatusDisplayText}", FontSize = 12 });

                subBorder.Child = subStack;
                subSection.Children.Add(subBorder);
            }

            stackPanel.Children.Add(subSection);
        }

        // Orders Section
        if (detail.Orders.Count > 0)
        {
            var orderSection = new StackPanel { Spacing = 8 };
            orderSection.Children.Add(new TextBlock 
            { 
                Text = $"Lịch sử mua hàng ({detail.Orders.Count})", 
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                FontSize = 16,
                Margin = new Thickness(0, 8, 0, 8)
            });

            foreach (var order in detail.Orders)
            {
                var orderBorder = new Border
                {
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var orderStack = new StackPanel { Spacing = 4 };
                orderStack.Children.Add(new TextBlock { Text = $"Đơn hàng #{order.OrderId}", FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });
                orderStack.Children.Add(new TextBlock { Text = $"Số tiền: {order.AmountDisplay}", FontSize = 12 });
                orderStack.Children.Add(new TextBlock { Text = $"Phươnng thức: {order.PaymentMethod}", FontSize = 12, Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray) });
                orderStack.Children.Add(new TextBlock { Text = $"Trạng thái: {order.StatusDisplayText}", FontSize = 12 });
                orderStack.Children.Add(new TextBlock { Text = $"Ngày tạo: {order.CreatedAt:dd/MM/yyyy HH:mm}", FontSize = 12, Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray) });

                orderBorder.Child = orderStack;
                orderSection.Children.Add(orderBorder);
            }

            stackPanel.Children.Add(orderSection);
        }

        var scrollViewer = new ScrollViewer 
        { 
            Content = stackPanel, 
            MaxHeight = 500, 
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto 
        };

        ContentDialog dialog = new ContentDialog
        {
            Title = $"Chi tiêt: {detail.User.FullName}",
            Content = scrollViewer,
            CloseButtonText = "Đóng",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.Content.XamlRoot
        };

        await dialog.ShowAsync();
    }

    private void AddInfoRow(Grid grid, int row, string label, string value)
    {
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var labelText = new TextBlock 
        { 
            Text = label, 
            Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetRow(labelText, row);
        Grid.SetColumn(labelText, 0);
        grid.Children.Add(labelText);

        var valueText = new TextBlock 
        { 
            Text = value ?? "N/A",
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetRow(valueText, row);
        Grid.SetColumn(valueText, 1);
        grid.Children.Add(valueText);
    }

    private async void OnToggleStatusClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.DataContext is not AdminUserResult user)
            return;

        var currentStatus = user.Status?.ToUpper();
        var actionText = currentStatus == "ACTIVE" ? "khóa" : "mở khóa";
        var newStatusText = currentStatus == "ACTIVE" ? "LOCKED" : "ACTIVE";

        ContentDialog confirmDialog = new ContentDialog
        {
            Title = "Xác nhận thay đổi trạng thái",
            Content = $"Bạn có chắc muốn {actionText} tài khoản '{user.FullName}'?",
            PrimaryButtonText = "Xác nhận",
            CloseButtonText = "Huỷ",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.Content.XamlRoot
        };

        var result = await confirmDialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var userName = user.FullName;
            await ViewModel.ToggleUserStatusCommand.ExecuteAsync(user);

            if (ViewModel.IsSuccess)
            {
                ShowNotification("Thành công", $"Đã {actionText} tài khoản '{userName}' thành công!", InfoBarSeverity.Success);
            }
            else
            {
                ShowNotification("Lỗi", ViewModel.ErrorMessage ?? $"Không th {actionText} tài khoản.", InfoBarSeverity.Error);
            }
        }
    }
}
