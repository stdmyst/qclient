using System.Text.Json;
using qclient.tests.Models;
using qclient.QClient;

namespace qclient.tests;

public class HttpClientFixture : IDisposable
{
    public readonly HttpClient Client;
    
    public HttpClientFixture()
    {
        Client =  new HttpClient();
        Client.BaseAddress = new Uri("https://localhost:7227");
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}

public class ClientTests(HttpClientFixture clientFixture) : IClassFixture<HttpClientFixture>
{
    [Theory]
    [InlineData(1)]
    public async Task RequestAsync_ExistedId_UserWithPassedId(int id)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, $"api/user/?id={id}");

        var rest = new Client();
        var userResp = await rest.RequestAsync<User>(clientFixture.Client, message);
        
        Assert.Equal(ClientResponseStatus.Success, userResp.ResponseStatus);
        Assert.Equal(userResp.SerializedResponse?.Id, id);
    }
    
    [Theory]
    [InlineData(0)]
    public async Task RequestAsync_NotExistedId_ErrorStatus(int id)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, $"api/user/?id={id}");

        var rest = new Client();
        var userResp = await rest.RequestAsync<User>(clientFixture.Client, message);
        
        Assert.Equal(ClientResponseStatus.Error, userResp.ResponseStatus);
    }
    
}