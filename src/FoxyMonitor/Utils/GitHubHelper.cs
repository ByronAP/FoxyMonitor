using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FoxyMonitor.Utils
{
    internal static class GitHubHelper
    {
        public static async Task<GitHubReleaseResponse> GetLatestReleaseAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            client.DefaultRequestHeaders.Add("User-Agent", $"FoxyMonitor/{Assembly.GetEntryAssembly().GetName().Version}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            using var response = await client.GetAsync($"{Properties.Settings.Default.GitHubRepoUrl}/releases/latest");

            _ = response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

#pragma warning disable CS8603 // Possible null reference return.
            return JsonSerializer.Deserialize<GitHubReleaseResponse>(jsonString);
#pragma warning restore CS8603 // Possible null reference return.
        }

        internal class GitHubReleaseResponse
        {
            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; } = string.Empty;

            [JsonPropertyName("tag_name")]
            public string TagName { get; set; } = string.Empty;

            [JsonPropertyName("draft")]
            public bool Draft { get; set; }
        }
    }
}
