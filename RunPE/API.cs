using System;
using System.Runtime.InteropServices;

namespace RunPE.Properties
{
    public class API
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProcessInformation
        {
            public readonly IntPtr ProcessHandle;
            public readonly IntPtr ThreadHandle;
            public readonly uint ProcessId;
            private readonly uint ThreadId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct StartupInformation
        {
            public uint Size;
            private readonly string Reserved1;
            private readonly string Desktop;
            private readonly string Title;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            private readonly byte[] Misc;

            private readonly IntPtr Reserved2;
            private readonly IntPtr StdInput;
            private readonly IntPtr StdOutput;
            private readonly IntPtr StdError;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CreateProcessA(string path, string commandLine, IntPtr processAttributes,
            IntPtr threadAttributes,
            bool inheritHandles, uint creationFlags, IntPtr environment, string currentDirectory,
            ref StartupInformation startupInfo, ref ProcessInformation processInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(IntPtr thread, int[] context);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr process, int ebx, ref int baseAdress, int bufferSize,
            ref int bytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualAllocEx(IntPtr process, int imageBase, int sizeOfImage, int type, int protect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr process, int baseAddress, byte[] payload, int bufferSize,
            ref int bytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(IntPtr thread, int[] context);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(IntPtr thread);
    }
}