using qclient.QClient.Implementations;

namespace qclient.QClient.Interfaces;

public interface IClient
{
    Task<ClientResponse<T>> RequestAsync<T>(HttpClient client, IMessageCreator messageCreator) where T : class;
    Task<ClientResponse<IList<T>>> RequestAsyncWithPagination<T>(HttpClient httpClient,
        IMessageCreator messageCreator, Func<IMessageCreator, T, IMessageCreator> onNextRequestUpdate) where T : class, IPagin, new();
}