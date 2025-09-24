using qclient.tests.Models;
using qclient.QClient;
using qclient.QClient.Enums;
using qclient.QClient.Implementations;

namespace qclient.tests;

public class ClientTests()
{
    public class RequestAsyncSingle(HttpClientFixture clientFixture) : IClassFixture<HttpClientFixture>
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ExistedId_ReturnsSuccessAndCorrectUser(int id)
        {
            var messageCreator = new MessageCreator();
            var message = messageCreator.SetEndpoint("api/user/").SetOrUpdateQueryParameter("id", id.ToString()).GetHttpRequestMessage(HttpMethod.Get);
            var rest = new Client();
        
            var userResp = await rest.RequestAsync<User>(clientFixture.Client, message);
            var status =  userResp.ResponseStatus;
            var userId = userResp.SerializedResponse?.Id;
        
            Assert.Equal(ClientResponseStatus.Success, status);
            Assert.Equal(userId, id);
        }
    
        [Theory]
        [InlineData(0)]
        public async Task NotExistedId_ReturnsErrorAndNull(int id)
        {
            var messageCreator = new MessageCreator();
            var message = messageCreator.SetEndpoint("api/user/").SetOrUpdateQueryParameter("id", id.ToString()).GetHttpRequestMessage(HttpMethod.Get);
            var rest = new Client();
        
            var userResp = await rest.RequestAsync<User>(clientFixture.Client, message);
            var status =  userResp.ResponseStatus;
            var user = userResp.SerializedResponse;
        
            Assert.Equal(ClientResponseStatus.Error, status);
            Assert.True(user == null);
        }

        [Fact]
        public async Task All_ReturnsSuccessAndUsers()
        {
            var messageCreator = new MessageCreator();
            var message = messageCreator.SetEndpoint("api/users/").GetHttpRequestMessage(HttpMethod.Get);
            var rest = new Client();
        
            var usersResp = await rest.RequestAsync<User[]>(clientFixture.Client, message);
            var status =  usersResp.ResponseStatus;
            var users = usersResp.SerializedResponse;
        
            Assert.Equal(ClientResponseStatus.Success, status);
            Assert.True(users?.FirstOrDefault() != null);
        }
    }
}