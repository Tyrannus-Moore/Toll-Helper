namespace TollAssistMonitor
{
    partial class FrmMonitor
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lstViewDetails = new System.Windows.Forms.ListView();
            this.columnProcessID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProcessName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnMemoryUsageRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnMemoryUsageInKB = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstViewMonitorItems = new System.Windows.Forms.ListView();
            this.columnIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripAddMonitor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tlmsAddMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.tlmsReomveMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lstViewHeartBeat = new System.Windows.Forms.ListView();
            this.columnHBProcessID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHBProcessName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHBCPUUsageRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHBMemoryUsageRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLastTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIconTask = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmlaShowMainForm = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmlExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerNotifyIcon = new System.Windows.Forms.Timer(this.components);
            this.timerStartHide = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.contextMenuStripAddMonitor.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.contextMenuNotify.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(395, 377);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lstViewDetails);
            this.tabPage1.Controls.Add(this.lstViewMonitorItems);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(387, 351);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "监视项";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lstViewDetails
            // 
            this.lstViewDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnProcessID,
            this.columnProcessName,
            this.columnMemoryUsageRate,
            this.columnMemoryUsageInKB});
            this.lstViewDetails.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lstViewDetails.FullRowSelect = true;
            this.lstViewDetails.GridLines = true;
            this.lstViewDetails.Location = new System.Drawing.Point(3, 219);
            this.lstViewDetails.Name = "lstViewDetails";
            this.lstViewDetails.Size = new System.Drawing.Size(381, 129);
            this.lstViewDetails.TabIndex = 1;
            this.lstViewDetails.UseCompatibleStateImageBehavior = false;
            this.lstViewDetails.View = System.Windows.Forms.View.Details;
            // 
            // columnProcessID
            // 
            this.columnProcessID.Text = "进程ID";
            // 
            // columnProcessName
            // 
            this.columnProcessName.Text = "进程名称";
            // 
            // columnMemoryUsageRate
            // 
            this.columnMemoryUsageRate.Text = "内存使用率";
            this.columnMemoryUsageRate.Width = 84;
            // 
            // columnMemoryUsageInKB
            // 
            this.columnMemoryUsageInKB.Text = "内存使用量(KB)";
            this.columnMemoryUsageInKB.Width = 114;
            // 
            // lstViewMonitorItems
            // 
            this.lstViewMonitorItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstViewMonitorItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnIndex,
            this.columnPath});
            this.lstViewMonitorItems.ContextMenuStrip = this.contextMenuStripAddMonitor;
            this.lstViewMonitorItems.FullRowSelect = true;
            this.lstViewMonitorItems.GridLines = true;
            this.lstViewMonitorItems.Location = new System.Drawing.Point(3, 0);
            this.lstViewMonitorItems.Name = "lstViewMonitorItems";
            this.lstViewMonitorItems.Size = new System.Drawing.Size(381, 217);
            this.lstViewMonitorItems.TabIndex = 0;
            this.lstViewMonitorItems.UseCompatibleStateImageBehavior = false;
            this.lstViewMonitorItems.View = System.Windows.Forms.View.Details;
            this.lstViewMonitorItems.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstViewMonitorItems_MouseClick);
            // 
            // columnIndex
            // 
            this.columnIndex.Text = "编号";
            // 
            // columnPath
            // 
            this.columnPath.Text = "程序路径";
            this.columnPath.Width = 317;
            // 
            // contextMenuStripAddMonitor
            // 
            this.contextMenuStripAddMonitor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlmsAddMonitor,
            this.tlmsReomveMonitor});
            this.contextMenuStripAddMonitor.Name = "contextMenuStripAddMonitor";
            this.contextMenuStripAddMonitor.Size = new System.Drawing.Size(125, 48);
            // 
            // tlmsAddMonitor
            // 
            this.tlmsAddMonitor.Image = global::TollAssistMonitor.Properties.Resources.add;
            this.tlmsAddMonitor.Name = "tlmsAddMonitor";
            this.tlmsAddMonitor.Size = new System.Drawing.Size(124, 22);
            this.tlmsAddMonitor.Text = "添加监视";
            this.tlmsAddMonitor.Click += new System.EventHandler(this.tlmsAddMonitor_Click);
            // 
            // tlmsReomveMonitor
            // 
            this.tlmsReomveMonitor.Image = global::TollAssistMonitor.Properties.Resources.delete1;
            this.tlmsReomveMonitor.Name = "tlmsReomveMonitor";
            this.tlmsReomveMonitor.Size = new System.Drawing.Size(124, 22);
            this.tlmsReomveMonitor.Text = "移除监视";
            this.tlmsReomveMonitor.Click += new System.EventHandler(this.tlmsReomveMonitor_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lstViewHeartBeat);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(387, 351);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "心跳记录";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lstViewHeartBeat
            // 
            this.lstViewHeartBeat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHBProcessID,
            this.columnHBProcessName,
            this.columnHBCPUUsageRate,
            this.columnHBMemoryUsageRate,
            this.columnLastTime});
            this.lstViewHeartBeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstViewHeartBeat.FullRowSelect = true;
            this.lstViewHeartBeat.GridLines = true;
            this.lstViewHeartBeat.Location = new System.Drawing.Point(3, 3);
            this.lstViewHeartBeat.Name = "lstViewHeartBeat";
            this.lstViewHeartBeat.Size = new System.Drawing.Size(381, 345);
            this.lstViewHeartBeat.TabIndex = 0;
            this.lstViewHeartBeat.UseCompatibleStateImageBehavior = false;
            this.lstViewHeartBeat.View = System.Windows.Forms.View.Details;
            // 
            // columnHBProcessID
            // 
            this.columnHBProcessID.Text = "进程ID";
            // 
            // columnHBProcessName
            // 
            this.columnHBProcessName.Text = "进程名称";
            // 
            // columnHBCPUUsageRate
            // 
            this.columnHBCPUUsageRate.Text = "CPU使用率";
            this.columnHBCPUUsageRate.Width = 96;
            // 
            // columnHBMemoryUsageRate
            // 
            this.columnHBMemoryUsageRate.Text = "内存使用率";
            this.columnHBMemoryUsageRate.Width = 112;
            // 
            // columnLastTime
            // 
            this.columnLastTime.Text = "接收时间";
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIconTask
            // 
            this.notifyIconTask.ContextMenuStrip = this.contextMenuNotify;
            this.notifyIconTask.Text = "notifyIcon1";
            this.notifyIconTask.Visible = true;
            // 
            // contextMenuNotify
            // 
            this.contextMenuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmlaShowMainForm,
            this.tsmlExit});
            this.contextMenuNotify.Name = "contextMenuNotify";
            this.contextMenuNotify.Size = new System.Drawing.Size(137, 48);
            // 
            // tsmlaShowMainForm
            // 
            this.tsmlaShowMainForm.Image = global::TollAssistMonitor.Properties.Resources.application_xp;
            this.tsmlaShowMainForm.Name = "tsmlaShowMainForm";
            this.tsmlaShowMainForm.Size = new System.Drawing.Size(136, 22);
            this.tsmlaShowMainForm.Text = "显示主界面";
            this.tsmlaShowMainForm.Click += new System.EventHandler(this.tsmlaShowMainForm_Click);
            // 
            // tsmlExit
            // 
            this.tsmlExit.Image = global::TollAssistMonitor.Properties.Resources.cancel;
            this.tsmlExit.Name = "tsmlExit";
            this.tsmlExit.Size = new System.Drawing.Size(136, 22);
            this.tsmlExit.Text = "退出监视";
            this.tsmlExit.Click += new System.EventHandler(this.tsmlExit_Click);
            // 
            // timerNotifyIcon
            // 
            this.timerNotifyIcon.Enabled = true;
            this.timerNotifyIcon.Interval = 1000;
            this.timerNotifyIcon.Tick += new System.EventHandler(this.timerNotifyIcon_Tick);
            // 
            // timerStartHide
            // 
            this.timerStartHide.Interval = 1000;
            this.timerStartHide.Tick += new System.EventHandler(this.timerStartHide_Tick);
            // 
            // FrmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 377);
            this.Controls.Add(this.tabControl1);
            this.Name = "FrmMonitor";
            this.ShowInTaskbar = false;
            this.Text = "监视";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMonitor_FormClosing);
            this.Load += new System.EventHandler(this.FrmMonitor_Load);
            this.SizeChanged += new System.EventHandler(this.FrmMonitor_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.contextMenuStripAddMonitor.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.contextMenuNotify.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView lstViewDetails;
        private System.Windows.Forms.ListView lstViewMonitorItems;
        private System.Windows.Forms.ColumnHeader columnIndex;
        private System.Windows.Forms.ColumnHeader columnPath;
        private System.Windows.Forms.ColumnHeader columnProcessID;
        private System.Windows.Forms.ColumnHeader columnMemoryUsageRate;
        private System.Windows.Forms.ColumnHeader columnProcessName;
        private System.Windows.Forms.ColumnHeader columnMemoryUsageInKB;
        private System.Windows.Forms.ListView lstViewHeartBeat;
        private System.Windows.Forms.ColumnHeader columnHBProcessID;
        private System.Windows.Forms.ColumnHeader columnHBCPUUsageRate;
        private System.Windows.Forms.ColumnHeader columnHBMemoryUsageRate;
        private System.Windows.Forms.ColumnHeader columnLastTime;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAddMonitor;
        private System.Windows.Forms.ToolStripMenuItem tlmsAddMonitor;
        private System.Windows.Forms.ToolStripMenuItem tlmsReomveMonitor;
        private System.Windows.Forms.ColumnHeader columnHBProcessName;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NotifyIcon notifyIconTask;
        private System.Windows.Forms.Timer timerNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuNotify;
        private System.Windows.Forms.ToolStripMenuItem tsmlaShowMainForm;
        private System.Windows.Forms.ToolStripMenuItem tsmlExit;
        private System.Windows.Forms.Timer timerStartHide;
    }
}