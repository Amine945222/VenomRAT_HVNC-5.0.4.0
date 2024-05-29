﻿// Decompiled with JetBrains decompiler
// Type: MessagePackLib.MessagePack.MsgPack
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace MessagePackLib.MessagePack
{
  public class MsgPack : IEnumerable
  {
    private string name;
    private string lowerName;
    private object innerValue;
    private MsgPackType valueType;
    private MsgPack parent;
    private List<MsgPack> children = new List<MsgPack>();
    private MsgPackArray refAsArray;

    private void SetName(string value)
    {
      this.name = value;
      this.lowerName = this.name.ToLower();
    }

    private void Clear()
    {
      for (int index = 0; index < this.children.Count; ++index)
        this.children[index].Clear();
      this.children.Clear();
    }

    private MsgPack InnerAdd()
    {
      MsgPack msgPack = new MsgPack();
      msgPack.parent = this;
      this.children.Add(msgPack);
      return msgPack;
    }

    private int IndexOf(string name)
    {
      int num1 = -1;
      int num2 = -1;
      string lower = name.ToLower();
      foreach (MsgPack child in this.children)
      {
        ++num1;
        if (lower.Equals(child.lowerName))
        {
          num2 = num1;
          break;
        }
      }
      return num2;
    }

    public MsgPack FindObject(string name)
    {
      int index = this.IndexOf(name);
      return index == -1 ? (MsgPack) null : this.children[index];
    }

    private MsgPack InnerAddMapChild()
    {
      if (this.valueType != MsgPackType.Map)
      {
        this.Clear();
        this.valueType = MsgPackType.Map;
      }
      return this.InnerAdd();
    }

    private MsgPack InnerAddArrayChild()
    {
      if (this.valueType != MsgPackType.Array)
      {
        this.Clear();
        this.valueType = MsgPackType.Array;
      }
      return this.InnerAdd();
    }

    public MsgPack AddArrayChild() => this.InnerAddArrayChild();

    private void WriteMap(Stream ms)
    {
      int count = this.children.Count;
      if (count <= 15)
      {
        byte num = (byte) (128U + (uint) (byte) count);
        ms.WriteByte(num);
      }
      else if (count <= (int) ushort.MaxValue)
      {
        byte num = 222;
        ms.WriteByte(num);
        byte[] buffer = BytesTools.SwapBytes(BitConverter.GetBytes((short) count));
        ms.Write(buffer, 0, buffer.Length);
      }
      else
      {
        byte num = 223;
        ms.WriteByte(num);
        byte[] buffer = BytesTools.SwapBytes(BitConverter.GetBytes(count));
        ms.Write(buffer, 0, buffer.Length);
      }
      for (int index = 0; index < count; ++index)
      {
        WriteTools.WriteString(ms, this.children[index].name);
        this.children[index].Encode2Stream(ms);
      }
    }

    private void WirteArray(Stream ms)
    {
      int count = this.children.Count;
      if (count <= 15)
      {
        byte num = (byte) (144U + (uint) (byte) count);
        ms.WriteByte(num);
      }
      else if (count <= (int) ushort.MaxValue)
      {
        byte num = 220;
        ms.WriteByte(num);
        byte[] buffer = BytesTools.SwapBytes(BitConverter.GetBytes((short) count));
        ms.Write(buffer, 0, buffer.Length);
      }
      else
      {
        byte num = 221;
        ms.WriteByte(num);
        byte[] buffer = BytesTools.SwapBytes(BitConverter.GetBytes(count));
        ms.Write(buffer, 0, buffer.Length);
      }
      for (int index = 0; index < count; ++index)
        this.children[index].Encode2Stream(ms);
    }

    public void SetAsInteger(long value)
    {
      this.innerValue = (object) value;
      this.valueType = MsgPackType.Integer;
    }

    public void SetAsUInt64(ulong value)
    {
      this.innerValue = (object) value;
      this.valueType = MsgPackType.UInt64;
    }

    public ulong GetAsUInt64()
    {
      switch (this.valueType)
      {
        case MsgPackType.String:
          return ulong.Parse(this.innerValue.ToString().Trim());
        case MsgPackType.Integer:
          return Convert.ToUInt64((long) this.innerValue);
        case MsgPackType.UInt64:
          return (ulong) this.innerValue;
        case MsgPackType.Float:
          return Convert.ToUInt64((double) this.innerValue);
        case MsgPackType.Single:
          return Convert.ToUInt64((float) this.innerValue);
        case MsgPackType.DateTime:
          return Convert.ToUInt64((DateTime) this.innerValue);
        default:
          return 0;
      }
    }

    public long GetAsInteger()
    {
      switch (this.valueType)
      {
        case MsgPackType.String:
          return long.Parse(this.innerValue.ToString().Trim());
        case MsgPackType.Integer:
          return (long) this.innerValue;
        case MsgPackType.UInt64:
          return Convert.ToInt64((long) this.innerValue);
        case MsgPackType.Float:
          return Convert.ToInt64((double) this.innerValue);
        case MsgPackType.Single:
          return Convert.ToInt64((float) this.innerValue);
        case MsgPackType.DateTime:
          return Convert.ToInt64((DateTime) this.innerValue);
        default:
          return 0;
      }
    }

    public double GetAsFloat()
    {
      switch (this.valueType)
      {
        case MsgPackType.String:
          return double.Parse((string) this.innerValue);
        case MsgPackType.Integer:
          return Convert.ToDouble((long) this.innerValue);
        case MsgPackType.Float:
          return (double) this.innerValue;
        case MsgPackType.Single:
          return (double) (float) this.innerValue;
        case MsgPackType.DateTime:
          return (double) Convert.ToInt64((DateTime) this.innerValue);
        default:
          return 0.0;
      }
    }

    public void SetAsBytes(byte[] value)
    {
      this.innerValue = (object) value;
      this.valueType = MsgPackType.Binary;
    }

    public byte[] GetAsBytes()
    {
      switch (this.valueType)
      {
        case MsgPackType.String:
          return BytesTools.GetUtf8Bytes(this.innerValue.ToString());
        case MsgPackType.Integer:
          return BitConverter.GetBytes((long) this.innerValue);
        case MsgPackType.Float:
          return BitConverter.GetBytes((double) this.innerValue);
        case MsgPackType.Single:
          return BitConverter.GetBytes((float) this.innerValue);
        case MsgPackType.DateTime:
          return BitConverter.GetBytes(((DateTime) this.innerValue).ToBinary());
        case MsgPackType.Binary:
          return (byte[]) this.innerValue;
        default:
          return new byte[0];
      }
    }

    public void Add(string key, string value)
    {
      MsgPack msgPack = this.InnerAddArrayChild();
      msgPack.name = key;
      msgPack.SetAsString(value);
    }

    public void Add(string key, int value)
    {
      MsgPack msgPack = this.InnerAddArrayChild();
      msgPack.name = key;
      msgPack.SetAsInteger((long) value);
    }

    public bool LoadFileAsBytes(string fileName)
    {
      if (!File.Exists(fileName))
        return false;
      FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
      byte[] buffer = new byte[fileStream.Length];
      fileStream.Read(buffer, 0, (int) fileStream.Length);
      fileStream.Close();
      fileStream.Dispose();
      this.SetAsBytes(buffer);
      return true;
    }

    public bool SaveBytesToFile(string fileName)
    {
      if (this.innerValue == null)
        return false;
      FileStream fileStream = new FileStream(fileName, FileMode.Append);
      fileStream.Write((byte[]) this.innerValue, 0, ((byte[]) this.innerValue).Length);
      fileStream.Close();
      fileStream.Dispose();
      return true;
    }

    public MsgPack ForcePathObject(string path)
    {
      MsgPack msgPack1 = this;
      string[] strArray = path.Trim().Split('.', '/', '\\');
      if (strArray.Length == 0)
        return (MsgPack) null;
      if (strArray.Length > 1)
      {
        for (int index = 0; index < strArray.Length - 1; ++index)
        {
          string name = strArray[index];
          MsgPack msgPack2 = msgPack1.FindObject(name);
          if (msgPack2 == null)
          {
            msgPack1 = msgPack1.InnerAddMapChild();
            msgPack1.SetName(name);
          }
          else
            msgPack1 = msgPack2;
        }
      }
      string name1 = strArray[strArray.Length - 1];
      int index1 = msgPack1.IndexOf(name1);
      if (index1 > -1)
        return msgPack1.children[index1];
      MsgPack msgPack3 = msgPack1.InnerAddMapChild();
      msgPack3.SetName(name1);
      return msgPack3;
    }

    public void SetAsNull()
    {
      this.Clear();
      this.innerValue = (object) null;
      this.valueType = MsgPackType.Null;
    }

    public void SetAsString(string value)
    {
      this.innerValue = (object) value;
      this.valueType = MsgPackType.String;
    }

    public string GetAsString() => this.innerValue == null ? "" : this.innerValue.ToString();

    public void SetAsBoolean(bool bVal)
    {
      this.valueType = MsgPackType.Boolean;
      this.innerValue = (object) bVal;
    }

    public void SetAsSingle(float fVal)
    {
      this.valueType = MsgPackType.Single;
      this.innerValue = (object) fVal;
    }

    public void SetAsFloat(double fVal)
    {
      this.valueType = MsgPackType.Float;
      this.innerValue = (object) fVal;
    }

    public void DecodeFromBytes(byte[] bytes)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        bytes = Zip.Decompress(bytes);
        ms.Write(bytes, 0, bytes.Length);
        ms.Position = 0L;
        this.DecodeFromStream((Stream) ms);
      }
    }

    public void DecodeFromFile(string fileName)
    {
      FileStream ms = new FileStream(fileName, FileMode.Open);
      this.DecodeFromStream((Stream) ms);
      ms.Dispose();
    }

    public void DecodeFromStream(Stream ms)
    {
      byte strFlag = (byte) ms.ReadByte();
      if (strFlag <= (byte) 127)
        this.SetAsInteger((long) strFlag);
      else if (strFlag >= (byte) 128 && strFlag <= (byte) 143)
      {
        this.Clear();
        this.valueType = MsgPackType.Map;
        int num = (int) strFlag - 128;
        for (int index = 0; index < num; ++index)
        {
          MsgPack msgPack = this.InnerAdd();
          msgPack.SetName(ReadTools.ReadString(ms));
          msgPack.DecodeFromStream(ms);
        }
      }
      else if (strFlag >= (byte) 144 && strFlag <= (byte) 159)
      {
        this.Clear();
        this.valueType = MsgPackType.Array;
        int num = (int) strFlag - 144;
        for (int index = 0; index < num; ++index)
          this.InnerAdd().DecodeFromStream(ms);
      }
      else if (strFlag >= (byte) 160 && strFlag <= (byte) 191)
      {
        int len = (int) strFlag - 160;
        this.SetAsString(ReadTools.ReadString(ms, len));
      }
      else if (strFlag >= (byte) 224 && strFlag <= byte.MaxValue)
      {
        this.SetAsInteger((long) (sbyte) strFlag);
      }
      else
      {
        switch (strFlag)
        {
          case 192:
            this.SetAsNull();
            break;
          case 193:
            throw new Exception("(never used) type $c1");
          case 194:
            this.SetAsBoolean(false);
            break;
          case 195:
            this.SetAsBoolean(true);
            break;
          case 196:
            int count = ms.ReadByte();
            byte[] buffer1 = new byte[count];
            ms.Read(buffer1, 0, count);
            this.SetAsBytes(buffer1);
            break;
          case 197:
            byte[] numArray1 = new byte[2];
            ms.Read(numArray1, 0, 2);
            int uint16 = (int) BitConverter.ToUInt16(BytesTools.SwapBytes(numArray1), 0);
            byte[] buffer2 = new byte[uint16];
            ms.Read(buffer2, 0, uint16);
            this.SetAsBytes(buffer2);
            break;
          case 198:
            byte[] numArray2 = new byte[4];
            ms.Read(numArray2, 0, 4);
            int int32_1 = BitConverter.ToInt32(BytesTools.SwapBytes(numArray2), 0);
            byte[] buffer3 = new byte[int32_1];
            ms.Read(buffer3, 0, int32_1);
            this.SetAsBytes(buffer3);
            break;
          default:
            if (strFlag == (byte) 199 || strFlag == (byte) 200 || strFlag == (byte) 201)
              throw new Exception("(ext8,ext16,ex32) type $c7,$c8,$c9");
            if (strFlag == (byte) 202)
            {
              byte[] numArray3 = new byte[4];
              ms.Read(numArray3, 0, 4);
              this.SetAsSingle(BitConverter.ToSingle(BytesTools.SwapBytes(numArray3), 0));
              break;
            }
            if (strFlag == (byte) 203)
            {
              byte[] numArray4 = new byte[8];
              ms.Read(numArray4, 0, 8);
              this.SetAsFloat(BitConverter.ToDouble(BytesTools.SwapBytes(numArray4), 0));
              break;
            }
            if (strFlag == (byte) 204)
            {
              this.SetAsInteger((long) (byte) ms.ReadByte());
              break;
            }
            if (strFlag == (byte) 205)
            {
              byte[] numArray5 = new byte[2];
              ms.Read(numArray5, 0, 2);
              this.SetAsInteger((long) BitConverter.ToUInt16(BytesTools.SwapBytes(numArray5), 0));
              break;
            }
            if (strFlag == (byte) 206)
            {
              byte[] numArray6 = new byte[4];
              ms.Read(numArray6, 0, 4);
              this.SetAsInteger((long) BitConverter.ToUInt32(BytesTools.SwapBytes(numArray6), 0));
              break;
            }
            if (strFlag == (byte) 207)
            {
              byte[] numArray7 = new byte[8];
              ms.Read(numArray7, 0, 8);
              this.SetAsUInt64(BitConverter.ToUInt64(BytesTools.SwapBytes(numArray7), 0));
              break;
            }
            if (strFlag == (byte) 220)
            {
              byte[] numArray8 = new byte[2];
              ms.Read(numArray8, 0, 2);
              int int16 = (int) BitConverter.ToInt16(BytesTools.SwapBytes(numArray8), 0);
              this.Clear();
              this.valueType = MsgPackType.Array;
              for (int index = 0; index < int16; ++index)
                this.InnerAdd().DecodeFromStream(ms);
              break;
            }
            if (strFlag == (byte) 221)
            {
              byte[] numArray9 = new byte[4];
              ms.Read(numArray9, 0, 4);
              int int16 = (int) BitConverter.ToInt16(BytesTools.SwapBytes(numArray9), 0);
              this.Clear();
              this.valueType = MsgPackType.Array;
              for (int index = 0; index < int16; ++index)
                this.InnerAdd().DecodeFromStream(ms);
              break;
            }
            if (strFlag == (byte) 217)
            {
              this.SetAsString(ReadTools.ReadString(strFlag, ms));
              break;
            }
            if (strFlag == (byte) 222)
            {
              byte[] numArray10 = new byte[2];
              ms.Read(numArray10, 0, 2);
              int int16 = (int) BitConverter.ToInt16(BytesTools.SwapBytes(numArray10), 0);
              this.Clear();
              this.valueType = MsgPackType.Map;
              for (int index = 0; index < int16; ++index)
              {
                MsgPack msgPack = this.InnerAdd();
                msgPack.SetName(ReadTools.ReadString(ms));
                msgPack.DecodeFromStream(ms);
              }
              break;
            }
            if (strFlag == (byte) 222)
            {
              byte[] numArray11 = new byte[2];
              ms.Read(numArray11, 0, 2);
              int int16 = (int) BitConverter.ToInt16(BytesTools.SwapBytes(numArray11), 0);
              this.Clear();
              this.valueType = MsgPackType.Map;
              for (int index = 0; index < int16; ++index)
              {
                MsgPack msgPack = this.InnerAdd();
                msgPack.SetName(ReadTools.ReadString(ms));
                msgPack.DecodeFromStream(ms);
              }
              break;
            }
            if (strFlag == (byte) 223)
            {
              byte[] numArray12 = new byte[4];
              ms.Read(numArray12, 0, 4);
              int int32_2 = BitConverter.ToInt32(BytesTools.SwapBytes(numArray12), 0);
              this.Clear();
              this.valueType = MsgPackType.Map;
              for (int index = 0; index < int32_2; ++index)
              {
                MsgPack msgPack = this.InnerAdd();
                msgPack.SetName(ReadTools.ReadString(ms));
                msgPack.DecodeFromStream(ms);
              }
              break;
            }
            if (strFlag == (byte) 218)
            {
              this.SetAsString(ReadTools.ReadString(strFlag, ms));
              break;
            }
            if (strFlag == (byte) 219)
            {
              this.SetAsString(ReadTools.ReadString(strFlag, ms));
              break;
            }
            if (strFlag == (byte) 208)
            {
              this.SetAsInteger((long) (sbyte) ms.ReadByte());
              break;
            }
            if (strFlag == (byte) 209)
            {
              byte[] numArray13 = new byte[2];
              ms.Read(numArray13, 0, 2);
              this.SetAsInteger((long) BitConverter.ToInt16(BytesTools.SwapBytes(numArray13), 0));
              break;
            }
            if (strFlag == (byte) 210)
            {
              byte[] numArray14 = new byte[4];
              ms.Read(numArray14, 0, 4);
              this.SetAsInteger((long) BitConverter.ToInt32(BytesTools.SwapBytes(numArray14), 0));
              break;
            }
            if (strFlag != (byte) 211)
              break;
            byte[] numArray15 = new byte[8];
            ms.Read(numArray15, 0, 8);
            this.SetAsInteger(BitConverter.ToInt64(BytesTools.SwapBytes(numArray15), 0));
            break;
        }
      }
    }

    public byte[] Encode2Bytes()
    {
      using (MemoryStream ms = new MemoryStream())
      {
        this.Encode2Stream((Stream) ms);
        byte[] numArray = new byte[ms.Length];
        ms.Position = 0L;
        ms.Read(numArray, 0, (int) ms.Length);
        return Zip.Compress(numArray);
      }
    }

    public void Encode2Stream(Stream ms)
    {
      switch (this.valueType)
      {
        case MsgPackType.Unknown:
        case MsgPackType.Null:
          WriteTools.WriteNull(ms);
          break;
        case MsgPackType.Map:
          this.WriteMap(ms);
          break;
        case MsgPackType.Array:
          this.WirteArray(ms);
          break;
        case MsgPackType.String:
          WriteTools.WriteString(ms, (string) this.innerValue);
          break;
        case MsgPackType.Integer:
          WriteTools.WriteInteger(ms, (long) this.innerValue);
          break;
        case MsgPackType.UInt64:
          WriteTools.WriteUInt64(ms, (ulong) this.innerValue);
          break;
        case MsgPackType.Boolean:
          WriteTools.WriteBoolean(ms, (bool) this.innerValue);
          break;
        case MsgPackType.Float:
          WriteTools.WriteFloat(ms, (double) this.innerValue);
          break;
        case MsgPackType.Single:
          WriteTools.WriteFloat(ms, (double) (float) this.innerValue);
          break;
        case MsgPackType.DateTime:
          WriteTools.WriteInteger(ms, this.GetAsInteger());
          break;
        case MsgPackType.Binary:
          WriteTools.WriteBinary(ms, (byte[]) this.innerValue);
          break;
        default:
          WriteTools.WriteNull(ms);
          break;
      }
    }

    public string AsString
    {
      get => this.GetAsString();
      set => this.SetAsString(value);
    }

    public long AsInteger
    {
      get => this.GetAsInteger();
      set => this.SetAsInteger(value);
    }

    public double AsFloat
    {
      get => this.GetAsFloat();
      set => this.SetAsFloat(value);
    }

    public MsgPackArray AsArray
    {
      get
      {
        lock (this)
        {
          if (this.refAsArray == null)
            this.refAsArray = new MsgPackArray(this, this.children);
        }
        return this.refAsArray;
      }
    }

    public MsgPackType ValueType => this.valueType;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new MsgPackEnum(this.children);
  }
}