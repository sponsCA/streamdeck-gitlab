using Newtonsoft.Json;

namespace Streamdeck_Gitlab.Common;

public class PluginSettings
{
    [JsonProperty(PropertyName = "token")] public string Token { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "serverUrl")]
    public string ServerUrl { get; set; } = "https://gitlab.com";

    [JsonProperty(PropertyName = "titleFormat")]
    public string TitleFormat { get; set; } = string.Empty;

    #region MyMRsCounter and OtherMRsCounter

    [JsonProperty(PropertyName = "userName")]
    public string Username { get; set; } = string.Empty;

    #endregion

    #region MyMRsCounter

    [JsonProperty(PropertyName = "onlyApprovedMrs")]
    public bool OnlyApprovedMrs { get; set; } = false;

    [JsonProperty(PropertyName = "onlyUnapprovedMrs")]
    public bool OnlyUnapprovedMrs { get; set; } = false;

    #endregion
}
