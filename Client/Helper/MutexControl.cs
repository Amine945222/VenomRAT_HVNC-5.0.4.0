// Decompiled with JetBrains decompiler
// Type: Client.Helper.MutexControl
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System.Threading;


namespace Client.Helper
{
  public static class MutexControl
  {
    public static Mutex currentApp;

    public static bool CreateMutex()
    {
      bool createdNew;
      MutexControl.currentApp = new Mutex(false, Settings.MTX, out createdNew);
      return createdNew;
    }

    public static void CloseMutex()
    {
      if (MutexControl.currentApp == null)
        return;
      MutexControl.currentApp.Close();
      MutexControl.currentApp = (Mutex) null;
    }
  }
}
