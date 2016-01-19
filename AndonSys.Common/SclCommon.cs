using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LabLed.Components
{
    public static class SclCommon
    {
        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_NetInitial")]
        public static extern bool DLL_SCL_NetInitial(ushort nDevID, string Password, string RemoteIP, int SecTimeOut, int Retry, ushort UDPPort, bool bSCL2008);

        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_Close")]
        public static extern bool DLL_SCL_Close(ushort nDevID);

        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_SendData")]
        public static extern bool DLL_SCL_SendData(ushort DevID, int Offset, int SendBytes, IntPtr Buff);

        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_SaveFile")]
        public static extern bool DLL_SCL_SaveFile(ushort DevID, ushort DrvNo, string FileName, int Length, int Da, int Ti);

        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_SendFile")]
        public static extern bool DLL_SCL_SendFile(ushort DevID,int DrvNo,string Path,string FileName);

        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_Replay")]
        public static extern bool DLL_SCL_Replay(ushort DevID, int Drv, int Index);

        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_PictToXMPFile")]
        public static extern bool DLL_SCL_PictToXMPFile(int ColorType, int W, int H, bool bStretched, string PictFile, string XMPFile);

        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_ShowString")]
        public static extern bool DLL_SCL_ShowString(ushort DevID, int PTextInfo, string Str);

        [DllImport("SCL_API_stdcall.dll", EntryPoint = "SCL_SetExtSW")]
        public static extern bool SCL_SetExtSW(ushort nDevID, ushort OnOff);


        public struct TextInfoType
        {
            public short Left;                          
            public short Top;                           
            public short Width;                       
            public short Height;                      
            public int Color;                       
            public short ASCFont;                    
            public short HZFont;                     
            public short XPos;                     
            public short YPos;                       
        }

    }
}
