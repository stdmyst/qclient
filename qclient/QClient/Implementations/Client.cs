using System.Text.Json;
using qclient.QClient.Constants;
using qclient.QClient.Enums;
using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

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
            do
            {
                var response = await RequestAsync<T>(httpClient, messageCreator.GetHttpRequestMessage(HttpMethod.Get));
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
            while (responseObj.CanBeRequested);
        }
        catch
        {
            clientResponse.ResponseStatus = ClientResponseStatus.Error;
        }

        return clientResponse;
    }
}