using System.Diagnostics;
using BarRaider.SdTools;
using Streamdeck_Gitlab.Client;

namespace Streamdeck_Gitlab.Common;

public abstract class BaseCounter : KeypadBase
{
    private const uint InactiveState = 0;
    private const uint ActiveState = 1;
    protected readonly PluginSettings Settings;
    protected readonly GitlabClient GitlabClient;

    // SDK does not allow us to set the tick interval, this is a workaround to only poke the API every X ticks (defined in GlobalConstants)
    private int _tickCount = GlobalConstants.RefreshRate;

    protected BaseCounter(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        Settings = new PluginSettings();
        GitlabClient = new GitlabClient();
        Tools.AutoPopulateSettings(Settings, payload.Settings);

        this.UpdateClientAsync().Wait();
        this.UpdateCountAsync().Wait();
    }

    public override void Dispose()
    {
    }

    public override async void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        Tools.AutoPopulateSettings(this.Settings, payload.Settings);
        await this.GitlabClient.UpdateSettings(this.Settings);
        await this.UpdateCountAsync();
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
    }


    public override void KeyPressed(KeyPayload payload)
    {
    }

    public override async void KeyReleased(KeyPayload payload)
    {
        Process.Start(new ProcessStartInfo(this.GetUrl()) { UseShellExecute = true });
        await this.UpdateCountAsync();
    }

    public override async void OnTick()
    {
        if (_tickCount < GlobalConstants.RefreshRate)
        {
            _tickCount++;
            return;
        }

        await this.UpdateCountAsync();

        _tickCount = 1;
    }

    private async Task UpdateClientAsync()
    {
        var isValid = await GitlabClient.IsValid();
        if (!isValid)
        {
            await GitlabClient.UpdateSettings(Settings);
        }
    }

    private async Task UpdateCountAsync()
    {
        var todosCount = await this.GetCountAsync();

        switch (todosCount)
        {
            case null:
                await Connection.ShowAlert();
                return;
            case > 0:
                await Connection.SetStateAsync(ActiveState);
                break;
            default:
                await Connection.SetStateAsync(InactiveState);
                break;
        }

        await Connection.SetTitleAsync(todosCount.ToString());
    }

    protected abstract string GetUrl();

    protected abstract Task<int?> GetCountAsync();
}
