﻿// Decompiled with JetBrains decompiler
// Type: MessagePackLib.MessagePack.WriteTools
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.IO;


namespace MessagePackLib.MessagePack
{
  internal class WriteTools
  {
    public static void WriteNull(Stream ms) => ms.WriteByte((byte) 192);

    public static void WriteString(Stream ms, string strVal)
    {
      byte[] utf8Bytes = BytesTools.GetUtf8Bytes(strVal);
      int length = utf8Bytes.Length;
      if (length <= 31)
      {
        byte num = (byte) (160U + (uint) (byte) length);
        ms.WriteByte(num);
      }
      else if (length <= (int) byte.MaxValue)
      {
        byte num1 = 217;
        ms.WriteByte(num1);
        byte num2 = (byte) length;
        ms.WriteByte(num2);
      }
      else if (length <= (int) ushort.MaxValue)
      {
        byte num = 218;
        ms.WriteByte(num);
        byte[] buffer = BytesTools.SwapBytes(BitConverter.GetBytes((short) length));
        ms.Write(buffer, 0, buffer.Length);
      }
      else
      {
        byte num = 219;
        ms.WriteByte(num);
        byte[] buffer = BytesTools.SwapBytes(BitConverter.GetBytes(length));
        ms.Write(buffer, 0, buffer.Length);
      }
      ms.Write(utf8Bytes, 0, utf8Bytes.Length);
    }

    public static void WriteBinary(Stream ms, byte[] rawBytes)
    {
      int length = rawBytes.Length;
      if (length <= (int) byte.MaxValue)
      {
        byte num1 = 196;
        ms.WriteByte(num1);
        byte num2 = (byte) length;
        ms.WriteByte(num2);
      }
      else if (length <= (int) ushort.MaxValue)
      {
        byte num = 197;
        ms.WriteByte(num);
        byte[] buffer = BytesTools.SwapBytes(BitConverter.GetBytes((short) length));
        ms.Write(buffer, 0, buffer.Length);
      }
      else
      {
        byte num = 198;
        ms.WriteByte(num);
        byte[] buffer = BytesTools.SwapBytes(BitConverter.GetBytes(length));
        ms.Write(buffer, 0, buffer.Length);
      }
      ms.Write(rawBytes, 0, rawBytes.Length);
    }

    public static void WriteFloat(Stream ms, double fVal)
    {
      ms.WriteByte((byte) 203);
      ms.Write(BytesTools.SwapDouble(fVal), 0, 8);
    }

    public static void WriteSingle(Stream ms, float fVal)
    {
      ms.WriteByte((byte) 202);
      ms.Write(BytesTools.SwapBytes(BitConverter.GetBytes(fVal)), 0, 4);
    }

    public static void WriteBoolean(Stream ms, bool bVal)
    {
      if (bVal)
        ms.WriteByte((byte) 195);
      else
        ms.WriteByte((byte) 194);
    }

    public static void WriteUInt64(Stream ms, ulong iVal)
    {
      ms.WriteByte((byte) 207);
      byte[] bytes = BitConverter.GetBytes(iVal);
      ms.Write(BytesTools.SwapBytes(bytes), 0, 8);
    }

    public static void WriteInteger(Stream ms, long iVal)
    {
      if (iVal >= 0L)
      {
        if (iVal <= (long) sbyte.MaxValue)
          ms.WriteByte((byte) iVal);
        else if (iVal <= (long) byte.MaxValue)
        {
          ms.WriteByte((byte) 204);
          ms.WriteByte((byte) iVal);
        }
        else if (iVal <= (long) ushort.MaxValue)
        {
          ms.WriteByte((byte) 205);
          ms.Write(BytesTools.SwapInt16((short) iVal), 0, 2);
        }
        else if (iVal <= (long) uint.MaxValue)
        {
          ms.WriteByte((byte) 206);
          ms.Write(BytesTools.SwapInt32((int) iVal), 0, 4);
        }
        else
        {
          ms.WriteByte((byte) 211);
          ms.Write(BytesTools.SwapInt64(iVal), 0, 8);
        }
      }
      else if (iVal <= (long) int.MinValue)
      {
        ms.WriteByte((byte) 211);
        ms.Write(BytesTools.SwapInt64(iVal), 0, 8);
      }
      else if (iVal <= (long) short.MinValue)
      {
        ms.WriteByte((byte) 210);
        ms.Write(BytesTools.SwapInt32((int) iVal), 0, 4);
      }
      else if (iVal <= (long) sbyte.MinValue)
      {
        ms.WriteByte((byte) 209);
        ms.Write(BytesTools.SwapInt16((short) iVal), 0, 2);
      }
      else if (iVal <= -32L)
      {
        ms.WriteByte((byte) 208);
        ms.WriteByte((byte) iVal);
      }
      else
        ms.WriteByte((byte) iVal);
    }
  }
}