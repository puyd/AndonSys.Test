using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AndonSys.Common;
using System.Diagnostics;

namespace AndonSys.TestModbus
{
    public partial class fmMain : Form
    {
        ModbusHelper md;
        TLoopThread th;
        String txt;

        delegate void OnInvoke();


        public fmMain()
        {
            InitializeComponent();
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            string[] p = new string[1];
            p[0] = "COM1";

            md= new ModbusHelper(p);
            md.Open();

            th = new TLoopThread(OnThread);
            th.Start();
        }

        void OnThread()
        {
            DateTime t = DateTime.Now;

            short[] d1,d2;
            lock (md)
            {
                try
                {
                    d1 = md.ReadData(0, 1, 0, 50);
                    d2 = md.ReadData(0, 1, 50, 50);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return;
                }
            }

           string s = "";

            for (int i = 0; i < d1.Length; i++)
            {
                s = s + string.Format("[{0:D3}] {1:D4} {1:X4}\r\n", i, d1[i]);
            }
            
            for (int i = 0; i < d2.Length; i++)
            {
                s = s + string.Format("[{0:D3}] {1:D4} {1:X4}\r\n", i+50, d2[i]);
            }

            if (s != txt)
            {
                txt = s;

                this.Invoke(new OnInvoke(SetText));
                Debug.WriteLine("Data Changed!");
            }

            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - t.Ticks);
            Debug.WriteLine(ts.TotalMilliseconds);

        }

        void SetText()
        {
            textBox1.Text = txt;
        }

        private void fmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            th.WaitForStop();
            md.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lock (md)
            {
                md.WriteData(0, 1, 9, 0);
                md.WriteData(0, 1, 5, 0);
                md.WriteData(0, 1, 0, 1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lock (md)
            {
                md.WriteData(0, 1, 19, 0);

                md.ClearData(0, 1, 20, 40);
                //md.ClearData(0, 1, 30, 10);
                //md.ClearData(0, 1, 40, 10);
                //md.ClearData(0, 1, 50, 10);

                md.WriteData(0, 1, 0, 2);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lock (md)
            {
                md.WriteData(0, 1, 2, 0);
                md.WriteData(0, 1, 0, 9);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            lock (md)
            {
                md.WriteData(0, 1, 2, 1);
                md.WriteData(0, 1, 0, 9);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            short[] d = new short[12];

            for (short i = 0; i < d.Length/2; i++)
            {
                d[i * 2] = i;
                //d[i * 2+1] = (short)(i % 4);
                d[i * 2 + 1] = (short)(new Random()).Next(4);
            }

            lock (md)
            {
                md.WriteData(0, 1, 60, d);
            }
        }




    }
}
