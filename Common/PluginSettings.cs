using Newtonsoft.Json;

public class PluginSettings
{
    [JsonProperty(PropertyName = "token")] public string Token { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "serverUrl")]
    public string ServerUrl { get; set; } = "https://gitlab.com";

    [JsonProperty(PropertyName = "userName")]
    public string Username { get; set; } = string.Empty;
}
