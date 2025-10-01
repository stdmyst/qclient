using System.Net;
using System.Text.Json;
using qclient.tests.Models;
using qclient.QClient.Implementations;

namespace qclient.tests;

public class ClientTests
{
    public class RequestAsync(HttpClientFixture clientFixture) : IClassFixture<HttpClientFixture>
    {
        [Fact]
        public async Task All_ReturnsUsers()
        {
            var mc = new MessageCreator().SetEndpoint("api/users/");
            
            var users = await clientFixture.QClient.RequestAsync<IList<User>>(mc.GetHttpRequestMessage(HttpMethod.Get), CancellationToken.None);
            var count = users.Count();
            
            Assert.Equal(2, count);
            Assert.Equal(1, users[0].Id);
            Assert.Equal(2, users[1].Id);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ExistedId_ReturnsUser(int id)
        {
            var mc = new MessageCreator().SetEndpoint("api/user/").SetOrUpdateQueryParameter("id", id.ToString());
            
            var user = await clientFixture.QClient.RequestAsync<User>(mc.GetHttpRequestMessage(HttpMethod.Get), CancellationToken.None);
            
            Assert.Equal(user.Id, id);
        }
        
        [Theory]
        [InlineData(0)]
        public async Task NotExistedId_ThrowsWith404StatusCode(int id)
        {
            var mc = new MessageCreator().SetEndpoint("api/user/").SetOrUpdateQueryParameter("id", id.ToString());
            var task = clientFixture.QClient.RequestAsync<User>(mc.GetHttpRequestMessage(HttpMethod.Get), CancellationToken.None);
            
            var exception = await Record.ExceptionAsync(() => task) as HttpRequestException;
        
            Assert.True(exception != null);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
        
        [Fact]
        public async Task NotValidType_ThrowsJsonException()
        {
            var mc = new MessageCreator().SetEndpoint("api/user/").SetOrUpdateQueryParameter("id", "1");
            var task = clientFixture.QClient.RequestAsync<int>(mc.GetHttpRequestMessage(HttpMethod.Get), CancellationToken.None);
            
            var exception = await Record.ExceptionAsync(() => task);
        
            Assert.IsType<JsonException>(exception);
        }
        
        [Fact]
        public async Task CancellationRequested_ThrowsTaskCanceledException()
        {
            var mc = new MessageCreator().SetEndpoint("api/users/");
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            var task = clientFixture.QClient.RequestAsync<User>(mc.GetHttpRequestMessage(HttpMethod.Get), ct);
            Task[] tasks =  [task, cts.CancelAsync()];
            
            var exception = await Record.ExceptionAsync(() => Task.WhenAll(tasks));
        
            Assert.IsType<TaskCanceledException>(exception);
        }

        public class RequestAsyncWithPagin(HttpClientFixture clientFixture) : IClassFixture<HttpClientFixture>
        {
            [Fact]
            public async Task All_ReturnsPaginatedUsers()
            {
                var mc = new MessageCreator().SetEndpoint("api/usersWithPagin/");

                var packages = await clientFixture.QClient.RequestAsyncWithPagination(mc, clientFixture.PaginationShouldContinue, CancellationToken.None);
                var users = packages.SelectMany(p => p.Users).ToArray();
                var usersCount = users.Length;
                
                Assert.True(users != null);
                Assert.Equal(2, usersCount);
                Assert.True(users[0].Id == 1);
                Assert.True(users[1].Id == 2);
            }
        }
        
        [Fact]
        public async Task NoStopLogicProvided_ThrowsNullReferenceException()
        {
            var mc = new MessageCreator().SetEndpoint("api/usersWithPagin/");
            var task = clientFixture.QClient.RequestAsyncWithPagination(mc, clientFixture.PaginationAlwaysContinue, CancellationToken.None);
            
            var exception = await Record.ExceptionAsync(() => task);
           
            Assert.IsType<NullReferenceException>(exception);
        }
    }
}