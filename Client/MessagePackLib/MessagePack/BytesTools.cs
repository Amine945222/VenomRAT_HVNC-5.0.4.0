// Decompiled with JetBrains decompiler
// Type: MessagePackLib.MessagePack.BytesTools
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.Text;


namespace MessagePackLib.MessagePack
{
  public class BytesTools
  {
    private static UTF8Encoding utf8Encode = new UTF8Encoding();

    public static byte[] GetUtf8Bytes(string s) => BytesTools.utf8Encode.GetBytes(s);

    public static string GetString(byte[] utf8Bytes) => BytesTools.utf8Encode.GetString(utf8Bytes);

    public static string BytesAsString(byte[] bytes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in bytes)
        stringBuilder.Append(string.Format("{0:D3} ", (object) num));
      return stringBuilder.ToString();
    }

    public static string BytesAsHexString(byte[] bytes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in bytes)
        stringBuilder.Append(string.Format("{0:X2} ", (object) num));
      return stringBuilder.ToString();
    }

    public static byte[] SwapBytes(byte[] v)
    {
      byte[] numArray = new byte[v.Length];
      int index1 = v.Length - 1;
      for (int index2 = 0; index2 < numArray.Length; ++index2)
      {
        numArray[index2] = v[index1];
        --index1;
      }
      return numArray;
    }

    public static byte[] SwapInt64(long v) => BytesTools.SwapBytes(BitConverter.GetBytes(v));

    public static byte[] SwapInt32(int v)
    {
      byte[] numArray = new byte[4]
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) v
      };
      numArray[2] = (byte) (v >> 8);
      numArray[1] = (byte) (v >> 16);
      numArray[0] = (byte) (v >> 24);
      return numArray;
    }

    public static byte[] SwapInt16(short v)
    {
      byte[] numArray = new byte[2]{ (byte) 0, (byte) v };
      numArray[0] = (byte) ((uint) v >> 8);
      return numArray;
    }

    public static byte[] SwapDouble(double v) => BytesTools.SwapBytes(BitConverter.GetBytes(v));
  }
}
