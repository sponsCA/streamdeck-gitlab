using Newtonsoft.Json;

public class RmrrCounterPluginSettings : PluginSettings
{
    [JsonProperty(PropertyName = "userName")]
    public string Username { get; set; } = string.Empty;
}
