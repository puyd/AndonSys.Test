using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace AndonSys.Common
{
    /// <summary>
    /// 串口操作的基类
    /// </summary>
    public class COMPortHelper
    {
        /// <summary>
        /// 串口对象
        /// </summary>
        protected List<SerialPort> ports=new List<SerialPort>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="PortName">串口名称</param>
        public COMPortHelper(params string[] PortName)
        {
            for (int i = 0; i < PortName.Length; i++)
            {
                SerialPort port;
                
                try
                {
                    port = new SerialPort(PortName[i]);
                    ports.Add(port);

                    
                }
                catch (Exception e)
                {
                    port = null;
                    throw e;
                }
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~COMPortHelper()
        {
            foreach (SerialPort port in ports)
            {
                if (port != null) port.Dispose();
            }
        }
        
        /// <summary>
        /// 向串口发送byte数组
        /// </summary>
        /// <param name="id">串口ID</param>
        /// <param name="Bytes">要发送的byte数组</param>
        protected void SendBytes(int id,byte[] Bytes)
        {
            if (!ports[id].IsOpen) ports[id].Open();

            ports[id].DiscardInBuffer();
            ports[id].DiscardOutBuffer();

            ports[id].Write(Bytes, 0, Bytes.Length);
        }

        /// <summary>
        /// 从串口读取指定长度的数据到byte数组
        /// </summary>
        /// <param name="id">串口ID</param> 
        /// <param name="Bytes">存放读取结果的数组</param>
        /// <param name="Count">要读取的字节数</param>
        /// <returns>实际读取的字节数</returns>
        protected int ReadBytes(int id,byte[] Bytes, int Count)
        {
            if (!ports[id].IsOpen) ports[id].Open();
            int i = ports[id].Read(Bytes, 0, Count);

            return i;
        }


        /// <summary>
        /// 从串口读取连续数据到byte数组,直到超时
        /// </summary>
        /// <param name="id">串口ID</param>
        /// <param name="Bytes">存放读取结果的数组</param>
        /// <returns>实际读取的字节数</returns>
        protected int ReadBytes(int id,byte[] Bytes)
        {
            if (!ports[id].IsOpen) ports[id].Open();

            int cnt = 0;

            try
            {
                do
                {
                    byte b = (byte)ports[id].ReadByte();
                    Bytes[cnt] = b;
                    cnt++;

                } while (cnt < Bytes.Length);
            }
            catch
            {
            };

            return cnt;
        }
    }
    
    /// <summary>
    /// 串口条码枪的操作类
    /// </summary>
    public class BarcodeScannerHelper : COMPortHelper
    {
        private TLoopThread Listener = null; 
        
        private List<List<string>> recvs = new List<List<string>>();

        public int BaudRate = 9600;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="PortName">串口名称</param>
        public BarcodeScannerHelper(params string[] PortName):base(PortName)
        {
            Listener = new TLoopThread(OnListenerLoop);

            for (int i = 0; i < PortName.Length; i++)
            {
                recvs.Add(new List<string>());
            }
        }

        /// <summary>
        /// 设置串口初始化参数
        /// </summary>
        /// <param name="id">串口ID</param>
        protected void InitPort(int id)
        {
            ports[id].BaudRate = BaudRate;
            ports[id].Parity = Parity.None;
            ports[id].StopBits = StopBits.One;
            ports[id].DataBits = 8;

            ports[id].DtrEnable = true;
            ports[id].RtsEnable = true;
            ports[id].ReceivedBytesThreshold = 15;

            ports[id].WriteTimeout = 50;
            ports[id].WriteBufferSize = 1024;
            ports[id].ReadTimeout = 50;
            ports[id].ReadBufferSize = 1024;
        }

        /// <summary>
        /// 监听进程的回调函数
        /// </summary>
        /// <param name="id">串口ID</param> 
        protected void OnListen(int id)
        {
            byte[] buf = new byte[50];
            int cnt = ReadBytes(id, buf);

            if (cnt > 0)
            {
                string s = Encoding.ASCII.GetString(buf, 0, cnt);
                recvs[id].Add(s);
            }
        }

        /// <summary>
        /// 获取接收的数据
        /// </summary>
        /// <param name="id">串口ID</param>
        /// <returns>返回接收的条形码</returns>
        public string GetRecv(int id)
        {
            if (recvs[id].Count == 0) return null;

            string s = recvs[id][0];

            recvs[id].RemoveAt(0);
            return s;
        }

        /// <summary>
        /// 启用扫描枪串口
        /// </summary>
        public void Open()
        {
            for (int id = 0; id < ports.Count; id++)
            {
                recvs[id].Clear();
                InitPort(id);
                ports[id].Open();
            }
            StartListener();
        }

        /// <summary>
        /// 关闭扫描枪串口
        /// </summary>
        public void Close()
        {
            WaitForStopListener();
            for (int id = 0; id < ports.Count; id++)
            {
                ports[id].Close();
                recvs[id].Clear();
            }
        }


        /// <summary>
        /// 监听线程的一个循环过程
        /// </summary>
        private void OnListenerLoop()
        {
            try
            {
                for (int id=0;id<ports.Count;id++)
                {
                    OnListen(id);
                    Thread.Sleep(50);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 启动监听线程
        /// </summary>
        protected void StartListener()
        {
            Listener.Start();
        }

        /// <summary>
        /// 中止监听线程
        /// </summary>
        protected void StopListener()
        {
            Listener.Stop();
        }

        /// <summary>
        /// 中止并等待监听线程结束
        /// </summary>
        /// <param name="timeout">等待超时时间</param>
        protected void WaitForStopListener(int timeout)
        {
            Listener.WaitForStop(timeout);
        }

        /// <summary>
        /// 中止并等待监听线程结束
        /// </summary>
        protected void WaitForStopListener()
        {
            Listener.WaitForStop();
        }

    }
    
    /// <summary>
    /// 控制卡操作类的基类
    /// </summary>
    public class CustomCardHelper : COMPortHelper
    {

        /// <summary>
        /// 卡返回数据的基类，默认为10个byte的数组
        /// </summary>
        public class TCardRecv
        {
            /// <summary>
            /// 返回数据的卡地址
            /// </summary>
            public string Head = "";
            
            /// <summary>
            /// 返回数据的卡地址
            /// </summary>
            public byte Addr = 0;
            
            /// <summary>
            /// 返回数据对应的指令
            /// </summary>
            public byte Func = 0;

            /// <summary>
            /// 返回数据对应的指令参数
            /// </summary>
            public int FuncData = 0;

            /// <summary>
            /// 存放返回数据的数组
            /// </summary>
            public byte[] Data = new byte[10];

            /// <summary>
            /// 默认访问器
            /// </summary>
            /// <param name="index">要读取的位置</param>
            /// <returns>返回指定位置的字节</returns>
            public byte this[int index]
            {
                get { return Data[index]; }
                set { Data[index] = value; }
            }

            /// <summary>
            /// ToString()方法重载
            /// </summary>
            /// <returns>以字符串方式返回数组内容</returns>
            public override String ToString()
            {
                return FUNC.BytesToString(Data);
            }

        }

        /// <summary>
        /// 向串口发送指令
        /// </summary>
        /// <param name="id">串口ID</param>
        /// <param name="addr">卡地址</param>
        /// <param name="func">功能号</param>
        /// <param name="data">参数，高字节在前</param>
        protected void SendCMD(int id,byte addr, byte func, int data)
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

            SendBytes(id,cmd);

        }

        /// <summary>
        /// 读取卡的返回数据
        /// </summary>
        /// <param name="id">串口ID</param>
        /// <param name="data">基于TCardRecv的子类，用于保存返回数据</param>
        /// <returns>读取成功返回True，否则返回False</returns>
        protected bool ReadData(int id,TCardRecv data)
        {
            byte[] buf = new byte[15];

            int i = ReadBytes(id,buf, 15);
            if (i != 15) return false;

            byte c = FUNC.CRC8(buf, 0, 14);
            if (c != buf[14]) return false;

            char[] ch = new char[] {Convert.ToChar(buf[0]),Convert.ToChar(buf[1]),Convert.ToChar(buf[2]) };
            data.Head = new string(ch, 0, 3);

 
            if (data.Head.ToLower()!="wat") return false;

            for (i = 0; i < 10; i++) { data[i] = buf[i + 4]; }

            data.Addr = buf[3];

            return true;
        }


        /// <summary>
        /// 执行卡指令,并读取返回数据
        /// </summary>
        /// <param name="id">串口ID</param>
        /// <param name="addr">指令地址</param>
        /// <param name="func">指令码</param>
        /// <param name="data">指令参数</param>
        /// <param name="recv">基于TCardRecv的对象，用于接收返回数据</param>
        /// <returns>执行成功返回True，否则返回False</returns>
        protected bool ExcuteFunc(int id,byte addr, byte func, int data, TCardRecv recv)
        {
            try
            {
                SendCMD(id,addr, func, data);
                
                Thread.Sleep(30);
                
                recv.Addr = addr;
                recv.Func = func;
                recv.FuncData = data;
                
                return ReadData(id, recv);
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        private List<List<TCardRecv>> recvs = new List<List<TCardRecv>>();
        

        /// <summary>
        /// 设置串口初始化参数
        /// </summary>
        protected void InitPort(int id)
        {
            ports[id].BaudRate = 57600;
            ports[id].Parity = Parity.None;
            ports[id].StopBits = StopBits.One;
            ports[id].DataBits = 8;

            ports[id].DtrEnable = true;
            ports[id].RtsEnable = true;
            ports[id].ReceivedBytesThreshold = 15;

            ports[id].WriteTimeout = 50;
            ports[id].WriteBufferSize = 1024;
            ports[id].ReadTimeout = 50;
            ports[id].ReadBufferSize = 1024;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="PortName">串口名称</param>
        public CustomCardHelper(params string[] PortName):base(PortName)
        {
            recvs.Add(null);

            for (int i = 0; i < PortName.Length; i++)
            {
                recvs.Add(new List<TCardRecv>());
            }

        }

        /// <summary>
        /// 启用控制卡串口
        /// </summary>
        public void Open()
        {
            for (int id = 1; id < ports.Count; id++)
            {
                recvs[id].Clear();
                InitPort(id);
                ports[id].Open();
            }
        }

        /// <summary>
        /// 关闭控制卡串口
        /// </summary>
        public void Close()
        {
            for (int id = 1; id < ports.Count; id++)
            {
                ports[id].Close();
                recvs[id].Clear();
            }
        }
    }

    public class ModbusHelper : COMPortHelper
    {
        public int BaudRate = 9600;
        
        /// <summary>
        /// 设置串口初始化参数
        /// </summary>
        /// <param name="id">串口ID</param>
        protected void InitPort(int id)
        {
            ports[id].BaudRate = BaudRate;
            ports[id].Parity = Parity.None;
            ports[id].StopBits = StopBits.One;
            ports[id].DataBits = 8;

            //ports[id].DtrEnable = true;
            //ports[id].RtsEnable = true;
            ports[id].ReceivedBytesThreshold = 15;

            ports[id].WriteTimeout = 1000;
            ports[id].WriteBufferSize = 1024;
            ports[id].ReadTimeout = 1000;
            ports[id].ReadBufferSize = 1024;
        }

        public ModbusHelper(params string[] PortName)
            : base(PortName)
        {
        }

        public void Open()
        {
            for (int i = 0; i < ports.Count; i++)
            {
                InitPort(i);
                ports[i].Open();
            }
        }

        public void Close()
        {
            for (int i = 0; i < ports.Count; i++) 
            {
                ports[i].Close();
            }
        }

        private void GetCRC(byte[] message, byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }

        private void BuildMessage(byte sub, byte cmd, ushort addr, ushort cnt, byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = sub;
            message[1] = cmd;
            message[2] = (byte)(addr >> 8);
            message[3] = (byte)addr;
            message[4] = (byte)(cnt >> 8);
            message[5] = (byte)cnt;

            GetCRC(message, CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }

        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }

        public short[] ReadData(int id, byte sub, ushort addr, ushort cnt)
        {
            short[] values = new short[cnt];

            //Ensure port is open:
            if (ports[id].IsOpen)
            {
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * cnt];
                //Build outgoing modbus message:
                BuildMessage(sub, 3, addr, cnt, message);
                //Send modbus message to Serial Port:
                SendBytes(id, message);

                int c = ReadBytes(id, response);

                if (c != response.Length) throw (new Exception(string.Format("{0} modbus read error!", ports[id].PortName)));

                if (!CheckResponse(response)) throw (new Exception(string.Format("{0} modbus read CRC error!", ports[id].PortName)));

                //Return requested register values:
                for (int i = 0; i < (response.Length - 5) / 2; i++)
                {
                    values[i] = response[2 * i + 3];
                    values[i] <<= 8;
                    values[i] += response[2 * i + 4];
                }

                return values;
            }
            else
            {
                throw (new Exception(string.Format("{0} is closed!", ports[id].PortName)));
            }

        }


        public void WriteData(int id, byte sub, ushort addr, params short[] values)
        {
            if (ports[id].IsOpen)
            {
                byte cnt = (byte)values.Length;

                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                byte[] message = new byte[9 + cnt * 2];
                
                //Build outgoing modbus message:
                message[6] = (byte)(cnt * 2);

                //Put write values into message prior to sending:
                for (int i = 0; i < cnt; i++)
                {
                    message[7 + 2 * i] = (byte)(values[i] >> 8);
                    message[8 + 2 * i] = (byte)(values[i]);
                }

                BuildMessage(sub, 16, addr, cnt, message);

                //Send modbus message to Serial Port:
                SendBytes(id, message);

                //Function 3 response buffer:
                byte[] response = new byte[8];

                int c = ReadBytes(id, response);

                if (c != response.Length) throw (new Exception(string.Format("{0} modbus read error!", ports[id].PortName)));

                if (!CheckResponse(response)) throw (new Exception(string.Format("{0} modbus read CRC error!", ports[id].PortName)));

                //Return requested register values:
                //for (int i = 0; i < (response.Length - 5) / 2; i++)
                //{
                //    values[i] = response[2 * i + 3];
                //    values[i] <<= 8;
                //    values[i] += response[2 * i + 4];
                //}
            }
            else
            {
                throw (new Exception(string.Format("{0} is closed!", ports[id].PortName)));
            }
        }

        public void ClearData(int id, byte sub, ushort addr, int size)
        {
            short[] d = new short[size];
            for (int i = 0; i < size; i++) d[i] = 0;
            WriteData(id, sub, addr, d);
        }

        
    }
}
