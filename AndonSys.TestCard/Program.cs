using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AndonSys.Common;

namespace AndonSys.TestCard
{
    class Program
    {
        static void Main(string[] args)
        {
            CONFIG.Load();

            CARD.Open(); 
            
            int[] d = new int[5];

            d[0] = FUNC.ToInt32(0x27, 0x26, 0x13, 0x1A);
            d[1] = FUNC.ToInt32(0x04, 0x04, 0x0C, 0x00);
            d[2] = 0;
            d[3] = 0;
            d[4] = 0;

            CARD.SendCMD(1, 1, 1, 0x10, d, 100);

            Console.WriteLine("选择工作模式：1 有线 2 无线");
            int mode=-1;
            int.TryParse(Console.ReadLine(),out mode);
            switch (mode)
            {
                case 1: Test1(); break;
                case 2: Test2(); break;
            }

            CARD.Close();
        }

        private static void Test1()
        {
            do
            {

                try
                {
                    Console.WriteLine("输入：串口ID 地址 命令 [数据 重复次数]");
                    string[] s = Console.ReadLine().Split(',', ' ');

                    if (s[0] == "") break;

                    int id = Convert.ToByte(s[0]);
                    byte addr = Convert.ToByte(s[1], 16);
                    byte cmd = Convert.ToByte(s[2], 16);
                    int data = (s.Length > 3) ? Convert.ToInt32(s[3], 16) : 0;
                    int cnt = (s.Length > 4) ? Convert.ToInt32(s[4]) : 1;

                    TCardRecv recv = new TCardRecv();


                    for (int i = 0; i < cnt; i++)
                    {
                        DateTime d1 = DateTime.Now;

                        if (CARD.Excute(id, addr, cmd, data, recv, 100))
                        {
                            TimeSpan t = new TimeSpan(DateTime.Now.Ticks - d1.Ticks);

                            Console.Write(t.TotalMilliseconds.ToString("0ms:").PadRight(8, ' '));
                            Console.Write(recv.ToString());

                            string js = System.Text.Encoding.Default.GetString(recv.Data);

                            Console.WriteLine("     " + js);
                        }
                    }
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (true);
        }

        private static void Test2()
        {
            do
            {

                try
                {
                    Console.WriteLine("输入：串口ID 地址 命令 [数据 接收次数]");
                    string[] s = Console.ReadLine().Split(',', ' ');

                    if (s[0] == "") break;

                    int id = Convert.ToByte(s[0]);
                    byte addr = Convert.ToByte(s[1], 16);
                    byte cmd = Convert.ToByte(s[2], 16);
                    int data = (s.Length > 3) ? Convert.ToInt32(s[3], 16) : 0;
                    int cnt = (s.Length > 4) ? Convert.ToInt32(s[4]) : 1;

                    TCardRecv recv = new TCardRecv();

                    CARD.SendCMD(id, addr, cmd, data, 100);

                    for (int i=0;i<cnt;i++)
                    {
                        DateTime d1 = DateTime.Now; 
                        
                        bool bl=CARD.ReadData(id, recv, 200);

                        TimeSpan t = new TimeSpan(DateTime.Now.Ticks - d1.Ticks);

                        Console.Write(t.TotalMilliseconds.ToString("0ms:").PadRight(8, ' '));

                        if (bl)
                        {
                            Console.Write(recv.ToString());

                            string js = System.Text.Encoding.Default.GetString(recv.Data);

                            Console.WriteLine("     " + js);
                        }
                        else
                        {
                            Console.WriteLine("读取超时");
                        }
                    }
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (true);
        }
    }
}
