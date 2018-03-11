using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Windows;
using Microsoft.DirectX.DirectSound;
using System.Net;
using System.Net.Sockets;

namespace VoiceChatClientWindows.Models.VoiceClient
{
    public class VoiceClient
    {
        private Socket socket;
        private byte[] bufferBytes = new byte[2205];
        private Thread thread;

        public VoiceClient()
        {
            DirectHelper.OnBufferFulfill += new EventHandler(SendVoiceBuffer);
        }

        public void Connect(string ServerIP, int Port)
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    Thread.Sleep(20);
                    socket.Close();
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ServerIP), Port);
                socket.Blocking = false;

                socket.BeginConnect(endPoint, new AsyncCallback(OnConnect), socket);

                thread = new Thread(new ThreadStart(DirectHelper.StartCapturing));
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception) { }
        }

        public void OnConnect(IAsyncResult iAsyncResult)
        {
            Socket sock = (Socket)iAsyncResult.AsyncState;

            try
            {
                if (sock.Connected)
                {
                    SetupRecieveCallback(sock);
                }
                else
                {
                    Disconncet();
                }
            }
            catch (Exception e)
            {
                //
            }
        }

        public void SendBuffer(byte[] buffer)
        {
            socket.Send(buffer, SocketFlags.None);
        }

        public void OnRecievedData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;

            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    DirectHelper.PlayReceivedVoice(bufferBytes);

                    SetupRecieveCallback(sock);
                }
                else
                {
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }
            catch (Exception) { }
        }

        public void SetupRecieveCallback(Socket sock)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                sock.BeginReceive(bufferBytes, 0, bufferBytes.Length, SocketFlags.None, recieveData, sock);
            }
            catch (Exception) { }
        }

        public void SendVoiceBuffer(object VoiceBuffer, EventArgs e)
        {
            byte[] Buffer = (byte[])VoiceBuffer;

            SendBuffer(Buffer);

        }

        public void Disconncet()
        {
            try
            {
                if (socket != null)
                {
                    socket.Close();
                }
            }
            catch (Exception) { }
        }
    }
}
