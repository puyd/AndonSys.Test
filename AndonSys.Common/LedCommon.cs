//===============================================================================
//
//	LabLed
//  组件 --  LED显示屏 组件  
//  
//   ----- 用于对机房LED显示屏控制
//         
//===============================================================================
//
// Copyright (C) 2002-2007 北京顶点时空软件工作室
// 保留所有的权利.
// 
// 创建日期: 2007-12-10
// 创 建 者: Liushiying (lsy@sogou.com)
// 创建说明: LED显示屏控制类
// 修改日期: 2007-12-10
// 修 改 者: Liushiying (lsy@sogou.com)
// 修改说明:  
//
//===============================================================================

namespace LabLed.Components
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	/// <summary>
	/// LED显示屏控制类
	/// </summary>
	public class LedCommon : IDisposable
	{

		#region 常量及枚举定义
		/// <summary>
		/// 通讯方式常量
		/// </summary>
		public enum eDevType 
		{
			DEV_COM = 0,                //串口通讯    0
			DEV_UDP,	                //UDP通讯     1
			DEV_MODEM		            //Modem通讯   2
		}
		/// <summary>
		/// 端口常量
		/// </summary>
		public enum eCommPort
		{
			Address = 0,                //网络地址
			ComPort = 4                //用COM4作为通讯端口
		}

		/// <summary>
		/// 串行通讯速度
		/// </summary>
		public enum eBaudRate
		{
			SBR_9600 = 0,               //串口速率9600   0
			SBR_14400,                  //串口速率14400  1
			SBR_19200,                  //串口速率19200  2
			SBR_38400,                  //串口速率38400  3
			SBR_57600,                  //串口速率57600  4
			SBR_115200                  //串口速率115200 5
		}

		/// <summary>
		/// 播放类型
		/// </summary>
		public enum eRootType
		{
			ROOT_PLAY = 17,             //下发节目为播放数据 17
			ROOT_DOWNLOAD				//下发节目为保存并播放 18
		}

		/// <summary>
		/// 显示屏类型常量
		/// </summary>
		public enum eScreenType
		{
			SCREEN_UNICOLOR = 1,        //单色显示屏 1
			SCREEN_COLOR,	            //双色显示屏 2
			SCREEN_FULLCOLOR,	        //全彩色显示屏 3
			SCREEN_GRAY		            //256级灰度屏 4
		}

		/// <summary>
		/// 响应消息常量
		/// </summary>
		public enum eResponseMessage
		{
			LM_RX_COMPLETE = 1,          //接收结束 1
			LM_TX_COMPLETE,		         //发送结束 2
			LM_RESPOND,		             //收到应答 3
			LM_TIMEOUT,		             //超时 4
			LM_NOTIFY,		             //通知消息 5
			LM_PARAM,		
			LM_TX_PROGRESS,		         //发送中 7
			LM_RX_PROGRESS		         //接收中 8
		}

		/// <summary>
		/// 电源状态常量
		/// </summary>
		public enum ePowerStatus
		{
			LED_POWER_OFF = 0,          //显示屏电源已关闭 0
			LED_POWER_ON	            //显示屏电源打开 1
		}

		//时间格式常量
		public enum eTimeFormat
		{
			DF_YMD = 1,                 //  1 年月日  "2004年12月31日"
			DF_HN,		                //  2 时分    "19:20"
			DF_HNS,		                //  3 时分秒  "19:20:30"
			DF_Y,		                //  4 年      "2004"
			DF_M,		                //  5 月      "12" "01" 注意：始终显示两位数字
			DF_D,				        //  6 日
			DF_H,	                    //  7 时
			DF_N,			            //  8 分
			DF_S,		                //  9 秒
			DF_W		                // 10 星期    "星期三"
		}

		/// <summary>
		/// 正计时、倒计时format参数
		/// </summary>
		public enum eCountType
		{
			CF_DAY = 0,                 // 0 天数
			CF_HOUR,					// 1 小时数
			CF_HMS,						// 2 时分秒
			CF_HM,						// 3 时分
			CF_MS,						// 4 分秒
			CF_S						// 5 秒
		}

        public const int NOTIFY_EVENT = 1;
        public const int NOTIFY_BLOCK = 2;

        public const int FONT_SET_16 = 0;            //16点阵字符
		public const int FONT_SET_24 = 1;            //24点阵字符

		public const int PKC_QUERY = 4;
		public const int PKC_ADJUST_TIME = 6;
		public const int PKC_GET_POWER = 9;
		public const int PKC_SET_POWER = 10;
		public const int PKC_GET_BRIGHT = 11;
		public const int PKC_SET_BRIGHT = 12;
		#endregion 

		#region 结构体定义
		/// <summary>
		/// 结构体定义
		/// </summary>

		//设备定义
		public struct DEVICEPARAM
		{
			public uint devType;                      //通讯设备类型
			public uint Speed;                        //通讯速度(仅对串行通讯有用)
			public uint ComPort;
			public uint FlowCon;
			public uint locPort;                      //本地端口(对串行通讯为：串口号；对UDP通讯为：本地端口号，一般要大于1024)
			public uint rmtPort;                      //远程端口号(对UDP通讯有用，必须为6666)
			public uint memory;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Phone;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] Reserved;
		}

		//显示区域定义
		public struct RECT
		{            
			public int left;   //左            
			public int top;    //上            
			public int right;  //右            
			public int bottom; //下
		}

		public struct SYSTEMTIME
		{
            public int wYear;
            public int wMonth; 
            public int wDayOfWeek;
            public int wDay;
            public int wHour;
            public int wMinute;
            public int wSecond;
            public int wMilliseconds; 

		}
		public struct NotifyMessage
		{
			public int   Message;
			public int   Command;
			public int   Result;
			public int   Status;
			public int   Address;
			public int   Size;
			public int   Buffer;
			public DEVICEPARAM param;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string Host;
			public int   port;
		}
		#endregion

		/// <summary>
		/// API封闭
		/// </summary>
		
		public LedCommon()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		#region 函数 Dispose 释放资源
		/// <summary>
		/// 释放资源
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(true); 
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
		}		
		#endregion

		#region 打开通讯信道
        //long (_stdcall *LED_Open)(const PDeviceParam param, long Notify, long Window, long Message);
		[DllImport("LEDSender.DLL", EntryPoint="LED_Open")]
		public static extern int DLL_LED_Open(ref DEVICEPARAM param,int Notify, int Window, int Message);

		/// <summary>
		/// 打开通讯信道
		/// </summary>
		/// <param name="param">DEVICEPARAM结构的设备参数</param>
		/// <param name="Notify">是否产生通知消息 0:不产生 1:产生</param>
		/// <param name="Window">接收通知消息的窗口句柄</param>
		/// <param name="Message">消息标识</param>
		/// <returns></returns>
		public int LED_Open(ref DEVICEPARAM param, int Notify, int Window, int Message)
		{
			return DLL_LED_Open(ref param, Notify, Window, Message);			
		}
		#endregion

		#region 关闭通讯信道

		//void (_stdcall *LED_Close)(long dev);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_Close")]
		public static extern void DLL_LED_Close(int dev);
		/// <summary>
		/// 关闭已打开的通讯设备
		/// </summary>
		/// <param name="dev">LED_Open函数的返回值</param>
		public void LED_Close(int dev)
		{
			System.Threading.Thread.Sleep(100);
			DLL_LED_Close(dev);
		}
		#endregion 

        #region 关闭所有通讯信道

        //void (_stdcall *LED_CloseAll)();
        [DllImport("LEDSender.DLL", EntryPoint = "LED_CloseAll")]
        public static extern void DLL_LED_CloseAll();
        /// <summary>
        /// 关闭已打开的通讯设备
        /// </summary>
        public void LED_CloseAll()
        {
            System.Threading.Thread.Sleep(100);
            DLL_LED_CloseAll();
        }
        #endregion

        #region 查询某个通讯信道的状态

        //long (_stdcall *LED_GetDeviceStatus)(long dev);
        [DllImport("LEDSender.DLL", EntryPoint = "LED_GetDeviceStatus")]
        public static extern int DLL_LED_GetDeviceStatus(int dev);
        /// <summary>
        /// 查询某个通讯信道的状态
        /// </summary>
        /// <param name="dev">LED_Open函数的返回值</param>
        /// <return>通讯状态 0:该信道空闲，可以通讯 1:信道正在通讯中 -1:信道未打开或者打开错误</return>
        public int LED_GetDeviceStatus(int dev)
        {
            return DLL_LED_GetDeviceStatus(dev);
        }
        #endregion

        #region 查询显示屏是否能够通讯
		//void (_stdcall *LED_Query)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint="LED_Query")]
		public static extern void DLL_LED_Query(int dev, byte Address, string Host, ushort port);
		/// <summary>
		/// 查询显示屏是否能够通讯
		/// </summary>
		/// <param name="dev">该参数是LED_Open函数的返回值</param>
		/// <param name="Address"></param>
		/// <param name="Host">显示屏IP地址 (仅对UDP有效);串口通讯可以写任意地址或者空</param>
		/// <param name="port">显示屏端口号(如果是UDP通讯，该端口为6666)</param>
		public void LED_Query(int dev, byte Address, string Host, ushort port)
		{
			DLL_LED_Query(dev, Address, Host, port);
		}
		#endregion 

		#region 用计算机时钟校正显示屏内时钟
		//void (_stdcall *LED_AdjustTime)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint="LED_AdjustTime")]
		public static extern void DLL_LED_AdjustTime(int dev, byte Address, string Host, ushort port);
		/// <summary>
		/// 计算机时钟校正显示屏内时钟
		/// </summary>
		/// <param name="dev"></param>
		/// <param name="Address"></param>
		/// <param name="Host"></param>
		/// <param name="port"></param>
		public void LED_AdjustTime(int dev, byte Address, string Host, ushort port)
		{
			DLL_LED_AdjustTime(dev, Address, Host, port);
		}
		#endregion

		#region 发送数据到显示屏
		//void (_stdcall *LED_SendToScreen)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint="LED_SendToScreen")]
		public static extern void DLL_LED_SendToScreen(int dev, byte Address, string Host, ushort port);

		/// <summary>
		/// 将形成的节目数据发送到显示屏
		/// </summary>
		/// <param name="dev"></param>
		/// <param name="Address"></param>
		/// <param name="Host"></param>
		/// <param name="port"></param>
		public void LED_SendToScreen(int dev, byte Address, string Host, ushort port)
		{
			DLL_LED_SendToScreen(dev, Address, Host, port);
		}
		#endregion

		#region 获取电源状态
		//void (_stdcall *LED_GetPower)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint="LED_GetPower")]
		public static extern void DLL_LED_GetPower(int dev, byte Address, string Host, ushort port);
		/// <summary>
		/// 获取电源状态
		/// </summary>
		/// <param name="dev"></param>
		/// <param name="Address"></param>
		/// <param name="Host"></param>
		/// <param name="port"></param>
		public void LED_GetPower(int dev, byte Address, string Host, ushort port)
		{
			DLL_LED_GetPower(dev, Address, Host, port);
		}
		#endregion 

		#region 设置显示屏电源
		//void (_stdcall *LED_SetPower)(long dev, BYTE Address, char *Host, WORD port, DWORD Power);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_SetPower")]
		public static extern void DLL_LED_SetPower(int dev, byte Address, string Host, ushort port, uint Power);

		/// <summary>
		/// 打开或关闭显示屏电源
		/// </summary>
		/// <param name="dev"></param>
		/// <param name="Address"></param>
		/// <param name="Host"></param>
		/// <param name="port"></param>
		/// <param name="Power">LED_POWER_ON 打开电源, LED_POWER_OFF 关闭电源</param>		
		public void LED_SetPower(int dev, byte Address, string Host, ushort port, ePowerStatus Power)
		{
			DLL_LED_SetPower(dev, Address ,Host ,port , (uint)Power);
		}
		#endregion

		#region 获取显示屏亮度
		//void (_stdcall *LED_GetBrightness)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_GetBrightness")]
		public static extern void DLL_LED_GetBrightness(int dev, byte Address, string Host, ushort port);

		/// <summary>
		/// 获取显示屏亮度
		/// </summary>
		/// <param name="dev"></param>
		/// <param name="Address"></param>
		/// <param name="Host"></param>
		/// <param name="port"></param>
		public void LED_GetBrightness(int dev, byte Address, string Host, ushort port)
		{
			DLL_LED_GetBrightness(dev, Address ,Host ,port);
		}

		#endregion

		#region 设置显示屏亮度
		//void (_stdcall *LED_SetBrightness)(long dev, BYTE Address, char *Host, WORD port, int Brightness);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_SetBrightness")]
		public static extern void DLL_LED_SetBrightness(int dev, byte Address, string Host, ushort port, int Brightness);

		/// <summary>
		/// 设置显示屏亮度
		/// </summary>
		/// <param name="dev"></param>
		/// <param name="Address"></param>
		/// <param name="Host"></param>
		/// <param name="port"></param>
		/// <param name="Brightness"></param>
		public void LED_SetBrightness(int dev, byte Address, string Host, ushort port, int Brightness)
		{
			DLL_LED_SetBrightness(dev, Address ,Host ,port, Brightness);
		}

		#endregion

		#region 获取消息
		//void (_stdcall *LED_GetNotifyMessage)(PNotifyMessage Notify);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_GetNotifyMessage")]
		public static extern void DLL_LED_GetNotifyMessage(ref NotifyMessage Notify);
		public void LED_GetNotifyMessage(ref NotifyMessage Notify)
		{
			DLL_LED_GetNotifyMessage(ref Notify);
		}
		#endregion

        #region 获取某个设备的消息
        //long (_stdcall *LED_GetDeviceNotifyMessage)(long dev, PNotifyMessage Notify);
        [DllImport("LEDSender.DLL", EntryPoint = "LED_GetDeviceNotifyMessage")]
        public static extern int DLL_LED_GetDeviceNotifyMessage(int dev, ref NotifyMessage Notify);
        public int LED_GetDeviceNotifyMessage(int dev, ref NotifyMessage Notify)
        {
            return DLL_LED_GetDeviceNotifyMessage(dev, ref Notify);
        }
        #endregion
        
        #region 得取选项
		//long (_stdcall *LED_GetOption)(int Index)
		[DllImport("LEDSender.DLL", EntryPoint = "LED_GetOption")]
		public static extern int DLL_LED_GetOption(int Index);
		public void LED_GetOption(int Index)
		{
			DLL_LED_GetOption(Index);
		}
		#endregion

		#region 设置选项
		//long (_stdcall *LED_SetOption)(int Index, DWORD Value);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_SetOption")]
		public static extern int DLL_LED_SetOption(int Index, uint Value);
		public void LED_SetOption(int Index, uint Value)
		{
			DLL_LED_SetOption(Index, Value);
		}
		#endregion

		#region 开始形成显示屏数据
		//long (_stdcall *MakeRoot)(long RootType, long ScreenType);
		[DllImport("LEDSender.DLL", EntryPoint="MakeRoot")]
		public static extern int DLL_MakeRoot(int RootType, int ScreenType);

		/// <summary>
		/// 开始形成显示屏数据
		/// </summary>
		/// <param name="RootType">ROOT_PLAY 仅用于播放, ROOT_DOWNLOAD 下载并播放，无特殊必要，请不要使用下载</param>
		/// <param name="ScreenType">显示屏类型 SCREEN_UNICOLOR 单色显示屏, SCREEN_COLOR 双色显示屏</param>
		/// <returns></returns>
		public int MakeRoot(eRootType RootType, eScreenType ScreenType)
		{
			return DLL_MakeRoot((int)RootType, (int)ScreenType);
		}
		#endregion

		#region 增加页面，并指定显示时间
		//long (_stdcall *AddLeaf)(long DisplayTime); 
		[DllImport("LEDSender.DLL", EntryPoint="AddLeaf")]
		public static extern int DLL_AddLeaf(int DisplayTime);

		/// <summary>
		/// 增加页面，并指定显示时间
		/// </summary>
		/// <param name="DisplayTime">页面显示时间，单位为毫秒(ms)</param>
		/// <returns></returns>
		public int AddLeaf(int DisplayTime)
		{			
			return DLL_AddLeaf(DisplayTime);
		}
		#endregion

		#region 在当前显示页面上创建一个显示区域，显示内容来自于dc
		//long (_stdcall *AddWindow)(HDC dc,short width, short height, LPRECT rect, long method, long speed, long transparent);
		[DllImport("LEDSender.DLL", EntryPoint="AddWindow")]
		public static extern int DLL_AddWindow(int dc, short width, short height, ref RECT rect, int method, int speed,int transparent);

		/// <summary>
		/// 在当前显示页面上创建一个显示区域，显示内容来自于dc
		/// </summary>
		/// <param name="dc">设备句柄</param>
		/// <param name="width">截取的宽度</param>
		/// <param name="height">截取的高度</param>
		/// <param name="rect">显示区域</param>
		/// <param name="method">显示方式</param>
		/// <param name="speed">显示速度</param>
		/// <param name="transparent">是否透明</param>
		/// <returns></returns>
		public int AddWindow(int dc, short width, short height, ref RECT rect, int method, int speed,int transparent)
		{
			return DLL_AddWindow(dc, width, height, ref rect, method, speed,transparent);
		}
		#endregion

        #region 在当前显示页面上创建一个显示区域，显示内容来自于图片文件
        //long (_stdcall *AddPicture)(char *filename, LPRECT rect, long method, long speed, long transparent, long stretch);
        [DllImport("LEDSender.DLL", EntryPoint = "AddPicture")]
        public static extern int DLL_AddPicture(string filename, ref RECT rect, int method, int speed, int transparent, int stretch);

        /// <summary>
        /// 在当前显示页面上创建一个显示区域，显示内容来自于dc
        /// </summary>
        /// <param name="filemane">图片文件名</param>
        /// <param name="rect">显示区域</param>
        /// <param name="method">显示方式</param>
        /// <param name="speed">显示速度</param>
        /// <param name="transparent">是否透明</param>
        /// <param name="stretch">是否按照显示区域拉伸</param>
        /// <returns></returns>
        public int AddPicture(string filename, ref RECT rect, int method, int speed, int transparent, int stretch)
        {
            return DLL_AddPicture(filename, ref rect, method, speed, transparent, stretch);
        }
        #endregion
        
        #region  在当前页面创建一个数字时钟
		//long (_stdcall *AddDateTime)(LPRECT rect, long transparent, char *fontname, long fontsize, long fontcolor, long format, long fontstyle);
		[DllImport("LEDSender.DLL", EntryPoint="AddDateTime")]
		public static extern int DLL_AddDateTime(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, int fontstyle);
		/// <summary>
		/// 在当前页面创建一个数字时钟
		/// </summary>
		/// <param name="rect">显示区域</param>
		/// <param name="transparent">是否透明</param>
		/// <param name="fontname">字体名</param>
		/// <param name="fontsize">字体大小</param>
		/// <param name="fontcolor">字体颜色</param>
		/// <param name="format">时钟格式</param>
		/// <returns></returns>
		public int AddDateTime(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, eTimeFormat format, int fontstyle)
		{
			return DLL_AddDateTime(ref rect, transparent, fontname, fontsize, fontcolor, (int)format, fontstyle);
		}
		#endregion

		#region  显示内码汉字
		//long (_stdcall *AddString)(char *str, LPRECT rect, long method, long speed, long transparent, long fontset, long fontcolor);
		[DllImport("LEDSender.DLL", EntryPoint="AddString")]
		public static extern int DLL_AddString(string str, ref RECT rect, int method, int speed, int transparent,int fontset, int fontcolor);
		
		public int AddString(string str, ref RECT rect, int method, int speed, int transparent,int fontset, int fontcolor)
		{
			return DLL_AddString(str, ref rect, method, speed, transparent, fontset, fontcolor);
		}
		#endregion

		#region 显示windows汉字
        //long (_stdcall *AddText)(char *str, LPRECT rect, long method, long speed, long transparent, char *fontname, long fontsize, long fontcolor, long fontstyle = 0);
		[DllImport("LEDSender.DLL", EntryPoint="AddText")]
		public static extern int DLL_AddText(string str, ref RECT rect, int method, int speed, int transparent,string fontname, int fontsize, int fontcolor, int fontstyle);

		public int AddText(string str, ref RECT rect, int method, int speed, int transparent,string fontname, int fontsize, int fontcolor, int fontstyle)
		{
			return DLL_AddText(str, ref rect, method, speed, transparent,fontname, fontsize, fontcolor, fontstyle);
		}
		#endregion

        #region 显示动画
		//long (_stdcall *AddMovie)(char *filename, LPRECT rect, long stretch);
		[DllImport("LEDSender.DLL", EntryPoint="AddMovie")]
		public static extern int DLL_AddMovie(string filename, ref RECT rect, int stretch);
		
		public int AddMovie(string filename, ref RECT rect, int stretch)
		{
			return DLL_AddMovie(filename, ref rect, stretch);
		}
		#endregion

		#region 当前页面创建一个正计时显示区域
		//long (_stdcall *AddCountUp)(LPRECT rect, long transparent, char *fontname, long fontsize, long fontcolor, long format, LPSYSTEMTIME starttime);
		[DllImport("LEDSender.DLL", EntryPoint="AddCountUp")]
		public static extern int DLL_AddCountUp(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, ref SYSTEMTIME starttime);
		public int AddCountUp(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, ref SYSTEMTIME starttime)
		{
			return DLL_AddCountUp(ref rect, transparent, fontname, fontsize, fontcolor, format, ref starttime);
		}
		#endregion

		#region 在当前显示页面创建一个倒计时显示区域
		//long (_stdcall *AddCountDown)(LPRECT rect,long transparent, char *fontname, long fontsize, long fontcolor, long format, LPSYSTEMTIME endtime);
		[DllImport("LEDSender.DLL", EntryPoint="AddCountDown")]
		public static extern int DLL_AddCountDown(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, ref SYSTEMTIME endtime);
		public int AddCountDown(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, ref SYSTEMTIME endtime)
		{
			return DLL_AddCountDown(ref rect, transparent, fontname, fontsize, fontcolor, format, ref endtime);
		}
		#endregion

		#region 在当前显示页面创建一个模拟时钟
        //long (_stdcall *AddClock)(LPRECT rect, long transparent, long WidthH, long WidthM, long DotH, long DotM, DWORD ColorH, DWORD ColorM, DWORD ColorS, DWORD ColorD, DWORD ColorN);
        [DllImport("LEDSender.DLL", EntryPoint = "AddClock")]
        public static extern int DLL_AddClock(ref RECT rect, int transparent, int WidthH, int WidthM, int DotH, int DotM, uint ColorH, uint ColorM, uint ColorS, uint ColorD, uint ColorN);
        /// <summary>
        /// 在当前显示页面上创建一个显示区域，显示内容来自于dc
        /// </summary>
        /// <param name="rect">显示区域</param>
        /// <param name="transparent">是否透明</param>
        /// <param name="DotM">整点半径</param>
        /// <param name="DotH">3,6,9点半径</param>
        /// <param name="ColorH">小时指针颜色</param>
        /// <param name="ColorM">分钟指针颜色</param>
        /// <param name="ColorS">秒指针颜色</param>
        /// <param name="ColorD">3,6,9点颜色</param>
        /// <param name="ColorN">整点颜色</param>
        /// <param name="WidthH">小时指针宽度</param>
        /// <param name="WidthM">分钟指针宽度</param>
        /// <returns></returns>
        public int AddClock(ref RECT rect, int transparent, int WidthH, int WidthM, int DotH, int DotM, uint ColorH, uint ColorM, uint ColorS, uint ColorD, uint ColorN)
		{
			return DLL_AddClock(ref rect, transparent, WidthH, WidthM, DotH, DotM, ColorH, ColorM, ColorS, ColorD, ColorN);
		}
		#endregion
        [DllImport("kernel32.dll")]
        public static extern void GetLocalTime(ref SYSTEMTIME lpSystemTime); 
	}
}
