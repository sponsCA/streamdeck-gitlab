using BarRaider.SdTools;
using Streamdeck_Gitlab.Common;
using Streamdeck_Gitlab.MyMRsCounter.Settings;

namespace Streamdeck_Gitlab.MyMRsCounter;

[PluginActionId("dev.spons.gitlab.mymrscounter")]
public class MyMRsCounter : CounterBase
{
    public MyMRsCounter(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
    }

    protected override string GetUrl()
    {
        var requestUri = $"{this.Settings.ServerUrl}/dashboard/merge_requests?scope=all&state=opened&author_username={this.Settings.Username}";

        switch (this.Settings.MyMrsStatus)
        {
            case MyMrsStatusEnum.All:
                break;
            case MyMrsStatusEnum.OnlyApproved:
                requestUri += $"&approved_by_usernames[]=Any";
                break;
            case MyMrsStatusEnum.OnlyUnapproved:
                requestUri += $"&approved_by_usernames[]=None";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return requestUri;
    }

    protected override Task<int?> GetCountAsync()
    {
        return this.GitlabClient.GetMyMRsCount();
    }

    protected override string FormatTitle(string count)
    {
        if (!string.IsNullOrEmpty(this.Settings.TitleFormat))
        {
            return this.Settings.TitleFormat.Replace("{count}", count);
        }

        var title = string.Empty;

        switch (this.Settings.MyMrsStatus)
        {
            case MyMrsStatusEnum.All:
                break;
            case MyMrsStatusEnum.OnlyApproved:
                title += "✅";
                break;
            case MyMrsStatusEnum.OnlyUnapproved:
                title += "🕰️";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return title + count;
    }
}
