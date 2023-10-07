using Newtonsoft.Json.Linq;
using Streamdeck_Gitlab.Common;

namespace Streamdeck_Gitlab.Client;

public class GitlabClient
{
    private HttpClient? _client;

    private GitlabClientSettings Settings { get; set; } = new GitlabClientSettings();

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
            var mergeRequestsResult =
                await this._client!.GetAsync(
                    $"merge_requests?state=opened&scope=all&reviewer_username={this.Settings.UserName}&not[approved_by_usernames]={this.Settings.UserName}&not[author_username]={this.Settings.UserName}");
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

                if (approvedByArray.Any(x => x.Value<JObject>()?["user"]?["username"]?.ToString() == this.Settings.UserName))

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

    public async Task<int?> GetMyMRsCount()
    {
        if (this._client == null)
        {
            return null;
        }

        try
        {
            var requestUri = $"merge_requests?state=opened&scope=all&author_username={this.Settings.UserName}";

            switch (this.Settings.MyMrsStatus)
            {
                case MyMrsStatusEnum.All:
                    break;
                case MyMrsStatusEnum.OnlyApproved:
                    requestUri += $"&approved_by_usernames[]=Any";
                    break;
                case MyMrsStatusEnum.OnlyUnapproved:
                    requestUri += $"&approved_by_usernames[]=None";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var mergeRequestsResult = await this._client!.GetAsync(requestUri);
            if (!mergeRequestsResult.IsSuccessStatusCode)
            {
                return null;
            }

            var mergeRequestsContent = await mergeRequestsResult.Content.ReadAsStringAsync();

            var mergeRequests = JArray.Parse(mergeRequestsContent);

            return mergeRequests.Count;
        }

        catch
        {
            return null;
        }
    }

    public async Task UpdateSettings(PluginSettings settings)
    {
        var gitlabClientSettings = GitlabClientSettings.FromPluginSettings(settings);
        if (_client == null || !Equals(this.Settings, gitlabClientSettings))
        {
            if (!Uri.IsWellFormedUriString(settings.ServerUrl, UriKind.Absolute))
            {
                return;
            }

            if (settings.Token.Length != 20)
            {
                return;
            }

            this.Settings = gitlabClientSettings;

            var client = new HttpClient() { BaseAddress = new Uri(this.Settings.ServerUrl + "/api/v4/") };
            client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", this.Settings.Token);

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
