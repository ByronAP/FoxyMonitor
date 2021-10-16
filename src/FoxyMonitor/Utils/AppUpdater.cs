using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FoxyMonitor.Utils
{
    internal static class AppUpdater
    {
        internal static async Task<CheckForUpdateResponse> CheckForUpdateAsync(ILogger? logger)
        {
            var response = new CheckForUpdateResponse();
            response.HasUpdate = false;

            logger?.LogInformation("Checking GitHub for application updates.");

            var latestRelease = await GitHubHelper.GetLatestReleaseAsync();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var currentVersion = Assembly.GetEntryAssembly().GetName().Version;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            logger?.LogInformation("Retrieved latest application version from GitHub, {LatestVersionTag} installed version is {CurrentVersion}", latestRelease.TagName, $"{currentVersion?.Major}.{currentVersion?.Minor}.{currentVersion?.Revision}");

            Properties.Settings.Default.LastUpdateCheck = DateTimeOffset.UtcNow;
            Properties.Settings.Default.Save();

            if (latestRelease.Draft)
            {
                logger?.LogInformation("Latest application version is a draft, exiting update.");
                return response;
            }

            var tagSections = latestRelease.TagName.Split('.');

            if (tagSections.Length < 3)
            {
                logger?.LogError("Latest version tag {LatestVersionTag} from GitHub is not valid.");
                return response;
            }

            var majorVersion = int.Parse(tagSections[0]);
            var minorVersion = int.Parse(tagSections[1]);
            var revisionVersion = int.Parse(tagSections[2]);

            bool hasUpdate = false;

            if (majorVersion > currentVersion?.Major)
            {
                hasUpdate = true;
            }
            else
            {
                if (majorVersion == currentVersion?.Major && minorVersion > currentVersion.Minor)
                {
                    hasUpdate = true;
                }
                else
                {
                    if (majorVersion == currentVersion?.Major && minorVersion == currentVersion.Minor && revisionVersion > currentVersion.Revision)
                    {
                        hasUpdate = true;
                    }

                }
            }

            response.HasUpdate = hasUpdate;
            response.GitHubReleaseResponse = latestRelease;

            return response;
        }

        internal class CheckForUpdateResponse
        {
            public bool HasUpdate { get; set; }
            public GitHubHelper.GitHubReleaseResponse? GitHubReleaseResponse { get; set; }
        }
    }
}
