namespace TollAssistUI
{
    partial class UCDrag
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
            this.components = new System.ComponentModel.Container();
            this.picBoxDrag = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timerHit = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxDrag)).BeginInit();
            this.SuspendLayout();
            // 
            // picBoxDrag
            // 
            this.picBoxDrag.Location = new System.Drawing.Point(2, 1);
            this.picBoxDrag.Name = "picBoxDrag";
            this.picBoxDrag.Size = new System.Drawing.Size(32, 32);
            this.picBoxDrag.TabIndex = 0;
            this.picBoxDrag.TabStop = false;
            this.picBoxDrag.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBoxDrag_MouseDown);
            this.picBoxDrag.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picBoxDrag_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(47, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "←拖动靶标调整坐标";
            // 
            // timerHit
            // 
            this.timerHit.Interval = 500;
            this.timerHit.Tick += new System.EventHandler(this.timerHit_Tick);
            // 
            // UCDrag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picBoxDrag);
            this.Name = "UCDrag";
            this.Size = new System.Drawing.Size(459, 37);
            this.Load += new System.EventHandler(this.UCDrag_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxDrag)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBoxDrag;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerHit;
    }
}
