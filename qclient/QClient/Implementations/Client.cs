using System.Text.Json;
using qclient.QClient.Constants;
using qclient.QClient.Enums;
using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

public class Client : IClient
{
    public async Task<ClientResponse<T>> RequestAsync<T>(HttpClient httpClient, IMessageCreator messageCreator) where T : class
    {
        var httpResponse = await httpClient.SendAsync(messageCreator.GetHttpRequestMessage(HttpMethod.Get));
        var clientResponse = new ClientResponse<T>();
        if (httpResponse.IsSuccessStatusCode)
        {
            try
            {
                var package = await JsonSerializer.DeserializeAsync<T>(httpResponse.Content.ReadAsStreamAsync().Result, SerializationConstants.JsonSerializerOptionsDefault);
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

    public async Task<ClientResponse<IList<T>>> RequestAsyncWithPagination<T>(HttpClient httpClient, IMessageCreator messageCreator, Func<IMessageCreator, T, IMessageCreator> onNextRequest)
        where T : class, IPagin, new()
    {
        var clientResponse = new ClientResponse<IList<T>>
        {
            ResponseStatus = ClientResponseStatus.Success,
            SerializedResponse = new List<T>()
        };
        T responseObj = new();

        try
        {
            while (!responseObj.IsLast)
            {
                var response = await RequestAsync<T>(httpClient, messageCreator);
                if (response.ResponseStatus == ClientResponseStatus.Error)
                {
                    clientResponse.ResponseStatus = ClientResponseStatus.Error;
                    clientResponse.SerializedResponse = null;
                    break;
                }
                responseObj = response.SerializedResponse!;
                clientResponse.SerializedResponse.Add(responseObj);
            
                messageCreator = onNextRequest(messageCreator, responseObj);
            }
        }
        catch
        {
            clientResponse.ResponseStatus = ClientResponseStatus.Error;
        }

        return clientResponse;
    }
}