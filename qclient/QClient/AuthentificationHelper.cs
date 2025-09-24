using System.Net.Http.Headers;

namespace qclient.QClient;

public static class AuthentificationHelper
{
    public static AuthenticationHeaderValue GetBasicAuthenticationHeaderValue(string login, string password)
    {
        var base64String = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(login + ':' + password));

        return new AuthenticationHeaderValue(scheme: "Basic", parameter: base64String);
    }
}
