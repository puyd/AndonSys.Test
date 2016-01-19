using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using LabLed.Components;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;

using System.Net.NetworkInformation;

using System.IO;

namespace AndonSys.Common
{
    /// <summary>
    /// LED屏的控制类，可以控制一组屏显示相同的内容
    /// </summary>
    public class LEDManager
    {
        /// <summary>
        /// LED类
        /// </summary>
        public class LED
        {
            /// <summary>
            /// 屏幕连接标志
            /// </summary>
            public bool IsConnected=false;
            
            /// <summary>
            /// 颜色常量
            /// </summary>
            public static class Color
            {
                /// <summary>
                /// Red
                /// </summary>
                public const int Red = 0x00FF;
                /// <summary>
                /// Green
                /// </summary>
                public const int Green = 0xFF00;
                /// <summary>
                /// Yellow
                /// </summary>
                public const int Yellow = 0xFFFF;
            }

            /// <summary>
            /// LED屏地址
            /// </summary>
            public byte Addr;

            /// <summary>
            /// LED屏IP
            /// </summary>
            public string IP;

            /// <summary>
            /// LED屏端口号
            /// </summary>
            public ushort Port;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="addr">屏地址</param>
            /// <param name="ip">IP地址</param>
            /// <param name="port">端口号</param>
            public LED(byte addr, string ip, ushort port)
            {
                Addr = addr;
                IP = ip;
                Port = port;

                Ping(50);
            }

            /// <summary>
            /// ping大屏地址
            /// </summary>
            /// <param name="timeout">超时毫秒数</param>
            /// <returns>ping通返回true，否则返回false</returns>
            public bool Ping(int timeout)
            {
                if (IP == "0.0.0.0") return false; 
                
                Ping p = new Ping();

                PingReply r = p.Send(IP,timeout);

                p.Dispose();

                IsConnected = (r.Status == IPStatus.Success);

                return (r.Status == IPStatus.Success);
            }

        }
        
        const int WM_LED_NOTIFY = 1025; 
        
        int formHandle = 0;
        uint locPort=0;
        int dev=0;
        LedCommon LEDSender = new LedCommon();

        int Width,Height;

        /// <summary>
        /// 重新连接屏幕的时间间隔
        /// </summary>
        public int ReConnectTime = 60;

        /// <summary>
        /// ping屏幕地址的时间间隔
        /// </summary>
        public int PingTime = 10;

        List<LED> list = new List<LED>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fmMain">主窗体句柄</param>
        /// <param name="locPort">本地TCP端口号</param>
        /// <param name="Width">屏宽度</param>
        /// <param name="Height">屏高度</param>
        public LEDManager(Form fmMain,uint locPort,int Width,int Height)
        {
            if (fmMain == null)
            {
                formHandle = 0;
            }
            else
            {
                fmMain.Handle.ToInt32();
            }
            this.locPort = locPort;

            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~LEDManager()
        {
            LEDSender.Dispose();
        }

        /// <summary>
        /// 定义一块LED屏
        /// </summary>
        /// <param name="addr">屏地址</param>
        /// <param name="ip">IP</param>
        /// <param name="port">屏端口</param>
        public void AddLED(byte addr, string ip, ushort port)
        {
            list.Add(new LED(addr,ip,port));
        }

        /// <summary>
        /// 打开LED控制
        /// </summary>
        public void Open()
        {
            LedCommon.DEVICEPARAM param = new LedCommon.DEVICEPARAM();

            param.devType = (uint)LedCommon.eDevType.DEV_UDP;
            param.locPort = locPort;
            param.rmtPort = (uint)6666;

            dev = LEDSender.LED_Open(ref param, 0, formHandle, WM_LED_NOTIFY + 2);
        }

        /// <summary>
        /// 关闭LED控制
        /// </summary>
        public void Close()
        {
            LEDSender.LED_Close(dev);
        }

        /// <summary>
        /// 创建一个新屏幕
        /// </summary>
        /// <param name="DisplayTime">持续时间</param>
        public void NewScreen(int DisplayTime)
        {
            LEDSender.MakeRoot(LedCommon.eRootType.ROOT_PLAY, LedCommon.eScreenType.SCREEN_COLOR);
            LEDSender.AddLeaf(DisplayTime*1000);
        }

        /// <summary>
        /// 创建一个新页面
        /// </summary>
        /// <param name="DisplayTime">持续时间</param>
        public void NewPage(int DisplayTime)
        {
            LEDSender.AddLeaf(DisplayTime*1000);
        }

        DateTime lastPing = DateTime.Now;
        DateTime lastOpen = DateTime.Now;

        void TestLink()
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - lastPing.Ticks);

            if (ts.TotalSeconds > PingTime)
            {
                foreach (LED led in list)
                {
                    if ((led.IP == "") || (led.IP == "0.0.0.0")) continue;
                    led.Ping(50);
                }
                lastPing = DateTime.Now;
            }

            ts = new TimeSpan(DateTime.Now.Ticks - lastOpen.Ticks);

            if (ts.TotalSeconds > ReConnectTime)
            {
                Close();
                Open();
                lastOpen = DateTime.Now;
            }

        }
        
        /// <summary>
        /// 发送当前显示内容
        /// </summary>
        public void ShowScreen()
        {
            foreach (LED led in list)
            {
                if ((led.IP == "") || (led.IP == "0.0.0.0")||(!led.IsConnected)) continue;
                LEDSender.LED_SendToScreen(dev, led.Addr, led.IP, led.Port);
            }

            TestLink();
        }

        /// <summary>
        /// 在当前页面添加文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="rect">显示范围</param>
        /// <param name="method">显示方式</param>
        /// <param name="speed">显示速度</param>
        /// <param name="font">字体</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        public void AddText(String text, LedCommon.RECT rect, int method, int speed, string font, int size, int color)
        {
            LEDSender.AddText(text, ref rect, method, speed, 1, font, size, color, 1);
        }

        /// <summary>
        /// 在当前页面添加文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="X">X坐标</param>
        /// <param name="Y">Y坐标</param>
        /// <param name="method">显示方式</param>
        /// <param name="speed">显示速度</param>
        /// <param name="font">字体</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        public void AddText(String text, int X, int Y, int method,int speed,string font,int size,int color)
        {
            LedCommon.RECT r;
            r.left = X;
            r.top = Y;
            r.right = Width;
            r.bottom = Height;
            
            AddText(text, r, method, speed, font, size, color);
        }

        /// <summary>
        /// 在当前页面添加文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="X">X坐标</param>
        /// <param name="Y">Y坐标</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        public void AddText(String text, int X, int Y, int size, int color)
        {
            AddText(text, X, Y, 1, 8, "宋体", size, color);
        }

        /// <summary>
        /// 在当前页面添加来自HDC句柄的图形数据
        /// </summary>
        /// <param name="Handle">HDC句柄</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void AddHDC(IntPtr Handle, int x, int y, int width, int height)
        {
            LedCommon.RECT r;
            r.left = x;
            r.top = y;
            r.right = width;
            r.bottom = height;

            int i = Handle.ToInt32();

            LEDSender.AddWindow(i, (short)width, (short)height, ref r, 1, 8, 1);
        }

        /// <summary>
        /// 在当前页面添加图形文件
        /// </summary>
        /// <param name="pic">图形文件名</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void AddPicture(string pic,int x,int y,int width,int height)
        {
            LedCommon.RECT r;
            r.left = x;
            r.top = y;
            r.right = width;
            r.bottom = height;

            LEDSender.AddPicture(pic, ref r, 1, 8, 1, 1);
        }

    }

    /// <summary>
    /// LED辅助类
    /// </summary>
    public static class LedHelper
    {
        class Led
        {
            public string IP;
            public byte Addr;

            public Graphics g;
            
            public IntPtr mdc;
            public IntPtr hbmp;

            public int dev;
            public Size Size;

            public int state = 0;

            public Bitmap bmp = null;

            /// <summary>
            /// 屏幕连接标志
            /// </summary>
            public bool IsConnected = false;

            public Led(string ip,byte addr,int width,int height)
            {
                IP = ip;
                Addr = addr;
                Size = new Size(width, height);

                switch (CardType)
                {
                    case 1:
                        IntPtr dc = Win32.CreateDC("DISPLAY", null, null, (IntPtr)null);
                        mdc = Win32.CreateCompatibleDC(dc);
                        hbmp = Win32.CreateCompatibleBitmap(dc, width, height);
                        Win32.SelectObject(mdc, hbmp);
                        Win32.DeleteDC(dc);

                        g = Graphics.FromHdc(mdc);
                        break;

                    case 3:
                        bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(bmp);
                        break;
                }

                Ping(50);
            }

            ~Led()
            {
                g.Dispose();
                switch (CardType)
                {
                    case 1:
                        Win32.DeleteDC(mdc);
                        Win32.DeleteObject(hbmp);
                        break;
                    case 3:
                        bmp.Dispose();
                        break;
                }
            }

            /// <summary>
            /// ping大屏地址
            /// </summary>
            /// <param name="timeout">超时毫秒数</param>
            /// <returns>ping通返回true，否则返回false</returns>
            public bool Ping(int timeout)
            {
                if (IP == "0.0.0.0") return false;

                long s = DateTime.Now.Ticks;

                Ping ping = new Ping(); 
                
                PingReply r = ping.Send(IP, timeout);

                ping.Dispose();

                IsConnected = (r.Status == IPStatus.Success);

                TimeSpan t = new TimeSpan(DateTime.Now.Ticks - s);

                //Debug.WriteLine(t.TotalSeconds);

                return (r.Status == IPStatus.Success);
            }
        }

        static List<Led> list=new List<Led>();

        static int CardType = 1;    //1 视展卡；2 励研卡；3 视展卡直连

        //static TLoopThread thread = new TLoopThread(OnThread);

        /// <summary>
        /// 重新连接屏幕的时间间隔
        /// </summary>
        public static int ReConnectTime = 60;

        /// <summary>
        /// ping屏幕地址的时间间隔
        /// </summary>
        public static int PingTime = 10;

        /// <summary>
        /// 本地TCP端口号
        /// </summary>
        public static uint LocPort = 9500; 
        
        static public void Init(int type)
        {
            CardType = type;
            list.Clear();
        }

        static public int AddLed(string IP, byte Addr, int width, int height)
        {
            Led l = new Led(IP, Addr,width,height);
            list.Add(l);
            return list.Count - 1;
        }
        
        static void Open1(int id)
        {
            Led led = list[id];

            if ((led.IP == "") || (led.IP == "0.0.0.0")||(led.state==1)) return;

            try
            {
                LedCommon.DEVICEPARAM param = new LedCommon.DEVICEPARAM();

                param.devType = (uint)LedCommon.eDevType.DEV_UDP;
                param.locPort = (uint)(LocPort + id);
                param.rmtPort = (uint)6666;

                led.dev = LedCommon.DLL_LED_Open(ref param, LedCommon.NOTIFY_EVENT, 0, 0);
                led.state = 1;
            }
            catch (Exception e)
            {
                throw new Exception("LedOpen(" + id.ToString() + ") Error:" + e.Message);
            }

        }

        static void Open3(int id)
        {
            Led led = list[id];

            if ((led.IP == "") || (led.IP == "0.0.0.0") || (led.state == 1)) return;

            try
            {
                if (LedDll.LedOpen((byte)(id+1), led.IP, "6666", led.Addr, 5000))
                {
                    led.state = 1;
                }
            }
            catch (Exception e)
            {
                throw new Exception("LedOpen(" + id.ToString() + ") Error:" + e.Message);
            }

        }
        
        static void Open1()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Open1(i);
            }
        }

        static void Open3()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Open3(i);
            }
        }

        static public void Open()
        {
            switch (CardType)
            {
                case 1: Open1(); return;
                case 3: Open3(); return;
            }
        }

        static public void LedEnable(int id)
        {
            list[id].state = 11;
        }

        static public void LedDisable(int id)
        {
            list[id].state = 10;
        }

        static void SendToScreen1()
        {
            for (int i=0;i<list.Count;i++)
            {
                Led led=list[i];

                if ((led.IP == "") || (led.IP == "0.0.0.0") || (!led.IsConnected)||(led.state==0)) continue;

                switch (led.state)
                {
                    case 10: 
                        Close1(i); 
                        continue;  
                    case 11: 
                        Open1(i);  
                        break;
                }

                LedCommon.DLL_MakeRoot((int)LedCommon.eRootType.ROOT_PLAY, (int)LedCommon.eScreenType.SCREEN_COLOR);
                LedCommon.DLL_AddLeaf(1000);

                LedCommon.RECT r;
                r.left = 0;
                r.top = 0;
                r.right = 256;
                r.bottom = 256;

                int dc = led.mdc.ToInt32();

                LedCommon.DLL_AddWindow(dc, 256, 256, ref r, 1, 8, 1);

                try
                {
                    LedCommon.DLL_LED_SendToScreen(led.dev, led.Addr, led.IP, (ushort)6666);
                }
                catch (Exception e)
                {
                    FUNC.Sleep(100); 
                    Close1(i); 
                    
                    FUNC.Sleep(100);
                    Open1(i);

                    throw new Exception("SendToScreen(" + i.ToString() + ") Error:" + e.Message);
                }

                FUNC.Sleep(20);
            }

            
        }


        static void SendToScreen3()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Led led = list[i];

                if ((led.IP == "") || (led.IP == "0.0.0.0") || (!led.IsConnected) || (led.state == 0)) continue;

                switch (led.state)
                {
                    case 10:
                        Close3(i);
                        continue;
                    case 11:
                        Open3(i);
                        break;
                }


                try
                {
                    LedDll.LedSend((byte)(i+1), led.bmp);
                }

                catch (Exception e)
                {
                    FUNC.Sleep(100);
                    Close3(i);

                    FUNC.Sleep(100);
                    Open3(i);

                    throw new Exception("SendToScreen(" + i.ToString() + ") Error:" + e.Message);
                }

                FUNC.Sleep(20);
            }


        }

        static DateTime lastPing = DateTime.Now;
        static DateTime lastOpen = DateTime.Now;

        static void Reconnect()
        {
            Close();
            Open();
        }

        static void TestLink()
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - lastPing.Ticks);

            if (ts.TotalSeconds > PingTime)
            {
                foreach (Led led in list)
                {
                    if ((led.IP == "") || (led.IP == "0.0.0.0")) continue;
                    if (!led.Ping(50))
                    {
                        Debug.WriteLine(string.Format("Ping {0} {1}.", led.IP, led.IsConnected));
                    }
                }
                lastPing = DateTime.Now;
            }

            ts = new TimeSpan(DateTime.Now.Ticks - lastOpen.Ticks);

            if (ts.TotalSeconds > ReConnectTime)
            {
                Debug.WriteLine("ReConnect");

                Reconnect();

                lastOpen = DateTime.Now;
            }

        }

        static public void  SendToScreen()
        {
            switch (CardType)
            {
                case 1: 
                    SendToScreen1(); 
                    TestLink(); 
                    break;
                case 3: 
                    SendToScreen3(); break;
            }
        }

        static public void Close()
        {
            switch (CardType)
            {
                case 1: Close1(); return;
                case 3: Close3(); return;
            }

        }

        static void Close1(int id )
        {
            if (list[id].state == 0) return;

            try
            {
                LedCommon.DLL_LED_Close(list[id].dev);
                list[id].state = 0;
            }
            catch (Exception e)
            {
                throw new Exception("LedClose(" + id.ToString() + ") Error:" + e.Message);
            }
            
                        
        }

        static void Close3(int id)
        {
            if (list[id].state == 0) return;

            try
            {
                //LedDll.LedClose((byte)id);
                list[id].state = 0;
            }
            catch (Exception e)
            {
                throw new Exception("LedClose(" + id.ToString() + ") Error:" + e.Message);
            }


        }
        
        static void Close1()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Close1(i);
            }
        }

        static void Close3()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Close3(i);
            }
        }
        
        static public Graphics GetGraphics(int Index)
        {
            return list[Index].g;
        }

        static public void AssignCanvas(int Index,Canvas cav)
        {
            lock (cav)
            {
                switch (CardType)
                {
                    case 1:
                    case 3: 
                        Graphics g = GetGraphics(Index);
                        cav.Draw(g, 0, 0,1);
                        break;
                    

                }
            }
        }
    }
}
