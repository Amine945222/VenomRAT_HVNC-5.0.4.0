// Decompiled with JetBrains decompiler
// Type: Client.Helper.NativeMethods
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Client.Helper
{
  public static class NativeMethods
  {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern void RtlSetProcessIsCritical(uint v1, uint v2, uint v3);

    public enum EXECUTION_STATE : uint
    {
      ES_SYSTEM_REQUIRED = 1,
      ES_DISPLAY_REQUIRED = 2,
      ES_CONTINUOUS = 2147483648, // 0x80000000
    }

    internal struct LASTINPUTINFO
    {
      public static readonly int SizeOf = Marshal.SizeOf(typeof (NativeMethods.LASTINPUTINFO));
      [MarshalAs(UnmanagedType.U4)]
      public uint cbSize;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwTime;
    }
  }
}
