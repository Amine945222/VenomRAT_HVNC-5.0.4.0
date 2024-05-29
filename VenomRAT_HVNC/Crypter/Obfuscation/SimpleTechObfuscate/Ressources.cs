using System;
using dnlib.DotNet;

namespace Client.Obfuscation.SimpleTechObfuscate;

public class Ressources
{
    public static void Obfuscate(ModuleDef md, String name, String encName)
    {
        if (name == "") return;
        foreach (var resouce in md.Resources)
        {
            String newName = resouce.Name.Replace(name, encName);
            if(resouce.Name != newName)Console.WriteLine($"{resouce.Name} -> {newName}");
            resouce.Name = newName;
        }
    }
}