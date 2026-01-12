using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Customers;

/// <summary>
/// Simplified IAM model inspired by nopCommerce Customer + CustomerRole.
/// </summary>
public class AppUser : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }

    /// <summary>
    /// MFA placeholder; real implementation should store secrets / factors.
    /// </summary>
    public bool IsMfaEnabled { get; set; }

    public UserType UserType { get; set; }

    /// <summary>
    /// For vendors, links to Vendor entity.
    /// </summary>
    public int? VendorId { get; set; }
}

