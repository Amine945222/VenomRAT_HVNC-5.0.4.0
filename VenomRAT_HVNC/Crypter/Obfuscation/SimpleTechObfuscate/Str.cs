using System;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Client.Obfuscation.SimpleTechObfuscate;

public class Str
{
    public static void Obfuscate(ModuleDef md)
    {
        //foreach (var type in md.Types) // only gets parent(non-nested) classes

        // types(namespace.class) in module
        foreach (var type in md.GetTypes())
        {
            // methods in type
            foreach (MethodDef method in type.Methods)
            {
                // empty method check
                if (!method.HasBody) continue;
                // iterate over instructions of method
                for (int i = 0; i < method.Body.Instructions.Count(); i++)
                {
                    // check for LoadString opcode
                    // CIL is Stackbased (data is pushed on stack rather than register)
                    // ref: https://en.wikipedia.org/wiki/Common_Intermediate_Language
                    // ld = load (push onto stack), st = store (store into variable)
                    if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                    {
                        // c# variable has for loop scope only
                        String regString = method.Body.Instructions[i].Operand.ToString();
                        String encString = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(regString));
                        Console.WriteLine($"{regString} -> {encString}");
                        // methodology for adding code: write it in plain c#, compile, then view IL in dnspy
                        method.Body.Instructions[i].OpCode =
                            OpCodes.Nop; // errors occur if instruction not replaced with Nop
                        method.Body.Instructions.Insert(i + 1,
                            new Instruction(OpCodes.Call,
                                md.Import(typeof(System.Text.Encoding).GetMethod("get_UTF8",
                                    new Type[] { })))); // Load string onto stack
                        method.Body.Instructions.Insert(i + 2,
                            new Instruction(OpCodes.Ldstr, encString)); // Load string onto stack
                        method.Body.Instructions.Insert(i + 3,
                            new Instruction(OpCodes.Call,
                                md.Import(typeof(System.Convert).GetMethod("FromBase64String",
                                    new Type[]
                                    {
                                        typeof(string)
                                    })))); // call method FromBase64String with string parameter loaded from stack, returned value will be loaded onto stack
                        method.Body.Instructions.Insert(i + 4,
                            new Instruction(OpCodes.Callvirt,
                                md.Import(typeof(System.Text.Encoding).GetMethod("GetString",
                                    new Type[]
                                    {
                                        typeof(byte[])
                                    })))); // call method GetString with bytes parameter loaded from stack 
                        i += 4; //skip the Instructions as to not recurse on them
                    }
                }
                //method.Body.KeepOldMaxStack = true;
            }
        }
    }
}