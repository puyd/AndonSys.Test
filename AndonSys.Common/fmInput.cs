using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AndonSys.Common
{
    public partial class fmInput : Form
    {
        public fmInput()
        {
            InitializeComponent();
        }

        public static bool InputBox(string Title,string Text,ref string Input)
        {
            fmInput fm=new fmInput();

            fm.lbText.Text=Text;
            fm.Text=Title;

            fm.edInput.Text = Input;

            bool bl=fm.ShowDialog()==DialogResult.OK;
            if (bl) Input = fm.edInput.Text;

            fm.Dispose();

            return bl;
        }
    }
}
