using Streamdeck_Gitlab.Common;

namespace Streamdeck_Gitlab.Client;

public class GitlabClientSettings
{
    public string Token { get; }
    public string ServerUrl { get; }
    public string UserName { get; }
    public bool OnlyApprovedMrs { get; }

    public bool OnlyUnapprovedMrs { get; }

    public GitlabClientSettings()
    {
        this.Token = string.Empty;
        this.ServerUrl = string.Empty;
        this.UserName = string.Empty;
        this.OnlyApprovedMrs = false;
        this.OnlyUnapprovedMrs = false;
    }

    public GitlabClientSettings(string token, string serverUrl, string userName, bool onlyApprovedMrs, bool onlyUnapprovedMrs)
    {
        this.Token = token;
        this.ServerUrl = serverUrl;
        this.UserName = userName;
        this.OnlyApprovedMrs = onlyApprovedMrs;
        this.OnlyUnapprovedMrs = onlyUnapprovedMrs;
    }

    public static GitlabClientSettings FromPluginSettings(PluginSettings pluginSettings)
    {
        return new GitlabClientSettings(pluginSettings.Token, pluginSettings.ServerUrl, pluginSettings.Username, pluginSettings.OnlyApprovedMrs, pluginSettings.OnlyUnapprovedMrs);
    }

    private bool Equals(GitlabClientSettings other)
    {
        return this.Token == other.Token && this.ServerUrl == other.ServerUrl && this.UserName == other.UserName && this.OnlyApprovedMrs == other.OnlyApprovedMrs && this.OnlyUnapprovedMrs == other.OnlyUnapprovedMrs;
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
        return HashCode.Combine(this.Token, this.ServerUrl, this.UserName, this.OnlyApprovedMrs, this.OnlyUnapprovedMrs);
    }
}
