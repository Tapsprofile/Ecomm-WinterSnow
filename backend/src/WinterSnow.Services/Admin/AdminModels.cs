namespace WinterSnow.Services.Admin;

public class PendingVendor
{
    public int VendorId { get; set; }
    public required string Name { get; set; }
    public bool IsKycApproved { get; set; }
    public bool IsActive { get; set; }
}

public class PendingProduct
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public int VendorId { get; set; }
    public bool Published { get; set; }
    public bool IsApprovedByAdmin { get; set; }
}

public class FinanceSummary
{
    public decimal PlatformRevenueGross { get; set; }
    public decimal TotalCommissionsEarned { get; set; }
    public int OrdersCount { get; set; }
}

public class SystemHealthSnapshot
{
    public string Status { get; set; } = "OK";
    public double UptimeSeconds { get; set; }
    public double ApiResponseP50Ms { get; set; }
    public double ApiResponseP95Ms { get; set; }
    public List<string> RecentErrors { get; set; } = [];
}

