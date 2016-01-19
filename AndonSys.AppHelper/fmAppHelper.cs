using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using System.Threading;
using AndonSys.Common;

namespace AndonSys.AppHelper
{
    public partial class fmAppHelper : Form,IAppHelper
    {
        const int APP_NORMAL = 0;
        const int APP_NOTRUN = 1;
        const int APP_NOTFOUND = 2;

        Mutex AppMutex = null;
        
        DataTable AppList;
        Log log;

        public fmAppHelper()
        {
            InitializeComponent();
            AppHelper.form = this;
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            bool r;
            AppMutex = new Mutex(true, "AndonSys.AppHelper", out r);
            if (!r)
            {
                MessageBox.Show("系统已运行!",this.Text);
                Close();
                return;
            }
            
            CONFIG.Load();

            log = new Log(Application.StartupPath, "AppHelper", Log.DEBUG_LEVEL);

            log.Debug("系统运行");
           
            gdApp.AutoGenerateColumns = false;
           
            LoadApp();
            
            tbApp.Show();

            timer.Enabled = true;
        }

        void LoadApp()
        {
            AppList = new DataTable();
            
            AppList.Columns.Add("App", Type.GetType("System.String"));
            AppList.Columns.Add("Sta", Type.GetType("System.Int32"));
            AppList.Columns.Add("StaText", Type.GetType("System.String"));

            int cnt = CONFIG.GetInt("APPHELPER", "App.Num", 0);

            for (int i = 1; i <= cnt; i++)
            {
                string f = CONFIG.GetText("APPHELPER", "App." + i.ToString());

                AppList.Rows.Add(f,APP_NOTRUN,GetStatusText(APP_NOTRUN));
            }

            gdApp.DataSource = AppList;

            CheckApp();
        }

        int GetAppStatus(int id)
        {
            return (int)AppList.Rows[id]["Sta"];
        }

        string GetStatusText(int status)
        {
            string[] t={
                "正常","未运行","进程无效"    
            };

            return t[status]; 
        }

        private void gdApp_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex<0) return;

            switch (GetAppStatus(e.RowIndex))
            {
                case APP_NORMAL: e.CellStyle.ForeColor = Color.Green;  break;
                default: e.CellStyle.ForeColor = Color.Red;  break;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            CheckApp();
        }

        void CheckApp()
        {
            foreach (DataRow r in AppList.Rows)
            {
                string app = (string)r["App"];

                if (AppHelper.IsProcRun(app))
                {
                    r["Sta"] = APP_NORMAL;
                    r["StaText"] = GetStatusText(APP_NORMAL);
                }
                else if (!File.Exists(app))
                {
                    r["Sta"] = APP_NOTFOUND;
                    r["StaText"] = GetStatusText(APP_NOTFOUND);
                }
                else if (AppHelper.StartProc(app))
                {
                    r["Sta"] = APP_NORMAL;
                    r["StaText"] = GetStatusText(APP_NORMAL);
                }
                else
                {
                    r["Sta"] = APP_NOTRUN;
                    r["StaText"] = GetStatusText(APP_NOTRUN);
                }
            }

        }

        public void TestLog(string log)
        {
            edLog.Text = log;
        }


        #region IAppHelper 成员

        public void Test()
        {
            Debug.WriteLine("Test");
        }

        #endregion
    }
}
