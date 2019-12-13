namespace TollAssitServerUI
{
    partial class FrmSeverMain
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richLogInfo = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblPageCount = new System.Windows.Forms.Label();
            this.btnExportAll = new System.Windows.Forms.Button();
            this.bntExportPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.dgvNewPlatteView = new System.Windows.Forms.DataGridView();
            this.ColNewPlatte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCartype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCarNumberColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMonlevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCardNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCompanyCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPlazCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLanName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLanNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboMonLevel = new System.Windows.Forms.ComboBox();
            this.bntQuery = new System.Windows.Forms.Button();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.dtpBeginTime = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCenterDBApply = new System.Windows.Forms.Button();
            this.txtCenterDBPwd = new System.Windows.Forms.TextBox();
            this.txtCenterDBUser = new System.Windows.Forms.TextBox();
            this.txtCenterDBIP = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnCenterShareApply = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtRemoteAddress = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnCustomRecordExportApply = new System.Windows.Forms.Button();
            this.cboCustomRecordExportFrequency = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkCustomRecordAutoExport = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btnNewPlatteExportApply = new System.Windows.Forms.Button();
            this.cboNewPlatteExportFrequency = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.chkCarRecordAutoExport = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblLastUpdateTime = new System.Windows.Forms.Label();
            this.lblCurrVersion = new System.Windows.Forms.Label();
            this.btnSoftUpdate = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnStationUpdate = new System.Windows.Forms.Button();
            this.cboPlazCode = new System.Windows.Forms.ComboBox();
            this.cboCompanyCode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslblStartDateTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslblResource = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNewPlatteView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(754, 532);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richLogInfo);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(746, 506);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "系统消息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // richLogInfo
            // 
            this.richLogInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richLogInfo.Location = new System.Drawing.Point(3, 3);
            this.richLogInfo.Name = "richLogInfo";
            this.richLogInfo.Size = new System.Drawing.Size(740, 500);
            this.richLogInfo.TabIndex = 0;
            this.richLogInfo.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lblPageCount);
            this.tabPage2.Controls.Add(this.btnExportAll);
            this.tabPage2.Controls.Add(this.bntExportPage);
            this.tabPage2.Controls.Add(this.btnNextPage);
            this.tabPage2.Controls.Add(this.btnPrevPage);
            this.tabPage2.Controls.Add(this.dgvNewPlatteView);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(746, 506);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "车辆记录查询";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblPageCount
            // 
            this.lblPageCount.AutoSize = true;
            this.lblPageCount.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPageCount.Location = new System.Drawing.Point(8, 463);
            this.lblPageCount.Name = "lblPageCount";
            this.lblPageCount.Size = new System.Drawing.Size(95, 12);
            this.lblPageCount.TabIndex = 7;
            this.lblPageCount.Text = "当页共有记录0条";
            // 
            // btnExportAll
            // 
            this.btnExportAll.Location = new System.Drawing.Point(663, 458);
            this.btnExportAll.Name = "btnExportAll";
            this.btnExportAll.Size = new System.Drawing.Size(75, 23);
            this.btnExportAll.TabIndex = 6;
            this.btnExportAll.Text = "导出所有";
            this.btnExportAll.UseVisualStyleBackColor = true;
            this.btnExportAll.Click += new System.EventHandler(this.btnExportAll_Click);
            // 
            // bntExportPage
            // 
            this.bntExportPage.Location = new System.Drawing.Point(578, 458);
            this.bntExportPage.Name = "bntExportPage";
            this.bntExportPage.Size = new System.Drawing.Size(75, 23);
            this.bntExportPage.TabIndex = 5;
            this.bntExportPage.Text = "导出本页";
            this.bntExportPage.UseVisualStyleBackColor = true;
            this.bntExportPage.Click += new System.EventHandler(this.bntExportPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Location = new System.Drawing.Point(496, 458);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(75, 23);
            this.btnNextPage.TabIndex = 4;
            this.btnNextPage.Text = "下一页";
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.Location = new System.Drawing.Point(413, 458);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(75, 23);
            this.btnPrevPage.TabIndex = 3;
            this.btnPrevPage.Text = "上一页";
            this.btnPrevPage.UseVisualStyleBackColor = true;
            this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);
            // 
            // dgvNewPlatteView
            // 
            this.dgvNewPlatteView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNewPlatteView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColNewPlatte,
            this.ColCartype,
            this.ColCarNumberColor,
            this.ColMonlevel,
            this.ColCardNumber,
            this.ColCompanyCode,
            this.ColPlazCode,
            this.ColLanName,
            this.ColLanNum,
            this.ColDTime});
            this.dgvNewPlatteView.Location = new System.Drawing.Point(3, 68);
            this.dgvNewPlatteView.Name = "dgvNewPlatteView";
            this.dgvNewPlatteView.ReadOnly = true;
            this.dgvNewPlatteView.RowHeadersVisible = false;
            this.dgvNewPlatteView.RowTemplate.Height = 23;
            this.dgvNewPlatteView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNewPlatteView.Size = new System.Drawing.Size(740, 384);
            this.dgvNewPlatteView.TabIndex = 2;
            // 
            // ColNewPlatte
            // 
            this.ColNewPlatte.HeaderText = "车牌号";
            this.ColNewPlatte.Name = "ColNewPlatte";
            this.ColNewPlatte.ReadOnly = true;
            // 
            // ColCartype
            // 
            this.ColCartype.HeaderText = "车型";
            this.ColCartype.Name = "ColCartype";
            this.ColCartype.ReadOnly = true;
            this.ColCartype.Visible = false;
            // 
            // ColCarNumberColor
            // 
            this.ColCarNumberColor.HeaderText = "车牌颜色";
            this.ColCarNumberColor.Name = "ColCarNumberColor";
            this.ColCarNumberColor.ReadOnly = true;
            // 
            // ColMonlevel
            // 
            this.ColMonlevel.HeaderText = "监视级别";
            this.ColMonlevel.Name = "ColMonlevel";
            this.ColMonlevel.ReadOnly = true;
            // 
            // ColCardNumber
            // 
            this.ColCardNumber.HeaderText = "卡号";
            this.ColCardNumber.Name = "ColCardNumber";
            this.ColCardNumber.ReadOnly = true;
            // 
            // ColCompanyCode
            // 
            this.ColCompanyCode.HeaderText = "路公司代码";
            this.ColCompanyCode.Name = "ColCompanyCode";
            this.ColCompanyCode.ReadOnly = true;
            // 
            // ColPlazCode
            // 
            this.ColPlazCode.HeaderText = "收费站代码";
            this.ColPlazCode.Name = "ColPlazCode";
            this.ColPlazCode.ReadOnly = true;
            // 
            // ColLanName
            // 
            this.ColLanName.HeaderText = "出入口";
            this.ColLanName.Name = "ColLanName";
            this.ColLanName.ReadOnly = true;
            // 
            // ColLanNum
            // 
            this.ColLanNum.HeaderText = "车道编号";
            this.ColLanNum.Name = "ColLanNum";
            this.ColLanNum.ReadOnly = true;
            // 
            // ColDTime
            // 
            this.ColDTime.HeaderText = "采集时间";
            this.ColDTime.Name = "ColDTime";
            this.ColDTime.ReadOnly = true;
            this.ColDTime.Width = 120;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboMonLevel);
            this.groupBox1.Controls.Add(this.bntQuery);
            this.groupBox1.Controls.Add(this.dtpEndTime);
            this.groupBox1.Controls.Add(this.dtpBeginTime);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(735, 56);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "查询参数";
            // 
            // cboMonLevel
            // 
            this.cboMonLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMonLevel.FormattingEnabled = true;
            this.cboMonLevel.Location = new System.Drawing.Point(429, 24);
            this.cboMonLevel.Name = "cboMonLevel";
            this.cboMonLevel.Size = new System.Drawing.Size(101, 20);
            this.cboMonLevel.TabIndex = 5;
            // 
            // bntQuery
            // 
            this.bntQuery.Location = new System.Drawing.Point(546, 23);
            this.bntQuery.Name = "bntQuery";
            this.bntQuery.Size = new System.Drawing.Size(75, 23);
            this.bntQuery.TabIndex = 4;
            this.bntQuery.Text = "查询";
            this.bntQuery.UseVisualStyleBackColor = true;
            this.bntQuery.Click += new System.EventHandler(this.bntQuery_Click);
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.Location = new System.Drawing.Point(246, 23);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.Size = new System.Drawing.Size(168, 21);
            this.dtpEndTime.TabIndex = 3;
            // 
            // dtpBeginTime
            // 
            this.dtpBeginTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpBeginTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBeginTime.Location = new System.Drawing.Point(46, 23);
            this.dtpBeginTime.Name = "dtpBeginTime";
            this.dtpBeginTime.Size = new System.Drawing.Size(168, 21);
            this.dtpBeginTime.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "To:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "From:";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(746, 506);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "系统设置";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnCenterDBApply);
            this.groupBox2.Controls.Add(this.txtCenterDBPwd);
            this.groupBox2.Controls.Add(this.txtCenterDBUser);
            this.groupBox2.Controls.Add(this.txtCenterDBIP);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Location = new System.Drawing.Point(5, 375);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(737, 126);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "中心数据库设置(重启生效)";
            // 
            // btnCenterDBApply
            // 
            this.btnCenterDBApply.Location = new System.Drawing.Point(406, 83);
            this.btnCenterDBApply.Name = "btnCenterDBApply";
            this.btnCenterDBApply.Size = new System.Drawing.Size(75, 23);
            this.btnCenterDBApply.TabIndex = 17;
            this.btnCenterDBApply.Text = "应用";
            this.btnCenterDBApply.UseVisualStyleBackColor = true;
            this.btnCenterDBApply.Click += new System.EventHandler(this.btnCenterDBApply_Click);
            // 
            // txtCenterDBPwd
            // 
            this.txtCenterDBPwd.Location = new System.Drawing.Point(73, 83);
            this.txtCenterDBPwd.Name = "txtCenterDBPwd";
            this.txtCenterDBPwd.PasswordChar = '★';
            this.txtCenterDBPwd.Size = new System.Drawing.Size(322, 21);
            this.txtCenterDBPwd.TabIndex = 15;
            this.txtCenterDBPwd.Text = "ggj1998";
            // 
            // txtCenterDBUser
            // 
            this.txtCenterDBUser.Location = new System.Drawing.Point(73, 55);
            this.txtCenterDBUser.Name = "txtCenterDBUser";
            this.txtCenterDBUser.Size = new System.Drawing.Size(322, 21);
            this.txtCenterDBUser.TabIndex = 14;
            this.txtCenterDBUser.Text = "sa";
            // 
            // txtCenterDBIP
            // 
            this.txtCenterDBIP.Location = new System.Drawing.Point(73, 26);
            this.txtCenterDBIP.Name = "txtCenterDBIP";
            this.txtCenterDBIP.Size = new System.Drawing.Size(322, 21);
            this.txtCenterDBIP.TabIndex = 13;
            this.txtCenterDBIP.Text = "179.50.1.1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(40, 87);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 12);
            this.label12.TabIndex = 12;
            this.label12.Text = "密码:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(28, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 12);
            this.label13.TabIndex = 11;
            this.label13.Text = "用户名:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 30);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 12);
            this.label14.TabIndex = 10;
            this.label14.Text = "库IP地址:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnCenterShareApply);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.txtPassword);
            this.groupBox5.Controls.Add(this.txtUserName);
            this.groupBox5.Controls.Add(this.txtRemoteAddress);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Location = new System.Drawing.Point(6, 243);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(737, 126);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "中心共享文件夹设置(重启生效)";
            // 
            // btnCenterShareApply
            // 
            this.btnCenterShareApply.Location = new System.Drawing.Point(406, 83);
            this.btnCenterShareApply.Name = "btnCenterShareApply";
            this.btnCenterShareApply.Size = new System.Drawing.Size(75, 23);
            this.btnCenterShareApply.TabIndex = 17;
            this.btnCenterShareApply.Text = "应用";
            this.btnCenterShareApply.UseVisualStyleBackColor = true;
            this.btnCenterShareApply.Click += new System.EventHandler(this.btnCenterShareApply_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Location = new System.Drawing.Point(404, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "\\\\IP\\共享文件夹";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(73, 83);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '★';
            this.txtPassword.Size = new System.Drawing.Size(322, 21);
            this.txtPassword.TabIndex = 15;
            this.txtPassword.Text = "lsh";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(73, 55);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(322, 21);
            this.txtUserName.TabIndex = 14;
            this.txtUserName.Text = "lsh";
            // 
            // txtRemoteAddress
            // 
            this.txtRemoteAddress.Location = new System.Drawing.Point(73, 26);
            this.txtRemoteAddress.Name = "txtRemoteAddress";
            this.txtRemoteAddress.Size = new System.Drawing.Size(322, 21);
            this.txtRemoteAddress.TabIndex = 13;
            this.txtRemoteAddress.Text = "192.168.100.55\\share";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(40, 87);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "密码:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 12);
            this.label10.TabIndex = 11;
            this.label10.Text = "用户名:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 10;
            this.label11.Text = "远程地址:";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.groupBox6);
            this.groupBox7.Controls.Add(this.groupBox8);
            this.groupBox7.Location = new System.Drawing.Point(6, 147);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(737, 87);
            this.groupBox7.TabIndex = 4;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "导出设置(重启生效)";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnCustomRecordExportApply);
            this.groupBox6.Controls.Add(this.cboCustomRecordExportFrequency);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.chkCustomRecordAutoExport);
            this.groupBox6.Location = new System.Drawing.Point(311, 20);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(426, 56);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Visible = false;
            // 
            // btnCustomRecordExportApply
            // 
            this.btnCustomRecordExportApply.Location = new System.Drawing.Point(129, 20);
            this.btnCustomRecordExportApply.Name = "btnCustomRecordExportApply";
            this.btnCustomRecordExportApply.Size = new System.Drawing.Size(75, 23);
            this.btnCustomRecordExportApply.TabIndex = 3;
            this.btnCustomRecordExportApply.Text = "应用";
            this.btnCustomRecordExportApply.UseVisualStyleBackColor = true;
            this.btnCustomRecordExportApply.Click += new System.EventHandler(this.btnCustomRecordExportApply_Click);
            // 
            // cboCustomRecordExportFrequency
            // 
            this.cboCustomRecordExportFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCustomRecordExportFrequency.FormattingEnabled = true;
            this.cboCustomRecordExportFrequency.Items.AddRange(new object[] {
            "按天导出",
            "按周导出",
            "按月导出"});
            this.cboCustomRecordExportFrequency.Location = new System.Drawing.Point(47, 23);
            this.cboCustomRecordExportFrequency.Name = "cboCustomRecordExportFrequency";
            this.cboCustomRecordExportFrequency.Size = new System.Drawing.Size(76, 20);
            this.cboCustomRecordExportFrequency.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "频率:";
            // 
            // chkCustomRecordAutoExport
            // 
            this.chkCustomRecordAutoExport.AutoSize = true;
            this.chkCustomRecordAutoExport.Location = new System.Drawing.Point(0, 2);
            this.chkCustomRecordAutoExport.Name = "chkCustomRecordAutoExport";
            this.chkCustomRecordAutoExport.Size = new System.Drawing.Size(156, 16);
            this.chkCustomRecordAutoExport.TabIndex = 0;
            this.chkCustomRecordAutoExport.Text = "消费记录自动导出并上传";
            this.chkCustomRecordAutoExport.UseVisualStyleBackColor = true;
            this.chkCustomRecordAutoExport.Visible = false;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.btnNewPlatteExportApply);
            this.groupBox8.Controls.Add(this.cboNewPlatteExportFrequency);
            this.groupBox8.Controls.Add(this.label8);
            this.groupBox8.Controls.Add(this.chkCarRecordAutoExport);
            this.groupBox8.Location = new System.Drawing.Point(9, 19);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(296, 56);
            this.groupBox8.TabIndex = 3;
            this.groupBox8.TabStop = false;
            // 
            // btnNewPlatteExportApply
            // 
            this.btnNewPlatteExportApply.Location = new System.Drawing.Point(129, 20);
            this.btnNewPlatteExportApply.Name = "btnNewPlatteExportApply";
            this.btnNewPlatteExportApply.Size = new System.Drawing.Size(75, 23);
            this.btnNewPlatteExportApply.TabIndex = 3;
            this.btnNewPlatteExportApply.Text = "应用";
            this.btnNewPlatteExportApply.UseVisualStyleBackColor = true;
            this.btnNewPlatteExportApply.Click += new System.EventHandler(this.btnCarRecordExportApply_Click);
            // 
            // cboNewPlatteExportFrequency
            // 
            this.cboNewPlatteExportFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNewPlatteExportFrequency.FormattingEnabled = true;
            this.cboNewPlatteExportFrequency.Items.AddRange(new object[] {
            "按天导出",
            "按周导出",
            "按月导出"});
            this.cboNewPlatteExportFrequency.Location = new System.Drawing.Point(47, 23);
            this.cboNewPlatteExportFrequency.Name = "cboNewPlatteExportFrequency";
            this.cboNewPlatteExportFrequency.Size = new System.Drawing.Size(76, 20);
            this.cboNewPlatteExportFrequency.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "频率:";
            // 
            // chkCarRecordAutoExport
            // 
            this.chkCarRecordAutoExport.AutoSize = true;
            this.chkCarRecordAutoExport.Location = new System.Drawing.Point(0, 2);
            this.chkCarRecordAutoExport.Name = "chkCarRecordAutoExport";
            this.chkCarRecordAutoExport.Size = new System.Drawing.Size(156, 16);
            this.chkCarRecordAutoExport.TabIndex = 0;
            this.chkCarRecordAutoExport.Text = "车辆记录自动导出并上传";
            this.chkCarRecordAutoExport.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblLastUpdateTime);
            this.groupBox4.Controls.Add(this.lblCurrVersion);
            this.groupBox4.Controls.Add(this.btnSoftUpdate);
            this.groupBox4.Location = new System.Drawing.Point(317, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(426, 133);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "软件升级";
            // 
            // lblLastUpdateTime
            // 
            this.lblLastUpdateTime.AutoSize = true;
            this.lblLastUpdateTime.Location = new System.Drawing.Point(124, 61);
            this.lblLastUpdateTime.Name = "lblLastUpdateTime";
            this.lblLastUpdateTime.Size = new System.Drawing.Size(65, 12);
            this.lblLastUpdateTime.TabIndex = 2;
            this.lblLastUpdateTime.Text = "更新时间:0";
            // 
            // lblCurrVersion
            // 
            this.lblCurrVersion.AutoSize = true;
            this.lblCurrVersion.Location = new System.Drawing.Point(124, 30);
            this.lblCurrVersion.Name = "lblCurrVersion";
            this.lblCurrVersion.Size = new System.Drawing.Size(65, 12);
            this.lblCurrVersion.TabIndex = 1;
            this.lblCurrVersion.Text = "当前版本:0";
            // 
            // btnSoftUpdate
            // 
            this.btnSoftUpdate.Location = new System.Drawing.Point(146, 98);
            this.btnSoftUpdate.Name = "btnSoftUpdate";
            this.btnSoftUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnSoftUpdate.TabIndex = 0;
            this.btnSoftUpdate.Text = "检查更新";
            this.btnSoftUpdate.UseVisualStyleBackColor = true;
            this.btnSoftUpdate.Click += new System.EventHandler(this.btnSoftUpdate_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnStationUpdate);
            this.groupBox3.Controls.Add(this.cboPlazCode);
            this.groupBox3.Controls.Add(this.cboCompanyCode);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(5, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(306, 133);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "站点参数";
            // 
            // btnStationUpdate
            // 
            this.btnStationUpdate.Location = new System.Drawing.Point(207, 98);
            this.btnStationUpdate.Name = "btnStationUpdate";
            this.btnStationUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnStationUpdate.TabIndex = 4;
            this.btnStationUpdate.Text = "应用修改";
            this.btnStationUpdate.UseVisualStyleBackColor = true;
            this.btnStationUpdate.Click += new System.EventHandler(this.btnStationUpdate_Click);
            // 
            // cboPlazCode
            // 
            this.cboPlazCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlazCode.FormattingEnabled = true;
            this.cboPlazCode.Location = new System.Drawing.Point(114, 61);
            this.cboPlazCode.Name = "cboPlazCode";
            this.cboPlazCode.Size = new System.Drawing.Size(168, 20);
            this.cboPlazCode.TabIndex = 3;
            // 
            // cboCompanyCode
            // 
            this.cboCompanyCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCompanyCode.FormattingEnabled = true;
            this.cboCompanyCode.Location = new System.Drawing.Point(114, 26);
            this.cboCompanyCode.Name = "cboCompanyCode";
            this.cboCompanyCode.Size = new System.Drawing.Size(168, 20);
            this.cboCompanyCode.TabIndex = 2;
            this.cboCompanyCode.SelectedIndexChanged += new System.EventHandler(this.cboCompanyCode_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "收费站代码(*):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "路公司代码(*):";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslblStartDateTime,
            this.tsslblResource});
            this.statusStrip1.Location = new System.Drawing.Point(0, 536);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(754, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslblStartDateTime
            // 
            this.tsslblStartDateTime.ForeColor = System.Drawing.Color.Red;
            this.tsslblStartDateTime.Name = "tsslblStartDateTime";
            this.tsslblStartDateTime.Size = new System.Drawing.Size(201, 17);
            this.tsslblStartDateTime.Text = "系统启动时间:2017-05-27 10:10:10";
            // 
            // tsslblResource
            // 
            this.tsslblResource.Name = "tsslblResource";
            this.tsslblResource.Size = new System.Drawing.Size(56, 17);
            this.tsslblResource.Text = "系统资源";
            // 
            // FrmSeverMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 558);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmSeverMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "收费助手调度服务";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSeverMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmSeverMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNewPlatteView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpBeginTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.Button bntQuery;
        private System.Windows.Forms.DataGridView dgvNewPlatteView;
        private System.Windows.Forms.Button btnExportAll;
        private System.Windows.Forms.Button bntExportPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.RichTextBox richLogInfo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslblStartDateTime;
        private System.Windows.Forms.Label lblPageCount;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cboPlazCode;
        private System.Windows.Forms.ComboBox cboCompanyCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSoftUpdate;
        private System.Windows.Forms.Label lblCurrVersion;
        private System.Windows.Forms.Button btnStationUpdate;
        private System.Windows.Forms.ToolStripStatusLabel tsslblResource;
        private System.Windows.Forms.Label lblLastUpdateTime;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnCustomRecordExportApply;
        private System.Windows.Forms.ComboBox cboCustomRecordExportFrequency;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkCustomRecordAutoExport;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button btnNewPlatteExportApply;
        private System.Windows.Forms.ComboBox cboNewPlatteExportFrequency;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkCarRecordAutoExport;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnCenterShareApply;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtRemoteAddress;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCenterDBApply;
        private System.Windows.Forms.TextBox txtCenterDBPwd;
        private System.Windows.Forms.TextBox txtCenterDBUser;
        private System.Windows.Forms.TextBox txtCenterDBIP;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColNewPlatte;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCartype;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCarNumberColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMonlevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCardNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCompanyCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPlazCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLanName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLanNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDTime;
        private System.Windows.Forms.ComboBox cboMonLevel;
    }
}