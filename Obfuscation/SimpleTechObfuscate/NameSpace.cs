using dnlib.DotNet;

namespace Obfuscation.SimpleTechObfuscate;

public class NameSpace
{
    public static void Obfuscate(ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            string encName = new GenerateKey(10).GenerateStrenghCharacter();
            if (type.Namespace == Settings.NamespaceRunPe)
            {
                Settings.NamespaceRunPeObf = new GenerateKey(10).GenerateStrenghCharacter();
                type.Namespace = Settings.NamespaceRunPeObf;
                continue;
            }

            type.Namespace = encName;
        }
    }
}