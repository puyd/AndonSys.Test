namespace AndonSys.LedShow
{
    partial class fmMain
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
            this.tbMain = new System.Windows.Forms.TabControl();
            this.tbXML = new System.Windows.Forms.TabPage();
            this.edXml = new System.Windows.Forms.RichTextBox();
            this.tbDisp = new System.Windows.Forms.TabPage();
            this.pic = new System.Windows.Forms.PictureBox();
            this.tmDisp = new System.Windows.Forms.Timer(this.components);
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbMain.SuspendLayout();
            this.tbXML.SuspendLayout();
            this.tbDisp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbMain
            // 
            this.tbMain.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tbMain.Controls.Add(this.tbXML);
            this.tbMain.Controls.Add(this.tbDisp);
            this.tbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMain.ItemSize = new System.Drawing.Size(120, 18);
            this.tbMain.Location = new System.Drawing.Point(0, 0);
            this.tbMain.Name = "tbMain";
            this.tbMain.SelectedIndex = 0;
            this.tbMain.Size = new System.Drawing.Size(915, 656);
            this.tbMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tbMain.TabIndex = 0;
            // 
            // tbXML
            // 
            this.tbXML.Controls.Add(this.edXml);
            this.tbXML.Location = new System.Drawing.Point(4, 4);
            this.tbXML.Name = "tbXML";
            this.tbXML.Padding = new System.Windows.Forms.Padding(3);
            this.tbXML.Size = new System.Drawing.Size(907, 630);
            this.tbXML.TabIndex = 0;
            this.tbXML.Text = "XML";
            this.tbXML.UseVisualStyleBackColor = true;
            // 
            // edXml
            // 
            this.edXml.AcceptsTab = true;
            this.edXml.ContextMenuStrip = this.contextMenuStrip1;
            this.edXml.DetectUrls = false;
            this.edXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edXml.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.edXml.Location = new System.Drawing.Point(3, 3);
            this.edXml.Name = "edXml";
            this.edXml.Size = new System.Drawing.Size(901, 624);
            this.edXml.TabIndex = 0;
            this.edXml.Text = "";
            this.edXml.WordWrap = false;
            // 
            // tbDisp
            // 
            this.tbDisp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tbDisp.Controls.Add(this.pic);
            this.tbDisp.Location = new System.Drawing.Point(4, 4);
            this.tbDisp.Name = "tbDisp";
            this.tbDisp.Padding = new System.Windows.Forms.Padding(3);
            this.tbDisp.Size = new System.Drawing.Size(907, 630);
            this.tbDisp.TabIndex = 1;
            this.tbDisp.Text = "Display";
            this.tbDisp.UseVisualStyleBackColor = true;
            this.tbDisp.Leave += new System.EventHandler(this.tbDisp_Leave);
            this.tbDisp.Enter += new System.EventHandler(this.tbDisp_Enter);
            // 
            // pic
            // 
            this.pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic.Location = new System.Drawing.Point(3, 3);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(901, 624);
            this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pic.TabIndex = 0;
            this.pic.TabStop = false;
            // 
            // tmDisp
            // 
            this.tmDisp.Interval = 50;
            this.tmDisp.Tick += new System.EventHandler(this.tmDisp_Tick);
            // 
            // dlgSave
            // 
            this.dlgSave.DefaultExt = "xml";
            this.dlgSave.Filter = "XML文件|*.xml";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToXMLToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(150, 26);
            // 
            // saveToXMLToolStripMenuItem
            // 
            this.saveToXMLToolStripMenuItem.Name = "saveToXMLToolStripMenuItem";
            this.saveToXMLToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToXMLToolStripMenuItem.Text = "Save to XML";
            this.saveToXMLToolStripMenuItem.Click += new System.EventHandler(this.saveToXMLToolStripMenuItem_Click);
            // 
            // fmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 656);
            this.Controls.Add(this.tbMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "fmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Led显示";
            this.Load += new System.EventHandler(this.fmMain_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.fmMain_FormClosed);
            this.tbMain.ResumeLayout(false);
            this.tbXML.ResumeLayout(false);
            this.tbDisp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbMain;
        private System.Windows.Forms.TabPage tbXML;
        private System.Windows.Forms.TabPage tbDisp;
        private System.Windows.Forms.RichTextBox edXml;
        private System.Windows.Forms.Timer tmDisp;
        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem saveToXMLToolStripMenuItem;
    }
}

