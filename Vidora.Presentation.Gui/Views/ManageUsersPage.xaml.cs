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
            ShowNotification("Error", "Unable to load user details.", InfoBarSeverity.Error);
            return;
        }

        var detail = ViewModel.SelectedUserDetail;

        // Build dialog content
        var stackPanel = new StackPanel { Width = 500, Padding = new Thickness(0, 0, 15, 0), Spacing = 16 };

        // User Info Section
        var userInfoSection = new StackPanel { Spacing = 8 };
        userInfoSection.Children.Add(new TextBlock 
        { 
            Text = "User Information", 
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 8)
        });

        var userInfoGrid = new Grid { ColumnSpacing = 12, RowSpacing = 8 };
        userInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        userInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        AddInfoRow(userInfoGrid, 0, "Full Name:", detail.User.FullName);
        AddInfoRow(userInfoGrid, 1, "Username:", detail.User.Username);
        AddInfoRow(userInfoGrid, 2, "Email:", detail.User.Email);
        AddInfoRow(userInfoGrid, 3, "Role:", detail.User.RoleDisplayText);
        AddInfoRow(userInfoGrid, 4, "Status:", detail.User.StatusDisplayText);
        AddInfoRow(userInfoGrid, 5, "Created At:", detail.User.CreatedAt.ToString("dd/MM/yyyy HH:mm"));
        AddInfoRow(userInfoGrid, 6, "Gender:", detail.User.Gender ?? "Not updated");
        AddInfoRow(userInfoGrid, 7, "Birthday:", detail.User.Birthday?.ToString("dd/MM/yyyy") ?? "Not updated");

        userInfoSection.Children.Add(userInfoGrid);
        stackPanel.Children.Add(userInfoSection);

        // Subscriptions Section
        if (detail.Subscriptions.Count > 0)
        {
            var subSection = new StackPanel { Spacing = 8 };
            subSection.Children.Add(new TextBlock 
            { 
                Text = $"Subscriptions ({detail.Subscriptions.Count})", 
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
                subStack.Children.Add(new TextBlock { Text = $"From: {sub.StartDate:dd/MM/yyyy} - To: {sub.EndDate:dd/MM/yyyy}", FontSize = 12, Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray) });
                subStack.Children.Add(new TextBlock { Text = $"Status: {sub.StatusDisplayText}", FontSize = 12 });

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
                Text = $"Order History ({detail.Orders.Count})", 
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
                orderStack.Children.Add(new TextBlock { Text = $"Order #{order.OrderId}", FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });
                orderStack.Children.Add(new TextBlock { Text = $"Amount: {order.AmountDisplay}", FontSize = 12 });
                orderStack.Children.Add(new TextBlock { Text = $"Payment Method: {order.PaymentMethod}", FontSize = 12, Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray) });
                orderStack.Children.Add(new TextBlock { Text = $"Status: {order.StatusDisplayText}", FontSize = 12 });
                orderStack.Children.Add(new TextBlock { Text = $"Created At: {order.CreatedAt:dd/MM/yyyy HH:mm}", FontSize = 12, Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray) });

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
            Title = $"Details: {detail.User.FullName}",
            Content = scrollViewer,
            CloseButtonText = "Close",
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
        var actionText = currentStatus == "ACTIVE" ? "lock" : "unlock";
        var newStatusText = currentStatus == "ACTIVE" ? "LOCKED" : "ACTIVE";

        ContentDialog confirmDialog = new ContentDialog
        {
            Title = "Confirm Status Change",
            Content = $"Are you sure you want to {actionText} the account '{user.FullName}'?",
            PrimaryButtonText = "Confirm",
            CloseButtonText = "Cancel",
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
                ShowNotification("Success", $"Account '{userName}' has been {actionText}ed successfully!", InfoBarSeverity.Success);
            }
            else
            {
                ShowNotification("Error", ViewModel.ErrorMessage ?? $"Unable to {actionText} the account.", InfoBarSeverity.Error);
            }
        }
    }
}
