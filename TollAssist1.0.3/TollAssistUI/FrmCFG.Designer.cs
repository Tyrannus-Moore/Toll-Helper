namespace TollAssistUI
{
    partial class FrmCFG
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
            this.lblConnStat = new System.Windows.Forms.Label();
            this.picConnStat = new System.Windows.Forms.PictureBox();
            this.pnlDragHit = new System.Windows.Forms.Panel();
            this.pnlSetContent = new System.Windows.Forms.Panel();
            this.pnlTipsInfo = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSetTip = new System.Windows.Forms.Label();
            this.pnlClose = new System.Windows.Forms.Panel();
            this.pnlHelp = new System.Windows.Forms.Panel();
            this.pnlContactWay = new System.Windows.Forms.Panel();
            this.pnlExitApplication = new System.Windows.Forms.Panel();
            this.pnlHideForm = new System.Windows.Forms.Panel();
            this.pnlSaveSet = new System.Windows.Forms.Panel();
            this.pnlOverLoadSet = new System.Windows.Forms.Panel();
            this.pnlTipSet = new System.Windows.Forms.Panel();
            this.pnlPlateNumberSet = new System.Windows.Forms.Panel();
            this.pnlSiteSet = new System.Windows.Forms.Panel();
            this.pnlTitel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picConnStat)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblConnStat
            // 
            this.lblConnStat.AutoSize = true;
            this.lblConnStat.BackColor = System.Drawing.Color.Transparent;
            this.lblConnStat.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.lblConnStat.ForeColor = System.Drawing.Color.Black;
            this.lblConnStat.Location = new System.Drawing.Point(64, 487);
            this.lblConnStat.Name = "lblConnStat";
            this.lblConnStat.Size = new System.Drawing.Size(64, 24);
            this.lblConnStat.TabIndex = 15;
            this.lblConnStat.Text = "已断开";
            // 
            // picConnStat
            // 
            this.picConnStat.BackColor = System.Drawing.Color.Transparent;
            this.picConnStat.Location = new System.Drawing.Point(56, 396);
            this.picConnStat.Name = "picConnStat";
            this.picConnStat.Size = new System.Drawing.Size(81, 81);
            this.picConnStat.TabIndex = 14;
            this.picConnStat.TabStop = false;
            // 
            // pnlDragHit
            // 
            this.pnlDragHit.Location = new System.Drawing.Point(455, 47);
            this.pnlDragHit.Name = "pnlDragHit";
            this.pnlDragHit.Size = new System.Drawing.Size(435, 50);
            this.pnlDragHit.TabIndex = 13;
            // 
            // pnlSetContent
            // 
            this.pnlSetContent.Location = new System.Drawing.Point(3, 0);
            this.pnlSetContent.Name = "pnlSetContent";
            this.pnlSetContent.Size = new System.Drawing.Size(749, 297);
            this.pnlSetContent.TabIndex = 0;
            this.pnlSetContent.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlSetContent_Paint);
            // 
            // pnlTipsInfo
            // 
            this.pnlTipsInfo.Location = new System.Drawing.Point(2, 303);
            this.pnlTipsInfo.Name = "pnlTipsInfo";
            this.pnlTipsInfo.Size = new System.Drawing.Size(750, 50);
            this.pnlTipsInfo.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlTipsInfo);
            this.panel1.Controls.Add(this.pnlSetContent);
            this.panel1.Location = new System.Drawing.Point(233, 106);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(755, 358);
            this.panel1.TabIndex = 12;
            // 
            // lblSetTip
            // 
            this.lblSetTip.AutoSize = true;
            this.lblSetTip.BackColor = System.Drawing.Color.Transparent;
            this.lblSetTip.Font = new System.Drawing.Font("华文彩云", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSetTip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(238)))));
            this.lblSetTip.Location = new System.Drawing.Point(230, 67);
            this.lblSetTip.Name = "lblSetTip";
            this.lblSetTip.Size = new System.Drawing.Size(50, 15);
            this.lblSetTip.TabIndex = 11;
            this.lblSetTip.Text = "label1";
            // 
            // pnlClose
            // 
            this.pnlClose.BackColor = System.Drawing.Color.Transparent;
            this.pnlClose.Location = new System.Drawing.Point(960, 0);
            this.pnlClose.Name = "pnlClose";
            this.pnlClose.Size = new System.Drawing.Size(40, 44);
            this.pnlClose.TabIndex = 10;
            this.pnlClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlClose_MouseClick);
            this.pnlClose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlClose.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlClose.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlClose.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlHelp
            // 
            this.pnlHelp.BackColor = System.Drawing.Color.Transparent;
            this.pnlHelp.Location = new System.Drawing.Point(932, -1);
            this.pnlHelp.Name = "pnlHelp";
            this.pnlHelp.Size = new System.Drawing.Size(28, 44);
            this.pnlHelp.TabIndex = 9;
            this.pnlHelp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlHelp.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlHelp.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlHelp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlContactWay
            // 
            this.pnlContactWay.BackColor = System.Drawing.Color.Transparent;
            this.pnlContactWay.Location = new System.Drawing.Point(899, 0);
            this.pnlContactWay.Name = "pnlContactWay";
            this.pnlContactWay.Size = new System.Drawing.Size(33, 44);
            this.pnlContactWay.TabIndex = 8;
            this.pnlContactWay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlContactWay.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlContactWay.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlContactWay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlExitApplication
            // 
            this.pnlExitApplication.BackColor = System.Drawing.Color.Transparent;
            this.pnlExitApplication.Location = new System.Drawing.Point(720, 486);
            this.pnlExitApplication.Name = "pnlExitApplication";
            this.pnlExitApplication.Size = new System.Drawing.Size(162, 45);
            this.pnlExitApplication.TabIndex = 7;
            this.pnlExitApplication.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlExitApplication_MouseClick);
            this.pnlExitApplication.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlExitApplication.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlExitApplication.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlExitApplication.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlHideForm
            // 
            this.pnlHideForm.BackColor = System.Drawing.Color.Transparent;
            this.pnlHideForm.Location = new System.Drawing.Point(503, 486);
            this.pnlHideForm.Name = "pnlHideForm";
            this.pnlHideForm.Size = new System.Drawing.Size(162, 45);
            this.pnlHideForm.TabIndex = 6;
            this.pnlHideForm.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlHideForm_Paint);
            this.pnlHideForm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlHideForm_MouseClick);
            this.pnlHideForm.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlHideForm.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlHideForm.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlHideForm.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlSaveSet
            // 
            this.pnlSaveSet.BackColor = System.Drawing.Color.Transparent;
            this.pnlSaveSet.Location = new System.Drawing.Point(288, 487);
            this.pnlSaveSet.Name = "pnlSaveSet";
            this.pnlSaveSet.Size = new System.Drawing.Size(162, 45);
            this.pnlSaveSet.TabIndex = 5;
            this.pnlSaveSet.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlSaveSet_MouseClick);
            this.pnlSaveSet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlSaveSet.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlSaveSet.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlSaveSet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlOverLoadSet
            // 
            this.pnlOverLoadSet.BackColor = System.Drawing.Color.Transparent;
            this.pnlOverLoadSet.Location = new System.Drawing.Point(13, 309);
            this.pnlOverLoadSet.Name = "pnlOverLoadSet";
            this.pnlOverLoadSet.Size = new System.Drawing.Size(173, 61);
            this.pnlOverLoadSet.TabIndex = 4;
            this.pnlOverLoadSet.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlOverLoadSet_MouseClick);
            this.pnlOverLoadSet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlOverLoadSet.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlOverLoadSet.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlOverLoadSet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlTipSet
            // 
            this.pnlTipSet.BackColor = System.Drawing.Color.Transparent;
            this.pnlTipSet.Location = new System.Drawing.Point(12, 225);
            this.pnlTipSet.Name = "pnlTipSet";
            this.pnlTipSet.Size = new System.Drawing.Size(173, 61);
            this.pnlTipSet.TabIndex = 3;
            this.pnlTipSet.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlTipSet_MouseClick);
            this.pnlTipSet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlTipSet.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlTipSet.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlTipSet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlPlateNumberSet
            // 
            this.pnlPlateNumberSet.BackColor = System.Drawing.Color.Transparent;
            this.pnlPlateNumberSet.Location = new System.Drawing.Point(11, 138);
            this.pnlPlateNumberSet.Name = "pnlPlateNumberSet";
            this.pnlPlateNumberSet.Size = new System.Drawing.Size(173, 61);
            this.pnlPlateNumberSet.TabIndex = 2;
            this.pnlPlateNumberSet.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlPlateNumberSet_MouseClick);
            this.pnlPlateNumberSet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlPlateNumberSet.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlPlateNumberSet.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlPlateNumberSet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlSiteSet
            // 
            this.pnlSiteSet.BackColor = System.Drawing.Color.Transparent;
            this.pnlSiteSet.Location = new System.Drawing.Point(11, 56);
            this.pnlSiteSet.Name = "pnlSiteSet";
            this.pnlSiteSet.Size = new System.Drawing.Size(173, 61);
            this.pnlSiteSet.TabIndex = 1;
            this.pnlSiteSet.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlSiteSet_MouseClick);
            this.pnlSiteSet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseDown);
            this.pnlSiteSet.MouseEnter += new System.EventHandler(this.pnlContactWay_MouseEnter);
            this.pnlSiteSet.MouseLeave += new System.EventHandler(this.pnlContactWay_MouseLeave);
            this.pnlSiteSet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContactWay_MouseUp);
            // 
            // pnlTitel
            // 
            this.pnlTitel.BackColor = System.Drawing.Color.Transparent;
            this.pnlTitel.Location = new System.Drawing.Point(0, 0);
            this.pnlTitel.Name = "pnlTitel";
            this.pnlTitel.Size = new System.Drawing.Size(893, 44);
            this.pnlTitel.TabIndex = 0;
            this.pnlTitel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTitel_MouseDown);
            this.pnlTitel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlTitel_MouseMove);
            this.pnlTitel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlTitel_MouseUp);
            // 
            // FrmCFG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TollAssistUI.Properties.Resources.川高;
            this.ClientSize = new System.Drawing.Size(1000, 560);
            this.Controls.Add(this.lblConnStat);
            this.Controls.Add(this.picConnStat);
            this.Controls.Add(this.pnlDragHit);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblSetTip);
            this.Controls.Add(this.pnlClose);
            this.Controls.Add(this.pnlHelp);
            this.Controls.Add(this.pnlContactWay);
            this.Controls.Add(this.pnlExitApplication);
            this.Controls.Add(this.pnlHideForm);
            this.Controls.Add(this.pnlSaveSet);
            this.Controls.Add(this.pnlOverLoadSet);
            this.Controls.Add(this.pnlTipSet);
            this.Controls.Add(this.pnlPlateNumberSet);
            this.Controls.Add(this.pnlSiteSet);
            this.Controls.Add(this.pnlTitel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FrmCFG";
            this.Text = "收费助手--配置";
            this.Load += new System.EventHandler(this.FrmCFG_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCFG_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picConnStat)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblConnStat;
        private System.Windows.Forms.PictureBox picConnStat;
        private System.Windows.Forms.Panel pnlDragHit;
        private System.Windows.Forms.Panel pnlSetContent;
        private System.Windows.Forms.Panel pnlTipsInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblSetTip;
        private System.Windows.Forms.Panel pnlClose;
        private System.Windows.Forms.Panel pnlHelp;
        private System.Windows.Forms.Panel pnlContactWay;
        private System.Windows.Forms.Panel pnlExitApplication;
        private System.Windows.Forms.Panel pnlHideForm;
        private System.Windows.Forms.Panel pnlSaveSet;
        private System.Windows.Forms.Panel pnlOverLoadSet;
        private System.Windows.Forms.Panel pnlTipSet;
        private System.Windows.Forms.Panel pnlPlateNumberSet;
        private System.Windows.Forms.Panel pnlSiteSet;
        private System.Windows.Forms.Panel pnlTitel;


    }
}