// Decompiled with JetBrains decompiler
// Type: Client.Helper.Camera
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


namespace Client.Helper
{
  internal class Camera
  {
    public static readonly Guid CLSID_VideoInputDeviceCategory = new Guid("{860BB310-5D01-11d0-BD3B-00A0C911CE86}");
    public static readonly Guid CLSID_SystemDeviceEnum = new Guid("{62BE5D10-60EB-11d0-BD3B-00A0C911CE86}");
    public static readonly Guid IID_IPropertyBag = new Guid("{55272A00-42CB-11CE-8135-00AA004BB851}");

    public static bool havecamera() => Camera.FindDevices().Length != 0;

    public static string[] FindDevices()
    {
      return Camera.GetFiltes(Camera.CLSID_VideoInputDeviceCategory).ToArray();
    }

    public static List<string> GetFiltes(Guid category)
    {
      List<string> result = new List<string>();
      Camera.EnumMonikers(category, (Func<IMoniker, Camera.IPropertyBag, bool>) ((moniker, prop) =>
      {
        object Var = (object) null;
        prop.Read("FriendlyName", ref Var, 0);
        result.Add((string) Var);
        return false;
      }));
      return result;
    }

    private static void EnumMonikers(Guid category, Func<IMoniker, Camera.IPropertyBag, bool> func)
    {
      IEnumMoniker ppEnumMoniker = (IEnumMoniker) null;
      Camera.ICreateDevEnum o1 = (Camera.ICreateDevEnum) null;
      try
      {
        o1 = (Camera.ICreateDevEnum) Activator.CreateInstance(Type.GetTypeFromCLSID(Camera.CLSID_SystemDeviceEnum));
        o1.CreateClassEnumerator(ref category, ref ppEnumMoniker, 0);
        if (ppEnumMoniker == null)
          return;
        IMoniker[] rgelt = new IMoniker[1];
        IntPtr zero = IntPtr.Zero;
        while (ppEnumMoniker.Next(rgelt.Length, rgelt, zero) == 0)
        {
          IMoniker o2 = rgelt[0];
          object ppvObj = (object) null;
          Guid iidIpropertyBag = Camera.IID_IPropertyBag;
          o2.BindToStorage((IBindCtx) null, (IMoniker) null, ref iidIpropertyBag, out ppvObj);
          Camera.IPropertyBag o3 = (Camera.IPropertyBag) ppvObj;
          try
          {
            if (func(o2, o3))
              break;
          }
          finally
          {
            Marshal.ReleaseComObject((object) o3);
            if (o2 != null)
              Marshal.ReleaseComObject((object) o2);
          }
        }
      }
      finally
      {
        if (ppEnumMoniker != null)
          Marshal.ReleaseComObject((object) ppEnumMoniker);
        if (o1 != null)
          Marshal.ReleaseComObject((object) o1);
      }
    }

    [ComVisible(true)]
    [Guid("29840822-5B84-11D0-BD3B-00A0C911CE86")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ICreateDevEnum
    {
      int CreateClassEnumerator([In] ref Guid pType, [In, Out] ref IEnumMoniker ppEnumMoniker, [In] int dwFlags);
    }

    [ComVisible(true)]
    [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IPropertyBag
    {
      int Read([MarshalAs(UnmanagedType.LPWStr)] string PropName, ref object Var, int ErrorLog);

      int Write(string PropName, ref object Var);
    }
  }
}
