namespace qclient.QClient.Interfaces;

public interface IPaginToken
{
    public string? Token { get; }

    public void SetNextToken<T>(T paginated) where T : class, IPagin;

    public bool IsLastPage { get; }
}
