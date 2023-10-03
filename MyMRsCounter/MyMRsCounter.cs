using BarRaider.SdTools;
using Streamdeck_Gitlab.Common;

namespace Streamdeck_Gitlab.MyMRsCounter;

[PluginActionId("dev.spons.gitlab.mymrscounter")]
public class MyMRsCounter : BaseCounter
{
    public MyMRsCounter(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
    }
    protected override string GetUrl()
    {
        var requestUri = $"{this.Settings.ServerUrl}/dashboard/merge_requests?scope=all&state=opened&author_username={this.Settings.Username}";

        if(this.Settings.OnlyApprovedMrs)
        {
            requestUri += $"&approved_by_usernames[]=Any";
        }
        if(this.Settings.OnlyUnapprovedMrs)
        {
            requestUri += $"&approved_by_usernames[]=None";
        }

        return requestUri;
    }

    protected override Task<int?> GetCountAsync()
    {
        return this.GitlabClient.GetMyMRsCount();
    }

    protected override string FormatTitle(string count)
    {
        if(!string.IsNullOrEmpty(this.Settings.TitleFormat))
        {
            return this.Settings.TitleFormat.Replace("{count}", count);
        }

        var title = string.Empty;
        if (this.Settings.OnlyApprovedMrs)
        {
            title += "✅";
        }

        if (this.Settings.OnlyUnapprovedMrs)
        {
            title += "🕰️";
        }

        return title + count;
    }
}
