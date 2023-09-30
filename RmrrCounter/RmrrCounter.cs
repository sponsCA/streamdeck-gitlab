using System.Diagnostics;
using BarRaider.SdTools;

[PluginActionId("dev.spons.gitlab.rmrrcounter")]
public class RmrrCounter : KeypadBase
{
    private const uint InactiveState = 0;
    private const uint ActiveState = 1;
    private readonly RmrrCounterPluginSettings _settings;
    private readonly GitlabClient _gitlabClient;

    private Logger _logger;

    // SDK does not allow us to set the tick interval, this is a workaround to only poke the API every 10 ticks (seconds)
    private int _tickCount = GlobalConstants.RefreshRate;

    public RmrrCounter(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        Connection.SetTitleAsync("Loading...").Wait();

        _logger = new Logger(BarRaider.SdTools.Logger.Instance);

        _settings = new RmrrCounterPluginSettings();
        _gitlabClient = new GitlabClient();

        Tools.AutoPopulateSettings(_settings, payload.Settings);

        this.UpdateClient().Wait();
        this.UpdateRmrrCount().Wait();
    }

    public override void Dispose()
    {
    }

    public override async void KeyPressed(KeyPayload payload)
    {
        return;
    }

    public override async void KeyReleased(KeyPayload payload)
    {
        Process.Start(new ProcessStartInfo($"{_settings.ServerUrl}/dashboard/merge_requests?scope=all&state=opened&reviewer_username={this._settings.Username}&not[approved_by_usernames][]={this._settings.Username}") { UseShellExecute = true });
        await this.UpdateRmrrCount();
    }

    public override async void OnTick()
    {
        if (_tickCount < GlobalConstants.RefreshRate)
        {
            _tickCount++;
            return;
        }

        await this.UpdateRmrrCount();

        _tickCount = 0;
        _tickCount++;
    }

    public override async void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        Tools.AutoPopulateSettings(_settings, payload.Settings);
        await _gitlabClient.UpdateSettings(_settings);
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
    }

    private async Task UpdateClient()
    {
        var isValid = await _gitlabClient.IsValid();
        if (!isValid)
        {
            await _gitlabClient.UpdateSettings(_settings);
        }
    }

    private async Task UpdateRmrrCount()
    {
        var rmrrCount = await _gitlabClient.GetRmrrCount();

        if (rmrrCount == null)
        {
            await Connection.ShowAlert();
            return;
        }

        if (rmrrCount > 0)
        {
            await Connection.SetStateAsync(ActiveState);
        }
        else
        {
            await Connection.SetStateAsync(InactiveState);
        }

        await Connection.SetTitleAsync(rmrrCount.ToString());
    }
}
