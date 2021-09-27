using System;
using System.Runtime.InteropServices;

namespace FoxyMonitor.Utils
{
    internal static class ConsoleUtils
    {
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = false, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public static bool ShowConsole()
        {
            var consolePtr = GetConsoleWindow();
            if (consolePtr == IntPtr.Zero)
            {
                // no console found so create one
                consolePtr = AllocConsole();
            }
            return ShowWindow(consolePtr, SW_SHOW);
        }

        public static bool HideConsole()
        {
            var consolePtr = GetConsoleWindow();
            return ShowWindow(consolePtr, SW_HIDE);
        }
    }
}
