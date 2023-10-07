using Streamdeck_Gitlab.Common;

namespace Streamdeck_Gitlab.Client;

public class GitlabClientSettings
{
    public string Token { get; }
    public string ServerUrl { get; }
    public string UserName { get; }
    public MyMrsStatusModeEnum MyMrsStatusModeMode { get; }

    public GitlabClientSettings()
    {
        this.Token = string.Empty;
        this.ServerUrl = string.Empty;
        this.UserName = string.Empty;
        this.MyMrsStatusModeMode = MyMrsStatusModeEnum.Both;
    }

    public GitlabClientSettings(string token, string serverUrl, string userName, MyMrsStatusModeEnum myMrsStatusModeMode)
    {
        this.Token = token;
        this.ServerUrl = serverUrl;
        this.UserName = userName;
        this.MyMrsStatusModeMode = myMrsStatusModeMode;
    }

    public static GitlabClientSettings FromPluginSettings(PluginSettings pluginSettings)
    {
        return new GitlabClientSettings(pluginSettings.Token, pluginSettings.ServerUrl, pluginSettings.Username, (MyMrsStatusModeEnum)pluginSettings.MyMrsStatusModeMode);
    }

    private bool Equals(GitlabClientSettings other)
    {
        return this.Token == other.Token && this.ServerUrl == other.ServerUrl && this.UserName == other.UserName && this.MyMrsStatusModeMode == other.MyMrsStatusModeMode;
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
        return HashCode.Combine(this.Token, this.ServerUrl, this.UserName, this.MyMrsStatusModeMode);
    }
}
