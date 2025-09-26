using qclient.tests.Models;

namespace qclient.tests;

public class HttpClientFixture : IDisposable
{
    public readonly HttpClient Client;
    public readonly PaginControllerShouldContinue PaginControllerShouldContinue;
    public readonly PaginControllerAlwaysContinue PaginControllerAlwaysContinue;
    
    public HttpClientFixture()
    {
        Client = new HttpClient();
        Client.BaseAddress = new Uri("https://localhost:7227");
        PaginControllerShouldContinue = new PaginControllerShouldContinue();
        PaginControllerAlwaysContinue = new PaginControllerAlwaysContinue();
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}