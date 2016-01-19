using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using System.IO;

using AndonSys.Common;

namespace AndonSys.LedShow
{
    public partial class fmMain : Form
    {
        public fmMain()
        {
            InitializeComponent();
        }

        Image img1,img2;

        private void fmMain_Load(object sender, EventArgs e)
        {
            //XmlDocument xml= NewXml();
            //ShowXml(xml);

            StreamReader sr = new StreamReader("LedShow.xml",Encoding.GetEncoding("GB2312"));
            edXml.Text = sr.ReadToEnd();
            sr.Close();

            img1=Image.FromFile(".\\img\\1.bmp");
            img2 = Image.FromFile(".\\img\\2.bmp");
        }

        XmlDocument NewXml()
        {
            XmlDocument xdoc = new XmlDocument();

            XmlDeclaration xdec = xdoc.CreateXmlDeclaration("1.0", "gb2312", null);

            xdoc.AppendChild(xdec);

            XmlElement root=(XmlElement)xdoc.AppendChild(xdoc.CreateElement("Display"));
            root.SetAttribute("Size", " 256,192");
            root.SetAttribute("Location", "0,0");

            XmlElement page1 = (XmlElement)root.AppendChild(xdoc.CreateElement("Page"));
            page1.SetAttribute("ID", "1");
            page1.SetAttribute("Delay", "5");
           
            XmlElement n = null;

            n = (XmlElement)page1.AppendChild(xdoc.CreateElement("FillRect"));
            n.SetAttribute("ID", "");
            n.SetAttribute("Location", "0,0");
            n.SetAttribute("Size", "96,24");
            n.SetAttribute("Color", "0x000000");
            n.SetAttribute("Flush", "0");

            n = (XmlElement)page1.AppendChild(xdoc.CreateElement("FillEllipse"));
            n.SetAttribute("ID", "信号1");
            n.SetAttribute("Location", "96,0");
            n.SetAttribute("Size", "24,24");
            n.SetAttribute("Color", "0xFF0000");
            n.SetAttribute("Flush", "1");

            n = (XmlElement)page1.AppendChild(xdoc.CreateElement("Ellipse"));
            n.SetAttribute("ID", "");
            n.SetAttribute("Location", "96,0");
            n.SetAttribute("Size", "24,24");
            n.SetAttribute("Color", "0xFFFF00");
            n.SetAttribute("Flush", "0");
            n.SetAttribute("Thick", "1");

            n = (XmlElement)page1.AppendChild(xdoc.CreateElement("TextBox"));
            n.SetAttribute("ID", "文本1");
            n.SetAttribute("Location", "0,0");
            n.SetAttribute("Size", "96,24");
            n.SetAttribute("Font", "宋体,16");
            n.SetAttribute("Color", "0x00FF00");
            n.SetAttribute("Scroll", "1,48");
            n.SetAttribute("Flush", "0");
            n.InnerText = "测试文本123456";

            XmlElement page2 = (XmlElement)root.AppendChild(xdoc.CreateElement("Page"));
            page2.SetAttribute("ID", "2");
            page2.SetAttribute("Delay", "5");

            n = (XmlElement)page2.AppendChild(xdoc.CreateElement("FillRect"));
            n.SetAttribute("ID", "");
            n.SetAttribute("Location", "2,2");
            n.SetAttribute("Size", "124,60");
            n.SetAttribute("Color", "0x000000");
            n.SetAttribute("Thick", "1");
            n.SetAttribute("Flush", "0");

            n = (XmlElement)page2.AppendChild(xdoc.CreateElement("TextBox"));
            n.SetAttribute("ID", "文本2");
            n.SetAttribute("Location", "2,2");
            n.SetAttribute("Size", "124,60");
            n.SetAttribute("Font", "宋体,16");
            n.SetAttribute("Color", "0xFF0000");
            n.SetAttribute("Scroll", "2,48");
            n.SetAttribute("Flush", "0");
            n.SetAttribute("Wrap", "1");
            n.InnerText = "通知:啥都木有了!啥都木有了!啥都木有了!啥都木有了!啥都木有了!啥都木有了!";

            n = (XmlElement)page2.AppendChild(xdoc.CreateElement("Rect"));
            n.SetAttribute("ID", "");
            n.SetAttribute("Location", "2,2");
            n.SetAttribute("Size", "124,60");
            n.SetAttribute("Color", "0xFFFF00");
            n.SetAttribute("Thick", "1");
            n.SetAttribute("Flush", "0");

            return xdoc;
        }

        String XmlToString(XmlDocument xdoc)
        {
            MemoryStream sm = new MemoryStream();

            xdoc.Save(sm);

            sm.Seek(0, SeekOrigin.Begin);

            StreamReader sr = new StreamReader(sm,System.Text.Encoding.GetEncoding("gb2312"));

            string xml = sr.ReadToEnd();

            sr.Dispose();
            sm.Dispose();

            return xml;
        }

        void ShowXml(XmlDocument xdoc)
        {
            string xml = XmlToString(xdoc);

            edXml.Text = xml;
        }

        XMLDisplay dsp;

        void BeforeRender(string page, Canvas cav, Graphics g)
        {
            if (page == "1")
            {
                Canvas.TextBox t = cav.FindItem("TIME") as Canvas.TextBox;
                if (t != null) t.Text = DateTime.Now.ToString("hh:mm:ss");
            }

            if (page == "2")
            {
                Canvas.PicBox pic = cav.FindItem("PIC") as Canvas.PicBox;

                switch (DateTime.Now.Second % 2)
                {
                    case 0: pic.Image = img1; break;
                    case 1: pic.Image = img2; break;
                }
            }

            g.Clear(Color.Blue);
        }

        void AfterRender(string page, Canvas cav, Graphics g)
        {
            if (page == "1")
            {
                g.FillEllipse(new SolidBrush(Color.Red), new Rectangle(10, 10, 30, 30));
            }
            else if (page == "2")
            {
                g.FillEllipse(new SolidBrush(Color.Green), new Rectangle(10, 10, 30, 30));
            }
            else if (page == "3")
            {
                g.DrawLine(new Pen(Color.Red), 0, 0, 100, 100);
            }
        }

        private void tbDisp_Enter(object sender, EventArgs e)
        {
            dsp = new XMLDisplay(edXml.Text);
            //dsp = XMLDisplay.FromFile(....);
            dsp.BeforeRender = BeforeRender;
            dsp.AfterRender = AfterRender;

            if (pic.Image != null)
            {
                pic.Image.Dispose();
            }

            int s = 2;
            pic.Image = new Bitmap(dsp.Size.Width*s,dsp.Size.Height*s);

            LedHelper.Init(3);
            LedHelper.AddLed("192.168.16.97", 1, dsp.Size.Width, dsp.Size.Height);
            LedHelper.Open();
           
            tmDisp.Enabled = true;
        }

        private void tmDisp_Tick(object sender, EventArgs e)
        {

            if (tbMain.SelectedTab != tbDisp) return;
            Canvas cav = dsp.GetPage();

            cav.Refresh();

            Graphics g = Graphics.FromImage(pic.Image);

            g.DrawImage(cav.Img, 0, 0,pic.Image.Width,pic.Image.Height);

            g.Dispose();

            pic.Refresh();

            LedHelper.AssignCanvas(0, cav);

            //LedHelper.SendToScreen();
        }

        private void tbDisp_Leave(object sender, EventArgs e)
        {
            tmDisp.Enabled = false;
            LedHelper.Close();
        }

        private void fmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            img1.Dispose();
            img2.Dispose();
        }

        private void saveToXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dlgSave.ShowDialog() != DialogResult.OK) return;

            StreamWriter wr = new StreamWriter(dlgSave.FileName, false, Encoding.GetEncoding("GB2312"));
            wr.Write(edXml.Text.Replace("\n","\r\n"));
            wr.Close();
            wr.Dispose();

            MessageBox.Show("Save OK!");
        }
    }
}
