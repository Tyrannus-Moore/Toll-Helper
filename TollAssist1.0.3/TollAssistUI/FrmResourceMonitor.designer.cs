namespace TollAssistUI
{
    partial class FrmResourceMonitor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmResourceMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "FrmResourceMonitor";
            this.Text = "收费助手--资源监视";
            this.Load += new System.EventHandler(this.FrmResourceMonitor_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FrmResourceMonitor_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrmResourceMonitor_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmResourceMonitor_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FrmResourceMonitor_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

    }
}