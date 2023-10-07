using BarRaider.SdTools;
using Streamdeck_Gitlab.Common;

namespace Streamdeck_Gitlab.TodosCounter;

[PluginActionId("dev.spons.gitlab.todoscounter")]
public class TodosCounter : CounterBase
{
    public TodosCounter(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
    }
    protected override string GetUrl()
    {
        return
            $"{this.Settings.ServerUrl}/dashboard/todos";
    }

    protected override Task<int?> GetCountAsync()
    {
        return this.GitlabClient.GetTodosCount();
    }
}
