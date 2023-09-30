using Newtonsoft.Json.Linq;

public class GitlabClient
{
    private HttpClient? _client;

    private string _token = string.Empty;
    private string _serverUrl = string.Empty;

    public async Task<bool> IsValid()
    {
        return this._client != null && await this.TestConnectionAsync();
    }

    public async Task<int?> GetTodosCount()
    {
        if (this._client == null)
        {
            return null;
        }

        try
        {
            var result = await this._client!.GetAsync("todos");

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await result.Content.ReadAsStringAsync();

            var jarray = JArray.Parse(content);

            return jarray.Count;
        }

        catch
        {
            return null;
        }
    }

    public async Task UpdateSettings(PluginSettings settings)
    {
        if (_client == null || settings.Token != _token || settings.ServerUrl != _serverUrl)
        {
            if (!Uri.IsWellFormedUriString(settings.ServerUrl, UriKind.Absolute))
            {
                return;
            }

            if (settings.Token.Length != 20)
            {
                return;
            }

            _serverUrl = settings.ServerUrl;
            _token = settings.Token;

            var client = new HttpClient() { BaseAddress = new Uri(settings.ServerUrl + "/api/v4/") };
            client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", this._token);

            this._client = client;
            return;
        }


        if (await TestConnectionAsync())
        {
            return;
        }

        this._client = null;
    }

    private async Task<bool> TestConnectionAsync()
    {
        try
        {
            var result = await this._client!.GetAsync("/metadata");

            return result.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
