// Decompiled with JetBrains decompiler
// Type: MessagePackLib.MessagePack.MsgPackArray
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System.Collections.Generic;


namespace MessagePackLib.MessagePack
{
  public class MsgPackArray
  {
    private List<MsgPack> children;
    private MsgPack owner;

    public MsgPackArray(MsgPack msgpackObj, List<MsgPack> listObj)
    {
      this.owner = msgpackObj;
      this.children = listObj;
    }

    public MsgPack Add() => this.owner.AddArrayChild();

    public MsgPack Add(string value)
    {
      MsgPack msgPack = this.owner.AddArrayChild();
      msgPack.AsString = value;
      return msgPack;
    }

    public MsgPack Add(long value)
    {
      MsgPack msgPack = this.owner.AddArrayChild();
      msgPack.SetAsInteger(value);
      return msgPack;
    }

    public MsgPack Add(double value)
    {
      MsgPack msgPack = this.owner.AddArrayChild();
      msgPack.SetAsFloat(value);
      return msgPack;
    }

    public MsgPack this[int index] => this.children[index];

    public int Length => this.children.Count;
  }
}
