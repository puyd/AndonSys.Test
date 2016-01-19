namespace AndonSys.AppHelper
{
    partial class fmAppHelper
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tbAll = new System.Windows.Forms.TabControl();
            this.tbApp = new System.Windows.Forms.TabPage();
            this.gdApp = new System.Windows.Forms.DataGridView();
            this.clmPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmStaText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbLog = new System.Windows.Forms.TabPage();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLog = new System.Windows.Forms.ComboBox();
            this.edLog = new System.Windows.Forms.TextBox();
            this.tbAll.SuspendLayout();
            this.tbApp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gdApp)).BeginInit();
            this.tbLog.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbAll
            // 
            this.tbAll.Controls.Add(this.tbApp);
            this.tbAll.Controls.Add(this.tbLog);
            this.tbAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAll.Location = new System.Drawing.Point(0, 0);
            this.tbAll.Name = "tbAll";
            this.tbAll.SelectedIndex = 0;
            this.tbAll.Size = new System.Drawing.Size(624, 442);
            this.tbAll.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tbAll.TabIndex = 0;
            // 
            // tbApp
            // 
            this.tbApp.Controls.Add(this.gdApp);
            this.tbApp.Location = new System.Drawing.Point(4, 22);
            this.tbApp.Name = "tbApp";
            this.tbApp.Padding = new System.Windows.Forms.Padding(3);
            this.tbApp.Size = new System.Drawing.Size(616, 416);
            this.tbApp.TabIndex = 0;
            this.tbApp.Text = "进程监护";
            this.tbApp.UseVisualStyleBackColor = true;
            // 
            // gdApp
            // 
            this.gdApp.AllowUserToAddRows = false;
            this.gdApp.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.gdApp.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gdApp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gdApp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdApp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmPath,
            this.clmStaText,
            this.clmSta});
            this.gdApp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gdApp.Location = new System.Drawing.Point(3, 3);
            this.gdApp.MultiSelect = false;
            this.gdApp.Name = "gdApp";
            this.gdApp.ReadOnly = true;
            this.gdApp.RowHeadersWidth = 25;
            this.gdApp.RowTemplate.Height = 23;
            this.gdApp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdApp.Size = new System.Drawing.Size(610, 410);
            this.gdApp.TabIndex = 0;
            this.gdApp.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gdApp_CellPainting);
            // 
            // clmPath
            // 
            this.clmPath.DataPropertyName = "App";
            this.clmPath.HeaderText = "进程";
            this.clmPath.Name = "clmPath";
            this.clmPath.ReadOnly = true;
            this.clmPath.Width = 400;
            // 
            // clmStaText
            // 
            this.clmStaText.DataPropertyName = "StaText";
            this.clmStaText.HeaderText = "状态";
            this.clmStaText.Name = "clmStaText";
            this.clmStaText.ReadOnly = true;
            // 
            // clmSta
            // 
            this.clmSta.DataPropertyName = "Sta";
            this.clmSta.HeaderText = "clmSta";
            this.clmSta.Name = "clmSta";
            this.clmSta.ReadOnly = true;
            this.clmSta.Visible = false;
            // 
            // tbLog
            // 
            this.tbLog.Controls.Add(this.splitContainer1);
            this.tbLog.Location = new System.Drawing.Point(4, 22);
            this.tbLog.Name = "tbLog";
            this.tbLog.Padding = new System.Windows.Forms.Padding(3);
            this.tbLog.Size = new System.Drawing.Size(616, 416);
            this.tbLog.TabIndex = 1;
            this.tbLog.Text = "日志记录";
            this.tbLog.UseVisualStyleBackColor = true;
            // 
            // timer
            // 
            this.timer.Interval = 5000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbLog);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.edLog);
            this.splitContainer1.Size = new System.Drawing.Size(610, 410);
            this.splitContainer1.SplitterDistance = 32;
            this.splitContainer1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "日志：";
            // 
            // cbLog
            // 
            this.cbLog.FormattingEnabled = true;
            this.cbLog.Location = new System.Drawing.Point(62, 8);
            this.cbLog.Name = "cbLog";
            this.cbLog.Size = new System.Drawing.Size(184, 20);
            this.cbLog.TabIndex = 1;
            // 
            // edLog
            // 
            this.edLog.BackColor = System.Drawing.Color.White;
            this.edLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edLog.Location = new System.Drawing.Point(0, 0);
            this.edLog.Multiline = true;
            this.edLog.Name = "edLog";
            this.edLog.ReadOnly = true;
            this.edLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.edLog.Size = new System.Drawing.Size(610, 374);
            this.edLog.TabIndex = 0;
            this.edLog.WordWrap = false;
            // 
            // fmAppHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.tbAll);
            this.Name = "fmAppHelper";
            this.Text = "AndonSys.AppHelper";
            this.Load += new System.EventHandler(this.fmMain_Load);
            this.tbAll.ResumeLayout(false);
            this.tbApp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gdApp)).EndInit();
            this.tbLog.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbAll;
        private System.Windows.Forms.TabPage tbApp;
        private System.Windows.Forms.TabPage tbLog;
        private System.Windows.Forms.DataGridView gdApp;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmStaText;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSta;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox cbLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox edLog;
    }
}

