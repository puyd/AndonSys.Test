using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.IO;

namespace AndonSys.Common
{
    /// <summary>
    /// 显示类，用于在内置的画布上显示文字及图像
    /// </summary>
    public class Canvas
    {
        /// <summary>
        /// 图形对象基类
        /// </summary>
        public abstract class DrawItemClass
        {
            public Rectangle Rect;
            public bool Visible = true;
            public string ID = "";

            public abstract void Draw(Graphics g, int OrgX, int OrgY, double delta);

            protected void InitRect(int x, int y, int width, int height)
            {
                Rect = new Rectangle(x, y, width, height);
            }

            public int Left { get { return Rect.Left; } }
            public int Top { get { return Rect.Top; } }
            public int Width { get { return Rect.Width; } }
            public int Height { get { return Rect.Height; } }

            public bool SetClip(Graphics g, Window win)
            {
                Rectangle rc = new Rectangle(Left + win.Left, Top + win.Top, Width + 1, Height + 1);

                if (rc.Left < win.Left) rc.X = win.Left;
                if (rc.Top < win.Top) rc.Y = win.Top;

                if (rc.Left + rc.Width >= win.Left + win.Width) rc.Width = win.Left + win.Width - rc.Left;
                if (rc.Top + rc.Height >= win.Top + win.Height) rc.Height = win.Top + win.Height - rc.Top;

                if (rc.Width > 0 && rc.Height > 0)
                {
                    g.SetClip(rc);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 图形对象-矩形
        /// </summary>
        public class FillRect : DrawItemClass
        {
            public Brush Brush = null;
            public bool Flush = false;
            double lastflush = 0;

            public FillRect(int x, int y, int width, int height, Color color)
            {
                InitRect(x, y, width, height);
                Brush = new SolidBrush(color);
            }

            public FillRect(XmlNode xml)
            {
                XmlElement n = xml as XmlElement;

                ID = n.GetAttribute("ID");

                string[] s = n.GetAttribute("Location").Split(',');
                if (s[0] == "") s = new string[] { "0", "0" };
                int x = int.Parse(s[0]);
                int y = int.Parse(s[1]);

                s = n.GetAttribute("Size").Split(',');
                if (s[0] == "") s = new string[] { "256", "128" };
                int width = int.Parse(s[0]);
                int height = int.Parse(s[1]);

                string cs = n.GetAttribute("Color");
                if (cs == "") cs = "0xFFFFFF";
                cs = cs.Substring(2, 6);

                Color c = Color.FromArgb(int.Parse(cs, System.Globalization.NumberStyles.HexNumber));

                InitRect(x, y, width, height);
                Brush = new SolidBrush(Color.FromArgb(255, c));

                string f = n.GetAttribute("Flush");
                if (f == "") f = "0";
                Flush = (int.Parse(f) == 1);

            }


            public override void Draw(Graphics g, int OrgX, int OrgY, double delta)
            {
                lastflush = lastflush + delta;
                while (lastflush > 1000) lastflush = lastflush - 1000;

                if (!Flush || lastflush > 500)
                {

                    Rectangle rect = new Rectangle(Left + OrgX, Top + OrgY, Width, Height);

                    if (Brush != null)
                        g.FillRectangle(Brush, rect);
                }
            }
        }

        /// <summary>
        /// 图形对象-矩形
        /// </summary>
        public class Rect : DrawItemClass
        {
            public Pen Pen = null;
            public bool Flush = false;
            double lastflush = 0;

            public Rect(int x, int y, int width, int height, Color color, float thick)
            {
                InitRect(x, y, width, height);
                Pen = new Pen(color, thick);
            }

            public Rect(XmlNode xml)
            {
                XmlElement n = xml as XmlElement;

                ID = n.GetAttribute("ID");

                string[] s = n.GetAttribute("Location").Split(',');
                if (s[0] == "") s = new string[] { "0", "0" };
                int x = int.Parse(s[0]);
                int y = int.Parse(s[1]);

                s = n.GetAttribute("Size").Split(',');
                if (s[0] == "") s = new string[] { "256", "128" };
                int width = int.Parse(s[0]);
                int height = int.Parse(s[1]);

                string cs = n.GetAttribute("Color");
                if (cs == "") cs = "0xFFFFFF";
                cs = cs.Substring(2, 6);

                Color c = Color.FromArgb(int.Parse(cs, System.Globalization.NumberStyles.HexNumber));

                string t = n.GetAttribute("Thick");
                if (t == "") t = "1";
                int thick = int.Parse(t);

                InitRect(x, y, width, height);
                Pen = new Pen(Color.FromArgb(255, c), thick);

                string f = n.GetAttribute("Flush");
                if (f == "") f = "0";
                Flush = (int.Parse(f) == 1);

            }


            public override void Draw(Graphics g, int OrgX, int OrgY, double delta)
            {
                lastflush = lastflush + delta;
                while (lastflush > 1000) lastflush = lastflush - 1000;

                if (!Flush || lastflush > 500)
                {

                    Rectangle rect = new Rectangle(Left + OrgX, Top + OrgY, Width, Height);

                    if (Pen != null)
                        g.DrawRectangle(Pen, rect);
                }
            }
        }

        /// <summary>
        /// 图形对象-圆形
        /// </summary>
        public class FillEllipse : DrawItemClass
        {
            public Brush Brush = null;
            public bool Flush = false;

            double lastflush = 0;

            public FillEllipse(int x, int y, int width, int height, Color color)
            {
                InitRect(x, y, width, height);
                Brush = new SolidBrush(color);
            }

            public FillEllipse(XmlNode xml)
            {
                XmlElement n = xml as XmlElement;

                ID = n.GetAttribute("ID");

                string[] s = n.GetAttribute("Location").Split(',');
                if (s[0] == "") s = new string[] { "0", "0" };
                int x = int.Parse(s[0]);
                int y = int.Parse(s[1]);

                s = n.GetAttribute("Size").Split(',');
                if (s[0] == "") s = new string[] { "256", "128" };
                int width = int.Parse(s[0]);
                int height = int.Parse(s[1]);

                string cs = n.GetAttribute("Color");
                if (cs == "") cs = "0xFFFFFF";
                cs = cs.Substring(2, 6);

                Color c = Color.FromArgb(int.Parse(cs, System.Globalization.NumberStyles.HexNumber));

                InitRect(x, y, width, height);
                Brush = new SolidBrush(Color.FromArgb(255, c));

                string f = n.GetAttribute("Flush");
                if (f == "") f = "0";
                Flush = (int.Parse(f) == 1);
            }

            public override void Draw(Graphics g, int OrgX, int OrgY, double delta)
            {
                lastflush = lastflush + delta;
                while (lastflush > 1000) lastflush = lastflush - 1000;

                if (!Flush || lastflush > 500)
                {
                    Rectangle rect = new Rectangle(Left + OrgX, Top + OrgY, Width, Height);
                    g.FillEllipse(Brush, rect);
                }
            }
        }

        /// <summary>
        /// 图形对象-圆形
        /// </summary>
        public class Ellipse : DrawItemClass
        {
            public Pen Pen = null;
            public bool Flush = false;

            double lastflush = 0;

            public Ellipse(int x, int y, int width, int height, Color color, float thick)
            {
                InitRect(x, y, width, height);
                Pen = new Pen(color, thick);
            }

            public Ellipse(XmlNode xml)
            {
                XmlElement n = xml as XmlElement;

                ID = n.GetAttribute("ID");

                string[] s = n.GetAttribute("Location").Split(',');
                if (s[0] == "") s = new string[] { "0", "0" };
                int x = int.Parse(s[0]);
                int y = int.Parse(s[1]);

                s = n.GetAttribute("Size").Split(',');
                if (s[0] == "") s = new string[] { "256", "128" };
                int width = int.Parse(s[0]);
                int height = int.Parse(s[1]);

                string cs = n.GetAttribute("Color");
                if (cs == "") cs = "0xFFFFFF";
                cs = cs.Substring(2, 6);

                Color c = Color.FromArgb(int.Parse(cs, System.Globalization.NumberStyles.HexNumber));

                string t = n.GetAttribute("Thick");
                if (t == "") t = "1";
                int thick = int.Parse(t);

                InitRect(x, y, width, height);
                Pen = new Pen(Color.FromArgb(255, c), thick);

                string f = n.GetAttribute("Flush");
                if (f == "") f = "0";
                Flush = (int.Parse(f) == 1);
            }

            public override void Draw(Graphics g, int OrgX, int OrgY, double delta)
            {
                lastflush = lastflush + delta;
                while (lastflush > 1000) lastflush = lastflush - 1000;

                if (!Flush || lastflush > 500)
                {
                    Rectangle rect = new Rectangle(Left + OrgX, Top + OrgY, Width, Height);
                    g.DrawEllipse(Pen, rect);
                }
            }
        }

        /// <summary>
        /// 图形对象-静态文本
        /// </summary>
        public class TextBox : DrawItemClass
        {
            public string Text;
            public Color Color;
            public Font Font;

            public int Scroll = 0;
            public int Speed = 10;

            public bool Wrap = false;

            void Init(int x, int y, int width, int height, string s, string fontname, int size, Color color)
            {
                InitRect(x, y, width, height);
                Color = color;

                Text = s;
                Font = new Font(fontname, size);
            }

            public TextBox(int x, int y, int width, int height, string s, string fontname, int size, Color color)
            {
                Init(x, y, width, height, s, fontname, size, color);
            }

            public TextBox(XmlNode xml)
            {
                XmlElement n = xml as XmlElement;

                ID = n.GetAttribute("ID");

                string[] s = n.GetAttribute("Location").Split(',');
                if (s[0] == "") s = new string[] { "0", "0" };
                int x = int.Parse(s[0]);
                int y = int.Parse(s[1]);

                s = n.GetAttribute("Size").Split(',');
                if (s[0] == "") s = new string[] { "256", "128" };
                int width = int.Parse(s[0]);
                int height = int.Parse(s[1]);

                string text = n.InnerText;

                s = n.GetAttribute("Font").Split(',');
                if (s[0] == "") s = new string[] { "宋体", "16" };
                string fontname = s[0];
                int fontsize = int.Parse(s[1]);

                string cs = n.GetAttribute("Color");
                if (cs == "") cs = "0xFFFFFF";
                cs = cs.Substring(2, 6);

                Color c = Color.FromArgb(int.Parse(cs, System.Globalization.NumberStyles.HexNumber));

                Init(x, y, width, height, text, fontname, fontsize, Color.FromArgb(255, c));

                s = n.GetAttribute("Scroll").Split(',');
                if (s.Length == 0) s = new string[] { "0", "0" };
                Scroll = int.Parse(s[0]);
                Speed = int.Parse(s[1]);

                string f = n.GetAttribute("Flush");
                if (f == "") f = "0";
                Flush = (int.Parse(f) == 1);

                string w = n.GetAttribute("Wrap");
                if (w == "") w = "0";
                Wrap = (int.Parse(w) == 1);

            }

            double lastpos = 0;
            double lastflush = 0;
            public bool Flush = false;

            public override void Draw(Graphics g, int OrgX, int OrgY, double delta)
            {
                lastflush = lastflush + delta;
                while (lastflush > 1000) lastflush = lastflush - 1000;

                if (Font == null) Font = new Font("宋体", 11);

                string st = "";

                int i = 0;

                while (i < Text.Length)
                {
                    if (Text[i] != '&')
                    {
                        st = st + Text[i];
                        i++;
                    }
                    else
                    {
                        i++;
                        if (Text[i] == '&') st = st + Text[i];
                        i++;
                    }
                }


                SizeF sz;

                if (Wrap)
                {
                    sz = g.MeasureString(st, Font, Width, new StringFormat());
                }
                else
                {
                    sz = g.MeasureString(st, Font, int.MaxValue, new StringFormat(StringFormatFlags.NoWrap));
                }

                //g.DrawString(st, Font, new SolidBrush(Color), Left+OrgX, Top+OrgY);
                //return;

                lastpos = (Scroll == 0) ? 0 : lastpos - Speed * delta / 1000;

                int x = Left;
                int y = Top;

                switch (Scroll)
                {
                    case 1:
                        if (Speed >= 0 && lastpos + sz.Width < 0) lastpos = Width;
                        if (Speed < 0 && lastpos > sz.Width) lastpos = -sz.Width;
                        x = x + (int)lastpos;
                        break;
                    case 2:
                        if (Speed >= 0 && lastpos + sz.Height < 0) lastpos = Height;
                        if (Speed < 0 && lastpos > sz.Height) lastpos = -sz.Height;
                        y = y + (int)lastpos;
                        break;
                }

                x = x + OrgX;
                y = y + OrgY;

                i = 0;

                int xi = x;
                int yi = y;

                Brush brush = new SolidBrush(Color);

                while (i < Text.Length)
                {
                    if (Text[i] == '\r')
                    {
                        i++;
                        SizeF tz = g.MeasureString("中", Font);
                        xi = x;
                        yi = yi + (int)tz.Height;
                        if (Text[i] == '\n') i++;
                    }
                    else if (Text[i] != '&')
                    {
                        st = Text[i].ToString();

                        SizeF tz = g.MeasureString(st + st + st + st + st, Font);
                        if (st == " ") tz = g.MeasureString("aaaaa", Font);

                        int w = (int)tz.Width / 5;

                        if (xi + w >= (x + sz.Width) && Wrap)
                        {
                            xi = x;
                            yi = yi + (int)tz.Height;
                        }

                        if ((!Flush) || (lastflush > 500))
                        {
                            g.DrawString(st, Font, brush, xi, yi);
                        }

                        xi = xi + w;

                        i++;
                    }
                    else
                    {
                        i++;
                        switch (Text[i])
                        {
                            case '&': continue;
                            case 'r': brush = new SolidBrush(Color.Red); break;
                            case 'g': brush = new SolidBrush(Color.Green); break;
                            case 'y': brush = new SolidBrush(Color.Yellow); break;
                            case 'b': brush = new SolidBrush(Color.Black); break;
                            case 'F': Flush = !Flush; break;
                        }
                        i++;
                    }
                }
            }



        }


        public class PicBox : DrawItemClass
        {
            public Image Image = null;

            public PicBox(int x, int y, int width, int height)
            {
                InitRect(x, y, width, height);
            }

            public PicBox(XmlNode xml)
            {
                XmlElement n = xml as XmlElement;

                ID = n.GetAttribute("ID");

                string[] s = n.GetAttribute("Location").Split(',');
                if (s[0] == "") s = new string[] { "0", "0" };
                int x = int.Parse(s[0]);
                int y = int.Parse(s[1]);

                s = n.GetAttribute("Size").Split(',');
                if (s[0] == "") s = new string[] { "256", "128" };
                int width = int.Parse(s[0]);
                int height = int.Parse(s[1]);

                InitRect(x, y, width, height);
             }


            public override void Draw(Graphics g, int OrgX, int OrgY, double delta)
            {
                Rectangle rect = new Rectangle(Left + OrgX, Top + OrgY, Width, Height);

                if (Image == null) return;

                g.DrawImageUnscaledAndClipped(Image, rect);
            }
        }

        public class Window
        {
            List<DrawItemClass> list = new List<DrawItemClass>();

            public int Left;
            public int Top;
            public int Width;
            public int Height;

            public Window(int x, int y, int width, int height)
            {
                Left = x;
                Top = y;
                Width = width;
                Height = height;
            }

            public void AddItem(DrawItemClass item)
            {
                list.Add(item);
            }

            public void Draw(Graphics g, double delta)
            {
                foreach (DrawItemClass item in list)
                {
                    if (item.Visible && item.SetClip(g, this))
                    {
                        item.Draw(g, this.Left, this.Top, delta);
                    }
                }
            }

            public DrawItemClass FindItem(string ID)
            {
                foreach (DrawItemClass item in list)
                {
                    if (item.ID==ID) return item;
                }

                return null;
            }
        }

        Bitmap img;

        List<Window> windows = new List<Window>();

        public Canvas(int Width, int Height)
        {
            img = new Bitmap(Width, Height);
            AddWindow(0, 0, Width, Height);
        }

        ~Canvas()
        {
            img.Dispose();
        }

        public Bitmap Img
        {
            get { return img; }
        }

        public int AddWindow(int x, int y, int width, int height)
        {
            Rectangle rc = new Rectangle(x, y, width, height);

            lock (windows)
            {
                Window w = new Window(x, y, width, height);
                windows.Add(w);
                return windows.Count - 1;
            }
        }

        public DrawItemClass FindItem(string ID)
        {
            foreach (Window w in windows)
            {
                DrawItemClass item = w.FindItem(ID);
                if (item != null) return item;
            }
            return null;
        }

        public delegate void RenderEvent(Canvas cav,Graphics g);

        public RenderEvent BeforeRender = null;
        public RenderEvent AfterRender = null;

        public void Refresh()
        {
            Render();
        }

        public void Render()
        {
            DateTime n = DateTime.Now;
            TimeSpan t = new TimeSpan(n.Ticks - pretime.Ticks);
            pretime = n;
            lock (this)
            {
                Graphics g = Graphics.FromImage(img);
                g.PageUnit = GraphicsUnit.Pixel;
                g.Clear(Color.Black);

                if (BeforeRender != null) BeforeRender(this, g);

                foreach (Window win in windows)
                {
                    win.Draw(g, t.TotalMilliseconds);
                }

                if (AfterRender != null)
                {
                    g.SetClip(new Rectangle(0,0,img.Width,img.Height));
                    AfterRender(this, g);
                }

                g.Dispose(); ;
            }
        }

        public void AddItem(int win, DrawItemClass item)
        {
            windows[win].AddItem(item);
        }

        DateTime pretime = DateTime.Now;

        public void Draw(Graphics g, int x, int y, double scale)
        {
            lock (this)
            {
                Rectangle rect = new Rectangle(x, y, (int)(img.Width * scale), (int)(img.Height * scale));

                g.DrawImage(img, rect);
            }
        }

    }

    public class XMLDisplay
    {
        class Page : Canvas
        {
            public int Delay = 0;

            public Page(int width, int height) : base(width, height) { }
        }

        public delegate void DataBindEvent(String pageID,Canvas page);
        public delegate void RenderEvent(String pageID, Canvas page, Graphics g);


        XmlDocument XML = new XmlDocument();
        Dictionary<string, Page> Pages = new Dictionary<string, Page>();
        
        public Size Size;

        DateTime last = DateTime.Now;
        int curPage = 0;

        public DataBindEvent OnDataBind = null;
        public RenderEvent BeforeRender = null;
        public RenderEvent AfterRender = null;

        public XMLDisplay(string xml)
        {
            XML.LoadXml(xml);
            MakePages();
        }

        public static XMLDisplay FromFile(string file)
        {
            StreamReader r = new StreamReader(file);
            string xml = r.ReadToEnd();
            r.Dispose();

            return new XMLDisplay(xml);
        }
        
        public Canvas GetPage()
        {
            Page[] ps = Pages.Values.ToArray();

            Page p = ps[curPage];
            
            TimeSpan s = new TimeSpan(DateTime.Now.Ticks - last.Ticks);
            if (p.Delay < s.TotalSeconds)
            {
                curPage++;
                if (curPage >= ps.Length) curPage = 0;

                last = DateTime.Now;
            }

            return ps[curPage];
        }

        public Canvas GetPage(string id)
        {
            string[] k = Pages.Keys.ToArray();
            for (int i=0;i<k.Length;i++)
            {
                if (k[i]==id)
                {
                    curPage=i;
                    break;            
                }
            }
            Page[] ps = Pages.Values.ToArray();

            Page p = ps[curPage];

            return p;
        }

        public Canvas GetPage(int i)
        {
            curPage = i;

            Page[] ps = Pages.Values.ToArray();

            Page p = ps[curPage];

            return p;
        }

        string GetXML()
        {
            return XML.ToString();
        }

        void MakePages()
        {
            XmlElement root = (XmlElement)XML.SelectSingleNode("Display");
            string[] s = root.GetAttribute("Size").Split(',');
            Size = new Size(int.Parse(s[0]), int.Parse(s[1]));

            Pages.Clear();

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                Page cav = new Page(Size.Width, Size.Height);
                cav.BeforeRender = BeforePageRender;
                cav.AfterRender = AfterPageRender;

                XmlElement page = root.ChildNodes[i] as XmlElement;
                string id = page.GetAttribute("ID");
                cav.Delay = int.Parse(page.GetAttribute("Delay"));

                Pages.Add(id, cav);

                foreach (XmlNode n in page.ChildNodes)
                {

                    if (n.Name == "TextBox")
                    {
                        cav.AddItem(0, new Canvas.TextBox(n));
                    }
                    else if (n.Name == "FillRect")
                    {
                        cav.AddItem(0, new Canvas.FillRect(n));
                    }
                    else if (n.Name == "Rect")
                    {
                        cav.AddItem(0, new Canvas.Rect(n));
                    }
                    else if (n.Name == "FillEllipse")
                    {
                        cav.AddItem(0, new Canvas.FillEllipse(n));
                    }
                    else if (n.Name == "Ellipse")
                    {
                        cav.AddItem(0, new Canvas.Ellipse(n));
                    }
                    else if (n.Name == "PicBox")
                    {
                        cav.AddItem(0, new Canvas.PicBox(n));
                    }

                }

                cav.Refresh();
            }
        }

        void BeforePageRender(Canvas cav, Graphics g)
        {
            if (BeforeRender != null) 
                BeforeRender(Pages.Keys.ToArray()[curPage], cav, g);
        }

        void AfterPageRender(Canvas cav, Graphics g)
        {
            if (AfterRender != null)
                AfterRender(Pages.Keys.ToArray()[curPage], cav, g);
        }

    }

}