using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using qclient.QClient.Implementations;
using qclient.tests.Models;

namespace qclient.tests;

public class HttpClientFixture : IDisposable
{
    public readonly HttpClient HttpClient;
    public readonly Client QClient;
    public readonly PaginationShouldContinue PaginationShouldContinue;
    public readonly PaginationAlwaysContinue PaginationAlwaysContinue;
    
    public HttpClientFixture()
    {
        HttpClient = new HttpClient();
        HttpClient.BaseAddress = new Uri("https://localhost:7227");
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        QClient = new Client(new Logger<Client>(loggerFactory));
        PaginationShouldContinue = new PaginationShouldContinue();
        PaginationAlwaysContinue = new PaginationAlwaysContinue();
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}