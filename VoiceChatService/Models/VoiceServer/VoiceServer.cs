using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using Antlr.Runtime;
using Microsoft.Ajax.Utilities;
using Exception = System.Exception;

namespace VoiceChatService.Models.VoiceServer
{
    public partial class VoiceServer : IDisposable
    {
        private Socket Listener = null;
        private VoiceClient NewVoiceClient = null;
        private List<VoiceClient> Clients = new List<VoiceClient>();
        public int Port { get; set; }

        public void Start(int _port)
        {
            Port = _port;
            try
            {
                IPAddress[] ClientAddresses = null;
                IPHostEntry ipEntry = null;
                string Host = "";

                try
                {
                    Host = Dns.GetHostName();
                    ipEntry = Dns.GetHostByName(Host);
                    ClientAddresses = ipEntry.AddressList;
                }
                catch (Exception e)
                {
                    //
                }

                if (ClientAddresses == null || ClientAddresses.Length == 0)
                {
                    throw new NullReferenceException("ClientAddresses array is null");
                }

                Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Listener.Bind(new IPEndPoint(ClientAddresses[0], _port));
                Listener.Listen(20);
                Listener.BeginAccept(new AsyncCallback(OnAccepted), Listener);
            }
            catch (ObjectDisposedException e)
            {
                return;
            }
        }

        private void OnAccepted(IAsyncResult iAsyncResult)
        {
            try
            {
                Listener = iAsyncResult.AsyncState as Socket;
                AcceptClient(Listener.EndAccept(iAsyncResult));
                Listener.BeginAccept(new AsyncCallback(OnAccepted), Listener);
            }
            catch (ObjectDisposedException e)
            {
                return;
            }
        }

        private void OnReceived(IAsyncResult iAsyncResult)
        {
            try
            {
                VoiceClient client = iAsyncResult.AsyncState as VoiceClient;
                byte[] sendBytes = client.GetData(iAsyncResult);

                if (sendBytes.Length == 0)
                {
                    client.ReadOnly.Close();
                    Clients.Remove(client);
                }
                else
                {
                    //foreach (VoiceClient clientSend in Clients)
                    //{
                    //    if (client != clientSend)
                    //    {
                    //        try
                    //        {
                    //            clientSend.ReadOnly.Send(sendBytes);
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            clientSend.ReadOnly.Close();
                    //            Clients.Remove(client);
                    //            return;
                    //        }
                    //    }
                    //}
                    //client.CallBackReceive();

                    // PARALLEL ForEAch experiment:

                    Parallel.ForEach(Clients, (clientToSend, state) =>
                    {
                        if (client != clientToSend)
                        {
                            try
                            {
                                clientToSend.ReadOnly.Send(sendBytes);
                            }
                            catch (Exception e)
                            {
                                clientToSend.ReadOnly.Close();
                                Clients.Remove(client);
                                state.Break();
                                return;
                            }
                        }
                    });
                    client.CallBackReceive();
                }
            }
            catch (ObjectDisposedException e)
            {
                return;
            }
        }

        private void AcceptClient(Socket clientSocket)
        {
            try
            {
                NewVoiceClient = new VoiceClient(clientSocket, this);
                Clients.Add(NewVoiceClient);
                NewVoiceClient.CallBackReceive();
            }
            catch (ObjectDisposedException e)
            {
                return;
            }
        }

        public void Dispose()
        {
            Listener.Close();
            Listener?.Dispose();
        }
    }
}