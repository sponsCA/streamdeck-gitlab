using NUnit.Framework;

namespace Streamdeck_Gitlab_Tests
{
    public class Test
    {
        [Test]
        public async Task TestAPI()
        {
            var gitLabToken = "SECRET";

            var client = new HttpClient() { BaseAddress = new Uri("SECRET" + "/api/v4/") };
            client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", gitLabToken);
        }
    }
}
