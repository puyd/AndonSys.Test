using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AndonSys.Common;

namespace AndonSys.Patch
{
    public partial class fmMain : Form
    {
        public fmMain()
        {
            InitializeComponent();
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            CONFIG.Load();

            edDB.Text = CONFIG.GetText("SQLDB", "ConStr");
        }

        private void btnPatch_Click(object sender, EventArgs e)
        {
            TSQLDB db = new TSQLDB(edDB.Text);

            db.Open();

            db.ExcuteSQL(edSql.Text);

            db.Close();

            MessageBox.Show("OK!");
        }
    }
}
