namespace TollAssistUI
{
    partial class UCSiteSet
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
            this.txtLocalHostIP = new System.Windows.Forms.TextBox();
            this.lblSetTip = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cboCompanyCode = new System.Windows.Forms.ComboBox();
            this.cboPlazeCode = new System.Windows.Forms.ComboBox();
            this.cboLanName = new System.Windows.Forms.ComboBox();
            this.numLanNum = new System.Windows.Forms.NumericUpDown();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numLanNum)).BeginInit();
            this.SuspendLayout();
            // 
            // txtLocalHostIP
            // 
            this.txtLocalHostIP.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.txtLocalHostIP.Location = new System.Drawing.Point(170, 160);
            this.txtLocalHostIP.Name = "txtLocalHostIP";
            this.txtLocalHostIP.Size = new System.Drawing.Size(276, 31);
            this.txtLocalHostIP.TabIndex = 20;
            this.txtLocalHostIP.Tag = "本机IP地址";
            this.txtLocalHostIP.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cboCompanyCode_MouseClick);
            this.txtLocalHostIP.Validated += new System.EventHandler(this.txtLocalHostIP_Validated);
            // 
            // lblSetTip
            // 
            this.lblSetTip.AutoSize = true;
            this.lblSetTip.BackColor = System.Drawing.Color.Transparent;
            this.lblSetTip.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.lblSetTip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.lblSetTip.Location = new System.Drawing.Point(19, 198);
            this.lblSetTip.Name = "lblSetTip";
            this.lblSetTip.Size = new System.Drawing.Size(140, 24);
            this.lblSetTip.TabIndex = 19;
            this.lblSetTip.Text = "服务器IP地址(*):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label1.Location = new System.Drawing.Point(35, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 24);
            this.label1.TabIndex = 21;
            this.label1.Text = "路公司代码(*):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label2.Location = new System.Drawing.Point(35, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 24);
            this.label2.TabIndex = 22;
            this.label2.Text = "收费站代码(*):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label3.Location = new System.Drawing.Point(71, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 24);
            this.label3.TabIndex = 23;
            this.label3.Text = "出入口(*):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label4.Location = new System.Drawing.Point(53, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 24);
            this.label4.TabIndex = 24;
            this.label4.Text = "车道编号(*):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label5.Location = new System.Drawing.Point(57, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 24);
            this.label5.TabIndex = 25;
            this.label5.Text = "本机IP地址:";
            // 
            // cboCompanyCode
            // 
            this.cboCompanyCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCompanyCode.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.cboCompanyCode.FormattingEnabled = true;
            this.cboCompanyCode.Location = new System.Drawing.Point(170, 12);
            this.cboCompanyCode.Name = "cboCompanyCode";
            this.cboCompanyCode.Size = new System.Drawing.Size(276, 32);
            this.cboCompanyCode.TabIndex = 26;
            this.cboCompanyCode.Tag = "(必填项)下拉选择路公司代码";
            this.cboCompanyCode.SelectedIndexChanged += new System.EventHandler(this.cboCompanyCode_SelectedIndexChanged);
            this.cboCompanyCode.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cboCompanyCode_MouseClick);
            // 
            // cboPlazeCode
            // 
            this.cboPlazeCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlazeCode.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.cboPlazeCode.FormattingEnabled = true;
            this.cboPlazeCode.Location = new System.Drawing.Point(170, 49);
            this.cboPlazeCode.Name = "cboPlazeCode";
            this.cboPlazeCode.Size = new System.Drawing.Size(276, 32);
            this.cboPlazeCode.TabIndex = 27;
            this.cboPlazeCode.Tag = "(必填项)下拉选择收费站代码";
            this.cboPlazeCode.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cboCompanyCode_MouseClick);
            // 
            // cboLanName
            // 
            this.cboLanName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanName.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.cboLanName.FormattingEnabled = true;
            this.cboLanName.Items.AddRange(new object[] {
            "入口",
            "出口"});
            this.cboLanName.Location = new System.Drawing.Point(170, 85);
            this.cboLanName.Name = "cboLanName";
            this.cboLanName.Size = new System.Drawing.Size(276, 32);
            this.cboLanName.TabIndex = 28;
            this.cboLanName.Tag = "(必填项)下拉选择车道类型";
            this.cboLanName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cboCompanyCode_MouseClick);
            // 
            // numLanNum
            // 
            this.numLanNum.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.numLanNum.Location = new System.Drawing.Point(170, 121);
            this.numLanNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLanNum.Name = "numLanNum";
            this.numLanNum.Size = new System.Drawing.Size(276, 31);
            this.numLanNum.TabIndex = 29;
            this.numLanNum.Tag = "(必填项)设置车道编号";
            this.numLanNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLanNum.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cboCompanyCode_MouseClick);
            // 
            // txtServerIP
            // 
            this.txtServerIP.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.txtServerIP.Location = new System.Drawing.Point(170, 198);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(276, 31);
            this.txtServerIP.TabIndex = 30;
            this.txtServerIP.Tag = "(必填项)设置服务器IP地址,修改后请重启软件";
            this.txtServerIP.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cboCompanyCode_MouseClick);
            this.txtServerIP.Validated += new System.EventHandler(this.txtLocalHostIP_Validated);
            // 
            // UCSiteSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.numLanNum);
            this.Controls.Add(this.cboLanName);
            this.Controls.Add(this.cboPlazeCode);
            this.Controls.Add(this.cboCompanyCode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLocalHostIP);
            this.Controls.Add(this.lblSetTip);
            this.Name = "UCSiteSet";
            this.Size = new System.Drawing.Size(459, 239);
            this.Load += new System.EventHandler(this.UCSiteSet_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numLanNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLocalHostIP;
        private System.Windows.Forms.Label lblSetTip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboCompanyCode;
        private System.Windows.Forms.ComboBox cboPlazeCode;
        private System.Windows.Forms.ComboBox cboLanName;
        private System.Windows.Forms.NumericUpDown numLanNum;
        private System.Windows.Forms.TextBox txtServerIP;
    }
}
