using qclient.QClient.Enums;

namespace qclient.QClient.Interfaces;

public interface IClientResponse<T> where T : class
{
    public T? SerializedResponse { get; set; }
    public ClientResponseStatus ResponseStatus  { get; set; }
}