using System;
using System.IO;

namespace FoxyMonitor.Utils
{
    internal static class IOUtils
    {
        internal static string GetDataDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.Create),
                Constants.LocalDirectoryName,
                Constants.DataDirectoryName)
                .EnsureDirectoryExists();
        }

        internal static string GetLoggingDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.Create),
                Constants.LocalDirectoryName,
                Constants.LogsDirectoryName)
                .EnsureDirectoryExists();
        }

        private static string EnsureDirectoryExists(this string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}