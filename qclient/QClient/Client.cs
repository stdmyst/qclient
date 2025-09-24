using System.Text.Json;

namespace qclient.QClient;

public class Client : IClient
{
    public async Task<ClientResponse<T>> RequestAsync<T>(HttpClient httpClient, HttpRequestMessage message) where T : class
    {
        var httpResponse = await httpClient.SendAsync(message);
        var clientResponse = new ClientResponse<T>();
        if (httpResponse.IsSuccessStatusCode)
        {
            try
            {
                var package = await JsonSerializer.DeserializeAsync<T>(httpResponse.Content.ReadAsStreamAsync().Result, new JsonSerializerOptions {  PropertyNameCaseInsensitive = true });
                clientResponse.SerializedResponse = package;
                clientResponse.ResponseStatus = ClientResponseStatus.Success;
            }
            catch (JsonException e)
            {
                clientResponse.ResponseStatus = ClientResponseStatus.Error;
            }
        }
        else
        {
            clientResponse.ResponseStatus = ClientResponseStatus.Error;
        }
        
        return clientResponse;
    }
}