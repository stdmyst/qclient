using qclient.QClient.Implementations;

namespace qclient.QClient.Exceptions;

public class ClientResponseValidateException<T>(ClientResponse<T> response) : Exception 
    where T : class
{
    public ClientResponse<T> Response => response;
}