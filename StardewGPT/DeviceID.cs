using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace StardewGPT
{
	public class DeviceID
	{
    public static string GetMacId() {
      var macAddr = 
      (
          from nic in NetworkInterface.GetAllNetworkInterfaces()
          where nic.OperationalStatus == OperationalStatus.Up
          select nic.GetPhysicalAddress().ToString()
      ).FirstOrDefault();
      return macAddr;
    }
	}
}

