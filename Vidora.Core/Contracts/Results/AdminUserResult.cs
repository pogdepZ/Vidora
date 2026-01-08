using System;

namespace Vidora.Core.Contracts.Results;

/// <summary>
/// Result cho user item trong danh sách admin
/// </summary>
public class AdminUserResult
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Gender { get; set; }
    public DateTime? Birthday { get; set; }

    // Computed property de hien thi status de doc
    public string StatusDisplayText => Status?.ToUpper() switch
    {
        "ACTIVE" => "Active",
        "LOCKED" => "Locked",
        _ => Status ?? "N/A"
    };

    // Computed property de hien thi role de doc
    public string RoleDisplayText => Role?.ToUpper() switch
    {
        "ADMIN" => "Admin",
        "USER" => "User",
        _ => Role ?? "N/A"
    };

    // Computed property cho mau status
    public bool IsActive => Status?.ToUpper() == "ACTIVE";

    // Computed property cho ngay tao dang string
    public string CreatedAtDisplay => CreatedAt.ToString("dd/MM/yyyy");
}
