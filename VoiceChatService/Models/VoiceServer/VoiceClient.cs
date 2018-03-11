using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace VoiceChatService.Models.VoiceServer
{
    public partial class VoiceServer
    {
        sealed class VoiceClient
        {
            private VoiceServer voiceServer;
            private Socket ClientSocket = null;
            private byte[] bufferBytes = null;

            public VoiceClient(Socket NewSocket, VoiceServer _parentClass)
            {
                voiceServer = _parentClass;
                ClientSocket = NewSocket;
            }

            public Socket ReadOnly
            {
                get { return ClientSocket; }
            }

            public void CallBackReceive()
            {
                try
                {
                    bufferBytes = new byte[2205];
                    AsyncCallback receiveDataCallback = new AsyncCallback(voiceServer.OnReceived);
                    ClientSocket.BeginReceive(bufferBytes, 0, bufferBytes.Length, SocketFlags.None, receiveDataCallback, this);
                }
                catch (ObjectDisposedException e)
                {
                    return;
                }
            }

            public byte[] GetData(IAsyncResult iAsyncResult)
            {
                try
                {
                    int bytesRecorded = 0;

                    try
                    {
                        bytesRecorded = ClientSocket.EndReceive(iAsyncResult);
                    }
                    catch (Exception e)
                    {
                        //
                    }

                    byte[] bytesReturn = new byte[bytesRecorded];
                    Array.Copy(bufferBytes, bytesReturn, bytesRecorded);
                    return bytesReturn;
                }
                catch (ObjectDisposedException e)
                {
                    return null;
                }
            }
        }
    }
}