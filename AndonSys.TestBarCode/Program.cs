using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using AndonSys.Common;

namespace AndonSys.TestBarCode
{
    class Program
    {
        static void Main(string[] args)
        {
            
            BarcodeScannerHelper helper = new BarcodeScannerHelper("COM1");

            helper.BaudRate = 9600;

            helper.Open();

            do
            {
                Console.Write("读取次数:");
                int cnt = int.Parse(Console.ReadLine());
                if (cnt == 0) break;

                int i = 0;

                while (i < cnt)
                {
                    string recv = helper.GetRecv(0);

                    if ((recv!=null) && (recv != ""))
                    {
                        Console.WriteLine(i.ToString() + ": " + recv);
                        recv = "";
                        i++;
                    }
                }

            } while (true);

            helper.Close();

            Console.Write("OK");
            Console.ReadLine();
        }

    }
}
