// Decompiled with JetBrains decompiler
// Type: Client.Helper.SetRegistry
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using Client.Connection;
using Microsoft.Win32;
using System;


namespace Client.Helper
{
  public static class SetRegistry
  {
    private static readonly string ID = "Software\\" + Settings.hwId;

    public static bool SetValue(string name, byte[] value)
    {
      try
      {
        using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID, RegistryKeyPermissionCheck.ReadWriteSubTree))
        {
          subKey.SetValue(name, (object) value, RegistryValueKind.Binary);
          return true;
        }
      }
      catch (Exception ex)
      {
        ClientSocket.Error(ex.Message);
      }
      return false;
    }

    public static byte[] GetValue(string value)
    {
      try
      {
        using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID))
          return (byte[]) subKey.GetValue(value);
      }
      catch (Exception ex)
      {
        ClientSocket.Error(ex.Message);
      }
      return (byte[]) null;
    }

    public static bool DeleteValue(string name)
    {
      try
      {
        using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID))
        {
          subKey.DeleteValue(name);
          return true;
        }
      }
      catch (Exception ex)
      {
        ClientSocket.Error(ex.Message);
      }
      return false;
    }

    public static bool DeleteSubKey()
    {
      try
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("", true))
        {
          registryKey.DeleteSubKeyTree(SetRegistry.ID);
          return true;
        }
      }
      catch (Exception ex)
      {
        ClientSocket.Error(ex.Message);
      }
      return false;
    }
  }
}
