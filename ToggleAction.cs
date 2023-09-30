using BarRaider.SdTools;

[PluginActionId("dev.spons.gitlab.toggle")]
public class ToggleAction : KeypadBase
{
    private const uint InactiveState = 0;
    private const uint ActiveState = 1;
    private readonly PluginSettings _settings;
    private readonly GitlabClient _gitlabClient;

    // SDK does not allow us to set the tick interval, this is a workaround to only poke the API every 10 ticks (seconds)
    private int _tickCount = 2;

    public ToggleAction(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        Connection.SetTitleAsync("Loading...").Wait();

        _settings = new PluginSettings();
        _gitlabClient = new GitlabClient();

        Tools.AutoPopulateSettings(_settings, payload.Settings);

        this.UpdateClient().Wait();
    }

    public override void Dispose()
    {
    }

    public override void KeyPressed(KeyPayload payload)
    {
    }

    public override async void KeyReleased(KeyPayload payload)
    {
    }

    public override async void OnTick()
    {
        if (_tickCount < 2)
        {
            _tickCount++;
            return;
        }

        if (_settings.Feature == PluginSettingsFeature.Todos)
        {
            await this.UpdateTodosCount();
        }
        else
        {
            await Connection.ShowAlert();
        }

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

    private async Task UpdateTodosCount()
    {
        var todosCount = await _gitlabClient.GetTodosCount();

        if (todosCount == null)
        {
            await Connection.ShowAlert();
            return;
        }

        await Connection.SetTitleAsync(todosCount.ToString());
    }
}
