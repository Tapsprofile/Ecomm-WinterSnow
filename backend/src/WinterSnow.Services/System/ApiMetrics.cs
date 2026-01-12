using System.Collections.Concurrent;
using System.Diagnostics;

namespace WinterSnow.Services.System;

public class ApiMetrics
{
    private readonly ConcurrentQueue<double> _durationsMs = new();
    private readonly ConcurrentQueue<string> _errors = new();
    private const int MaxSamples = 500;
    private const int MaxErrors = 50;

    public DateTime StartedUtc { get; } = DateTime.UtcNow;

    public void RecordRequest(TimeSpan elapsed)
    {
        _durationsMs.Enqueue(elapsed.TotalMilliseconds);
        Trim(_durationsMs, MaxSamples);
    }

    public void RecordError(Exception ex)
    {
        _errors.Enqueue($"{DateTime.UtcNow:o} {ex.GetType().Name}: {ex.Message}");
        Trim(_errors, MaxErrors);
    }

    public (double p50, double p95) GetPercentiles()
    {
        var arr = _durationsMs.ToArray();
        if (arr.Length == 0)
            return (0, 0);
        Array.Sort(arr);
        return (Percentile(arr, 0.50), Percentile(arr, 0.95));
    }

    public List<string> GetRecentErrors() => _errors.ToArray().Reverse().Take(10).ToList();

    private static double Percentile(double[] sorted, double p)
    {
        if (sorted.Length == 0)
            return 0;
        var idx = (int)Math.Floor((sorted.Length - 1) * p);
        return sorted[Math.Clamp(idx, 0, sorted.Length - 1)];
    }

    private static void Trim<T>(ConcurrentQueue<T> q, int max)
    {
        while (q.Count > max && q.TryDequeue(out _))
        {
        }
    }
}

