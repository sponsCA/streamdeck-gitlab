using BarRaider.SdTools;

[PluginActionId("dev.spons.gitlab.mymrscounter")]
public class MyMRsCounter : BaseCounter
{
    public MyMRsCounter(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
    }
    protected override string GetUrl()
    {
        return
            $"{this.Settings.ServerUrl}/dashboard/merge_requests?scope=all&state=opened&author_username={this.Settings.Username}";
    }

    protected override Task<int?> GetCountAsync()
    {
        return this.GitlabClient.GetMyMRsCount();
    }
}
