namespace TollAssistPublisher
{
    partial class FrmPublisher
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlParam = new System.Windows.Forms.Panel();
            this.rdoRemotePublish = new System.Windows.Forms.RadioButton();
            this.rdoLocalPublish = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtResourcePath = new System.Windows.Forms.TextBox();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.richInfo = new System.Windows.Forms.RichTextBox();
            this.btnPublish = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlParam);
            this.groupBox1.Controls.Add(this.rdoRemotePublish);
            this.groupBox1.Controls.Add(this.rdoLocalPublish);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(539, 190);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "发布参数";
            // 
            // pnlParam
            // 
            this.pnlParam.Location = new System.Drawing.Point(6, 54);
            this.pnlParam.Name = "pnlParam";
            this.pnlParam.Size = new System.Drawing.Size(527, 130);
            this.pnlParam.TabIndex = 3;
            // 
            // rdoRemotePublish
            // 
            this.rdoRemotePublish.AutoSize = true;
            this.rdoRemotePublish.Location = new System.Drawing.Point(230, 23);
            this.rdoRemotePublish.Name = "rdoRemotePublish";
            this.rdoRemotePublish.Size = new System.Drawing.Size(71, 16);
            this.rdoRemotePublish.TabIndex = 2;
            this.rdoRemotePublish.Text = "远程发布";
            this.rdoRemotePublish.UseVisualStyleBackColor = true;
            this.rdoRemotePublish.CheckedChanged += new System.EventHandler(this.rdoLocalPublish_CheckedChanged);
            // 
            // rdoLocalPublish
            // 
            this.rdoLocalPublish.AutoSize = true;
            this.rdoLocalPublish.Location = new System.Drawing.Point(73, 23);
            this.rdoLocalPublish.Name = "rdoLocalPublish";
            this.rdoLocalPublish.Size = new System.Drawing.Size(71, 16);
            this.rdoLocalPublish.TabIndex = 1;
            this.rdoLocalPublish.Text = "本地发布";
            this.rdoLocalPublish.UseVisualStyleBackColor = true;
            this.rdoLocalPublish.CheckedChanged += new System.EventHandler(this.rdoLocalPublish_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "发布方式:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "资源路径:";
            // 
            // txtResourcePath
            // 
            this.txtResourcePath.Location = new System.Drawing.Point(64, 17);
            this.txtResourcePath.Name = "txtResourcePath";
            this.txtResourcePath.Size = new System.Drawing.Size(438, 21);
            this.txtResourcePath.TabIndex = 2;
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(508, 15);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(35, 23);
            this.btnOpenFolder.TabIndex = 3;
            this.btnOpenFolder.Text = "...";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // richInfo
            // 
            this.richInfo.Location = new System.Drawing.Point(4, 255);
            this.richInfo.Name = "richInfo";
            this.richInfo.ReadOnly = true;
            this.richInfo.Size = new System.Drawing.Size(539, 158);
            this.richInfo.TabIndex = 4;
            this.richInfo.Text = "";
            this.richInfo.WordWrap = false;
            // 
            // btnPublish
            // 
            this.btnPublish.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPublish.Location = new System.Drawing.Point(372, 419);
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Size = new System.Drawing.Size(75, 29);
            this.btnPublish.TabIndex = 5;
            this.btnPublish.Text = "发布";
            this.btnPublish.UseVisualStyleBackColor = true;
            this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClose.Location = new System.Drawing.Point(468, 419);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 29);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FrmPublisher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 454);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPublish);
            this.Controls.Add(this.richInfo);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.txtResourcePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmPublisher";
            this.Text = "发布程序";
            this.Load += new System.EventHandler(this.FrmPublisher_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoRemotePublish;
        private System.Windows.Forms.RadioButton rdoLocalPublish;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtResourcePath;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.RichTextBox richInfo;
        private System.Windows.Forms.Button btnPublish;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlParam;
    }
}