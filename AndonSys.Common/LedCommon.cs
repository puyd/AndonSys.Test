//===============================================================================
//
//	LabLed
//  ��� --  LED��ʾ�� ���  
//  
//   ----- ���ڶԻ���LED��ʾ������
//         
//===============================================================================
//
// Copyright (C) 2002-2007 ��������ʱ�����������
// �������е�Ȩ��.
// 
// ��������: 2007-12-10
// �� �� ��: Liushiying (lsy@sogou.com)
// ����˵��: LED��ʾ��������
// �޸�����: 2007-12-10
// �� �� ��: Liushiying (lsy@sogou.com)
// �޸�˵��:  
//
//===============================================================================

namespace LabLed.Components
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	/// <summary>
	/// LED��ʾ��������
	/// </summary>
	public class LedCommon : IDisposable
	{

		#region ������ö�ٶ���
		/// <summary>
		/// ͨѶ��ʽ����
		/// </summary>
		public enum eDevType 
		{
			DEV_COM = 0,                //����ͨѶ    0
			DEV_UDP,	                //UDPͨѶ     1
			DEV_MODEM		            //ModemͨѶ   2
		}
		/// <summary>
		/// �˿ڳ���
		/// </summary>
		public enum eCommPort
		{
			Address = 0,                //�����ַ
			ComPort = 4                //��COM4��ΪͨѶ�˿�
		}

		/// <summary>
		/// ����ͨѶ�ٶ�
		/// </summary>
		public enum eBaudRate
		{
			SBR_9600 = 0,               //��������9600   0
			SBR_14400,                  //��������14400  1
			SBR_19200,                  //��������19200  2
			SBR_38400,                  //��������38400  3
			SBR_57600,                  //��������57600  4
			SBR_115200                  //��������115200 5
		}

		/// <summary>
		/// ��������
		/// </summary>
		public enum eRootType
		{
			ROOT_PLAY = 17,             //�·���ĿΪ�������� 17
			ROOT_DOWNLOAD				//�·���ĿΪ���沢���� 18
		}

		/// <summary>
		/// ��ʾ�����ͳ���
		/// </summary>
		public enum eScreenType
		{
			SCREEN_UNICOLOR = 1,        //��ɫ��ʾ�� 1
			SCREEN_COLOR,	            //˫ɫ��ʾ�� 2
			SCREEN_FULLCOLOR,	        //ȫ��ɫ��ʾ�� 3
			SCREEN_GRAY		            //256���Ҷ��� 4
		}

		/// <summary>
		/// ��Ӧ��Ϣ����
		/// </summary>
		public enum eResponseMessage
		{
			LM_RX_COMPLETE = 1,          //���ս��� 1
			LM_TX_COMPLETE,		         //���ͽ��� 2
			LM_RESPOND,		             //�յ�Ӧ�� 3
			LM_TIMEOUT,		             //��ʱ 4
			LM_NOTIFY,		             //֪ͨ��Ϣ 5
			LM_PARAM,		
			LM_TX_PROGRESS,		         //������ 7
			LM_RX_PROGRESS		         //������ 8
		}

		/// <summary>
		/// ��Դ״̬����
		/// </summary>
		public enum ePowerStatus
		{
			LED_POWER_OFF = 0,          //��ʾ����Դ�ѹر� 0
			LED_POWER_ON	            //��ʾ����Դ�� 1
		}

		//ʱ���ʽ����
		public enum eTimeFormat
		{
			DF_YMD = 1,                 //  1 ������  "2004��12��31��"
			DF_HN,		                //  2 ʱ��    "19:20"
			DF_HNS,		                //  3 ʱ����  "19:20:30"
			DF_Y,		                //  4 ��      "2004"
			DF_M,		                //  5 ��      "12" "01" ע�⣺ʼ����ʾ��λ����
			DF_D,				        //  6 ��
			DF_H,	                    //  7 ʱ
			DF_N,			            //  8 ��
			DF_S,		                //  9 ��
			DF_W		                // 10 ����    "������"
		}

		/// <summary>
		/// ����ʱ������ʱformat����
		/// </summary>
		public enum eCountType
		{
			CF_DAY = 0,                 // 0 ����
			CF_HOUR,					// 1 Сʱ��
			CF_HMS,						// 2 ʱ����
			CF_HM,						// 3 ʱ��
			CF_MS,						// 4 ����
			CF_S						// 5 ��
		}

        public const int NOTIFY_EVENT = 1;
        public const int NOTIFY_BLOCK = 2;

        public const int FONT_SET_16 = 0;            //16�����ַ�
		public const int FONT_SET_24 = 1;            //24�����ַ�

		public const int PKC_QUERY = 4;
		public const int PKC_ADJUST_TIME = 6;
		public const int PKC_GET_POWER = 9;
		public const int PKC_SET_POWER = 10;
		public const int PKC_GET_BRIGHT = 11;
		public const int PKC_SET_BRIGHT = 12;
		#endregion 

		#region �ṹ�嶨��
		/// <summary>
		/// �ṹ�嶨��
		/// </summary>

		//�豸����
		public struct DEVICEPARAM
		{
			public uint devType;                      //ͨѶ�豸����
			public uint Speed;                        //ͨѶ�ٶ�(���Դ���ͨѶ����)
			public uint ComPort;
			public uint FlowCon;
			public uint locPort;                      //���ض˿�(�Դ���ͨѶΪ�����ںţ���UDPͨѶΪ�����ض˿ںţ�һ��Ҫ����1024)
			public uint rmtPort;                      //Զ�̶˿ں�(��UDPͨѶ���ã�����Ϊ6666)
			public uint memory;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Phone;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] Reserved;
		}

		//��ʾ������
		public struct RECT
		{            
			public int left;   //��            
			public int top;    //��            
			public int right;  //��            
			public int bottom; //��
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
		/// API���
		/// </summary>
		
		public LedCommon()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		#region ���� Dispose �ͷ���Դ
		/// <summary>
		/// �ͷ���Դ
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

		#region ��ͨѶ�ŵ�
        //long (_stdcall *LED_Open)(const PDeviceParam param, long Notify, long Window, long Message);
		[DllImport("LEDSender.DLL", EntryPoint="LED_Open")]
		public static extern int DLL_LED_Open(ref DEVICEPARAM param,int Notify, int Window, int Message);

		/// <summary>
		/// ��ͨѶ�ŵ�
		/// </summary>
		/// <param name="param">DEVICEPARAM�ṹ���豸����</param>
		/// <param name="Notify">�Ƿ����֪ͨ��Ϣ 0:������ 1:����</param>
		/// <param name="Window">����֪ͨ��Ϣ�Ĵ��ھ��</param>
		/// <param name="Message">��Ϣ��ʶ</param>
		/// <returns></returns>
		public int LED_Open(ref DEVICEPARAM param, int Notify, int Window, int Message)
		{
			return DLL_LED_Open(ref param, Notify, Window, Message);			
		}
		#endregion

		#region �ر�ͨѶ�ŵ�

		//void (_stdcall *LED_Close)(long dev);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_Close")]
		public static extern void DLL_LED_Close(int dev);
		/// <summary>
		/// �ر��Ѵ򿪵�ͨѶ�豸
		/// </summary>
		/// <param name="dev">LED_Open�����ķ���ֵ</param>
		public void LED_Close(int dev)
		{
			System.Threading.Thread.Sleep(100);
			DLL_LED_Close(dev);
		}
		#endregion 

        #region �ر�����ͨѶ�ŵ�

        //void (_stdcall *LED_CloseAll)();
        [DllImport("LEDSender.DLL", EntryPoint = "LED_CloseAll")]
        public static extern void DLL_LED_CloseAll();
        /// <summary>
        /// �ر��Ѵ򿪵�ͨѶ�豸
        /// </summary>
        public void LED_CloseAll()
        {
            System.Threading.Thread.Sleep(100);
            DLL_LED_CloseAll();
        }
        #endregion

        #region ��ѯĳ��ͨѶ�ŵ���״̬

        //long (_stdcall *LED_GetDeviceStatus)(long dev);
        [DllImport("LEDSender.DLL", EntryPoint = "LED_GetDeviceStatus")]
        public static extern int DLL_LED_GetDeviceStatus(int dev);
        /// <summary>
        /// ��ѯĳ��ͨѶ�ŵ���״̬
        /// </summary>
        /// <param name="dev">LED_Open�����ķ���ֵ</param>
        /// <return>ͨѶ״̬ 0:���ŵ����У�����ͨѶ 1:�ŵ�����ͨѶ�� -1:�ŵ�δ�򿪻��ߴ򿪴���</return>
        public int LED_GetDeviceStatus(int dev)
        {
            return DLL_LED_GetDeviceStatus(dev);
        }
        #endregion

        #region ��ѯ��ʾ���Ƿ��ܹ�ͨѶ
		//void (_stdcall *LED_Query)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint="LED_Query")]
		public static extern void DLL_LED_Query(int dev, byte Address, string Host, ushort port);
		/// <summary>
		/// ��ѯ��ʾ���Ƿ��ܹ�ͨѶ
		/// </summary>
		/// <param name="dev">�ò�����LED_Open�����ķ���ֵ</param>
		/// <param name="Address"></param>
		/// <param name="Host">��ʾ��IP��ַ (����UDP��Ч);����ͨѶ����д�����ַ���߿�</param>
		/// <param name="port">��ʾ���˿ں�(�����UDPͨѶ���ö˿�Ϊ6666)</param>
		public void LED_Query(int dev, byte Address, string Host, ushort port)
		{
			DLL_LED_Query(dev, Address, Host, port);
		}
		#endregion 

		#region �ü����ʱ��У����ʾ����ʱ��
		//void (_stdcall *LED_AdjustTime)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint="LED_AdjustTime")]
		public static extern void DLL_LED_AdjustTime(int dev, byte Address, string Host, ushort port);
		/// <summary>
		/// �����ʱ��У����ʾ����ʱ��
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

		#region �������ݵ���ʾ��
		//void (_stdcall *LED_SendToScreen)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint="LED_SendToScreen")]
		public static extern void DLL_LED_SendToScreen(int dev, byte Address, string Host, ushort port);

		/// <summary>
		/// ���γɵĽ�Ŀ���ݷ��͵���ʾ��
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

		#region ��ȡ��Դ״̬
		//void (_stdcall *LED_GetPower)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint="LED_GetPower")]
		public static extern void DLL_LED_GetPower(int dev, byte Address, string Host, ushort port);
		/// <summary>
		/// ��ȡ��Դ״̬
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

		#region ������ʾ����Դ
		//void (_stdcall *LED_SetPower)(long dev, BYTE Address, char *Host, WORD port, DWORD Power);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_SetPower")]
		public static extern void DLL_LED_SetPower(int dev, byte Address, string Host, ushort port, uint Power);

		/// <summary>
		/// �򿪻�ر���ʾ����Դ
		/// </summary>
		/// <param name="dev"></param>
		/// <param name="Address"></param>
		/// <param name="Host"></param>
		/// <param name="port"></param>
		/// <param name="Power">LED_POWER_ON �򿪵�Դ, LED_POWER_OFF �رյ�Դ</param>		
		public void LED_SetPower(int dev, byte Address, string Host, ushort port, ePowerStatus Power)
		{
			DLL_LED_SetPower(dev, Address ,Host ,port , (uint)Power);
		}
		#endregion

		#region ��ȡ��ʾ������
		//void (_stdcall *LED_GetBrightness)(long dev, BYTE Address, char *Host, WORD port);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_GetBrightness")]
		public static extern void DLL_LED_GetBrightness(int dev, byte Address, string Host, ushort port);

		/// <summary>
		/// ��ȡ��ʾ������
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

		#region ������ʾ������
		//void (_stdcall *LED_SetBrightness)(long dev, BYTE Address, char *Host, WORD port, int Brightness);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_SetBrightness")]
		public static extern void DLL_LED_SetBrightness(int dev, byte Address, string Host, ushort port, int Brightness);

		/// <summary>
		/// ������ʾ������
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

		#region ��ȡ��Ϣ
		//void (_stdcall *LED_GetNotifyMessage)(PNotifyMessage Notify);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_GetNotifyMessage")]
		public static extern void DLL_LED_GetNotifyMessage(ref NotifyMessage Notify);
		public void LED_GetNotifyMessage(ref NotifyMessage Notify)
		{
			DLL_LED_GetNotifyMessage(ref Notify);
		}
		#endregion

        #region ��ȡĳ���豸����Ϣ
        //long (_stdcall *LED_GetDeviceNotifyMessage)(long dev, PNotifyMessage Notify);
        [DllImport("LEDSender.DLL", EntryPoint = "LED_GetDeviceNotifyMessage")]
        public static extern int DLL_LED_GetDeviceNotifyMessage(int dev, ref NotifyMessage Notify);
        public int LED_GetDeviceNotifyMessage(int dev, ref NotifyMessage Notify)
        {
            return DLL_LED_GetDeviceNotifyMessage(dev, ref Notify);
        }
        #endregion
        
        #region ��ȡѡ��
		//long (_stdcall *LED_GetOption)(int Index)
		[DllImport("LEDSender.DLL", EntryPoint = "LED_GetOption")]
		public static extern int DLL_LED_GetOption(int Index);
		public void LED_GetOption(int Index)
		{
			DLL_LED_GetOption(Index);
		}
		#endregion

		#region ����ѡ��
		//long (_stdcall *LED_SetOption)(int Index, DWORD Value);
		[DllImport("LEDSender.DLL", EntryPoint = "LED_SetOption")]
		public static extern int DLL_LED_SetOption(int Index, uint Value);
		public void LED_SetOption(int Index, uint Value)
		{
			DLL_LED_SetOption(Index, Value);
		}
		#endregion

		#region ��ʼ�γ���ʾ������
		//long (_stdcall *MakeRoot)(long RootType, long ScreenType);
		[DllImport("LEDSender.DLL", EntryPoint="MakeRoot")]
		public static extern int DLL_MakeRoot(int RootType, int ScreenType);

		/// <summary>
		/// ��ʼ�γ���ʾ������
		/// </summary>
		/// <param name="RootType">ROOT_PLAY �����ڲ���, ROOT_DOWNLOAD ���ز����ţ��������Ҫ���벻Ҫʹ������</param>
		/// <param name="ScreenType">��ʾ������ SCREEN_UNICOLOR ��ɫ��ʾ��, SCREEN_COLOR ˫ɫ��ʾ��</param>
		/// <returns></returns>
		public int MakeRoot(eRootType RootType, eScreenType ScreenType)
		{
			return DLL_MakeRoot((int)RootType, (int)ScreenType);
		}
		#endregion

		#region ����ҳ�棬��ָ����ʾʱ��
		//long (_stdcall *AddLeaf)(long DisplayTime); 
		[DllImport("LEDSender.DLL", EntryPoint="AddLeaf")]
		public static extern int DLL_AddLeaf(int DisplayTime);

		/// <summary>
		/// ����ҳ�棬��ָ����ʾʱ��
		/// </summary>
		/// <param name="DisplayTime">ҳ����ʾʱ�䣬��λΪ����(ms)</param>
		/// <returns></returns>
		public int AddLeaf(int DisplayTime)
		{			
			return DLL_AddLeaf(DisplayTime);
		}
		#endregion

		#region �ڵ�ǰ��ʾҳ���ϴ���һ����ʾ������ʾ����������dc
		//long (_stdcall *AddWindow)(HDC dc,short width, short height, LPRECT rect, long method, long speed, long transparent);
		[DllImport("LEDSender.DLL", EntryPoint="AddWindow")]
		public static extern int DLL_AddWindow(int dc, short width, short height, ref RECT rect, int method, int speed,int transparent);

		/// <summary>
		/// �ڵ�ǰ��ʾҳ���ϴ���һ����ʾ������ʾ����������dc
		/// </summary>
		/// <param name="dc">�豸���</param>
		/// <param name="width">��ȡ�Ŀ��</param>
		/// <param name="height">��ȡ�ĸ߶�</param>
		/// <param name="rect">��ʾ����</param>
		/// <param name="method">��ʾ��ʽ</param>
		/// <param name="speed">��ʾ�ٶ�</param>
		/// <param name="transparent">�Ƿ�͸��</param>
		/// <returns></returns>
		public int AddWindow(int dc, short width, short height, ref RECT rect, int method, int speed,int transparent)
		{
			return DLL_AddWindow(dc, width, height, ref rect, method, speed,transparent);
		}
		#endregion

        #region �ڵ�ǰ��ʾҳ���ϴ���һ����ʾ������ʾ����������ͼƬ�ļ�
        //long (_stdcall *AddPicture)(char *filename, LPRECT rect, long method, long speed, long transparent, long stretch);
        [DllImport("LEDSender.DLL", EntryPoint = "AddPicture")]
        public static extern int DLL_AddPicture(string filename, ref RECT rect, int method, int speed, int transparent, int stretch);

        /// <summary>
        /// �ڵ�ǰ��ʾҳ���ϴ���һ����ʾ������ʾ����������dc
        /// </summary>
        /// <param name="filemane">ͼƬ�ļ���</param>
        /// <param name="rect">��ʾ����</param>
        /// <param name="method">��ʾ��ʽ</param>
        /// <param name="speed">��ʾ�ٶ�</param>
        /// <param name="transparent">�Ƿ�͸��</param>
        /// <param name="stretch">�Ƿ�����ʾ��������</param>
        /// <returns></returns>
        public int AddPicture(string filename, ref RECT rect, int method, int speed, int transparent, int stretch)
        {
            return DLL_AddPicture(filename, ref rect, method, speed, transparent, stretch);
        }
        #endregion
        
        #region  �ڵ�ǰҳ�洴��һ������ʱ��
		//long (_stdcall *AddDateTime)(LPRECT rect, long transparent, char *fontname, long fontsize, long fontcolor, long format, long fontstyle);
		[DllImport("LEDSender.DLL", EntryPoint="AddDateTime")]
		public static extern int DLL_AddDateTime(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, int fontstyle);
		/// <summary>
		/// �ڵ�ǰҳ�洴��һ������ʱ��
		/// </summary>
		/// <param name="rect">��ʾ����</param>
		/// <param name="transparent">�Ƿ�͸��</param>
		/// <param name="fontname">������</param>
		/// <param name="fontsize">�����С</param>
		/// <param name="fontcolor">������ɫ</param>
		/// <param name="format">ʱ�Ӹ�ʽ</param>
		/// <returns></returns>
		public int AddDateTime(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, eTimeFormat format, int fontstyle)
		{
			return DLL_AddDateTime(ref rect, transparent, fontname, fontsize, fontcolor, (int)format, fontstyle);
		}
		#endregion

		#region  ��ʾ���뺺��
		//long (_stdcall *AddString)(char *str, LPRECT rect, long method, long speed, long transparent, long fontset, long fontcolor);
		[DllImport("LEDSender.DLL", EntryPoint="AddString")]
		public static extern int DLL_AddString(string str, ref RECT rect, int method, int speed, int transparent,int fontset, int fontcolor);
		
		public int AddString(string str, ref RECT rect, int method, int speed, int transparent,int fontset, int fontcolor)
		{
			return DLL_AddString(str, ref rect, method, speed, transparent, fontset, fontcolor);
		}
		#endregion

		#region ��ʾwindows����
        //long (_stdcall *AddText)(char *str, LPRECT rect, long method, long speed, long transparent, char *fontname, long fontsize, long fontcolor, long fontstyle = 0);
		[DllImport("LEDSender.DLL", EntryPoint="AddText")]
		public static extern int DLL_AddText(string str, ref RECT rect, int method, int speed, int transparent,string fontname, int fontsize, int fontcolor, int fontstyle);

		public int AddText(string str, ref RECT rect, int method, int speed, int transparent,string fontname, int fontsize, int fontcolor, int fontstyle)
		{
			return DLL_AddText(str, ref rect, method, speed, transparent,fontname, fontsize, fontcolor, fontstyle);
		}
		#endregion

        #region ��ʾ����
		//long (_stdcall *AddMovie)(char *filename, LPRECT rect, long stretch);
		[DllImport("LEDSender.DLL", EntryPoint="AddMovie")]
		public static extern int DLL_AddMovie(string filename, ref RECT rect, int stretch);
		
		public int AddMovie(string filename, ref RECT rect, int stretch)
		{
			return DLL_AddMovie(filename, ref rect, stretch);
		}
		#endregion

		#region ��ǰҳ�洴��һ������ʱ��ʾ����
		//long (_stdcall *AddCountUp)(LPRECT rect, long transparent, char *fontname, long fontsize, long fontcolor, long format, LPSYSTEMTIME starttime);
		[DllImport("LEDSender.DLL", EntryPoint="AddCountUp")]
		public static extern int DLL_AddCountUp(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, ref SYSTEMTIME starttime);
		public int AddCountUp(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, ref SYSTEMTIME starttime)
		{
			return DLL_AddCountUp(ref rect, transparent, fontname, fontsize, fontcolor, format, ref starttime);
		}
		#endregion

		#region �ڵ�ǰ��ʾҳ�洴��һ������ʱ��ʾ����
		//long (_stdcall *AddCountDown)(LPRECT rect,long transparent, char *fontname, long fontsize, long fontcolor, long format, LPSYSTEMTIME endtime);
		[DllImport("LEDSender.DLL", EntryPoint="AddCountDown")]
		public static extern int DLL_AddCountDown(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, ref SYSTEMTIME endtime);
		public int AddCountDown(ref RECT rect, int transparent, string fontname, int fontsize, int fontcolor, int format, ref SYSTEMTIME endtime)
		{
			return DLL_AddCountDown(ref rect, transparent, fontname, fontsize, fontcolor, format, ref endtime);
		}
		#endregion

		#region �ڵ�ǰ��ʾҳ�洴��һ��ģ��ʱ��
        //long (_stdcall *AddClock)(LPRECT rect, long transparent, long WidthH, long WidthM, long DotH, long DotM, DWORD ColorH, DWORD ColorM, DWORD ColorS, DWORD ColorD, DWORD ColorN);
        [DllImport("LEDSender.DLL", EntryPoint = "AddClock")]
        public static extern int DLL_AddClock(ref RECT rect, int transparent, int WidthH, int WidthM, int DotH, int DotM, uint ColorH, uint ColorM, uint ColorS, uint ColorD, uint ColorN);
        /// <summary>
        /// �ڵ�ǰ��ʾҳ���ϴ���һ����ʾ������ʾ����������dc
        /// </summary>
        /// <param name="rect">��ʾ����</param>
        /// <param name="transparent">�Ƿ�͸��</param>
        /// <param name="DotM">����뾶</param>
        /// <param name="DotH">3,6,9��뾶</param>
        /// <param name="ColorH">Сʱָ����ɫ</param>
        /// <param name="ColorM">����ָ����ɫ</param>
        /// <param name="ColorS">��ָ����ɫ</param>
        /// <param name="ColorD">3,6,9����ɫ</param>
        /// <param name="ColorN">������ɫ</param>
        /// <param name="WidthH">Сʱָ����</param>
        /// <param name="WidthM">����ָ����</param>
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
