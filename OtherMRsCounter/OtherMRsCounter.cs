using BarRaider.SdTools;
using Streamdeck_Gitlab.Common;

namespace Streamdeck_Gitlab.OtherMRsCounter;

[PluginActionId("dev.spons.gitlab.othermrscounter")]
public class OtherMRsCounter : CounterBase
{
    public OtherMRsCounter(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
    }
    protected override string GetUrl()
    {
        return
            $"{this.Settings.ServerUrl}/dashboard/merge_requests?scope=all&state=opened&reviewer_username={this.Settings.Username}&not[approved_by_usernames]={this.Settings.Username}&not[author_username]={this.Settings.Username}";
    }

    protected override Task<int?> GetCountAsync()
    {
        return this.GitlabClient.GetOtherMRsCount();
    }
}
