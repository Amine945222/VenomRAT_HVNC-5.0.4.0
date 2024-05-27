// Decompiled with JetBrains decompiler
// Type: MessagePackLib.MessagePack.Zip
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.IO;
using System.IO.Compression;


namespace MessagePackLib.MessagePack
{
  public static class Zip
  {
    public static byte[] Decompress(byte[] input)
    {
      using (MemoryStream memoryStream = new MemoryStream(input))
      {
        byte[] buffer1 = new byte[4];
        memoryStream.Read(buffer1, 0, 4);
        int int32 = BitConverter.ToInt32(buffer1, 0);
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Decompress))
        {
          byte[] buffer2 = new byte[int32];
          gzipStream.Read(buffer2, 0, int32);
          return buffer2;
        }
      }
    }

    public static byte[] Compress(byte[] input)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        byte[] bytes = BitConverter.GetBytes(input.Length);
        memoryStream.Write(bytes, 0, 4);
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Compress))
        {
          gzipStream.Write(input, 0, input.Length);
          gzipStream.Flush();
        }
        return memoryStream.ToArray();
      }
    }
  }
}
