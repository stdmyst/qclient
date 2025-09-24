using qclient.QClient.Implementations;

namespace qclient.QClient.Interfaces;

public interface IClient
{
    Task<ClientResponse<T>> RequestAsync<T>(HttpClient client, HttpRequestMessage message) where T : class;
}