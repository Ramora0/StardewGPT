using System;
using StardewModdingAPI;
namespace StardewGPT.helpers
{
	public class Logger
	{
    private static IMonitor monitor;
    private static bool debug = true;
    
    public static void Init(IMonitor monitor)
    {
      Logger.monitor = monitor;
    }

    public static void Log(string message)
    {
      if (!Logger.debug)
      {
        return;
      }
      Logger.monitor.Log(message, LogLevel.Debug);
    }

    public static void Error(string message)
    {
      Logger.monitor.Log(message, LogLevel.Error);
    }

    public static void PersistentLog(string message, LogLevel level = LogLevel.Debug)
    {
      Logger.monitor.Log(message, level);
    }
	}
}

