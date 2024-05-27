// Decompiled with JetBrains decompiler
// Type: MessagePackLib.MessagePack.ReadTools
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.IO;


namespace MessagePackLib.MessagePack
{
  internal class ReadTools
  {
    public static string ReadString(Stream ms, int len)
    {
      byte[] numArray = new byte[len];
      ms.Read(numArray, 0, len);
      return BytesTools.GetString(numArray);
    }

    public static string ReadString(Stream ms) => ReadTools.ReadString((byte) ms.ReadByte(), ms);

    public static string ReadString(byte strFlag, Stream ms)
    {
      int count = 0;
      if (strFlag >= (byte) 160 && strFlag <= (byte) 191)
      {
        count = (int) strFlag - 160;
      }
      else
      {
        switch (strFlag)
        {
          case 217:
            count = ms.ReadByte();
            break;
          case 218:
            byte[] numArray1 = new byte[2];
            ms.Read(numArray1, 0, 2);
            count = (int) BitConverter.ToUInt16(BytesTools.SwapBytes(numArray1), 0);
            break;
          case 219:
            byte[] numArray2 = new byte[4];
            ms.Read(numArray2, 0, 4);
            count = BitConverter.ToInt32(BytesTools.SwapBytes(numArray2), 0);
            break;
        }
      }
      byte[] numArray3 = new byte[count];
      ms.Read(numArray3, 0, count);
      return BytesTools.GetString(numArray3);
    }
  }
}
