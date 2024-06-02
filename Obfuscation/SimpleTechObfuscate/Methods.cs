using dnlib.DotNet;

namespace Obfuscation.SimpleTechObfuscate;

public class Methods
{
    public static void obfuscate_methods(ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            foreach (MethodDef method in type.Methods)
            {
                if (method.IsRuntimeSpecialName) continue;

                if (method.DeclaringType.IsForwarder) continue;

                if (!method.HasBody) continue;

                if (method.IsConstructor) continue;

                if (method.HasOverrides) continue;

                string encName = new GenerateKey(10).GenerateStrenghCharacter();
                if (method.Name == Settings.MethodRunPe)
                {
                    Settings.MethodRunPeObf = new GenerateKey(10).GenerateStrenghCharacter();
                    method.Name = Settings.MethodRunPeObf;
                    continue;
                }

                method.Name = encName;
            }
        }
    }
}