using Newtonsoft.Json;

public class PluginSettings
{
    public PluginSettingsFeature Feature => string.IsNullOrWhiteSpace(this.RawFeature) ? PluginSettingsFeature.Todos : Enum.Parse<PluginSettingsFeature>(this.RawFeature);

    [JsonProperty(PropertyName = "token")] public string Token { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "serverUrl")]
    public string ServerUrl { get; set; } = string.Empty;


    [JsonProperty(PropertyName = "rawfeature")]
    public string RawFeature { get; set; } = string.Empty;
}

public enum PluginSettingsFeature
{
    Todos = 0,
    MergeRequests = 1,
}
