using System;
using StardewModdingAPI;
namespace StardewGPT.helpers
{
	public class Logger
	{
    private static IMonitor monitor;
    
    public static void Init(IMonitor monitor)
    {
      Logger.monitor = monitor;
    }

    public static void Log(string message, LogLevel level = LogLevel.Debug)
    {
      Logger.monitor.Log(message, level);
    }
	}
}

