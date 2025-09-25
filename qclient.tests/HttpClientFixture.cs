namespace qclient.tests;

public class HttpClientFixture : IDisposable
{
    public readonly HttpClient Client;
    
    public HttpClientFixture()
    {
        Client = new HttpClient();
        Client.BaseAddress = new Uri("https://localhost:7227");
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}