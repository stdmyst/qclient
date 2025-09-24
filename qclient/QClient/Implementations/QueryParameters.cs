namespace qclient.QClient.Implementations;

public class QueryParameters(Dictionary<string, string>? parameters = null)
{
    public Dictionary<string, string> Storage { get; set; } = parameters ??= new();

    public string AsUriString()
        => Storage.Count == 0
            ? ""
            : "/?" + String.Join("&", Storage.Select(p => $"{p.Key}={p.Value}").ToList());

    public void UpdateFromUri(Uri uri)
    {
        var queries = GetKeyValuePairsFromUri(uri);
        UpdateQueryParametersByKeyValuePairs(queries);
    }
    
    private KeyValuePair<string, string>[] GetKeyValuePairsFromUri(Uri uri)
        => uri.Query.Trim('?').Split('&') 
            .Select(p => p.Split('=')).Select(p => KeyValuePair.Create(p[0], p[1]))
            .ToArray();
    
    private void UpdateQueryParametersByKeyValuePairs(KeyValuePair<string, string>[] queries)
    {
        var keys = new string[queries.Length];
        var currentIndex = 0;
        foreach (var query in queries)
        {
            if (!IsStringCollectionContainsValue(keys, query.Key))
            {
                keys[currentIndex] = query.Key;
                currentIndex++;
                Storage[query.Key] = query.Value;
            }
        }
    }
    
    private bool IsStringCollectionContainsValue(ICollection<string> collection, string value) => collection.Contains(value);
}