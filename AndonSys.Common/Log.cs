using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AndonSys.Common
{
    public class Log
    {
        public const int DEBUG_LEVEL = 0;
        public const int INFO_LEVEL = 1;
        public const int ERROR_LEVEL = 2;

        string name;
        string path;
        int level;

        public Log(string path,string name,int level)
        {
            this.path = path;
            this.name = name;
            this.level = level;
        }

        public void Info(string s)
        {
            if (level > INFO_LEVEL) return;
        }

        public void Debug(string s)
        {
            if (level > DEBUG_LEVEL) return;

            string t = string.Format("DEBUG [{0}]: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm"), s);
            AddLog(t);
        }

        string fileName
        {
            get {
                return path + "\\Log\\" + DateTime.Now.ToString("yyyyMM")+"\\"+name+".log";
            }
        }

        StreamWriter Open()
        {
            string p = path + "\\Log";

            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
            p = p + "\\" + DateTime.Now.ToString("yyyyMM");
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);

            return new StreamWriter(fileName, true);

        }

        void AddLog(string s)
        {
            lock (this)
            {
                StreamWriter fs = Open();

                fs.WriteLine(s);

                fs.Close();
                fs.Dispose();
            }
        }
    }
}
