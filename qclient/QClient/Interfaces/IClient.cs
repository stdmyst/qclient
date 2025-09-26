using qclient.QClient.Implementations;

namespace qclient.QClient.Interfaces;

public interface IClient
{
    Task<ClientResponse<T>> RequestAsync<T>(HttpClient httpClient, HttpRequestMessage message) where T : class;

    Task<ClientResponse<IList<T>>> RequestAsyncWithPagination<T>(HttpClient httpClient, IMessageCreator mc,
        IPaginController<T> paginController) where T : class;
}