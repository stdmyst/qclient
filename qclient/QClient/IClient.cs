namespace qclient.QClient;

public interface IClient
{
    Task<ClientResponse<T>> RequestAsync<T>(HttpClient client, HttpRequestMessage message) where T : class;
}