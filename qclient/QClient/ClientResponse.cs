namespace qclient.QClient;

public class ClientResponse<T>() where T : class
{
    public T? SerializedResponse { get; set; }
    public ClientResponseStatus ResponseStatus  { get; set; }
}