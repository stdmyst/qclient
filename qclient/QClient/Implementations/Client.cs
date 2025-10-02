using System.Text.Json;
using Microsoft.Extensions.Logging;
using qclient.QClient.Constants;
using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

public class Client(ILogger<Client> logger) : IClient
{
    public async Task<T> RequestAsync<T>(HttpClient httpClient, HttpRequestMessage message, CancellationToken ct, JsonSerializerOptions? jsonOptions = null)
    {
        try
        {
            var httpResponse = await httpClient.SendAsync(message, ct);
            httpResponse.EnsureSuccessStatusCode();

            var stream = await httpResponse.Content.ReadAsStreamAsync(ct);
            var package = await JsonSerializer.DeserializeAsync<T>(stream, jsonOptions ?? Defaults.JsonSerializerOptionsDefault, ct)
                          ?? throw new JsonException($"Can not deserialize {typeof(T).Name}.");

            return package;
        }
        catch (HttpRequestException e)
        {
            var status = e.StatusCode == null ? "empty" : e.StatusCode.ToString();
            logger.LogError($"Can not get {typeof(T).Name} for {message.RequestUri}. Request status: {status}. Message: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            logger.LogError($"Can not get {typeof(T).Name} for {message.RequestUri}. Message: {e.Message}");
            throw;
        }
    }
    
    public async Task<IEnumerable<T>> RequestAsyncWithPagination<T>(HttpClient httpClient, IMessageCreator mc, IPaginationController<T> paginController, CancellationToken ct, JsonSerializerOptions? jsonOptions = null)
    {
        var result = new List<T>();
        while (true)
        {
            var response = await RequestAsync<T>(httpClient, mc.GetHttpRequestMessage(HttpMethod.Get), ct, jsonOptions);
            result.Add(response);
            
            if (!paginController.ShouldContinue(response))
                break;
            
            paginController.OnNext(mc, response);
        }
        
        return result;
    }
}