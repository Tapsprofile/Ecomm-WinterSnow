namespace WinterSnow.Services.Discovery;

public interface ISearchService
{
    Task<List<string>> AutocompleteAsync(string q, CancellationToken ct = default);
    Task<SearchResponse> SearchAsync(SearchQuery query, CancellationToken ct = default);
}

