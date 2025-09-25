using qclient.tests.Models;
using qclient.QClient.Enums;
using qclient.QClient.Implementations;

namespace qclient.tests;

public class ClientTests
{
    public class RequestAsync(HttpClientFixture clientFixture) : IClassFixture<HttpClientFixture>
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ExistedId_ReturnsSuccessAndCorrectUser(int id)
        {
            var messageCreator = new MessageCreator()
                .SetEndpoint("api/user/")
                .SetOrUpdateQueryParameter("id", id.ToString());
            var rest = new Client();
        
            var userResp = await rest.RequestAsync<User>(clientFixture.Client, messageCreator.GetHttpRequestMessage(HttpMethod.Get));
            var status =  userResp.ResponseStatus;
            var userId = userResp.SerializedResponse?.Id;
        
            Assert.Equal(ClientResponseStatus.Success, status);
            Assert.Equal(userId, id);
        }
    
        [Theory]
        [InlineData(0)]
        public async Task NotExistedId_ReturnsErrorAndNull(int id)
        {
            var messageCreator = new MessageCreator()
                .SetEndpoint("api/user/")
                .SetOrUpdateQueryParameter("id", id.ToString());
            var rest = new Client();
        
            var userResp = await rest.RequestAsync<User>(clientFixture.Client, messageCreator.GetHttpRequestMessage(HttpMethod.Get));
            var status =  userResp.ResponseStatus;
            var user = userResp.SerializedResponse;
        
            Assert.Equal(ClientResponseStatus.Error, status);
            Assert.True(user == null);
        }

        [Fact]
        public async Task All_ReturnsSuccessAndUsers()
        {
            var messageCreator = new MessageCreator().SetEndpoint("api/users/");
            var rest = new Client();
        
            var usersResp = await rest.RequestAsync<User[]>(clientFixture.Client, messageCreator.GetHttpRequestMessage(HttpMethod.Get));
            var status =  usersResp.ResponseStatus;
            var users = usersResp.SerializedResponse;
        
            Assert.Equal(ClientResponseStatus.Success, status);
            Assert.True(users?.FirstOrDefault() != null);
        }
    }

    public class RequestAsyncWithPagin(HttpClientFixture clientFixture) : IClassFixture<HttpClientFixture>
    {
        [Fact]
        public async Task All_ReturnsSuccessAndPaginatedUsers()
        {
            var messageCreator = new MessageCreator().SetEndpoint("api/usersWithPagin/");
            var rest = new Client();
            
            var resp = await rest.RequestAsyncWithPagination<PackageWithPagin>(
                clientFixture.Client, messageCreator, (mc, package) =>
                {
                    var token = package.PaginationToken;
                    if (token != null)
                    {
                        mc.SetOrUpdateQueryParameter("token", token);
                        package.CanBeRequested = true;
                    }
                    
                    return mc;
                });
            
            var status = resp.ResponseStatus;
            var users = resp.SerializedResponse?.SelectMany(package => package.Users).ToArray();
            
            Assert.Equal(ClientResponseStatus.Success, status);
            Assert.True(users != null);
            Assert.True(users[0].Id == 1);
            Assert.True(users[1].Id == 2);
        }
        
        [Fact]
        public async Task NoStopLogicProvided_OnlyFirstPageRequested()
        {
            var messageCreator = new MessageCreator().SetEndpoint("api/usersWithPaginWithoutIsLastProperty/");
            var rest = new Client();
            
            var resp = await rest.RequestAsyncWithPagination<PackageWithPagin>(
                clientFixture.Client, messageCreator, (mc, package) =>
                {
                    var token = package.PaginationToken;
                    if (token != null)
                        mc.SetOrUpdateQueryParameter("token", token);
                    return mc;
                });
            
            var status = resp.ResponseStatus;
            var users = resp.SerializedResponse?.SelectMany(package => package.Users).ToArray();
            
            Assert.Equal(ClientResponseStatus.Success, status);
            Assert.True(users != null);
            Assert.True(users.Length == 1);
            Assert.True(users[0].Id == 1);
        }
    }
}