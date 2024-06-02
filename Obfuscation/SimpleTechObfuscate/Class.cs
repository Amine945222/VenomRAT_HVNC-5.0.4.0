using dnlib.DotNet;

namespace Obfuscation.SimpleTechObfuscate;

public class Class
{
    public static void Obfuscate(ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            string encName = new GenerateKey(10).GenerateStrenghCharacter();
            if (type.Name == Settings.ClassRunPe)
            {
                Settings.ClassRunPeObf = new GenerateKey(10).GenerateStrenghCharacter();
                type.Name = Settings.ClassRunPeObf;
                continue;
            }

            type.Name = encName;
        }
    }
}