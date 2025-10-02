using System.Text.Json;

namespace qclient.QClient.Interfaces;

public interface IClient
{
    Task<T> RequestAsync<T>(HttpClient httpClient, HttpRequestMessage message, CancellationToken ct, JsonSerializerOptions? jsonOptions = null);
    Task<IEnumerable<T>> RequestAsyncWithPagination<T>(HttpClient httpClient, IMessageCreator mc, IPaginationController<T> paginationController, CancellationToken ct, JsonSerializerOptions? jsonOptions = null);
}