namespace WinterSnow.Services.Reviews;

public class CreateReviewRequest
{
    public int ProductId { get; set; }
    public int Rating { get; set; } // 1-5
    public string Title { get; set; } = "";
    public string ReviewText { get; set; } = "";
    public List<string>? MediaUrls { get; set; }
}

public class CreateReviewResult
{
    public int ReviewId { get; set; }
    public bool IsVerifiedPurchase { get; set; }
}

