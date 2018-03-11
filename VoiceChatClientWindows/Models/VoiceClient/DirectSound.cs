using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Microsoft.DirectX.DirectSound;

namespace VoiceChatClientWindows.Models.VoiceClient
{
    public static class DirectHelper
    {
        private static CaptureBufferDescription captureBufferDescription;
        private static AutoResetEvent autoResetEvent = null;
        private static Notify notify = null;
        private static WaveFormat waveFormat = new WaveFormat();
        private static Capture capture = null;
        private static Device device = null;
        private static CaptureBuffer captureBuffer;
        private static SecondaryBuffer playbackBuffer;
        private static BufferDescription playbackBufferDescription = null;
        private static int bufferSize = 0;
        private static byte[] byteData = new byte[2205];
        public static bool StopLoop = true;

        public static event EventHandler OnBufferFulfill;


        static DirectHelper()
        {
            SetVoiceDevices();
        }

        public static void SetVoiceDevices()
        {

            SetVoiceDevices(
            0,
            1,
            16,
            22050
            );    

        }
        public static void SetVoiceDevices(int deviceID, short channels, short bitsPerSample, int samplesPerSecond)
        {
            device = new Device();
            device.SetCooperativeLevel(new System.Windows.Forms.Control(), CooperativeLevel.Normal);
            CaptureDevicesCollection captureDeviceCollection = new CaptureDevicesCollection();
            DeviceInformation deviceInfo = captureDeviceCollection[deviceID];
            capture = new Capture(deviceInfo.DriverGuid);
            
            waveFormat = new WaveFormat();
            waveFormat.Channels = channels;
            waveFormat.FormatTag = WaveFormatTag.Pcm;
            waveFormat.SamplesPerSecond = samplesPerSecond;
            waveFormat.BitsPerSample = bitsPerSample;
            waveFormat.BlockAlign = (short)(channels * (bitsPerSample / (short)8)); 
            waveFormat.AverageBytesPerSecond = waveFormat.BlockAlign * samplesPerSecond;
            captureBufferDescription = new CaptureBufferDescription();
            captureBufferDescription.BufferBytes = waveFormat.AverageBytesPerSecond / 5;
            captureBufferDescription.Format = waveFormat;
            
            playbackBufferDescription = new BufferDescription();
            playbackBufferDescription.BufferBytes = waveFormat.AverageBytesPerSecond / 5;
            playbackBufferDescription.Format = waveFormat;
            playbackBufferDescription.GlobalFocus = true;
            playbackBuffer = new SecondaryBuffer(playbackBufferDescription, device);
            bufferSize = captureBufferDescription.BufferBytes;
        }

        public static void SetBufferEvents()
        {
            try
            {
                autoResetEvent = new AutoResetEvent(false);
                notify = new Notify(captureBuffer);
                
                BufferPositionNotify bufferPositionNotify1 = new BufferPositionNotify();
                bufferPositionNotify1.Offset = bufferSize / 2 - 1;
                bufferPositionNotify1.EventNotifyHandle = autoResetEvent.SafeWaitHandle.DangerousGetHandle();
                
                BufferPositionNotify bufferPositionNotify2 = new BufferPositionNotify();
                bufferPositionNotify2.Offset = bufferSize - 1;
                bufferPositionNotify2.EventNotifyHandle = autoResetEvent.SafeWaitHandle.DangerousGetHandle();

                notify.SetNotificationPositions(new BufferPositionNotify[] { bufferPositionNotify1, bufferPositionNotify2 });
            }
            catch { }
        }

        public static void StartCapturing()
        {
            try
            {
                captureBuffer = new CaptureBuffer(captureBufferDescription, capture);
                SetBufferEvents();
                int halfBuffer = bufferSize / 2;
                captureBuffer.Start(true);
                bool readFirstBufferPart = true;
                int offset = 0;
                MemoryStream memStream = new MemoryStream(halfBuffer);

                while (true)
                {
                    autoResetEvent.WaitOne();

                    memStream.Seek(0, SeekOrigin.Begin);
                    captureBuffer.Read(offset, memStream, halfBuffer, LockFlag.None);
                    readFirstBufferPart = !readFirstBufferPart;
                    offset = readFirstBufferPart ? 0 : halfBuffer; 

                    byte[] dataToWrite = ALawEncoder.ALawEncode(memStream.GetBuffer());

                    if (!StopLoop)
                        OnBufferFulfill(dataToWrite, null);
                }
            }
            catch (Exception e)
            {
                //
            }

        }
        public static void PlayReceivedVoice(byte[] byteData)
        {
            try
            {
                byte[] byteDecodedData = new byte[byteData.Length * 2];

                ALawDecoder.ALawDecode(byteData, out byteDecodedData);
                playbackBuffer = new SecondaryBuffer(playbackBufferDescription, device);
                playbackBuffer.Write(0, byteDecodedData, LockFlag.None); 
                playbackBuffer.Play(0, BufferPlayFlags.Default); 
            }
            catch { }
        }
    }
}
