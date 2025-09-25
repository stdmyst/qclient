using qclient.QClient.Enums;
using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

public class ClientResponse<T> : IClientResponse<T> where T : class
{
    public T? SerializedResponse { get; set; }
    public ClientResponseStatus ResponseStatus  { get; set; }
    public Exception? InnerException  { get; set; }
}