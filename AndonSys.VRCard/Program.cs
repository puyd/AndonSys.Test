using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using AndonSys.Common;

namespace AndonSys.VRCard
{
    class Program
    {
        static SerialPort com = null;

        class CardData
        {
            public Int16 a;
            public Int16 b;
            public DateTime last;
        }

        static Dictionary<byte, CardData> list = new Dictionary<byte, CardData>();

        static void Main(string[] args)
        {
            String port="COM9";

            if (args.Length>0) port=args[0];

            if (args.Length > 1) Process.Start(args[1]);

            try
            {
                com = new SerialPort(port, 57600, Parity.None, 8, StopBits.One);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey(false);
                return;
            }
            
            com.Open();
            while (true) 
            {
                byte[] buf = new byte[10];
                com.ReadTimeout = Timeout.Infinite;
                int n=com.Read(buf,0,10);
                //FUNC.Sleep(1000);
                //int n = 10;
                Write("recv: "+FUNC.BytesToString(buf,0,n));

                byte c = FUNC.CRC8(buf, 0, n - 1);
                if ((n == 10) && (c == buf[9]))
                {
                    WriteLine(" [OK]");
                    ExcuteCmd(buf[3], buf[4]);
                }
                else
                {
                    WriteLine(" [Failed]");
                }
                FUNC.Sleep(30);
            }
        }

        static void Write(String s)
        {
            TextWriter f = File.AppendText("VRCard.log");
            f.Write(s);
            f.Close();

            Console.Write(s);
        }

        static void WriteLine(String s)
        {
            TextWriter f = File.AppendText("VRCard.log");
            f.WriteLine(s);
            f.Close();

            Console.WriteLine(s);
        }

        static void ExcuteCmd(byte addr, byte cmd)
        {
            byte[] buf = new byte[15];
            
            buf[0] = Convert.ToByte('w');
            buf[1] = Convert.ToByte('a');
            buf[2] = Convert.ToByte('t');
            buf[3] = addr;

            for (int i = 4; i < 14; i++)
            {
                //buf[i] = (byte)FUNC.Random.Next(256);
                buf[i]=0;
            }

            

            if (!list.Keys.Contains(addr))
            {
                CardData d = new CardData();
                Random r=new Random();
                d.a = (Int16)r.Next(1000);
                d.b = (Int16)r.Next(2000);
                d.last = DateTime.Now;

                list.Add(addr, d);
            }


            TimeSpan t = new TimeSpan(DateTime.Now.Ticks - list[addr].last.Ticks); 
            
            if (t.TotalSeconds >= 3)
            {
                list[addr].a++;
                list[addr].b++; 
                list[addr].last = DateTime.Now;
            }


            buf[4] = addr;
            buf[7] = (byte)((cmd == 0x10)? 1 : 0);
            buf[10] = (byte)(list[addr].a / 256);
            buf[11] = (byte)(list[addr].a % 256);
            buf[12] = (byte)(list[addr].b / 256);
            buf[13] = (byte)(list[addr].b % 256);
            buf[14] = FUNC.CRC8(buf, 0, 14);

            //com.Write(buf, 3, 2);
            com.Write(buf, 0, 15);
            WriteLine("send: " + FUNC.BytesToString(buf, 0, 15));
            WriteLine("");
        }
    }
}
