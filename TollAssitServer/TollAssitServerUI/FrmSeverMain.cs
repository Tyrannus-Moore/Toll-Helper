using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommHandler;
using IDbHandler;
using TollAssistComm;
using HeartBeat;


namespace TollAssitServerUI
{
    public partial class FrmSeverMain : Form, IUpdateSotfwareNote, IProcessResourceMonitor
    {
        public FrmSeverMain()
        {
            InitializeComponent();

            //导出对话框--保存文件类型
            this.mSaveFileDialog.Title = "导出并保存";
            this.mSaveFileDialog.Filter = "文本文件(*.txt)|*.txt";

            //日志显示委托
            this.DoUpdateDisplayList = UpdateDisplayList;

            //资源占用显示委托
            this.ShowResourceUsage = UpdateResourceUsageToUI;
        }

        private SaveFileDialog mSaveFileDialog = new SaveFileDialog();

        //导出本页
        private void bntExportPage_Click(object sender, EventArgs e)
        {
            List<ASSISTICE.CarRecord> carRecords = this.dgvNewPlatteView.Tag as List<ASSISTICE.CarRecord>;
            if (carRecords == null)
            {
                MessageBox.Show("没有要导出的数据记录", "提示");
                return;
            }


            if (this.mSaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string error;
                if (CarRecordUploadWorker.ExportCarRecordToFile(carRecords, this.mSaveFileDialog.FileName, out error))
                {
                    MessageBox.Show("导出记录成功", "提示");
                }
                else
                {
                    MessageBox.Show(string.Format("导出记录失败:{0}", error), "提示");
                }
            }


        }

        //导出所有
        private void btnExportAll_Click(object sender, EventArgs e)
        {
            if (this.mSaveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            int monlevel = -1;
            if (IDbHandler.MySqlHandler.GetMonLevel().ContainsKey(this.cboMonLevel.Text))
                monlevel = IDbHandler.MySqlHandler.GetMonLevel()[this.cboMonLevel.Text];

            for (int from = 0; ; from += ROWPAGECOUNT)
            {
                string error;
                List<ASSISTICE.CarRecord> list = QueryCarRecords(this.mBegin, this.mEnd,monlevel, from * ROWPAGECOUNT, ROWPAGECOUNT, out error);
                if (list == null)
                {
                    MessageBox.Show(string.Format("执行查询发生异常:{0}", error), "提示");
                    break;
                }
                if (list.Count == 0)
                {
                    MessageBox.Show(string.Format("导出数据完成"), "提示");
                    break;
                }

                //执行导出
                if (!CarRecordUploadWorker.ExportCarRecordToFile(list, this.mSaveFileDialog.FileName, out error))
                {
                    MessageBox.Show(string.Format("导出记录失败:{0} \r\n导出操作终止", error), "提示");
                    break;
                }
                
            }
        }

        private const int ROWPAGECOUNT = 100;//单页最大记录条数
        private int mPageIndex = 0;//查询页所在位置
        private DateTime mBegin;
        private DateTime mEnd;

        //执行查询
        private void bntQuery_Click(object sender, EventArgs e)
        {


            if (dtpBeginTime.Value > dtpEndTime.Value)
            {
                MessageBox.Show("无效的查询条件", "提示");
                return;
            }

            this.btnPrevPage.Enabled = false;

            mPageIndex = 0;
            this.mBegin = dtpBeginTime.Value;
            this.mEnd = dtpEndTime.Value;


            DoQuery();
            //this.dgvNewPlatteView.Refresh();
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        private bool DoQuery()
        {
            string error;

            int monlevel = -1;
            if (IDbHandler.MySqlHandler.GetMonLevel().ContainsKey(this.cboMonLevel.Text))
                monlevel = IDbHandler.MySqlHandler.GetMonLevel()[this.cboMonLevel.Text];

            List<ASSISTICE.CarRecord> list = QueryCarRecords(this.mBegin, this.mEnd, monlevel, this.mPageIndex * ROWPAGECOUNT, ROWPAGECOUNT, out error);
            if (list == null)
            {
                MessageBox.Show(string.Format("执行查询发生异常:{0}", error), "提示");
                return false;
            }
            if (list.Count == 0)
            {
                MessageBox.Show(string.Format("已经没有数据了"), "提示");
                return false;
            }

            //绑定到界面
            //this.dgvNewPlatteView.DataSource = null;
            this.dgvNewPlatteView.Tag = list;

            this.dgvNewPlatteView.Rows.Clear();
            string strMonlevel = "未知";
            foreach (ASSISTICE.CarRecord item in list)
            {
                switch (item.monLevel) 
                {
                    case 0:
                        strMonlevel = "一般车辆";
                        break;
                    case 1:
                        strMonlevel = "涉嫌车辆";
                        break;
                    case 2:
                        strMonlevel = "保障车辆";
                        break;
                    case 3:
                        strMonlevel = "无效车辆";
                        break;
                    default:
                        strMonlevel = "未知车辆";
                        break;
                }
                this.dgvNewPlatteView.Rows.Add(item.number, item.type, item.color, strMonlevel, item.flag, item.companycode, item.plazcode, item.lanname, item.lannum, item.dtime);
            }


            this.lblPageCount.Text = string.Format("当页共有记录{0}条",list.Count);

            return true;
        }

        /// <summary>
        /// 查询新车记录
        /// </summary>
        /// <param name="begin">查询的开始时间(包含)</param>
        /// <param name="end">查询的结束时间(包含)</param>
        /// <param name="from">从0开始的记录位置</param>
        /// <param name="count">本次需要返回的记录数</param>
        /// <param name="error">错误消息</param>
        /// <returns>null表示有错误</returns>
        private List<ASSISTICE.CarRecord> QueryCarRecords(DateTime begin, DateTime end,int level, int from, int count, out string error)
        {
            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                error = "FrmSeverMain::QueryCarRecords()=>获取SqliteHandler实例失败";
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::QueryCarRecords()=>获取SqliteHandler实例失败");
                return null;
            }

            string sql = string.Format("select * from NewPlatte where (dtime>=datetime('{0}') and dtime<=datetime('{1}')) and monlevel={2} LIMIT {3} OFFSET {4}"
              , begin.ToString("yyyy-MM-dd HH:mm:ss"), end.ToString("yyyy-MM-dd HH:mm:ss"),level, count, from);

            List<ASSISTICE.CarRecord> carRecords = sqliteHandler.SearcherCarRecord(sql, out error);

            return carRecords;

        }

        //上一页查询
        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (this.mPageIndex == 0)
            {

                if (this.btnPrevPage.Enabled)
                    this.btnPrevPage.Enabled = false;

                MessageBox.Show("已经在首页了", "提示");

                return;
            }

            this.mPageIndex--;
            DoQuery();

        }

        //下一页查询
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            this.mPageIndex++;
            if (DoQuery())
            {
                if (!this.btnPrevPage.Enabled)
                    this.btnPrevPage.Enabled = true;
            }
            //else 
            //{
            //    if(this.btnNextPage.Enabled)
            //        this.btnNextPage.Enabled = false;
            //}

        }


        //窗体加载
        private void FrmSeverMain_Load(object sender, EventArgs e)
        {
            this.DoCallBackLoger=ShowLoger;
            DoCallBackLoger_Handle = System.Runtime.InteropServices.GCHandle.Alloc(this.DoCallBackLoger);

            Service.StartService(this.DoCallBackLoger, ASSISTUPDATEMODULEICE.UpdateType.Server, this);

            //2018-01-02 新增
            LoadMonLevelToQueryUI();

            //加载新车辆导出配置信息到UI界面
            LoadNewPlatteExportConfigToUI();

            //备注：20171217 取消消费记录功能
            //加载消费记录导出配置信息到UI界面
            //LoadCustomRecordExportConfigToUI();

            // 加载中心共享文件夹配置参数到UI界面
            LoadCenterShareFloderConfigToUI();

           // 加载中心数据库(MSSQL)配置参数到UI界面
            LoadCenterDBConfigToUI();

            System.Threading.Thread.Sleep(1000);//由于数据库实例可能还没有初始化故需要等待
            //加载路公司下拉列表
            LoadCompanyCodes();

            //将配置文件中的路公司信息和站点信息绑定到站点设置界面
            BindStationParamToUI();

            //加载系统版本信息到界面
            LoadSoftVersionToUI();

            //监视系统的资源占用情况
            this.mProcessResourceMonitorThread = new ProcessResourceMonitorThread(2, this);
            this.mProcessResourceMonitorThread.Start();


            //心跳服务
            this.mHeartBaetThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SendHeartBeatThreadFunc));
            this.mHeartBaetThread.IsBackground = true;
            this.mHeartBaetThread.Name = "HeartBaetThread";
            this.mHeartBaetThread.Start(null);


            //将程序添加到启动项中
            SetAutoStartup("TollAssistServer", Helper.GetProcessPath(System.Diagnostics.Process.GetCurrentProcess().Id));

            //系统启动时间
            this.tsslblStartDateTime.Text = string.Format("系统启动时间:{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        }

        /// <summary>
        /// 设置程序自动启动
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="path"></param>
        private void SetAutoStartup(string keyName, string path)
        {
            if (string.IsNullOrWhiteSpace(keyName) || string.IsNullOrWhiteSpace(path))
                return;

            string shortcutPath;
            if (Helper.CreateShortcut(path, keyName, out shortcutPath))
            {
                Helper.SetAutoStart(keyName, shortcutPath);
            }

        }

        /// <summary>
        /// 加载新车辆导出配置信息到UI界面
        /// </summary>
        private void LoadNewPlatteExportConfigToUI() 
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadNewPlatteExportConfigToUI()=>获取SysConfig实例失败");
                return;
            }

            this.chkCarRecordAutoExport.Checked = sysConfig.mAutoUploadCarRecord;

            switch (sysConfig.mUploadCarRecordFrequency) 
            {
                case AutoUploadFrequency.Day: this.cboNewPlatteExportFrequency.SelectedIndex = 0; break;
                case AutoUploadFrequency.Week: this.cboNewPlatteExportFrequency.SelectedIndex = 1; break;
                case AutoUploadFrequency.Month: this.cboNewPlatteExportFrequency.SelectedIndex = 2; break;
            }
            
        }

        /// <summary>
        /// 加载消费记录导出配置信息到UI界面
        /// </summary>
        private void LoadCustomRecordExportConfigToUI()
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadCustomRecordExportConfigToUI()=>获取SysConfig实例失败");
                return;
            }

            this.chkCustomRecordAutoExport.Checked = sysConfig.mAutoUploadCustomRecord;

            switch (sysConfig.mUploadCustomRecordFrequency)
            {
                case AutoUploadFrequency.Day: this.cboCustomRecordExportFrequency.SelectedIndex = 0; break;
                case AutoUploadFrequency.Week: this.cboCustomRecordExportFrequency.SelectedIndex = 1; break;
                case AutoUploadFrequency.Month: this.cboCustomRecordExportFrequency.SelectedIndex = 2; break;
            }

        }

        /// <summary>
        /// 加载中心共享文件夹配置参数到UI界面
        /// </summary>
        private void LoadCenterShareFloderConfigToUI() 
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::btnCenterShareApply_Click()=>获取SysConfig实例失败");
                return;
            }

            this.txtRemoteAddress.Text = sysConfig.mCenterShareFolderAddress;
            this.txtUserName.Text = sysConfig.mCenterShareFolderUser;
            this.txtPassword.Text = sysConfig.mCenterShareFolderPassword;
        }



        /// <summary>
        /// 加载中心数据库(MSSQL)配置参数到UI界面
        /// </summary>
        private void LoadCenterDBConfigToUI()
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::btnCenterShareApply_Click()=>获取SysConfig实例失败");
                return;
            }

            this.txtCenterDBIP.Text = sysConfig.mCenterMSSQLAddress;
            this.txtCenterDBUser.Text = sysConfig.mCenterMSSQLUser;
            this.txtCenterDBPwd.Text = sysConfig.mCenterMSSQLPassword;
        }

        ////更新配置信息
        //private void btnApply_Click(object sender, EventArgs e)
        //{
        //    //获取系统配置实例
        //    SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
        //    if (sysConfig == null)
        //    {
        //        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadNewPlatteExportConfigToUI()=>获取SysConfig实例失败");
        //        return;
        //    }

        //    sysConfig.mAutoUploadUnkownPlate = this.chkAutoExport.Checked;
        //    switch (this.cboExportFrequency.Text) 
        //    {
        //        case "按天导出": sysConfig.mUploadUnkownPlateFrequency = AutoUploadFrequency.Day; break;
        //        case "按周导出": sysConfig.mUploadUnkownPlateFrequency = AutoUploadFrequency.Week; break;
        //        case "按月导出": sysConfig.mUploadUnkownPlateFrequency = AutoUploadFrequency.Month; break;
        //    }

        //    //更新到磁盘
        //    SysConfig.SaveObjectToFile(sysConfig);
        //}

        
        /// <summary>
        /// 升级下载通知
        /// </summary>
        /// <param name="updateResult"></param>
        /// <param name="updateDir"></param>
        /// <param name="error"></param>
        public void UpdateSotfwareNote(UpdateDownloadResult updateResult, string updateDir, string error)
        {
            if (updateResult == UpdateDownloadResult.Downloaded)
            {
                //获取系统配置实例
                SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
                if (sysConfig == null)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::UpdateSotfwareNote()=>获取SysConfig实例失败");
                    return;
                }

                //需要对本软件进行更新升级操作
                HeartBeatData hbd = new HeartBeatData();
                hbd.ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
                hbd.HeartBeatInterval = sysConfig.mHeartBeatIntervalOfSecond;
                hbd.CpuUsageRate = this.mProcessCPURate;
                hbd.MemoryUsageRate = this.mProcessMemRate;
                hbd.Flag = HeartBeatFlag.Suspend;

                //系统等待心跳发送线程终止
                this.mHeartBaetThreadIsStop = true;
                this.mHeartBaetThread.Join();

                //向监视服务器发送暂停监视通知
                if (this.mHeartBeatClient != null)
                    this.mHeartBeatClient.SendHeartBeat(hbd);

                //系统暂时等待一段时间
                //System.Threading.Thread.Sleep(2000);

                //关闭本进程并启动升级进程进行升级操作
                int currentProcessId = hbd.ProcessId;
                string srcDir = updateDir;//拷贝的源文件夹
                string destDir = Helper.GetCurrentProcessPath();//拷贝的目的文件夹
                string showTitel = "服务程序升级中...";//升级程序显示标题内容
                string startProcessPath = Helper.GetProcessPath(hbd.ProcessId);//升级程序升级完成后需要启动的进程路径

                string closeForm = bool.TrueString;//指示升级完成后关闭升级程序
                int waitTime = 5000;//5秒,升级程序关闭之前的等待时间

                string args = string.Format("{0} {1} \"{2}\" \"{3}\" {4} \"{5}\" {6}", currentProcessId, showTitel, srcDir, destDir, closeForm, startProcessPath, waitTime);

                string updateExePath = "update.exe";
                //启动升级程序
                System.Diagnostics.Process p=System.Diagnostics.Process.Start(updateExePath, args);

                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateSotfwareNote.UpdateSotfwareNote()=>程序即将升级&&本进程即将退出");

                //关闭本进程
                Application.Exit();

            }
        }



        private System.Runtime.InteropServices.GCHandle DoCallBackLoger_Handle;
        private LOGCS.CallBackLoger DoCallBackLoger;

        //日志回调函数
        private void ShowLoger(string msg, LOGCS.LogLevel level) 
        {
            UpdateDisplayList(level, msg);
        }

        //更新文本显示委托
        private Action<LOGCS.LogLevel, string> DoUpdateDisplayList;
        private const int LOGCHARSIZE = 1024 * 1024 * 10;//MB最大可以显示的内容大小

        /// <summary>
        /// 更新显示列表
        /// </summary>
        /// <param name="info"></param>
        private void UpdateDisplayList(LOGCS.LogLevel level,string msg)
        {
            if (this.richLogInfo.InvokeRequired)
            {
                this.richLogInfo.Invoke(this.DoUpdateDisplayList, level, msg);
            }
            else
            {
                int index = this.richLogInfo.TextLength;
                if (index >= LOGCHARSIZE)
                {
                    this.richLogInfo.Clear();
                    index = 0;
                }
                this.richLogInfo.AppendText(msg);
                this.richLogInfo.Select(index, msg.Length);
                //根据日志级别判断
                switch (level)
                {
                    //LWARN，警告信息
                    case  LOGCS.LogLevel.WARN:
                        this.richLogInfo.SelectionColor = System.Drawing.Color.DarkOrange;
                        break;
                    //LDEBUG，BUG错误信息
                    case  LOGCS.LogLevel.DEBUG:
                        this.richLogInfo.SelectionColor = System.Drawing.Color.Blue;
                        break;
                    //LINFO，正常信息
                    case  LOGCS.LogLevel.INFO:
                        this.richLogInfo.SelectionColor = System.Drawing.Color.Black;
                        break;
                    //LERROR，错误信息
                    case  LOGCS.LogLevel.ERROR:
                        this.richLogInfo.SelectionColor = System.Drawing.Color.Red;
                        break;
                }
                this.richLogInfo.Select(this.richLogInfo.TextLength, 0);
                this.richLogInfo.ScrollToCaret();
            }
        }

        //加载路公司下拉列表
        private void LoadCompanyCodes() 
        {
            string sql = "select * from station where lb='1'";//lb==1表示路公司代码

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {           
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadCompanyCodes()=>获取SqliteHandler实例失败");
                return;
            }

            string error;
            List<ASSISTICE.Station> stations = sqliteHandler.SearcherStation(sql, out error);
            if (stations == null) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "FrmSeverMain::LoadCompanyCodes()=>调用SearcherStation异常:{0}",error);
                return;
            }
            if (stations.Count == 0)
                return;

            this.cboCompanyCode.Items.Clear();
            foreach (ASSISTICE.Station item in stations)
            {
                this.cboCompanyCode.Items.Add(string.Format("{0}{1}",item.bm,item.mc));//bm+mc
            }

            if (this.cboCompanyCode.Items.Count > 0)
                this.cboCompanyCode.SelectedIndex = 0;

        }

        //加载站点列表
        private void LoadPlazeCodes() 
        {
            if (string.IsNullOrWhiteSpace(this.cboCompanyCode.Text))
                return;

            if (this.cboCompanyCode.Text.Length < 3)
                return;
            //提取bm
            string bm = this.cboCompanyCode.Text.Substring(0, 3);

            string sql = string.Format("select * from station where lgs='{0}'", bm);

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadCompanyCodes()=>获取SqliteHandler实例失败");
                return;
            }

            string error;
            List<ASSISTICE.Station> stations = sqliteHandler.SearcherStation(sql, out error);
            if (stations == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "FrmSeverMain::LoadCompanyCodes()=>调用SearcherStation异常:{0}", error);
                return;
            }
            if (stations.Count == 0)
                return;

            this.cboPlazCode.Items.Clear();
            foreach (ASSISTICE.Station item in stations)
            {
                this.cboPlazCode.Items.Add(string.Format("{0}{1}", item.bm, item.mc));//bm+mc
            }

            if (this.cboPlazCode.Items.Count > 0)
                this.cboPlazCode.SelectedIndex = 0;
        }

        /// <summary>
        /// 将配置文件中的路公司信息和站点信息绑定到站点设置界面
        /// </summary>
        private void BindStationParamToUI() 
        {
             //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadCompanyCodes()=>获取SysConfig实例失败");
                return;
            }

            this.cboCompanyCode.SelectedItem = sysConfig.mCompanyCode;
            this.cboPlazCode.SelectedItem = sysConfig.mPlazCode;
        }

        private void cboCompanyCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadPlazeCodes();
        }

        /// <summary>
        /// 站点更新到配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStationUpdate_Click(object sender, EventArgs e)
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadNewPlatteExportConfigToUI()=>获取SysConfig实例失败");
                return;
            }

            sysConfig.mCompanyCode = this.cboCompanyCode.Text;
            sysConfig.mPlazCode = this.cboPlazCode.Text;
           

            //更新到磁盘
            SysConfig.SaveObjectToFile(sysConfig);
        }

        //手动更新
        private void btnSoftUpdate_Click(object sender, EventArgs e)
        {

            string oldText = this.btnSoftUpdate.Text;
            this.btnSoftUpdate.Text = "下载中";
            this.btnSoftUpdate.Enabled = false;

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::btnSoftUpdate_Click()=>获取SysConfig实例失败");
                this.btnSoftUpdate.Text = oldText;
                this.btnSoftUpdate.Enabled = true;
                return;
            }
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(stat =>
            {
                //当前版本号
                long currentVersion = SoftwareUpdateThread.GetLocalSerialNumber(System.IO.Path.Combine(Helper.GetCurrentProcessPath(), "PublishRecord.txt"));
                //本地下载路径
                string localDownDir = System.IO.Path.Combine(Helper.GetCurrentProcessPath(), sysConfig.mUpdateFolderName);

                SoftwareUpdate softwareUpdate = new SoftwareUpdate(ASSISTUPDATEMODULEICE.UpdateType.Server);

                string folderPath;
                string error;
                if (softwareUpdate.UpdateDownload(currentVersion, localDownDir, out folderPath, out error) == UpdateDownloadResult.Downloaded)
                {
                    this.UpdateSotfwareNote(UpdateDownloadResult.Downloaded, folderPath, error);
                }
                else 
                {
                    this.Invoke(new Action(()=>{
                        this.btnSoftUpdate.Text = oldText;
                        this.btnSoftUpdate.Enabled = true;
                    }));
                }

            }));

            thread.IsBackground = true;
            thread.Name = "手动更新软件线程";
            thread.Start(null);
           

        }

        //加载系统版本信息到界面
        private void LoadSoftVersionToUI()
        {  
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadSoftVersionToUI()=>获取SysConfig实例失败");
                return;
            }
            //当前版本号
            long currentVersion = SoftwareUpdateThread.GetLocalSerialNumber(System.IO.Path.Combine(Helper.GetCurrentProcessPath(), "PublishRecord.txt"));
            this.lblCurrVersion.Text = string.Format("当前版本:{0}",currentVersion);
            if (currentVersion > 0)
            {
                //转换当前版本号到日期时间格式
                DateTime lastUpdateTime = Helper.GetDateTime(currentVersion * 1000);
                this.lblLastUpdateTime.Text = string.Format("更新时间:{0}",lastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        #region 系统心跳服务

        private HeartBeatClient mHeartBeatClient = null;
        private System.Threading.Thread mHeartBaetThread = null;
        private bool mHeartBaetThreadIsStop = false;
        // <summary>
        /// 心跳发送线程函数
        /// 【线程函数】
        /// </summary>
        /// <param name="stat">SysConfig对象</param>
        private void SendHeartBeatThreadFunc(object stat)
        {

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::SendHeartBeatThreadFunc()=>获取SysConfig实例失败");
                return;
            }

            if (!sysConfig.mEnableHeartBeat)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "FrmSeverMain::SendHeartBeatThreadFunc()=>未启用心跳服务,心跳发送线程退出");
                return;
            }

            //解析IP地址和端口
            string remoteIP = sysConfig.mMonitorServerEndPoint.Substring(0, sysConfig.mMonitorServerEndPoint.IndexOf(':'));
            int port = int.Parse(sysConfig.mMonitorServerEndPoint.Substring(sysConfig.mMonitorServerEndPoint.IndexOf(':') + 1));

            //心跳客户端对象实例化
            this.mHeartBeatClient = new HeartBeatClient(remoteIP, port);

            HeartBeatData hbd=new HeartBeatData();
            hbd.ProcessId=System.Diagnostics.Process.GetCurrentProcess().Id;
            hbd.HeartBeatInterval=sysConfig.mHeartBeatIntervalOfSecond;


            while (!this.mHeartBaetThreadIsStop)
            {

                hbd.CpuUsageRate=this.mProcessCPURate;
                hbd.MemoryUsageRate=this.mProcessMemRate;
                hbd.Flag= HeartBeatFlag.None;

                mHeartBeatClient.SendHeartBeat(hbd);

                for (int i = 0; i < sysConfig.mHeartBeatIntervalOfSecond&&(!this.mHeartBaetThreadIsStop); i++)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                
            }



        }

        #endregion

        #region 进程资源占用情况统计

        private double mSysCPURate, mSysMemRate, mProcessCPURate, mProcessMemRate;

        //进程资源占用情况统计线程类
        private ProcessResourceMonitorThread mProcessResourceMonitorThread = null;

        /// <summary>
        /// 进程资源占用情况
        /// </summary>
        /// <param name="sysCPURate">系统CPU占用比[0~1]之间的值</param>
        /// <param name="sysMemRate">系统内存占用比[0~1]之间的值</param>
        /// <param name="processCPURate">当前进程CPU占用比[0~1]之间的值</param>
        /// <param name="processMemRate">当前进程内存占用比[0~1]之间的值</param>
        public void ProcessResource(double sysCPURate, double sysMemRate, double processCPURate, double processMemRate)
        {
            this.mSysCPURate = sysCPURate;
            this.mSysMemRate = sysMemRate;

            this.mProcessCPURate = processCPURate;
            this.mProcessMemRate = processMemRate;

            string usage = string.Format("进程CPU使用率:{0}% 进程内存使用率:{1}%",(processCPURate*100).ToString("F2"),(processMemRate*100).ToString("F2"));

            UpdateResourceUsageToUI(usage);
        }

        private Action<string> ShowResourceUsage;
        private void UpdateResourceUsageToUI(string usage) 
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(ShowResourceUsage, usage);
            }
            else 
            {
                this.tsslblResource.Text = usage;
            }
        }

        #endregion

        //车辆记录导出应用
        private void btnCarRecordExportApply_Click(object sender, EventArgs e)
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::btnNewPlatteExportApply_Click()=>获取SysConfig实例失败");
                return;
            }

            sysConfig.mAutoUploadCarRecord = this.chkCarRecordAutoExport.Checked;
            switch (this.cboNewPlatteExportFrequency.Text)
            {
                case "按天导出": sysConfig.mUploadCarRecordFrequency = AutoUploadFrequency.Day; break;
                case "按周导出": sysConfig.mUploadCarRecordFrequency = AutoUploadFrequency.Week; break;
                case "按月导出": sysConfig.mUploadCarRecordFrequency = AutoUploadFrequency.Month; break;
            }

            //更新到磁盘
            SysConfig.SaveObjectToFile(sysConfig);
        }

        //消费记录导出应用
        private void btnCustomRecordExportApply_Click(object sender, EventArgs e)
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::btnCustomRecordExportApply_Click()=>获取SysConfig实例失败");
                return;
            }

            sysConfig.mAutoUploadCustomRecord = this.chkCustomRecordAutoExport.Checked;
            switch (this.cboCustomRecordExportFrequency.Text)
            {
                case "按天导出": sysConfig.mUploadCustomRecordFrequency = AutoUploadFrequency.Day; break;
                case "按周导出": sysConfig.mUploadCustomRecordFrequency = AutoUploadFrequency.Week; break;
                case "按月导出": sysConfig.mUploadCustomRecordFrequency = AutoUploadFrequency.Month; break;
            }

            //更新到磁盘
            SysConfig.SaveObjectToFile(sysConfig);
        }

        //中心共享文件夹设置应用
        private void btnCenterShareApply_Click(object sender, EventArgs e)
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::btnCenterShareApply_Click()=>获取SysConfig实例失败");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtRemoteAddress.Text) || string.IsNullOrWhiteSpace(this.txtUserName.Text) || string.IsNullOrWhiteSpace(this.txtUserName.Text))
                return;



            if(!string.IsNullOrWhiteSpace(this.txtRemoteAddress.Text))
                sysConfig.mCenterShareFolderAddress =this.txtRemoteAddress.Text.Trim();
            if (!string.IsNullOrWhiteSpace(this.txtUserName.Text))
                sysConfig.mCenterShareFolderUser = this.txtUserName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(this.txtUserName.Text))
                sysConfig.mCenterShareFolderPassword = this.txtPassword.Text.Trim();

            //更新到磁盘
            SysConfig.SaveObjectToFile(sysConfig);
        }

        private void FrmSeverMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
               
                this.mProcessResourceMonitorThread.Stop();
                this.mProcessResourceMonitorThread.WaitThreadExit(1000);

                Service.StopService();
              
            }
        }

        //中心数据库设置应用
        private void btnCenterDBApply_Click(object sender, EventArgs e)
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::btnCenterShareApply_Click()=>获取SysConfig实例失败");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtCenterDBIP.Text) || string.IsNullOrWhiteSpace(this.txtCenterDBUser.Text) || string.IsNullOrWhiteSpace(this.txtCenterDBPwd.Text))
                return;



            if (!string.IsNullOrWhiteSpace(this.txtCenterDBIP.Text))
                sysConfig.mCenterMSSQLAddress = this.txtCenterDBIP.Text.Trim();
            if (!string.IsNullOrWhiteSpace(this.txtCenterDBUser.Text))
                sysConfig.mCenterMSSQLUser = this.txtCenterDBUser.Text.Trim();
            if (!string.IsNullOrWhiteSpace(this.txtCenterDBPwd.Text))
                sysConfig.mCenterMSSQLPassword = this.txtCenterDBPwd.Text.Trim();

            //更新到磁盘
            SysConfig.SaveObjectToFile(sysConfig);
        }

        /// <summary>
        /// 加载监视级别到查询UI
        /// </summary>
        private void LoadMonLevelToQueryUI() 
        {
            Dictionary<string, int> monLevel = IDbHandler.MySqlHandler.GetMonLevel();
            this.cboMonLevel.Items.Clear();
            foreach (var item in monLevel)
            {
                this.cboMonLevel.Items.Add(item.Key);
            }
            if (this.cboMonLevel.Items.Count > 0) 
            {
                this.cboMonLevel.SelectedIndex = 0;
            }
            this.cboMonLevel.Tag = monLevel;
        }

    }
}
