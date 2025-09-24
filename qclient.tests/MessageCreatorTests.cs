using System.Collections.Specialized;
using System.Web;
using qclient.QClient.Implementations;

namespace qclient.tests;

public class MessageCreatorTests
{
    [Fact]
    public void SetEndpoint_StringArgumentPassed_ArgumentWithUriLocalPathAreEquals()
    {
        var messageCreator = new MessageCreator();
        var endpoint = "/api/users";

        messageCreator.SetEndpoint(endpoint);
        var messageCreatorEndpoint = messageCreator.UriLocalPath;

        Assert.Equal(messageCreatorEndpoint, endpoint);
    }

    [Theory]
    [InlineData("http://localhost:8080/api/users?name=*el&email=*.com")]
    [InlineData("http://localhost:8080/api/tasks?id=1")]
    public void UpdatePropertiesFromUriString_UriStringPassed_UriLocalPathAreUpdated(string uriString)
    {
        var messageCreator = new MessageCreator();
        var localPath = SelectLocalPathFromStringUri(uriString);

        messageCreator.UpdatePropertiesFromUriString(uriString);

        Assert.True(AreUriLocalPathUpdated(messageCreator, localPath));
    }

    [Theory]
    [InlineData("http://localhost:8080/api/users?name=*el&email=*.com")]
    [InlineData("http://localhost:8080/api/tasks?id=1")]
    public void UpdatePropertiesFromUriString_UriStringPassed_QueryParametersAreUpdated(string uriString)
    {
        var messageCreator = new MessageCreator();
        var queryParameters = SelectQueryParametersFromStringUri(uriString);

        messageCreator.UpdatePropertiesFromUriString(uriString);

        Assert.True(AreQueryParametersUpdated(messageCreator, queryParameters));
    }
    
    [Fact]
    public void UpdatePropertiesFromUriString_DuplicateKeysPassed_OnlyFirstValueAdded()
    {
        var bothIdsKey = "id";
        var firstId = "1";
        var secondId = "2";
        var uri = $"http://localhost:8080/api/users?{bothIdsKey}={firstId}&{bothIdsKey}={secondId}";
        var messageCreator = new MessageCreator();

        messageCreator.UpdatePropertiesFromUriString(uri);
        
        Assert.Equal(firstId, messageCreator.QueryParameters.Storage[bothIdsKey]);
    }
    
    private string SelectLocalPathFromStringUri(string uriString) => new Uri(uriString).LocalPath;

    private Dictionary<string, string> SelectQueryParametersFromStringUri(string uriString)
    {
        var uri = new Uri(uriString);
        var queries = HttpUtility.ParseQueryString(uri.Query);

        return NameValueCollectionToDictionary(queries);
    }

    private Dictionary<string, string> NameValueCollectionToDictionary(NameValueCollection collection)
    {
        var targetCollection = new Dictionary<string, string>();
        
        foreach (var key in collection.AllKeys)
        {
            var value = collection.Get(key);
            if (key != null && value != null)
            {
                targetCollection[key] = value;
            }
        }

        return targetCollection;
    }

    private bool AreUriLocalPathUpdated(MessageCreator messageCreator, string localPath)
        => String.Equals(localPath, messageCreator.UriLocalPath, StringComparison.Ordinal);

    private bool AreQueryParametersUpdated(MessageCreator messageCreator, IDictionary<string, string> queryParameters)
    {
        var messageCreatorQueries = messageCreator.QueryParameters.Storage;
        
        foreach (var query in queryParameters)
            if (!(messageCreatorQueries.ContainsKey(query.Key) && messageCreatorQueries[query.Key].Equals(query.Value)))
                return false;
        
        return true;
    }
}