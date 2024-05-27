﻿// Decompiled with JetBrains decompiler
// Type: Client.Algorithm.Aes256
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;


namespace Client.Algorithm
{
  public class Aes256
  {
    private const int KeyLength = 32;
    private const int AuthKeyLength = 64;
    private const int IvLength = 16;
    private const int HmacSha256Length = 32;
    private readonly byte[] _key;
    private readonly byte[] _authKey;
    private static readonly byte[] Salt = Encoding.ASCII.GetBytes("DcRatByqwqdanchun");

    public Aes256(string masterKey)
    {
      if (string.IsNullOrEmpty(masterKey))
        throw new ArgumentException("masterKey can not be null or empty.");
      using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(masterKey, Aes256.Salt, 50000))
      {
        this._key = rfc2898DeriveBytes.GetBytes(32);
        this._authKey = rfc2898DeriveBytes.GetBytes(64);
      }
    }

    public string Encrypt(string input)
    {
      return Convert.ToBase64String(this.Encrypt(Encoding.UTF8.GetBytes(input)));
    }

    public byte[] Encrypt(byte[] input)
    {
      if (input == null)
        throw new ArgumentNullException("input can not be null.");
      using (MemoryStream memoryStream = new MemoryStream())
      {
        memoryStream.Position = 32L;
        using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
        {
          cryptoServiceProvider.KeySize = 256;
          cryptoServiceProvider.BlockSize = 128;
          cryptoServiceProvider.Mode = CipherMode.CBC;
          cryptoServiceProvider.Padding = PaddingMode.PKCS7;
          cryptoServiceProvider.Key = this._key;
          cryptoServiceProvider.GenerateIV();
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, cryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
          {
            memoryStream.Write(cryptoServiceProvider.IV, 0, cryptoServiceProvider.IV.Length);
            cryptoStream.Write(input, 0, input.Length);
            cryptoStream.FlushFinalBlock();
            using (HMACSHA256 hmacshA256 = new HMACSHA256(this._authKey))
            {
              byte[] hash = hmacshA256.ComputeHash(memoryStream.ToArray(), 32, memoryStream.ToArray().Length - 32);
              memoryStream.Position = 0L;
              memoryStream.Write(hash, 0, hash.Length);
            }
          }
        }
        return memoryStream.ToArray();
      }
    }

    public string Decrypt(string input)
    {
      return Encoding.UTF8.GetString(this.Decrypt(Convert.FromBase64String(input)));
    }

    public byte[] Decrypt(byte[] input)
    {
      if (input == null)
        throw new ArgumentNullException("input can not be null.");
      using (MemoryStream memoryStream = new MemoryStream(input))
      {
        using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
        {
          cryptoServiceProvider.KeySize = 256;
          cryptoServiceProvider.BlockSize = 128;
          cryptoServiceProvider.Mode = CipherMode.CBC;
          cryptoServiceProvider.Padding = PaddingMode.PKCS7;
          cryptoServiceProvider.Key = this._key;
          using (HMACSHA256 hmacshA256 = new HMACSHA256(this._authKey))
          {
            byte[] hash = hmacshA256.ComputeHash(memoryStream.ToArray(), 32, memoryStream.ToArray().Length - 32);
            byte[] numArray = new byte[32];
            memoryStream.Read(numArray, 0, numArray.Length);
            if (!this.AreEqual(hash, numArray))
              throw new CryptographicException("Invalid message authentication code (MAC).");
          }
          byte[] buffer = new byte[16];
          memoryStream.Read(buffer, 0, 16);
          cryptoServiceProvider.IV = buffer;
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, cryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read))
          {
            byte[] numArray = new byte[memoryStream.Length - 16L + 1L];
            byte[] dst = new byte[cryptoStream.Read(numArray, 0, numArray.Length)];
            Buffer.BlockCopy((Array) numArray, 0, (Array) dst, 0, dst.Length);
            return dst;
          }
        }
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private bool AreEqual(byte[] a1, byte[] a2)
    {
      bool flag = true;
      for (int index = 0; index < a1.Length; ++index)
      {
        if ((int) a1[index] != (int) a2[index])
          flag = false;
      }
      return flag;
    }
  }
}
