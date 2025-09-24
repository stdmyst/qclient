using System.Net.Http.Headers;
using qclient.QClient.Interfaces;

namespace qclient.QClient.Implementations;

public class MessageCreator : IMessageCreator
{
    private AuthenticationHeaderValue? _authenticationHeader;

    public string UriLocalPath { get; private set; } = String.Empty;
    public QueryParameters QueryParameters { get; } = new();
    
    
    public HttpRequestMessage GetHttpRequestMessage(HttpMethod method)
    {
        var parameters = GetUriWithQueryParams();
        var message = new HttpRequestMessage(method, parameters);
        
        if (_authenticationHeader != null)
            message.Headers.Authorization = _authenticationHeader;
        
        return message;
    }

    public IMessageCreator SetEndpoint(string endpoint)
    {
        UriLocalPath = endpoint;
        return this;
    }

    public IMessageCreator SetBasicAuthentification(string login, string password)
    {
        var auth = AuthentificationHelper.GetBasicAuthenticationHeaderValue(login, password);
        return SetAuthenticationHeaderValue(auth);
    }

    public IMessageCreator SetAuthenticationHeaderValue(AuthenticationHeaderValue auth)
    {
        _authenticationHeader = auth;
        return this;
    }
    
    public IMessageCreator UpdatePropertiesFromUriString(string uriString) 
    { 
        Uri uri = new Uri(uriString); 
        
        UriLocalPath = uri.LocalPath;
        QueryParameters.UpdateFromUri(uri);
        
        return this; 
    }

    public IMessageCreator SetOrUpdateQueryParameter(string key, string value)
    {
        QueryParameters.Storage[key] = value;
        return this;
    }

    private string GetUriWithQueryParams() => UriLocalPath.Trim('/') + QueryParameters.AsUriString();
    
    public T GetAs<T>() where T : class, IMessageCreator => (this as T)!;
}