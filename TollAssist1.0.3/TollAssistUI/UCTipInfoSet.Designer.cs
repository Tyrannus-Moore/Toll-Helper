namespace TollAssistUI
{
    partial class UCTipInfoSet
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.numTipsFontSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTipsPostion = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboResFormVisible = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblLastUpdateTime = new System.Windows.Forms.Label();
            this.lblCurrVersion = new System.Windows.Forms.Label();
            this.cboStartLogoShow = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numTipsFontSize)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // numTipsFontSize
            // 
            this.numTipsFontSize.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.numTipsFontSize.Location = new System.Drawing.Point(167, 7);
            this.numTipsFontSize.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numTipsFontSize.Minimum = new decimal(new int[] {
            18,
            0,
            0,
            0});
            this.numTipsFontSize.Name = "numTipsFontSize";
            this.numTipsFontSize.Size = new System.Drawing.Size(276, 31);
            this.numTipsFontSize.TabIndex = 31;
            this.numTipsFontSize.Tag = "(必填项)设置提示信息字体";
            this.numTipsFontSize.Value = new decimal(new int[] {
            18,
            0,
            0,
            0});
            this.numTipsFontSize.ValueChanged += new System.EventHandler(this.numTipsFontSize_ValueChanged);
            this.numTipsFontSize.MouseClick += new System.Windows.Forms.MouseEventHandler(this.numTipsFontSize_MouseClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label4.Location = new System.Drawing.Point(14, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 24);
            this.label4.TabIndex = 30;
            this.label4.Text = "提示信息字体(*):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label1.Location = new System.Drawing.Point(14, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 24);
            this.label1.TabIndex = 32;
            this.label1.Text = "提示信息坐标(*):";
            // 
            // txtTipsPostion
            // 
            this.txtTipsPostion.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.txtTipsPostion.Location = new System.Drawing.Point(167, 48);
            this.txtTipsPostion.Name = "txtTipsPostion";
            this.txtTipsPostion.ReadOnly = true;
            this.txtTipsPostion.Size = new System.Drawing.Size(276, 31);
            this.txtTipsPostion.TabIndex = 33;
            this.txtTipsPostion.Tag = "(必填项)设置提示信息坐标，请拖动右上角靶标到提示坐标上释放";
            this.txtTipsPostion.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtTipsPostion_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label2.Location = new System.Drawing.Point(34, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 24);
            this.label2.TabIndex = 34;
            this.label2.Text = "资源气泡窗口:";
            // 
            // cboResFormVisible
            // 
            this.cboResFormVisible.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboResFormVisible.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.cboResFormVisible.FormattingEnabled = true;
            this.cboResFormVisible.Items.AddRange(new object[] {
            "显示",
            "隐藏"});
            this.cboResFormVisible.Location = new System.Drawing.Point(167, 92);
            this.cboResFormVisible.Name = "cboResFormVisible";
            this.cboResFormVisible.Size = new System.Drawing.Size(276, 32);
            this.cboResFormVisible.TabIndex = 35;
            this.cboResFormVisible.Tag = "控制资源气泡窗体的可见性";
            this.cboResFormVisible.SelectedIndexChanged += new System.EventHandler(this.cboResFormVisible_SelectedIndexChanged);
            this.cboResFormVisible.MouseClick += new System.Windows.Forms.MouseEventHandler(this.numTipsFontSize_MouseClick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblLastUpdateTime);
            this.groupBox4.Controls.Add(this.lblCurrVersion);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.groupBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.groupBox4.Location = new System.Drawing.Point(17, 177);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(426, 100);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "软件版本";
            // 
            // lblLastUpdateTime
            // 
            this.lblLastUpdateTime.AutoSize = true;
            this.lblLastUpdateTime.Location = new System.Drawing.Point(110, 54);
            this.lblLastUpdateTime.Name = "lblLastUpdateTime";
            this.lblLastUpdateTime.Size = new System.Drawing.Size(97, 24);
            this.lblLastUpdateTime.TabIndex = 2;
            this.lblLastUpdateTime.Text = "更新时间:0";
            // 
            // lblCurrVersion
            // 
            this.lblCurrVersion.AutoSize = true;
            this.lblCurrVersion.Location = new System.Drawing.Point(109, 22);
            this.lblCurrVersion.Name = "lblCurrVersion";
            this.lblCurrVersion.Size = new System.Drawing.Size(97, 24);
            this.lblCurrVersion.TabIndex = 1;
            this.lblCurrVersion.Text = "当前版本:0";
            // 
            // cboStartLogoShow
            // 
            this.cboStartLogoShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStartLogoShow.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.cboStartLogoShow.FormattingEnabled = true;
            this.cboStartLogoShow.Items.AddRange(new object[] {
            "显示",
            "隐藏"});
            this.cboStartLogoShow.Location = new System.Drawing.Point(167, 139);
            this.cboStartLogoShow.Name = "cboStartLogoShow";
            this.cboStartLogoShow.Size = new System.Drawing.Size(276, 32);
            this.cboStartLogoShow.TabIndex = 38;
            this.cboStartLogoShow.Tag = "控制资源气泡窗体的可见性";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label3.Location = new System.Drawing.Point(27, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 24);
            this.label3.TabIndex = 37;
            this.label3.Text = "系统启动Logo:";
            // 
            // UCTipInfoSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboStartLogoShow);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.cboResFormVisible);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTipsPostion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numTipsFontSize);
            this.Controls.Add(this.label4);
            this.Name = "UCTipInfoSet";
            this.Size = new System.Drawing.Size(455, 286);
            ((System.ComponentModel.ISupportInitialize)(this.numTipsFontSize)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numTipsFontSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTipsPostion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboResFormVisible;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblLastUpdateTime;
        private System.Windows.Forms.Label lblCurrVersion;
        private System.Windows.Forms.ComboBox cboStartLogoShow;
        private System.Windows.Forms.Label label3;
    }
}
