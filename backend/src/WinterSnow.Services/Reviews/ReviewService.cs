using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Reviews;
using WinterSnow.Data;

namespace WinterSnow.Services.Reviews;

public class ReviewService : IReviewService
{
    private readonly WinterSnowDbContext _db;

    public ReviewService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task<CreateReviewResult> CreateAsync(int customerId, CreateReviewRequest req, CancellationToken ct = default)
    {
        if (req.ProductId <= 0)
            throw new InvalidOperationException("ProductId is required.");
        if (req.Rating < 1 || req.Rating > 5)
            throw new InvalidOperationException("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(req.Title))
            throw new InvalidOperationException("Title is required.");
        if (string.IsNullOrWhiteSpace(req.ReviewText))
            throw new InvalidOperationException("ReviewText is required.");

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId, ct);
        if (product is null)
            throw new InvalidOperationException("Product not found.");

        // Verified purchase check (WooCommerce-like "verified owner")
        var isVerifiedPurchase = await _db.Orders
            .Where(o => o.CustomerId == customerId)
            .Join(_db.OrderItems, o => o.Id, i => i.OrderId, (o, i) => new { i.ProductId })
            .AnyAsync(x => x.ProductId == req.ProductId, ct);

        var review = new Review
        {
            ProductId = req.ProductId,
            CustomerId = customerId,
            Rating = req.Rating,
            Title = req.Title.Trim(),
            ReviewText = req.ReviewText.Trim(),
            IsVerifiedPurchase = isVerifiedPurchase
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(ct);

        var urls = (req.MediaUrls ?? new List<string>())
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(u => u.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(5)
            .ToList();

        foreach (var u in urls)
        {
            _db.ReviewMedia.Add(new ReviewMedia
            {
                ReviewId = review.Id,
                Url = u,
                MediaType = "image"
            });
        }

        await _db.SaveChangesAsync(ct);

        return new CreateReviewResult { ReviewId = review.Id, IsVerifiedPurchase = isVerifiedPurchase };
    }
}

