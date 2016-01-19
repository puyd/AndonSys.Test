using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;
using AndonSys.Common;

namespace Andonsys.TestRFID
{
    public partial class fmMain : Form
    {
        public fmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs ev)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            s.Connect("192.168.16.170", 10000);

            byte[] buf;

            buf = new byte[] { 0x00,0x06,0x10,0x12,0x00,0x00 };
            
            s.Send(buf);

            byte[] rec=new byte[128];
            int r = 0;

            s.ReceiveTimeout = 1000;

            try
            {
                r = s.Receive(rec);
                Debug.WriteLine(FUNC.BytesToString(rec, 0, r));

                r = s.Receive(rec);
                Debug.WriteLine(FUNC.BytesToString(rec, 0, r));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            s.Disconnect(false);
        }
    }
}
