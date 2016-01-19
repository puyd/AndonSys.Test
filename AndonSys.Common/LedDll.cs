using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;

namespace AndonSys.Common
{
    public static class LedDll
    {
        [DllImport("andonled.dll", EntryPoint = "AndonLed_Open")]
        static extern UInt32 AndonLed_Open(Byte LedId, String IpAddress, String UdpPort, Byte LedAddr, int TimeOut);

        [DllImport("andonled.dll", EntryPoint = "AndonLed_Close")]
        static extern UInt32 AndonLed_Close(Byte LedId);

        [DllImport("andonled.dll", EntryPoint = "AndonLed_Send")]
        static extern UInt32 AndonLed_Send(Byte LedId, ref Byte Buffer, int BufferLen, int ColNum, int RowNum);

        [DllImport("andonled.dll", EntryPoint = "AndonLed_ErrorCount")]
        static extern UInt32 AndonLed_ErrorCount(Byte LedId);

        [DllImport("andonled.dll", EntryPoint = "AndonLed_LastTime")]
        static extern Int64 AndonLed_LastTime(Byte LedId);

        //TimeOut为发送或接收超时时间，以秒为单位，一般设为2.实际可以根据网络状态设置。
        public static bool LedOpen(Byte LedId, String IpAddress, String UdpPort, Byte LedAddr, int TimeOut)
        {
            return AndonLed_Open(LedId, IpAddress, UdpPort, LedAddr, TimeOut)==0;
        }

        public static bool LedClose(Byte LedId)
        {
            return AndonLed_Close(LedId) == 0;
        }

        public static bool LedSend(Byte LedId, ref Byte Buffer, int BufferLen, int ColNum, int RowNum)
        {
            return AndonLed_Send(LedId, ref Buffer, BufferLen, ColNum, RowNum)==0;
        }

        public static UInt32 ErrorCount(Byte LedId)
        {
            return AndonLed_ErrorCount(LedId);
        }

        public static DateTime LastTime(Byte LedId)
        {
            Int64 time_t = AndonLed_LastTime(LedId);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(time_t);

            return System.TimeZone.CurrentTimeZone.ToLocalTime(dt);
        }

        static byte[] BmpToBytes(Bitmap bmp)
        {
            BitmapData d = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int bytes = Math.Abs(d.Stride) * bmp.Height;
            byte[] ds = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(d.Scan0, ds, 0, bytes);
            
            bmp.UnlockBits(d);

            byte[] rs = new byte[bytes];
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    int nr = y * 3 + x * bmp.Height * 3;
                    int nd = x * 3 + y * d.Stride;

                    rs[nr] = ds[nd + 2];
                    rs[nr + 1] = ds[nd + 1];
                    rs[nr + 2] = ds[nd];

                }
            }

            return rs;
        }

        public static bool LedSend(Byte LedId, Bitmap bmp)
        {
            byte[] buf = BmpToBytes(bmp);

            return LedSend(LedId, ref buf[0], buf.Length, bmp.Width, bmp.Height);
        }

    }
}
