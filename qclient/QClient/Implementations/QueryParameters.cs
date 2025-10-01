namespace qclient.QClient.Implementations;

public class QueryParameters
{
    private const char StartQuerySymbol = '?';
        
    public Dictionary<string, string> Storage { get; } = new();

    public string AsUriString()
    {
        if (Storage.Count == 0)
            return String.Empty;

        string[] queryParts = Storage.Select(p => $"{p.Key}={p.Value}").ToArray();
        string query = StartQuerySymbol + String.Join("&", queryParts);

        return query;
    }

    public void UpdateFromUri(Uri uri)
    {
        var queries = GetQueriesAsKeyValuePairs(uri);
        UpdateQueryParameters(queries);
    }

    private KeyValuePair<string, string>[] GetQueriesAsKeyValuePairs(Uri uri)
    {
        var parts = uri.Query.Trim('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries);
        
        var queries = parts.Select(p => p.Split('=', StringSplitOptions.TrimEntries))
            .Where(p => p.Length == 2);
        
        return queries.Select(p => KeyValuePair.Create(p[0], p[1])).ToArray();
    }
    
    private void UpdateQueryParameters(KeyValuePair<string, string>[] queries)
    {
        var keys = new string[queries.Length];
        var currentIndex = 0;
        foreach (var query in queries)
        {
            if (keys.Contains(query.Key))
                continue;
            
            keys[currentIndex] = query.Key;
            currentIndex++;
            Storage[query.Key] = query.Value;
        }
    }
}
