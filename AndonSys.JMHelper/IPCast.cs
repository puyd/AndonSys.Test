using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace AndonSys.Common
{
    public static class IPCast
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PlayFile // 播放文件  
        {
            public int fid; // 文件序号（序号小于 0则取全路径）  
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string fname; // 文件名或全路径名 
            public int fvol;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SessionAttr		            // 会话属性
        {
	        public int	    sid;					// 任务ID
	        public int		status;					// 任务状态：0-停止, 1-播放, 2-暂停, 4-关闭
	        public int		type;					// 会话类型：1-终端点播，2-定时任务，3-文件播放
									                //			 4-声卡实时采播，5-双向对讲，6-报警触发任务
	        public int		grade;					        // 任务优先级：0~999，999最大
	        public int		t_play;					// 播放时间（秒）
	        public int		t_total;				// 总时间（秒，采播任务为0）
	        public int		iFile;					// 播放文件序号（采播任务为0）
	        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string   name;			        // 会话名称
        }

        /*-------------------------------------------------------------------------------------------------
        1.1[01] IPCAST_Connect: 连接到服务器
        函数:	BOOL IPCAST_Connect(LPCTSTR ipAddr, LPCTSTR user, LPCTSTR pass);
        参数:	ipAddr:		服务器IP地址
                user:		登录用户名(缺省可以用admin)
                pass：		登录密码(缺省可以用admin)
        返回:	成功：返回TRUE
                失败：返回FALSE
        -------------------------------------------------------------------------------------------------*/        
        [DllImport("IPCast_I.dll",EntryPoint = "IPCAST_Connect")]
        public static extern bool Connect(string ipAddr, string user, string pass);


        /*-------------------------------------------------------------------------------------------------
        1.2[02] IPCAST_DisConnect：断开连接
        函数:	void IPCAST_DisConnect();
        参数:	无
        返回:	无
        -------------------------------------------------------------------------------------------------*/
        [DllImport("IPCast_I.dll", EntryPoint = "IPCAST_DisConnect")]
        public static extern void DisConnect();


        /*-------------------------------------------------------------------------------------------------
        2.1[05] IPCAST_FilePlayStart：创建文件播放
        函数:	int IPCAST_FilePlayStart(PlayFile* pFList[], int fCount, ULONG* pTList, int tCount,
                                         int Grade, int CycMode, int CycCount, int CycTime);
        参数:	pFList:		广播文件列表，指向播放文件结构的数组指针
                            fid：播放的服务器上的文件ID，ID<=0则播放本机文件
                            fname：服务器文件显示名或本机文件全路径名。
                fCount:		广播文件的数目
                pTList:		需要添加到本会话的广播终端列表
                fCount:		需要添加的终端数目
                Grade:		广播等级（0~999，数值越大级别越高）（整数）
                CycMode:	播放模式
                            PLAY_CYC_DAN = 0x0001;			// 单曲播放（即只播放一次）
                            PLAY_CYC_DANCIRCLE = 0x0002;	// 单曲循环播放（循环播放一个曲目）
                            PLAY_CYC_DANORDE = 0x0003;		// 顺序播放（按序播放全部歌曲一次）
                            PLAY_CYC_ALLCIRCLE = 0x0004;	// 循环播放（循环播放所有歌曲）
                CycCount:	循环播放次数。（即要求循环播放多少次，0：表示无限次）
                CycTime:	循环播放时长（只有当CycCount = 0时有效，单位为秒。）
        返回:	大于0: 返回广播会话ID
                -1：会话创建失败
        -------------------------------------------------------------------------------------------------*/
        [DllImport("IPCast_I.dll", EntryPoint = "IPCAST_FilePlayStart")]
        public static extern int FilePlayStart(ref PlayFile[] pFList, int fCount, int[] pTList, int tCount,
										 int Grade, int CycMode, int CycCount, int CycTime);


        public static int FilePlayStart(string file, int CycMode, params int[] term)
        {
            PlayFile f = new PlayFile();

            f.fid = 0;
            f.fvol = 10;

            f.fname = file;


            PlayFile[] FList = new PlayFile[] { f };
            
            int cnt=term.Length;

            return FilePlayStart(ref FList, 1, term, cnt, 500, CycMode, 0, 0);
        }

        /*-------------------------------------------------------------------------------------------------
        2.2[06] IPCAST_FilePlayCtrl：文件播放控制
        函数:	BOOL IPCAST_FilePlayCtrl(ULONG sid, int cmd, int pos)
        参数:	sid:		广播会话ID
                cmd:		控制命令
                            PLAY_CTRL_STOP = 1;		// 停止广播
                            PLAY_CTRL_JUMPFILE = 2;	// 跳转至第pos曲播放
                            PLAY_CTRL_NEXT = 3;		// 跳至下一曲播放
                            PLAY_CTRL_PREV = 4;		// 跳至上一曲播放
                            PLAY_CTRL_PAUSE = 5;	// 暂停播放
                            PLAY_CTRL_RESTORE = 6;	// 恢复播放
                            PLAY_CTRL_JUMPTIME = 7;	// 跳转到当前曲pos秒处位置
                pos:		cmd = 2:	跳转的歌曲序号
                            cmd = 7:	跳转的曲目时间位置
        返回：	TRUE:	成功
                FALSE:	失败
        -------------------------------------------------------------------------------------------------*/
        [DllImport("IPCast_I.dll", EntryPoint = "IPCAST_FilePlayCtrl")]
        public static extern bool FilePlayCtrl(int sid, int cmd, int pos);

        public static bool FilePlayStop(int sid)
        {
            return FilePlayCtrl(sid, 1, 0);
        }


        /*-------------------------------------------------------------------------------------------------
        4.4[15] IPCAST_GetSessionStatus：获取会话状态属性
        函数:	BOOL IPCAST_GetSessionStatus(ULONG sid, LPSessionAttr pSession);
        参数:	sid:		需要查询的会话ID
		        pSession:	返回会话属性的指针
        返回：	TRUE:	成功
		        FALSE:	失败
        -------------------------------------------------------------------------------------------------*/
        [DllImport("IPCast_I.dll", EntryPoint = "IPCAST_GetSessionStatus")]
        public static extern bool GetSessionStatus(int sid, out SessionAttr pSession);

        public static int GetSessionStatus(int sid)
        {
            SessionAttr s;

            if (GetSessionStatus(sid, out s))
            {
                return s.status;
            }
            else
            {
                return -1;
            }


        }

        /*-------------------------------------------------------------------------------------------------
        4.10 IPCAST_RMSession：删除指定会话
        函数:	BOOL IPCAST_RMSession(ULONG sid);
        参数:	sid:		需要删除的会话ID
        返回：	TRUE：	成功
		        FALSE： 失败
        -------------------------------------------------------------------------------------------------*/
        [DllImport("IPCast_I.dll", EntryPoint = "IPCAST_RMSession")]
        public static extern bool RMSession(int sid);

        /*-------------------------------------------------------------------------------------------------
        4.3[14] IPCAST_GetSessionList：获取会话列表
        函数:	int IPCAST_GetSessionList(ULONG* pSList, int nSize);
        参数:	pSList:		保存返回会话ID列表的缓冲区，pSList==NULL或者nSize<=0只返回会话数目
                nSize:		允许返回的会话ID数目
        返回：	会话数目。pSList!=NULL 且 nSize>0 时，相应ID填写到pSList中。
        -------------------------------------------------------------------------------------------------*/
        [DllImport("IPCast_I.dll", EntryPoint = "IPCAST_GetSessionList")]
        static extern int GetSessionList([MarshalAs(UnmanagedType.LPArray)]int[] buf, int nSize);

        public static int[] GetSessionList()
        {

            int[] buf=new int[100];
            int cnt=0;

            cnt= GetSessionList(buf , buf.Length);
            
            if (cnt == 0) return null;

            int[] r = new int[cnt];
            for (int i = 0; i < cnt; i++)
            {
                r[i] = buf[i];
            }

            return r;
        }

    }
}
