namespace FoxyMonitor
{
    internal static class Constants
    {
        internal const string AppMutexName = "FoxyMonitor";
        internal const string AppId = "4070b592-e694-4091-8369-53dcf19dfe30";
        internal const string LocalDirectoryName = "FoxyMonitor";
        internal const string DbFileName = "fmdata.dat";
        internal const string DataDirectoryName = "data";
        internal const string LogsDirectoryName = "logs";
        internal const ulong MaxLogFileSize = 2_621_440; //2.5MB
        internal const uint MaxLogWriteQueueSize = 30;
        internal const string LogFileNameTemplate = "debug-<counter>.log";
        internal const string LogFileCounterFormat = "0000";
    }
}
