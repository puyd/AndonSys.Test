namespace AndonSys.Patch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmMain));
            this.label1 = new System.Windows.Forms.Label();
            this.edDB = new System.Windows.Forms.TextBox();
            this.edSql = new System.Windows.Forms.TextBox();
            this.btnPatch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库：";
            // 
            // edDB
            // 
            this.edDB.Location = new System.Drawing.Point(81, 6);
            this.edDB.Name = "edDB";
            this.edDB.Size = new System.Drawing.Size(363, 23);
            this.edDB.TabIndex = 1;
            // 
            // edSql
            // 
            this.edSql.Location = new System.Drawing.Point(15, 35);
            this.edSql.Multiline = true;
            this.edSql.Name = "edSql";
            this.edSql.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.edSql.Size = new System.Drawing.Size(429, 308);
            this.edSql.TabIndex = 2;
            this.edSql.Text = resources.GetString("edSql.Text");
            this.edSql.WordWrap = false;
            // 
            // btnPatch
            // 
            this.btnPatch.Location = new System.Drawing.Point(369, 349);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(75, 23);
            this.btnPatch.TabIndex = 3;
            this.btnPatch.Text = "Patch";
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            // 
            // fmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 385);
            this.Controls.Add(this.btnPatch);
            this.Controls.Add(this.edSql);
            this.Controls.Add(this.edDB);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "fmMain";
            this.Load += new System.EventHandler(this.fmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox edDB;
        private System.Windows.Forms.TextBox edSql;
        private System.Windows.Forms.Button btnPatch;
    }
}

