using System.Net.Http.Headers;
using qclient.QClient.Implementations;

namespace qclient.QClient.Interfaces;

public interface IMessageCreator
{
    string UriLocalPath { get; }
    QueryParameters QueryParameters { get; }
    HttpRequestMessage GetHttpRequestMessage(HttpMethod method);
    IMessageCreator SetEndpoint(string endpoint);
    IMessageCreator SetBasicAuthentification(string login, string password);
    IMessageCreator SetAuthenticationHeaderValue(AuthenticationHeaderValue auth);
    IMessageCreator UpdatePropertiesFromUriString(string uri);
    IMessageCreator SetOrUpdateQueryParameter(string key, string value);
    IMessageCreator RemoveQueryParameter(string key);
    T GetAs<T>() where T : class, IMessageCreator;
}