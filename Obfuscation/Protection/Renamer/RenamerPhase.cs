using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscation.Protection.Renamer;

internal class RenamerPhase
{
    public enum RenameMode
    {
        Ascii,
        Key,
        Normal
    }

    public const string Ascii = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private static readonly Random Random = new();

    public static readonly string[] NormalNameStrings =
    {
        "AbstractFactory", "Adapter", "Algorithm", "API", "Array", "BinarySearch", "BinaryTree", "BitManipulation",
        "Blockchain", "BubbleSort", "BuilderPattern", "Class", "CleanCode", "Closure", "CloudComputing", "Compiler",
        "Composition", "Concurrency", "Constructor", "ContinuousIntegration", "Cryptography", "DataStructure",
        "Database",
        "DecoratorPattern", "DependencyInjection", "DesignPattern", "DivideAndConquer", "Docker", "DynamicProgramming",
        "Encapsulation", "Encryption", "Enum", "EventDriven", "ExceptionHandling", "FactoryPattern",
        "FunctionalProgramming",
        "Generics", "Git", "Graph", "HashMap", "Heap", "Inheritance", "Interface", "IoT", "IteratorPattern", "JSON",
        "Lambda", "LinkedList", "MachineLearning", "Memoization", "Microservices", "Multithreading", "MVC", "Namespace",
        "OOP", "Overloading", "Overriding", "Polymorphism", "PrototypePattern", "Queue", "Recursion", "Refactoring",
        "RESTfulAPI", "SingletonPattern", "SOLIDPrinciples", "SortingAlgorithm", "Stack", "StrategyPattern",
        "StringManipulation",
        "TemplatePattern", "ThreadPool", "TreeTraversal", "UnitTesting", "UML", "VersionControl", "VirtualMachine",
        "WebSocket",
        "XML", "Yield", "ZeroMQ", "AbstractMethod", "BinarySearchTree", "BreadthFirstSearch", "CommandPattern",
        "DepthFirstSearch", "DynamicArray", "EventLoop", "GraphQL", "HashFunction", "HigherOrderFunction", "Immutable",
        "Kubernetes", "LinkedHashMap", "MergeSort", "Monolith", "NamespaceCollision", "ObserverPattern", "Prototype",
        "Quicksort", "RepositoryPattern", "ServiceOrientedArchitecture", "StatePattern", "Synchronous", "Tokenization",
        "Transaction", "TreeMap", "TwoFactorAuthentication", "UserAuthentication", "Vector", "WebFramework",
        "XMLHttpRequest",
        "YAML", "Zookeeper", "ActorModel", "Asynchronous", "Backtracking", "BloomFilter", "Buffer", "Callback", "CRC",
        "Decorator", "Dijkstra", "DSL", "EventEmitter", "FiniteStateMachine", "GarbageCollection", "GraphDatabase",
        "HeapSort",
        "Indexing", "Interpreter", "Jenkins", "KeyValueStore", "LazyEvaluation", "LRUCache", "MapReduce",
        "NamespacePolymorphism",
        "OrthogonalArray", "Promise", "Queueing", "RelationalDatabase", "Semaphore", "Spinlock", "Trie", "UUID",
        "WebAssembly",
        "XOR", "YAGNI", "ZeroDay"
    };


    private static readonly Dictionary<string, string> Names = new();

    public static string RandomString(int length, string chars)
    {
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    private static string GetRandomName()
    {
        return NormalNameStrings[Random.Next(NormalNameStrings.Length)];
    }

    public static string GenerateString(RenameMode mode)
    {
        return mode switch
        {
            RenameMode.Ascii => RandomString(Random.Next(3, 12), Ascii),
            RenameMode.Key => RandomString(16, Ascii),
            RenameMode.Normal => GetRandomName(),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }


    public static void ExecuteClassRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            if (type.Name == "GeneratedInternalTypeHelper" || type.Name == "Resources" || type.Name == "Settings")
            {
                continue;
            }


            if (Names.TryGetValue(type.Name, out string? nameValue))
            {
                type.Name = nameValue;
            }
            else
            {
                string newName = GenerateString(RenameMode.Ascii);

                Names.Add(type.Name, newName);
                type.Name = newName;
            }
        }

        ApplyChangesToResourcesClasses(module);
    }


    private static void ApplyChangesToResourcesClasses(ModuleDefMD module)
    {
        ModuleDefMD moduleToRename = module;
        foreach (Resource? resource in moduleToRename.Resources)
        {
            foreach (KeyValuePair<string, string> item in Names)
            {
                if (resource.Name.Contains(item.Key))
                {
                    resource.Name = resource.Name.Replace(item.Key, item.Value);
                }
            }
        }

        foreach (TypeDef? type in moduleToRename.GetTypes())
        {
            foreach (PropertyDef? property in type.Properties)
            {
                if (property.Name != "ResourceManager")
                {
                    continue;
                }

                IList<Instruction>? instr = property.GetMethod.Body.Instructions;

                for (int i = 0; i < instr.Count - 3; i++)
                {
                    if (instr[i].OpCode == OpCodes.Ldstr)
                    {
                        foreach (KeyValuePair<string, string> item in Names.Where(item =>
                                     item.Key == instr[i].Operand.ToString()))
                        {
                            instr[i].Operand = item.Value;
                        }
                    }
                }
            }
        }
    }


    public static void ExecuteFieldRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            foreach (FieldDef? field in type.Fields)
            {
                if (field.IsInitOnly)
                {
                    continue;
                }

                if (field.HasCustomAttributes) continue;
                if (Names.TryGetValue(field.Name, out string? nameValue))
                {
                    field.Name = nameValue;
                }
                else
                {
                    string newName = GenerateString(RenameMode.Ascii);

                    Names.Add(field.Name, newName);
                    field.Name = newName;
                }
            }
        }

        ApplyChangesToResourcesField(module);
    }

    private static void ApplyChangesToResourcesField(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (!type.IsGlobalModuleType)
            {
                foreach (MethodDef? methodDef in type.Methods)
                {
                    if (methodDef.Name != "InitializeComponent")
                    {
                        if (!methodDef.HasBody)
                        {
                            continue;
                        }

                        IList<Instruction> instructions = methodDef.Body.Instructions;
                        for (int i = 0; i < instructions.Count - 3; i++)
                        {
                            if (instructions[i].OpCode == OpCodes.Ldstr)
                            {
                                foreach (KeyValuePair<string, string> keyValuePair in Names)
                                {
                                    if (keyValuePair.Key == instructions[i].Operand.ToString())
                                    {
                                        instructions[i].Operand = keyValuePair.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static void ExecuteMethodRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }


            if (type.Name == "GeneratedInternalTypeHelper")
            {
                continue;
            }

            foreach (MethodDef? method in type.Methods)
            {
                if (!method.HasBody)
                {
                    continue;
                }

                if (method.IsVirtual || method.IsSpecialName)
                {
                    continue;
                }

                if (method.Name == ".ctor" || method.Name == ".cctor")
                {
                    continue;
                }

                method.Name = GenerateString(RenameMode.Ascii);
            }
        }
    }

    public static void ExecuteModuleRenaming(ModuleDefMD mod)
    {
        foreach (ModuleDef? module in mod.Assembly.Modules)
        {
            bool isWpf = false;
            foreach (AssemblyRef? asmRef in module.GetAssemblyRefs())
            {
                if (asmRef.Name == "WindowsBase" || asmRef.Name == "PresentationCore" ||
                    asmRef.Name == "PresentationFramework" || asmRef.Name == "System.Xaml")
                {
                    isWpf = true;
                }
            }

            if (!isWpf)
            {
                module.Name = GenerateString(RenameMode.Ascii);

                module.Assembly.CustomAttributes.Clear();
                module.Mvid = Guid.NewGuid();
                module.Assembly.Name = GenerateString(RenameMode.Ascii);
                module.Assembly.Version = new Version(Random.Next(1, 9), Random.Next(1, 9), Random.Next(1, 9),
                    Random.Next(1, 9));
            }
        }
    }

    public static void ExecuteNamespaceRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            if (type.Namespace == "")
            {
                continue;
            }

            if (Names.TryGetValue(type.Namespace, out string? nameValue))
            {
                type.Namespace = nameValue;
            }
            else
            {
                string newName = GenerateString(RenameMode.Ascii);

                Names.Add(type.Namespace, newName);
                type.Namespace = newName;
            }
        }

        ApplyChangesToResourcesNamespace(module);
    }

    private static void ApplyChangesToResourcesNamespace(ModuleDefMD module)
    {
        foreach (Resource? resource in module.Resources)
        {
            foreach (KeyValuePair<string, string> item in Names.Where(item => resource.Name.Contains(item.Key)))
            {
                resource.Name = resource.Name.Replace(item.Key, item.Value);
            }
        }

        foreach (TypeDef? type in module.GetTypes())
        {
            foreach (PropertyDef? property in type.Properties)
            {
                if (property.Name != "ResourceManager")
                {
                    continue;
                }


                IList<Instruction>? instr = property.GetMethod.Body.Instructions;

                for (int i = 0; i < instr.Count - 3; i++)
                {
                    if (instr[i].OpCode == OpCodes.Ldstr)
                    {
                        foreach (KeyValuePair<string, string> item in Names.Where(item =>
                                     item.Key == instr[i].Operand.ToString()))
                        {
                            instr[i].Operand = item.Value;
                        }
                    }
                }
            }
        }
    }

    public static void ExecutePropertiesRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            foreach (PropertyDef? property in type.Properties)
            {
                property.Name = GenerateString(RenameMode.Ascii);
            }
        }
    }
}