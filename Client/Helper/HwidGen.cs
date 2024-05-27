// Decompiled with JetBrains decompiler
// Type: Client.Helper.HwidGen
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Client.Helper
{
  public static class HwidGen
  {
    public static string HWID()
    {
      try
      {
        byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Environment.ProcessorCount.ToString() + Environment.UserName + Environment.MachineName + (object) Environment.OSVersion + (object) new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize));
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte num in hash)
          stringBuilder.Append(num.ToString("x2"));
        return stringBuilder.ToString().Substring(0, 20).ToUpper();
      }
      catch
      {
        return "Err HWID";
      }
    }
  }
}
