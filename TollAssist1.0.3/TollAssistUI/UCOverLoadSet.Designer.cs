namespace TollAssistUI
{
    partial class UCOverLoadSet
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
            this.txtOverloadRate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOverloadPostion = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtOverloadRate
            // 
            this.txtOverloadRate.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.txtOverloadRate.Location = new System.Drawing.Point(170, 6);
            this.txtOverloadRate.Name = "txtOverloadRate";
            this.txtOverloadRate.Size = new System.Drawing.Size(262, 31);
            this.txtOverloadRate.TabIndex = 35;
            this.txtOverloadRate.Tag = "正常装载的提示信息";
            this.txtOverloadRate.Text = "超载率：0.0%";
            this.txtOverloadRate.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtOverloadRate_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label1.Location = new System.Drawing.Point(75, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 24);
            this.label1.TabIndex = 34;
            this.label1.Text = "正常显示:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label2.Location = new System.Drawing.Point(4, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 24);
            this.label2.TabIndex = 36;
            this.label2.Text = "超载提示信息坐标:";
            // 
            // txtOverloadPostion
            // 
            this.txtOverloadPostion.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.txtOverloadPostion.Location = new System.Drawing.Point(170, 43);
            this.txtOverloadPostion.Name = "txtOverloadPostion";
            this.txtOverloadPostion.Size = new System.Drawing.Size(262, 31);
            this.txtOverloadPostion.TabIndex = 37;
            this.txtOverloadPostion.Tag = "设置超载提示文字，请拖动右上角靶标到[超载率]提示信息上释放";
            this.txtOverloadPostion.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtOverloadPostion_MouseClick);
            // 
            // UCOverLoadSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtOverloadPostion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOverloadRate);
            this.Controls.Add(this.label1);
            this.Name = "UCOverLoadSet";
            this.Size = new System.Drawing.Size(447, 86);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOverloadRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOverloadPostion;
    }
}
