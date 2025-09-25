using System.Text.Json;

namespace qclient.QClient.Constants;

public static class SerializationConstants
{
    public static readonly JsonSerializerOptions JsonSerializerOptionsDefault = new()
    { 
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
}