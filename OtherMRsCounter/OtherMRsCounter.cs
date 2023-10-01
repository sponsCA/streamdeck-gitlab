using BarRaider.SdTools;

[PluginActionId("dev.spons.gitlab.othermrscounter")]
public class OtherMRsCounter : BaseCounter
{
    public OtherMRsCounter(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
    }
    protected override string GetUrl()
    {
        return
            $"{this.Settings.ServerUrl}/dashboard/merge_requests?scope=all&state=opened&reviewer_username={this.Settings.Username}&not[approved_by_usernames][]={this.Settings.Username}";
    }

    protected override Task<int?> GetCountAsync()
    {
        return this.GitlabClient.GetOtherMRsCount();
    }
}
