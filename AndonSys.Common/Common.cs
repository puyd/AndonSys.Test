using System;
using System.Collections.Generic;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Forms;

using Microsoft.Data.ConnectionUI;


namespace AndonSys.Common
{
    #region CONFIG

    /// <summary>
    /// 配置文件的段落类
    /// </summary>
    public class TConfigNode : Dictionary<String, String>
    {
        /// <summary>
        /// Xml解析过程
        /// </summary>
        /// <param name="reader">Xml读取器</param>
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement) return;

            reader.ReadStartElement();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                String key = reader.Name;
                String value = reader.ReadString();
                this.Add(key, value);
                reader.ReadEndElement();    //结束当前节点，开始下一节点
            }
        }

        /// <summary>
        /// Xml写入过程
        /// </summary>
        /// <param name="writer">Xml书写器</param>
        public void WriteXml(XmlWriter writer)
        {
            foreach (String key in this.Keys)
            {
                writer.WriteStartElement(key);
                writer.WriteString(this[key]);
                writer.WriteEndElement();
            }
        }
    }

    
    /// <summary>
    /// 配置文件类
    /// </summary>
    [XmlRoot("Config", Namespace = "http://www.andonsystem.cn/")]
    public class TConfig : Dictionary<String, TConfigNode>, IXmlSerializable
    {
        #region IXmlSerializable
        /// <summary>
        /// 接口IXmlSerializable的实现，保留方法
        /// </summary>
        /// <returns>null</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 接口IXmlSerializable的实现,用于读取XML
        /// </summary>
        /// <param name="reader">Xml读取器</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement) return;
            
            reader.ReadStartElement();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                String key = reader.Name;
                TConfigNode node = new TConfigNode();
                this.Add(key, node);
                node.ReadXml(reader);
                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// 接口IXmlSerializable的实现,用于读取XML
        /// </summary>
        /// <param name="writer">Xml读取器</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (String key in this.Keys)
            {
                writer.WriteStartElement(key);
                this[key].WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion

        /// <summary>
        /// 查找一个配置段，如果不存在，则新建
        /// </summary>
        /// <param name="Name">配置段名称</param>
        /// <returns>匹配的配置段</returns>
        public TConfigNode FindNode(String Name)
        {
            TConfigNode n;
            if (this.ContainsKey(Name)) {
                n=this[Name];
            }
            else {
                n = new TConfigNode();
                Add(Name, n);
            }
            return n;
        }
    }

    /// <summary>
    /// 静态类，包含操作配置文件的使用方法
    /// </summary>
    static public class CONFIG
    {
        private static TConfig cfg = null;
        
        /// <summary>
        /// 配置文件的名称，默认为"AndonSys.cfg.xml"
        /// </summary>
        public static String FileName = Application.StartupPath+"\\AndonSys.cfg.xml";

        static void SetDefault()
        {
        }

        static void Init()
        {
            if (cfg == null)
            {
                cfg = new TConfig();
                SetDefault();
            }
        }

        /// <summary>
        /// 读取配置文件，如果不存在，则新建
        /// </summary>
        static public void Load()
        {
            if (File.Exists(FileName) != true)
            {
                Save();
            }
            else
            {
                FileStream fs = new FileStream(FileName, FileMode.Open);
                XmlSerializer ser = new XmlSerializer(typeof(TConfig));
                cfg = (TConfig)ser.Deserialize(fs);
                fs.Close();
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        static public void Save()
        {
            Init();
            
            FileStream fs = new FileStream(FileName, FileMode.Create);
            XmlSerializer ser = new XmlSerializer(typeof(TConfig));
            ser.Serialize(fs, cfg);
            fs.Close();
        }

        /// <summary>
        /// 从配置文件中读取指定的段落属性的值
        /// </summary>
        /// <param name="Node">段落名称</param>
        /// <param name="Key">属性名称</param>
        /// <param name="def">默认值</param>
        /// <returns>如果存在，则返回属性值，否则返回默认值</returns>
        static public String GetText(String Node, String Key,String def)
        {
            Init();
            TConfigNode node;
            try
            {
                node=cfg[Node];
                return cfg[Node][Key];
            }
            catch (KeyNotFoundException)
            {
                if (def!=null)
                {
                    SetText(Node,Key,def);
                    Save();
                }
                return def;
            }
        }

        /// <summary>
        /// 从配置文件中读取指定的段落属性的值
        /// </summary>
        /// <param name="Node">段落名称</param>
        /// <param name="Key">属性名称</param>
        /// <returns>如果存在，则返回属性值，否则返回null</returns>
        static public String GetText(String Node, String Key)
        {
            return GetText(Node, Key,null);
        }

        /// <summary>
        /// 向配置文件指定的段落写入一个属性值
        /// </summary>
        /// <param name="Node">段落名称</param>
        /// <param name="Key">属性名称</param>
        /// <param name="Val">要写入的值</param>
        static public void SetText(String Node, String Key, String Val)
        {
            Init();
            TConfigNode node;
            node = cfg.FindNode(Node);

            if (node.ContainsKey(Key))
            {
                node[Key] = Val;
            }
            else
            {
                node.Add(Key, Val);
            }
        }

        /// <summary>
        /// 从配置文件中读取指定的段落属性的值
        /// </summary>
        /// <param name="Node">段落名称</param>
        /// <param name="Key">属性名称</param>
        /// <param name="def">默认值</param>
        /// <returns>读取成功返回32位整型值，否则返回默认值</returns>
        static public int GetInt(String Node, String Key, int def)
        {
            String s = GetText(Node, Key,def.ToString());
            return int.Parse(s);
        }

        /// <summary>
        /// 从配置文件中读取指定的段落属性的值
        /// </summary>
        /// <param name="Node">段落名称</param>
        /// <param name="Key">属性名称</param>
        /// <param name="def">默认值</param>
        /// <returns>读取成功返回Rect的值，否则返回默认值</returns>
        static public Rect GetRect(String Node, String Key, Rect def)
        {
            String s = GetText(Node, Key, def.ToString());
            return Rect.Parse(s);
        }

        /// <summary>
        /// 从配置文件中读取指定的段落属性的值
        /// </summary>
        /// <param name="Node">段落名称</param>
        /// <param name="Key">属性名称</param>
        /// <param name="def">默认值</param>
        /// <returns>读取成功返回浮点数，否则返回默认值</returns>
        static public float GetFloat(String Node, String Key, float def)
        {
            String s = GetText(Node, Key, def.ToString());
            return float.Parse(s);
        }

        /// <summary>
        /// 从配置文件中读取指定的段落属性的值
        /// </summary>
        /// <param name="Node">段落名称</param>
        /// <param name="Key">属性名称</param>
        /// <param name="def">默认值</param>
        /// <returns>读取成功返回bool值，否则返回默认值</returns>
        static public bool GetBool(String Node, String Key, bool def)
        {
            String s = GetText(Node, Key, def.ToString());
            return bool.Parse(s);
        }

        /// <summary>
        /// 从配置文件中删除指定的段落
        /// </summary>
        /// <param name="Node">要删除的段落名称</param>
        static public void Delete(String Node)
        {
            cfg.Remove(Node);
            Save();
        }
    }
    
    #endregion

    #region CARD
    /// <summary>
    /// 用于操作控制卡及相关串口的静态类
    /// </summary>
    public static class CARD
    {
        static SerialPort[] com;
        public static int ExcuteDelay=20;

        static void SetDefault()
        {
            for (int i = 1; i < com.Length; i++)
            {
                if (com[i]==null) com[i] = new SerialPort();

                com[i].PortName = CONFIG.GetText("CARD","Port."+i.ToString(),"COM10");
                com[i].BaudRate = 57600;
                com[i].Parity = Parity.None;
                com[i].StopBits = StopBits.One;
                com[i].DataBits = 8;

                com[i].DtrEnable = true;
                com[i].ReceivedBytesThreshold = 15;
                com[i].RtsEnable = true;

                com[i].WriteTimeout = 500;
                com[i].WriteBufferSize = 1024;
                com[i].ReadTimeout = 500;
                com[i].ReadBufferSize = 1024;
            }
        }

        static void Init()
        {
            int n = CONFIG.GetInt("CARD", "Port.Num", 1);

            com = new SerialPort[n+1];

            SetDefault();
        }

        /// <summary>
        /// 控制卡对应的默认串口
        /// </summary>
        public static SerialPort[] Port
        {
            get
            {
                if (com == null) Init();
                return com;
            }
        }


        /// <summary>
        /// 开启串口
        /// </summary>
        public static void Open()
        {
            for (int i = 1; i < Port.Length; i++)
            {
                if (!Port[i].IsOpen) Port[i].Open();
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public static void Close()
        {
            for (int i = 1; i < Port.Length; i++)
            {
                if (Port[i].IsOpen) Port[i].Close();
            }
        }

        /// <summary>
        /// 向串口发送byte数组
        /// </summary>
        /// <param name="id">串口ID</param>
        /// <param name="Bytes">要发送的byte数组</param>
        /// <param name="TimeOut">等待超时的毫秒数</param>
        public static void SendBytes(int id,byte[] Bytes,int TimeOut)
        {
            //Debug.WriteLine(FUNC.BytesToString(Bytes));
            
            Open();

            //Port[id].DiscardInBuffer();
            //Port[id].DiscardOutBuffer();

            Port[id].WriteTimeout = TimeOut;
            Port[id].Write(Bytes, 0, Bytes.Length);
        }

        public static byte[] ReadBuf = null;
        
        /// <summary>
        /// 从串口读取数据到byte数组
        /// </summary>
        /// <param name="id">COM口序号</param>
        /// <param name="Bytes">存放读取结果的数组</param>
        /// <param name="Offset">存放数据的开始位置</param>
        /// <param name="Count">要读取的字节数</param>
        /// <param name="TimeOut">等待超时的毫秒数</param>
        /// <returns>实际读取的字节数</returns>
        public static int ReadBytes(int id,byte[] Bytes,int offset, int Count,int TimeOut)
        {
            
            int i=0;
            Open();
            Port[id].ReadTimeout = TimeOut;
            try
            {
                 i= Port[id].Read(Bytes, offset, Count);
            }
            catch //(Exception e)
            {
                //Debug.WriteLine(i.ToString()+" "+ e.Message);
                return i;
            }

            return i;
        }

        
        /// <summary>
        /// 向串口发送指令
        /// </summary>
        /// <param name="id">COM口序号</param>
        /// <param name="addr">卡地址</param>
        /// <param name="func">功能号</param>
        /// <param name="data">参数，高字节在前</param>
        /// <param name="TimeOut">等待超时的毫秒数</param>
        public static void SendCMD(int id,byte addr, byte func, int data , int TimeOut )
        {
            byte[] cmd = new byte[10];
            byte[] d = FUNC.GetBytes(data);

            cmd[0] = Convert.ToByte('W');
            cmd[1] = Convert.ToByte('A');
            cmd[2] = Convert.ToByte('T');
            cmd[3] = addr;
            cmd[4] = func;
            cmd[5] = d[0];
            cmd[6] = d[1];
            cmd[7] = d[2];
            cmd[8] = d[3];
            cmd[9] = FUNC.CRC8(cmd, 0, 9);

            SendBytes(id,cmd,TimeOut);
            
        }


        /// <summary>
        /// 向串口发送指令
        /// </summary>
        /// <param name="id">COM口序号</param>
        /// <param name="addr">卡地址</param>
        /// <param name="func">功能号</param>
        /// <param name="data">参数数组，高字节在前</param>
        /// <param name="TimeOut">等待超时的毫秒数</param>
        public static void SendCMD(int id, byte addr, byte func, int[] data, int TimeOut)
        {
            byte[] cmd = new byte[6 + data.Length * 4];
            cmd[0] = Convert.ToByte('W');
            cmd[1] = Convert.ToByte('A');
            cmd[2] = Convert.ToByte('T');
            cmd[3] = addr;
            cmd[4] = func;

            for (int i = 0; i < data.Length; i++)
            {
                byte[] d = FUNC.GetBytes(data[i]);

                cmd[5 + i*4] = d[0];
                cmd[6 + i*4] = d[1];
                cmd[7 + i*4] = d[2];
                cmd[8 + i*4] = d[3];
            }

            cmd[cmd.Length - 1] = FUNC.CRC8(cmd, 0, cmd.Length - 1);

            SendBytes(id, cmd, TimeOut);
        }

        /// <summary>
        /// 向串口发送指令
        /// </summary>
        /// <param name="id">COM口序号</param>
        /// <param name="mac">无线终端MAC</param>
        /// <param name="addr">卡地址</param>
        /// <param name="func">功能号</param>
        /// <param name="data">参数数组，高字节在前</param>
        /// <param name="TimeOut">等待超时的毫秒数</param>
        public static void SendCMD(int id, short mac, byte addr, byte func, int[] data, int TimeOut)
        {
            byte[] cmd = new byte[8 + data.Length * 4];

            FUNC.GetBytes(mac, out cmd[0], out cmd[1]);

            cmd[2] = Convert.ToByte('W');
            cmd[3] = Convert.ToByte('A');
            cmd[4] = Convert.ToByte('T');
            cmd[5] = addr;
            cmd[6] = func;

            for (int i = 0; i < data.Length; i++)
            {
                byte[] d = FUNC.GetBytes(data[i]);

                cmd[7 + i*4] = d[0];
                cmd[8 + i*4] = d[1];
                cmd[9 + i*4] = d[2];
                cmd[10 + i*4] = d[3];
            }

            cmd[cmd.Length - 1] = FUNC.CRC8(cmd, 2, cmd.Length - 3);

            for (int i = 0; i < cmd.Length; i++)
            {
                Debug.Write(cmd[i].ToString("X2")+" ");
            }

            SendBytes(id, cmd, TimeOut);
        }
        
        /// <summary>
        /// 读取卡的返回数据
        /// </summary>
        /// <param name="data">基于TCardRecv的子类，用于保存返回数据</param>
        /// <param name="TimeOut">超时的毫秒数</param>
        /// <returns>读取成功返回True，否则返回False</returns>
        public static bool ReadData(int id,TCardRecv data,int TimeOut)
        {
            byte[] buf = new byte[15];
            string Head="";
            int size = 0;

            while (size < buf.Length) 
            {
                //if (size > 0) FUNC.Sleep(10);

                int i = ReadBytes(id, buf, size, buf.Length-size, TimeOut);

                if (i == 0) return false;   //读超时

                size = size + i;

                Debug.WriteLine(string.Format("Card.ReadData:({0}) {1} ", size, FUNC.BytesToString(buf)));

                if (size >= 3)    //超过三字节，判断wat起始标记
                {
                    char[] ch = new char[] { Convert.ToChar(buf[0]), Convert.ToChar(buf[1]), Convert.ToChar(buf[2]) };
                    Head = new string(ch, 0, 3);

                    if (Head.ToLower() != "wat")   //起始字节不是wat,去掉一个字节,继续读取
                    {
                        for (int j = 0; j < size - 1; j++)
                        {
                            buf[j] = buf[j + 1];    //前移一个字节
                        }

                        size = size - 1;
                    }
                }
            }; 

            byte c = FUNC.CRC8(buf, 0, 14);
            if (c != buf[14]) return false;

            data.Head = Head;
            data.Addr = buf[3];

            for (int i = 0; i < 10; i++) { data[i] = buf[i + 4]; }

            return true;
        }

        /// <summary>
        /// 执行卡指令,并读取返回数据
        /// </summary>
        /// <param name="addr">指令地址</param>
        /// <param name="func">指令码</param>
        /// <param name="data">指令参数</param>
        /// <param name="recv">基于TCardRecv的对象，用于接收返回数据</param>
        /// <param name="TimeOut">超时的毫秒数</param>
        /// <returns>执行成功返回True，否则返回False</returns>
        public static bool Excute(byte addr, byte func, int data, TCardRecv recv, int TimeOut)
        {
            if (Port.Length < 1)
            {
                throw (new Exception("串口未指定!"));
            }
            else
            {
                return Excute(1, addr, func, data, recv, TimeOut);
            }
        }

        /// <summary>
        /// 执行卡指令,并读取返回数据
        /// </summary>
        /// <param name="addr">指令地址</param>
        /// <param name="func">指令码</param>
        /// <param name="data">指令参数</param>
        /// <param name="recv">基于TCardRecv的对象，用于接收返回数据</param>
        /// <param name="TimeOut">超时的毫秒数</param>
        /// <returns>执行成功返回True，否则返回False</returns>
        public static bool Excute(int id,byte addr, byte func, int data, TCardRecv recv, int TimeOut)
        {
            try
            {
                CARD.SendCMD(id,addr, func, data, TimeOut);
                if (recv != null)
                {
                    Thread.Sleep(ExcuteDelay);
                    return CARD.ReadData(id,recv, TimeOut);
                }
                else
                {
                    return true;
                }
            }
            catch (TimeoutException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 卡返回数据的基类，默认为10个byte的数组
    /// </summary>
    public class TCardRecv : CustomCardHelper.TCardRecv
    {
    }


    /// <summary>
    /// 循环线程的回调函数
    /// </summary>
    public delegate void TLoopThreadOnLoop();

    /// <summary>
    /// 创建一个不断循环的线程，每次循环，都通过OnLoop执行委托的方法
    /// </summary>
    public class TLoopThread
    {
        private volatile bool _stop;
        private TLoopThreadOnLoop OnLoop=null;


        /// <summary>
        /// 系统的线程实例
        /// </summary>
        public Thread Thread { get; set; }

        /// <summary>
        /// 线程执行的主体程序，该程序会循环调用OnLoop方法执行，并根据Stop标记中止线程
        /// </summary>
        protected void ThreadWork()
        {
            while (!_stop)
            {
                OnLoop();
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="OnLoop">指定在循环中委托调用的方法</param>
        public TLoopThread(TLoopThreadOnLoop OnLoop)
        {
            this.OnLoop = OnLoop;
            Thread = new Thread(ThreadWork);
            _stop=false;
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        public void Start()
        {
            _stop = false;
            Thread.Start();
            while (!Thread.IsAlive) ;
        }

        /// <summary>
        /// 通知线程中止
        /// </summary>
        public void Stop()
        {
            _stop = true;
        }

        /// <summary>
        /// 通知并等待线程中止
        /// </summary>
        /// <param name="TimeOut">等待超时时间</param>
        /// <returns></returns>
        public bool WaitForStop(int TimeOut)
        {
            _stop = true;
            return Thread.Join(TimeOut);

        }

        /// <summary>
        /// 通知并等待线程中止
        /// </summary>
        /// <returns></returns>
        public bool WaitForStop()
        {
            return WaitForStop(Timeout.Infinite);
        }
    }

    #endregion

    #region FUNC
    /// <summary>
    /// FUNC静态类包含一系列常用的方法
    /// </summary>
    public static class FUNC
    {

        /// <summary>
        /// 把byte数组转换成16进制的字符串
        /// </summary>
        /// <param name="Bytes">需要转换的byte数组</param>
        /// <returns>如果数组不为空，返回16进制的字符串真,否则返回空字符串("")</returns>
        static public String BytesToString(byte[] Bytes)
        {
            return BytesToString(Bytes, 0, Bytes.Length);
        }

        /// <summary>
        /// 把byte数组转换成16进制的字符串
        /// </summary>
        /// <param name="Bytes">需要转换的byte数组</param>
        /// <param name="offset">需要转换的起始点</param>
        /// <param name="count">需要转换的长度</param>
        /// <returns>如果数组不为空，返回16进制的字符串真,否则返回空字符串("")</returns>
        static public String BytesToString(byte[] Bytes,int offset,int count)
        {
            String s = "";

            for (int i = offset; i < (count+offset); i++)
            {
                if (s != "") s = s + " ";
                s = s + Bytes[i].ToString("X2");
            }

            return s;
        }
        /// <summary>
        /// 计算byte数组的CRC8校验值
        /// </summary>
        /// <param name="Bytes">需要校验的byte数组</param>
        /// <param name="offset">需要校验的起始位置</param>
        /// <param name="len">需要校验的长度</param>
        /// <returns>返回CRC8校验值</returns>
        public static byte CRC8(byte[] Bytes,int offset,int len)
        {
            byte crc;
            crc = 0;

            for (int j = offset; j < len + offset; j++)
            {
                
                crc ^= Bytes[j];


                for (int i = 0; i < 8; i++)
                {
                    if ((crc & (0x01)) == 1)
                    {
                        crc >>= 1;
                        crc ^= 0x8c;
                    }
                    else
                        crc >>= 1;
                }
            }
            return crc;
        }

        
        /// <summary>
        /// 计算byte数组的CRC8校验值
        /// </summary>
        /// <param name="Bytes">需要校验的byte数组</param>
        /// <returns></returns>
        public static byte CRC8(byte[] Bytes)
        {
            return CRC8(Bytes, 0, Bytes.Length);
        }

        /// <summary>
        /// 将当前线程挂起指定的时间
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public static void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        /// <summary>
        /// 把字符串转化成整数
        /// </summary>
        /// <param name="s">要转化的字符串</param>
        /// <param name="DefVal">转化不成功时，取的默认值</param>
        /// <returns>返回32位整数</returns>
        public static int StrToInt32(String s,int DefVal)
        {
            try
            {
                return Convert.ToInt32(s);
            }
            catch (Exception)
            {
                return DefVal;
            }
        }

        /// <summary>
        /// 随机数发生器
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// 把32位整数转化成byte数组，高位在前
        /// </summary>
        /// <param name="num">要转化的32位整数</param>
        /// <returns>返回转化后的数组</returns>
        public static byte[] GetBytes(int num)
        {
            byte[] b = BitConverter.GetBytes(num);
            byte[] r = new byte[4] { b[3], b[2], b[1], b[0] };
            return r;
        }

        /// <summary>
        /// 把16位整数转换成字节
        /// </summary>
        /// <param name="num">要转化的16位整数</param>
        /// <param name="b1">第1字节</param>
        /// <param name="b2">第2字节</param>
        public static void GetBytes(short num, out byte b1, out byte b2)
        {
            byte[] b = BitConverter.GetBytes(num);

            b1 = b[1];
            b2 = b[0];
        }

        /// <summary>
        /// 把32位整数转换成字节
        /// </summary>
        /// <param name="num">要转化的16位整数</param>
        /// <param name="b1">第1字节</param>
        /// <param name="b2">第2字节</param>
        /// <param name="b3">第3字节</param>
        /// <param name="b4">第4字节</param>
        public static void GetBytes(int num, out byte b1, out byte b2, out byte b3,out byte b4)
        {
            byte[] b = BitConverter.GetBytes(num);

            b1 = b[3];
            b2 = b[2];
            b3 = b[1];
            b4 = b[0];
        }
        /// <summary>
        /// 返回字节数组中从指定位置开始的2字节组成的16位无符号整数,高位在前
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="offset">起始位置</param>
        /// <returns></returns>
        public static UInt16 ToUInt16(byte[] value,int offset)
        {
            byte[] b=new byte[2]{value[offset+1],value[offset]};

            return BitConverter.ToUInt16(b, 0);
        }

        /// <summary>
        /// 返回字节数组中从指定位置开始的2字节组成的16位整数,高位在前
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="offset">起始位置</param>
        /// <returns></returns>
        public static Int16 ToInt16(byte[] value, int offset)
        {
            byte[] b = new byte[2] { value[offset + 1], value[offset] };

            return BitConverter.ToInt16(b, 0);
        }

        /// <summary>
        /// 返回2字节组成的16位无符号整数,高位在前
        /// </summary>
        /// <param name="b1">第1字节</param>
        /// <param name="b2">第2字节</param>
        /// <returns></returns>
        public static ushort ToUInt16(byte b1, byte b2)
        {
            byte[] b = new byte[2] { b2, b1 };

            return BitConverter.ToUInt16(b, 0);
        }

        /// <summary>
        /// 返回2字节组成的16位无符号整数,高位在前
        /// </summary>
        /// <param name="b1">第1字节</param>
        /// <param name="b2">第2字节</param>
        /// <returns></returns>
        public static short ToInt16(byte b1, byte b2)
        {
            byte[] b = new byte[2] { b2, b1 };

            return BitConverter.ToInt16(b, 0);
        }

        /// <summary>
        /// 返回字节数组中从指定位置开始的4字节组成的32位无符号整数,高位在前
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="offset">起始位置</param>
        /// <returns></returns>
        public static uint ToUInt32(byte[] value, int offset)
        {
            byte[] b = new byte[4] { value[offset + 3], value[offset+2], value[offset + 1], value[offset] };

            return BitConverter.ToUInt32(b, 0);
        }

        /// <summary>
        /// 返回4字节组成的32位无符号整数,高位在前
        /// </summary>
        /// <param name="b1">第1字节</param>
        /// <param name="b2">第2字节</param>
        /// <param name="b3">第3字节</param>
        /// <param name="b4">第4字节</param>
        /// <returns></returns>
        public static uint ToUInt32(byte b1, byte b2, byte b3, byte b4)
        {
            byte[] b = new byte[4] { b4, b3, b2, b1 };

            return BitConverter.ToUInt32(b, 0);
        }

        /// <summary>
        /// 返回字节数组中从指定位置开始的4字节组成的32位整数,高位在前
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="offset">起始位置</param>
        /// <returns></returns>
        public static int ToInt32(byte[] value, int offset)
        {
            byte[] b = new byte[4] { value[offset + 3], value[offset + 2], value[offset + 1], value[offset] };

            return BitConverter.ToInt32(b, 0);
        }

        /// <summary>
        /// 返回4字节组成的32位整数,高位在前
        /// </summary>
        /// <param name="b1">第1字节</param>
        /// <param name="b2">第2字节</param>
        /// <param name="b3">第3字节</param>
        /// <param name="b4">第4字节</param>
        /// <returns></returns>
        public static int ToInt32(byte b1,byte b2, byte b3, byte b4)
        {
            byte[] b = new byte[4] { b4, b3, b2, b1 };

            return BitConverter.ToInt32(b, 0);
        }


        /// <summary>
        /// 打开数据库连接配置窗口
        /// </summary>
        /// <param name="ConStr">要配置的数据库连接</param>
        /// <returns>配置成功返回True，否则返回False</returns>
        static public bool SetDBConStr(ref String ConStr)
        {
            DataConnectionDialog dlg = new DataConnectionDialog();
            dlg.DataSources.Add(DataSource.SqlDataSource);
            dlg.SelectedDataProvider = DataProvider.SqlDataProvider;
            dlg.ConnectionString = ConStr;
            if (DataConnectionDialog.Show(dlg) == DialogResult.OK)
            {
                ConStr = dlg.ConnectionString;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 把文本中指定的参数替换成相应的值
        /// </summary>
        /// <param name="Text">原始文本</param>
        /// <param name="Params">参数与值对应的列表</param>
        /// <returns>返回替换以后的文本</returns>
        static public String TextByParams(String Text, params String[] Params)
        {
            String s = Text;

            for (int i = 0; i < (Params.Length / 2); i++)
            {
                String os="%" + Params[2 * i] + "%";
                String ns=Params[2 * i + 1];
                s=s.Replace(os,ns);

                //Debug.WriteLine(s);
            }

            return s;
        }

        /// <summary>
        /// 显示一个错误信息的对话框
        /// </summary>
        /// <param name="msg">要显示的错误信息</param>
        /// <returns>返回按钮结果</returns>
        static public DialogResult ErrorDlg(String msg)
        {
            return MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 显示一个警告信息的对话框
        /// </summary>
        /// <param name="msg">要显示的警告信息</param>
        /// <returns>返回按钮结果</returns>
        static public DialogResult WarningDlg(String msg)
        {
            return MessageBox.Show(msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 显示一个提示信息的对话框
        /// </summary>
        /// <param name="msg">要显示的提示信息</param>
        /// <returns>返回按钮结果</returns>
        static public DialogResult InformationDlg(String msg)
        {
            return MessageBox.Show(msg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示一个输入框
        /// </summary>
        /// <param name="Title">提示</param>
        /// <param name="Text">提示</param>
        /// <param name="Input">输入文本</param>
        /// <returns>确定输入返回True，取消输入返回False</returns>
        static public bool InputBox(string Title,string Text, ref string Input)
        {
            return fmInput.InputBox(Title, Text, ref Input);   
        }

        /// <summary>
        /// 显示一个询问Yes/No的对话框
        /// </summary>
        /// <param name="msg">要显示的询问信息</param>
        /// <returns>返回按钮结果</returns>
        static public DialogResult QuestionDlg(String msg)
        {
            return MessageBox.Show(msg, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// 以RAW方式向打印机发送字符串
        /// </summary>
        /// <param name="PRN">打印机设备名</param>
        /// <param name="Text">打印内容</param>
        public static void RAWPrintString(string PRN, string Text)
        {
            RawPrinterHelper.SendStringToPrinter(PRN, Text);
        }

    }
    #endregion

    #region SQLDB

    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class TSQLDB
    {
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        private SqlConnection Connect;

        /// <summary>
        /// 事务处理对象
        /// </summary>
        private SqlTransaction Trans;
        
        /// <summary>
        /// SqlCommand对象
        /// </summary>
        private SqlCommand Command;


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ConStr">数据库连接串</param>
        public TSQLDB(String ConStr)
        {
            Connect = new SqlConnection(ConStr);
            Command = null;
            Trans = null;

        }

        /// <summary>
        /// 析构方法
        /// </summary>
        ~TSQLDB()
        {
            Connect.Dispose();
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        public void Open()
        {
            if (Connect.State == ConnectionState.Closed)
            {
                Connect.Open();

                Command = new SqlCommand();
                Command.Connection = Connect;
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            if (Trans != null) { Trans.Dispose(); Trans = null; }
            if (Command != null) { Command.Dispose(); Command = null; }

            Connect.Close();
        }

        /// <summary>
        /// 开始事务处理
        /// </summary>
        public void BeginTrans()
        {
            if (Trans != null) { Trans.Dispose();}
            Trans = Connect.BeginTransaction();
            Command.Transaction = Trans;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            Trans.Commit();
            
            Trans.Dispose();
            Trans = null;
            Command.Transaction = null;
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            Trans.Rollback();
            Trans.Dispose();
            Trans = null;
            Command.Transaction = null;

        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>执行成功返回True，否则返回False</returns>
        public bool ExcuteSQL(String sql)
        {
            try
            {
                Command.CommandText = sql;
                Command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException e)
            {
                Debug.WriteLine(sql); 
                Debug.WriteLine(e.Message);
                throw (e);
            }
        }
        /// <summary>
        /// 执行SQL语句，并返回SqlDataReader
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>执行成功，返回一个SqlDataReader对象</returns>
        public SqlDataReader QuerySQL(String sql)
        {
            try
            {
                Command.CommandText = sql;
                return Command.ExecuteReader();
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.Message);
                throw e;
            }
        
        }

        /// <summary>
        /// 执行SQL语句，返回一个DataTable对象
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>执行成功，返回一个DataTable对象</returns>
        public DataTable QueryTable(String sql)
        {
            DataTable t = new DataTable();
            SqlDataReader r = QuerySQL(sql);
            t.Load(r);
            r.Close();
            r.Dispose();
            return t;
        }

        /// <summary>
        /// 执行SQL语句，返回记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回记录数</returns>
        public int GetCountBySQL(string sql)
        {
            int n = 0;
            SqlDataReader r = QuerySQL(sql);

            while (r.Read()) n++;

            r.Close();
            r.Dispose();

            return n;
        }

        /// <summary>
        /// 执行SQL语句，返回第一行的第一个字段的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="def">默认值</param>
        /// <returns>执行成功，返回的值，否则返回默认值</returns>
        public T GetValBySQL<T>(string sql,T def)
        {
            T v = def;
            SqlDataReader r = QuerySQL(sql);

            if (r.Read()) v = (T)r[0];

            r.Close();
            r.Dispose();

            return v;
        }

        /// <summary>
        /// 执行SQL语句，返回所有行的第一个字段的值列表
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <returns>执行成功，返回列表，否则返回空的列表</returns>
        public List<T> GetListBySQL<T>(string sql)
        {
            List<T> list = new List<T>();

            SqlDataReader r = QuerySQL(sql);

            while (r.Read())
            {
                list.Add((T)r[0]);
            }
            r.Close();
            r.Dispose();

            return list;
        }

        /// <summary>
        /// 执行SQL语句，返回所有行第一字段和第二字段构成的Key,Value列表
        /// </summary>
        /// <typeparam name="TKey">第一字段的类型</typeparam>
        /// <typeparam name="TValue">第二字段的类型</typeparam>
        /// <param name="sql">要执行的SQL</param>
        /// <returns>执行成功，返回列表，否则返回空的列表</returns>
        public Dictionary<TKey, TValue> GetDictionaryBySQL<TKey, TValue>(string sql)
        {
            Dictionary<TKey, TValue> list = new Dictionary<TKey, TValue>();

            SqlDataReader r = QuerySQL(sql);

            while (r.Read())
            {
                list.Add((TKey)r[0],(TValue)r[1]);
            }
            r.Close();
            r.Dispose();

            return list;
        
        }

    }

    #endregion
}
