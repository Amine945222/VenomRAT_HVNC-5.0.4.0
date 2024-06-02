using System;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscation.SimpleTechObfuscate;

public class Str
{
    public static void Obfuscate(ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            foreach (MethodDef method in type.Methods)
            {
                if (!method.HasBody) continue;
                for (int i = 0; i < method.Body.Instructions.Count(); i++)
                {
                    if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                    {
                        String regString = method.Body.Instructions[i].Operand.ToString();
                        String encString = Convert.ToBase64String(Encoding.UTF8.GetBytes(regString));
                        Console.WriteLine($"{regString} -> {encString}");
                        method.Body.Instructions[i].OpCode =
                            OpCodes.Nop;
                        method.Body.Instructions.Insert(i + 1,
                            new Instruction(OpCodes.Call,
                                md.Import(typeof(Encoding).GetMethod("get_UTF8",
                                    new Type[] { }))));
                        method.Body.Instructions.Insert(i + 2,
                            new Instruction(OpCodes.Ldstr, encString)); // Load string onto stack
                        method.Body.Instructions.Insert(i + 3,
                            new Instruction(OpCodes.Call,
                                md.Import(typeof(Convert).GetMethod("FromBase64String",
                                    new[]
                                    {
                                        typeof(string)
                                    }))));
                        method.Body.Instructions.Insert(i + 4,
                            new Instruction(OpCodes.Callvirt,
                                md.Import(typeof(Encoding).GetMethod("GetString",
                                    new[]
                                    {
                                        typeof(byte[])
                                    }))));
                        i += 4;
                    }
                }
            }
        }
    }
}