using Newtonsoft.Json.Linq;

public class GitlabClient
{
    private HttpClient? _client;

    private string _token = string.Empty;
    private string _serverUrl = string.Empty;
    private string _userName = string.Empty;

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

    public async Task<int?> GetOtherMRsCount()
    {
        if (this._client == null)
        {
            return null;
        }

        try
        {
            var mergeRequestsResult = await this._client!.GetAsync($"merge_requests?state=opened&scope=all&reviewer_username={this._userName}");
            if (!mergeRequestsResult.IsSuccessStatusCode)
            {
                return null;
            }

            var mergeRequestsContent = await mergeRequestsResult.Content.ReadAsStringAsync();

            var mergeRequests = JArray.Parse(mergeRequestsContent);

            var mergeRequestsCount = mergeRequests.Count;

            if (mergeRequestsCount == 0)
            {
                return 0;
            }

            var unapprovedByUsername = 0;
            foreach (var mergeRequest in mergeRequests)
            {
                var mergeRequestApprovals = await this._client!.GetAsync($"projects/{mergeRequest["project_id"]}/merge_requests/{mergeRequest["iid"]}/approvals");
                if (!mergeRequestsResult.IsSuccessStatusCode)
                {
                    return null;
                }

                var mergeRequestApprovalsContent = await mergeRequestApprovals.Content.ReadAsStringAsync();

                var mergeRequestApprovalsJObject = JObject.Parse(mergeRequestApprovalsContent);

                var approvedByArray = mergeRequestApprovalsJObject["approved_by"]?.Value<JArray>();

                if (approvedByArray == null || approvedByArray.Count == 0)
                {
                    unapprovedByUsername++;
                    continue;
                }

                if (approvedByArray.Any(x => x.Value<JObject>()?["user"]?["username"]?.ToString() == this._userName))

                {
                    continue;
                }

                unapprovedByUsername++;
            }

            return unapprovedByUsername;
        }

        catch
        {
            return null;
        }
    }

    public async Task UpdateSettings(PluginSettings settings)
    {
        if (_client == null || settings.Token != _token || settings.ServerUrl != _serverUrl || settings.Username != _userName)
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
            _userName = settings.Username;

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
            var result = await this._client!.GetAsync("metadata");

            return result.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
