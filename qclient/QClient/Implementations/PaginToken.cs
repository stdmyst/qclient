using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

public class PaginToken :  IPaginToken
{
    private string? _token; 
    
    public string? Token => _token;
    
    public void SetNextToken<T>(T paginated) where T : class, IPagin
    {
        if (paginated.Token == null)
            IsLastPage = true;
        else
            _token = paginated.Token;
    }

    public bool IsLastPage { get; private set; } = false;
}