namespace WinterSnow.Services.Reviews;

public interface IReviewService
{
    Task<CreateReviewResult> CreateAsync(int customerId, CreateReviewRequest req, CancellationToken ct = default);
}

