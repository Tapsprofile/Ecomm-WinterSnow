namespace WinterSnow.Services.Returns;

public class CreateReturnRequest
{
    public int OrderId { get; set; }
    public int OrderItemId { get; set; }
    public required string Reason { get; set; }
    public string? Notes { get; set; }
}

public class ReturnDto
{
    public int ReturnId { get; set; }
    public int OrderId { get; set; }
    public int OrderItemId { get; set; }
    public int VendorId { get; set; }
    public required string Reason { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Requested";
    public string? ReturnLabelUrl { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}

