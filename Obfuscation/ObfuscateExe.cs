﻿using System.IO;
using dnlib.DotNet;
using MindLated.Protection.Anti;
using Obfuscation.Protection.Anti;
using Obfuscation.Protection.Arithmetic;
using Obfuscation.Protection.CtrlFlow;
using Obfuscation.Protection.INT;
using Obfuscation.Protection.LocalF;
using Obfuscation.Protection.Other;
using Obfuscation.Protection.Proxy;
using Obfuscation.Protection.String;
using Obfuscation.SimpleTechObfuscate;

namespace Obfuscation;

public class ObfuscateExe

{
    public static MethodDef Init;
    public static MethodDef Init2;

    public static void Run(string filePath)
    {
        ModuleDefMD md = ModuleDefMD.Load(filePath);
        
        if (File.Exists(filePath))
            File.Delete(filePath);

        
        AntiDe4dot.Execute(md.Assembly);
        AntiDebug.Execute(md);
        AntiDump.Execute(md);
        Arithmetic.Execute(md);

        ControlFlowObfuscation.Execute(md);
        AddIntPhase.Execute2(md);
        L2FV2.Execute(md);
        StackUnfConfusion.Execute(md);
        Watermark.Execute(md);
        ProxyInt.Execute(md);
        ProxyString.Execute(md);
        StringEncPhase.Execute(md);
        
        Str.Obfuscate(md);
        Methods.obfuscate_methods(md);
        Class.Obfuscate(md);
        NameSpace.Obfuscate(md);
        
        md.Write(filePath);
        md.Dispose();
    }
}