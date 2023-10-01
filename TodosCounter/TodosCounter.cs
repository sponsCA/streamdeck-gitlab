using System.Diagnostics;
using BarRaider.SdTools;

[PluginActionId("dev.spons.gitlab.todoscounter")]
public class TodosCounter : BaseCounter
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
