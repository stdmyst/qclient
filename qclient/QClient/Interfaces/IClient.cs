namespace qclient.QClient.Interfaces;

public interface IClient
{
    Task<T> RequestAsync<T>(HttpRequestMessage message, CancellationToken ct);
    Task<IList<T>> RequestAsyncWithPagination<T>(IMessageCreator mc, IPaginationController<T> paginationController, CancellationToken ct);
}