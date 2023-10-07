using Streamdeck_Gitlab.Common;

namespace Streamdeck_Gitlab.Client;

public class GitlabClientSettings
{
    public string Token { get; }
    public string ServerUrl { get; }
    public string UserName { get; }
    public MyMrsStatusModeEnum MyMrsStatusMode { get; }

    public GitlabClientSettings()
    {
        this.Token = string.Empty;
        this.ServerUrl = string.Empty;
        this.UserName = string.Empty;
        this.MyMrsStatusMode = MyMrsStatusModeEnum.Both;
    }

    public GitlabClientSettings(string token, string serverUrl, string userName, MyMrsStatusModeEnum myMrsStatusMode)
    {
        this.Token = token;
        this.ServerUrl = serverUrl;
        this.UserName = userName;
        this.MyMrsStatusMode = myMrsStatusMode;
    }

    public static GitlabClientSettings FromPluginSettings(PluginSettings pluginSettings)
    {
        return new GitlabClientSettings(pluginSettings.Token, pluginSettings.ServerUrl, pluginSettings.Username, (MyMrsStatusModeEnum)pluginSettings.MyMrsStatusMode);
    }

    private bool Equals(GitlabClientSettings other)
    {
        return this.Token == other.Token && this.ServerUrl == other.ServerUrl && this.UserName == other.UserName && this.MyMrsStatusMode == other.MyMrsStatusMode;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((GitlabClientSettings)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Token, this.ServerUrl, this.UserName, this.MyMrsStatusMode);
    }
}
