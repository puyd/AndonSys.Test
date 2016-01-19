using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using AndonSys.Common;
using LabLed.Components;
using System.Net.NetworkInformation;

namespace AndonSys.TestLed
{
    public partial class fmMain : Form
    {
        TLoopThread thread;

        Canvas cav;

        Canvas.TextBox txTime;

        Canvas.PicBox pic;

        Image img1;
        Image img2;


        //TestItem test;

        public fmMain()
        {
            InitializeComponent();
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            img1 = Image.FromFile(Application.StartupPath + "\\img\\1.bmp");
            img2 = Image.FromFile(Application.StartupPath + "\\img\\2.bmp");
            
            //TestSCL();

            //Ping p = new Ping();
         
            //PingReply r = p.Send("101.0.0.1");
            //if (r.Status == IPStatus.TimedOut)
            //{
            //    return;
            //}
            //p.Dispose();
            

            InitCanvas();

            //LedHelper.Init(2);
            //LedHelper.AddLed("192.168.16.98", 1);

            LedHelper.Init(1);
            
            //LedHelper.ReConnectTime = 5;
            //LedHelper.PingTime = 10;
            //LedHelper.LocPort = 8500;

            LedHelper.AddLed("192.168.16.99", 0,128,96);

            LedHelper.Open();

            thread = new TLoopThread(OnThread);
            thread.Start();
        }

        void TestSCL()
        {
            bool bl;

            bl = SclCommon.DLL_SCL_NetInitial(1, "", "192.168.16.93", 1, 1, 8500, true);

            bl = SclCommon.DLL_SCL_SendFile(1, 2, "", Application.StartupPath + "\\TEST.txt");
            bl = SclCommon.DLL_SCL_SendFile(1, 2, "", Application.StartupPath + "\\PLAYLIST.LY");
            bl = SclCommon.DLL_SCL_Replay(1, 2, 0);

            bl = SclCommon.DLL_SCL_Close(1);

        }


        private void fmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.WaitForStop();
            LedHelper.Close();

            img1.Dispose();
            img2.Dispose();
        }

        DateTime last = DateTime.Now;
        double delta = 0;

        void OnThread()
        {
            DateTime n = DateTime.Now;

            TimeSpan t = new TimeSpan(n.Ticks - last.Ticks);
            delta = t.TotalMilliseconds;
            last = n;

            txTime.Text = n.ToString("yyyy-MM-dd HH:mm:ss");
            
            cav.Refresh();
 
            LedHelper.AssignCanvas(0, cav);

            LedHelper.SendToScreen();
            Thread.Sleep(500);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //txTime.Text = DateTime.Now.ToString("HH:mm:ss") + "1\r\n&y&F" + delta.ToString("0.") + "&F&y1234567890&r哈哈哈哈";
            //test.Sign = 1;

            DoPrivew();
        }

        void InitCanvas()
        {
            cav = new Canvas(128, 64);

            Canvas.FillRect r1=new Canvas.FillRect(0, 0, 128, 64, Color.Green);
            cav.AddItem(0, r1 );

            Canvas.FillRect r2 = new Canvas.FillRect(10, 10, 80, 40, Color.Gray);
            r2.Flush = true;
            cav.AddItem(0, r2);

            pic = new Canvas.PicBox(32, 0, 64, 64);
            cav.AddItem(0, pic);

            Canvas.FillEllipse e1=new Canvas.FillEllipse(20, 10, 20, 20, Color.Yellow);
            cav.AddItem(0,e1 );

            Canvas.FillEllipse e2=new Canvas.FillEllipse(21, 11, 18, 18, Color.Red);
            e2.Flush = true;
            cav.AddItem(0,e2 );

            txTime = new Canvas.TextBox(10, 10, 100,40,"", "宋体", 9, Color.Black);
            txTime.Wrap = true;

            cav.AddItem(0, txTime);

            //test = new TestItem(50,40);
            //cav.AddItem(0, test);
        }

        void DoPrivew()
        {
            Graphics g = Graphics.FromHwnd(this.Handle);

            cav.Draw(g, 0, 0,2);

            g.Dispose();

        }

        class TestItem : Canvas.DrawItemClass
        {
            public int Sign = 0;
            int x, y;

            public TestItem(int x,int y)
            {
                InitRect(0, 0, 128, 64);
                this.x = x;
                this.y = y;
            }

            double flush = 0;

            public override void Draw(Graphics g, int OrgX, int OrgY, double delta)
            {
                flush = flush + delta;
                while (flush > 1000) flush = flush - 1000;
                
                if (flush < 500) return;
                
                Brush b;

                switch (Sign)
                {
                    case 0:b=new SolidBrush(Color.Yellow); break;
                    case 1:b=new SolidBrush(Color.Red); break;
                    default:b=new SolidBrush(Color.Black); break;
                }

                x = x + 1;
                if (x > 128) x = 0;

                g.FillEllipse(b, x, y, 20, 20);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pic.Image = img1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pic.Image = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pic.Image = img2;
        }

    }
}
