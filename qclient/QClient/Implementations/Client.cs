using System.Text.Json;
using qclient.QClient.Constants;
using qclient.QClient.Enums;
using qclient.QClient.Exceptions;
using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

public class Client : IClient
{
    public async Task<ClientResponse<T>> RequestAsync<T>(HttpClient httpClient, HttpRequestMessage message) where T : class
    {
        var httpResponse = await httpClient.SendAsync(message);
        var clientResponse = new ClientResponse<T>();
        try
        {
            httpResponse.EnsureSuccessStatusCode();

            var stream = await httpResponse.Content.ReadAsStreamAsync();
            var package = await JsonSerializer.DeserializeAsync<T>(stream, SerializationConstants.JsonSerializerOptionsDefault);
            if (package == null)
            {
                SetErrorWithException(clientResponse, new JsonException("Can not deserialize package."));
            }
            else
            {
                clientResponse.SerializedResponse = package;
                clientResponse.ResponseStatus = ClientResponseStatus.Success;
            }
        }
        catch (HttpRequestException e)
        {
            SetErrorWithException(clientResponse, e);
        }
        catch (Exception e)
        {
            SetErrorWithException(clientResponse, e);
        }

        return clientResponse;
    }

    public async Task<ClientResponse<IList<T>>> RequestAsyncWithPagination<T>(HttpClient httpClient, IMessageCreator mc, IPaginController<T> paginController)
        where T : class
    {
        var clientResponse = new ClientResponse<IList<T>>
        {
            ResponseStatus = ClientResponseStatus.Success,
            SerializedResponse = new List<T>()
        };

        try
        {
            T responseObj;
            while (true)
            {
                var response = await RequestAsync<T>(httpClient, mc.GetHttpRequestMessage(HttpMethod.Get));

                response.ValidateResponse();

                responseObj = response.SerializedResponse!;
                clientResponse.SerializedResponse.Add(responseObj);
                
                if (!paginController.ShouldContinue(responseObj))
                    break;
                mc = paginController.OnNext(mc, responseObj);
            }
        }
        catch (ClientResponseValidateException<T> e)
        {
            SetErrorWithException(clientResponse, e.Response.InnerException);
        }
        catch (Exception e)
        {
            SetErrorWithException(clientResponse, e);
        }

        return clientResponse;
    }

    private void SetErrorWithException<T>(ClientResponse<T> response, Exception? e) where T : class
    {
        response.ResponseStatus = ClientResponseStatus.Error;
        response.InnerException = e;
    }
}