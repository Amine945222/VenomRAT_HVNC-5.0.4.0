using System.IO;
using dnlib.DotNet;
using MindLated.Protection.Anti;
using MindLated.Protection.Arithmetic;
using MindLated.Protection.CtrlFlow;
using MindLated.Protection.INT;
using MindLated.Protection.InvalidMD;
using MindLated.Protection.LocalF;
using MindLated.Protection.Other;
using MindLated.Protection.Proxy;
using MindLated.Protection.Renamer;
using MindLated.Protection.String;
using MindLated.Protection.StringOnline;

namespace Client.Obfuscation;

public class Obfuscate

{
    public static MethodDef? Init;
    public static MethodDef? Init2;
    public static void Run(string filePath)
    {
        ModuleDefMD md = ModuleDefMD.Load(filePath);
        
        Arithmetic.Execute(md);
        ControlFlowObfuscation.Execute(md);
        AddIntPhase.Execute2(md);
        L2FV2.Execute(md);
        StackUnfConfusion.Execute(md);
        Watermark.Execute(md);
        ProxyInt.Execute(md);
        ProxyString.Execute(md);
            
        RenamerPhase.ExecuteNamespaceRenaming(md);
        RenamerPhase.ExecuteClassRenaming(md);
        RenamerPhase.ExecuteMethodRenaming(md);
        RenamerPhase.ExecuteFieldRenaming(md);
        RenamerPhase.ExecuteModuleRenaming(md);
        RenamerPhase.ExecutePropertiesRenaming(md);
        StringEncPhase.Execute(md);
            
            
        if (File.Exists(filePath))
            File.Delete(filePath);
        md.Write(filePath);
    }
}