using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using AndonSys.Common;

namespace AndonSys.JMHelper
{
    public partial class fmMain : Form
    {
        DB db = null;

        public fmMain()
        {
            InitializeComponent();
        }

        void Log(string s)
        {
            string log = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}", DateTime.Now, s);
            Debug.WriteLine(log);

            StreamWriter w = new StreamWriter("jm.log", true);
            w.WriteLine(log);
            w.Dispose();
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            CONFIG.Load();

            bool bl = IPCast.Connect("127.0.0.1", "admin", "admin");
            Log(string.Format("IPCast.Connect={0}", bl));

            StopAll();

            string con=CONFIG.GetText("SQLDB","ConStr","");

            db = new DB("System.Data.SqlClient", con);
            db.Open();

            InitClient();

            timer.Enabled = true;
        }

        void StopAll()
        {
            int[] s=null;
            try
            {
                s = IPCast.GetSessionList();

                Log(string.Format("IPCast.GetSessionList()={0}", s == null ? 0 : s.Length));
            }
            catch (Exception e)
            {
                Log(e.Message);
            }

            if (s != null)
            {
                foreach (int sid in s)
                {
                    Stop(sid);
                }

            }
        }

        private void fmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Enabled = false;


            db.Close();
            db.Dispose();

            IPCast.DisConnect();
            Log("IPCast.DisConnect");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DataTable t = LoadClient();

            for (int i = 0; i < t.Rows.Count; i++)
            {
                DataGridViewRow rc = grdView.Rows[i];
                DataRow rt = t.Rows[i];

                Play(rc, rt);
            }

            t.Dispose();
        }

        void Stop(int sid)
        {
            bool bl = false;

            try
            {
                bl = IPCast.FilePlayStop(sid);
                Log(string.Format("IPCast.FilePlayStop({0})={1}", sid, bl));
            }
            catch (Exception e)
            {
                Log(string.Format("IPCast.FilePlayStop({0})={1} (Error:{2})", sid, bl, e.Message));
            }

            bl = false;

            try
            {
                bl = IPCast.RMSession(sid);
                Log(string.Format("IPCast.RMSession({0})={1}", sid, bl));
            }
            catch (Exception e)
            {
                Log(string.Format("IPCast.RMSession({0})={1} (Error:{2})", sid, bl, e.Message));
            }
        }

        void Play(DataGridViewRow rc,DataRow rt)
        {
            int pc = int.Parse(rc.Cells["PlayID"].Value.ToString());
            int pt = int.Parse(rt["A40_PLAY_ID"].ToString());

            if (pc == pt) return;

            int sc = int.Parse(rc.Cells["SID"].Value.ToString());

            if (sc >0) Stop(sc);
            
            int tid = int.Parse(rt["A40_CLIENT_ID"].ToString());
            string mp3 = rt["A41_MP3_FILE"].ToString();
            int mod = int.Parse(rt["A40_PLAY_MODE"].ToString());

            int st = 0;

            if (pt > 0)
            {
                try
                {
                    st = IPCast.FilePlayStart(mp3, mod, tid);
                    Log(string.Format("IPCast.FilePlayStart({0},{1},{2})={3}", mp3, mod, tid, st));
                }
                catch (Exception e)
                {
                    Log(string.Format("IPCast.FilePlayStart({0},{1},{2})={3} (Error:{4})", mp3, mod, tid, st, e.Message));
                }

                int s = 0;

                if (st > 0)
                {
                    try
                    {
                        s = IPCast.GetSessionStatus(st);
                        Log(string.Format("GetSessionStatus({0})={1}", st, s));
                    }
                    catch (Exception e)
                    {
                        Log(string.Format("GetSessionStatus({0})={1} (Error:{2})", st, s, e.Message));
                    }
                }
            }

            rc.Cells["PlayID"].Value = pt;
            rc.Cells["PlayDesc"].Value = rt["A41_PLAY_DESC"];
            rc.Cells["SID"].Value = st;
               
        }

        void InitClient()
        {
            string sql = "SELECT A40_CLIENT_ID,A40_CLIENT_DESC,0 A40_PLAY_ID,'' A41_PLAY_DESC,0 SID FROM A40_CLIENT "+
                "order by A40_CLIENT_ID";

            DataTable t=db.QueryTable(sql);

            grdView.Rows.Clear();

            for (int i = 0; i < t.Rows.Count; i++)
            {
                DataRow r=t.Rows[i];
                grdView.Rows.Add(r["A40_CLIENT_ID"], r["A40_CLIENT_DESC"], r["A41_PLAY_DESC"], r["A40_PLAY_ID"], r["SID"]);
            }

            t.Dispose();
        }

        DataTable LoadClient()
        {
            string sql = "select * from A40_CLIENT left join A41_PLAY on (A40_PLAY_ID=A41_PLAY_ID) order by A40_CLIENT_ID";

            return db.QueryTable(sql);
        }
    }
}
