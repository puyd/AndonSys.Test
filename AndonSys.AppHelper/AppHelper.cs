using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.Runtime.InteropServices; 



namespace AndonSys.AppHelper
{
    public static class AppHelper
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);


        [DllImport("psapi.dll")]
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("psapi.dll")]
        static extern uint GetProcessImageFileName(IntPtr hProcess, [Out] StringBuilder lpImageFileName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, [Out] StringBuilder lpExeName, ref int nSize);

        [DllImport("kernel32.dll")]
        private static extern int GetLogicalDriveStrings(int nBufferLenght, StringBuilder lpBuffer);

        [DllImport("kernel32.dll")]
        static extern int QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);


        
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_READ = 0x0010;

        public static fmAppHelper form = null;

        private static string GetExecutablePath(int dwProcessId) 
        { 
            StringBuilder buffer = new StringBuilder(1024);
            IntPtr hprocess = OpenProcess(PROCESS_QUERY_INFORMATION, 0, dwProcessId); 
            if (hprocess != IntPtr.Zero) {
                try { 
                    int size = buffer.Capacity;
                    if (GetProcessImageFileName(hprocess, buffer, size) > 0) 
                    {
                        return buffer.ToString();
                    } 
                } 
                finally 
                { 
                    CloseHandle(hprocess); 
                } 
            } 
            return string.Empty; 
        }
        
        public static bool IsProcRun(string proc)
        {
            StringBuilder s=new StringBuilder(1024);
            QueryDosDevice(proc.Substring(0, 2), s, s.Capacity);

            string ps = s.ToString() + proc.Substring(2, proc.Length - 2);
            
            foreach (Process p in Process.GetProcesses() )
            {
                try
                {
                    string m = GetExecutablePath(p.Id);
                    //if (p.MainModule!=null) m= p.MainModule.FileName;
                    Debug.WriteLine(p.ProcessName+" "+m);
                    if (m.Equals(ps, StringComparison.CurrentCultureIgnoreCase)) return true;
                }
                catch
                {
                    Debug.WriteLine(p);
                }
            }
            
            return false;
        }

        public static bool StartProc(string proc)
        {
            try
            {
                Process.Start(proc);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
