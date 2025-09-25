using qclient.QClient.Enums;
using qclient.QClient.Exceptions;
using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

public class ClientResponse<T> : IClientResponse<T> where T : class
{
    public T? SerializedResponse { get; set; }
    public ClientResponseStatus ResponseStatus  { get; set; }
    public Exception? InnerException  { get; set; }

    public bool ValidateResponse()
        => ResponseStatus == ClientResponseStatus.Success && SerializedResponse != null
            ? true
            : throw new ClientResponseValidateException<T>(this);
}