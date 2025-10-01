using System.Text.Json;
using qclient.QClient.Constants;
using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

public class Client(HttpClient httpClient, JsonSerializerOptions? jsonOptions = null) : IClient
{
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions ?? Serialization.JsonSerializerOptionsDefault;
    
    public async Task<T> RequestAsync<T>(HttpRequestMessage message, CancellationToken ct)
    {
        var httpResponse = await httpClient.SendAsync(message, ct);
        httpResponse.EnsureSuccessStatusCode();
        
        var stream = await httpResponse.Content.ReadAsStreamAsync(ct);
        var package = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, ct) 
                      ?? throw new JsonException($"Can not deserialize {typeof(T).Name}.");
        
        return package;
    }
    
    public async Task<IList<T>> RequestAsyncWithPagination<T>(IMessageCreator mc, IPaginationController<T> paginController, CancellationToken ct)
    {
        var result = new List<T>();
        while (true)
        {
            var response = await RequestAsync<T>(mc.GetHttpRequestMessage(HttpMethod.Get), ct);
            result.Add(response);
            
            if (!paginController.ShouldContinue(response))
                break;
            
            paginController.OnNext(mc, response);
        }
        
        return result;
    }
}