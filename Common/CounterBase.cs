﻿using System.Diagnostics;
using BarRaider.SdTools;
using Streamdeck_Gitlab.Client;

namespace Streamdeck_Gitlab.Common;

public abstract class CounterBase : KeypadBase
{
    private const uint InactiveState = 0;
    private const uint ActiveState = 1;
    protected readonly PluginSettings Settings;
    protected readonly GitlabClient GitlabClient;

    // SDK does not allow us to set the tick interval, this is a workaround to only poke the API every X ticks (defined in GlobalConstants)
    private int _tickCount = GlobalConstants.RefreshRate;

    protected CounterBase(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        Settings = new PluginSettings();
        GitlabClient = new GitlabClient();
        Tools.AutoPopulateSettings(Settings, payload.Settings);

        this.UpdateClientAsync().Wait();
        this.UpdateCounterAsync().Wait();
    }

    public override void Dispose()
    {
    }

    public override async void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        Tools.AutoPopulateSettings(this.Settings, payload.Settings);
        await this.GitlabClient.UpdateSettings(this.Settings);
        await this.UpdateCounterAsync();
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
        await this.UpdateCounterAsync();
    }

    public override async void OnTick()
    {
        if (_tickCount < GlobalConstants.RefreshRate)
        {
            _tickCount++;
            return;
        }

        await this.UpdateCounterAsync();

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

    private async Task UpdateCounterAsync()
    {
        var count = await this.GetCountAsync();

        switch (count)
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

        await Connection.SetTitleAsync(FormatTitle(count.ToString()!));
    }

    protected virtual string FormatTitle(string count)
    {
        return !string.IsNullOrEmpty(this.Settings.TitleFormat) ? this.Settings.TitleFormat.Replace("{count}", count) : count;
    }

    protected abstract string GetUrl();

    protected abstract Task<int?> GetCountAsync();
}
