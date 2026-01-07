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

    // Computed property ?? hi?n th? status d? ??c
    public string StatusDisplayText => Status?.ToUpper() switch
    {
        "ACTIVE" => "Ho?t ??ng",
        "LOCKED" => "?ã khóa",
        _ => Status ?? "N/A"
    };

    // Computed property ?? hi?n th? role d? ??c
    public string RoleDisplayText => Role?.ToUpper() switch
    {
        "ADMIN" => "Qu?n tr? viên",
        "USER" => "Ng??i dùng",
        _ => Role ?? "N/A"
    };

    // Computed property cho màu status
    public bool IsActive => Status?.ToUpper() == "ACTIVE";

    // Computed property cho ngày t?o d?ng string
    public string CreatedAtDisplay => CreatedAt.ToString("dd/MM/yyyy");
}
