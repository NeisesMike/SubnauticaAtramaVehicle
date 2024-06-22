using BepInEx.Logging;
using Nautilus.Utility;

namespace R5Submersible
{
    public static class Logger
    {
        internal static ManualLogSource MyLog { get; set; }
        public static void Log(string message)
        {
            MyLog.LogInfo(message);
        }
        public static void Error(string message)
        {
            MyLog.LogError(message);
        }
        public static void Output(string msg, int x = 500, int y = 0)
        {
            BasicText message = new BasicText(x, y);
            message.ShowMessage(msg, 4);
        }
    }
}
