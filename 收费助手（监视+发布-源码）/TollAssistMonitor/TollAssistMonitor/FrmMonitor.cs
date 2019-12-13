using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Management;
using Microsoft.Win32;
using HeartBeat;


namespace TollAssistMonitor
{



    public partial class FrmMonitor : Form, IHeartBeatNote
    {
        public FrmMonitor()
        {
            InitializeComponent();

            //窗体图标
            this.Icon = global::TollAssistMonitor.Properties.Resources.anieye01;

            //窗口启动位置
            this.StartPosition = FormStartPosition.CenterScreen;

            //窗体默认大小
            this.mDefaultSize = this.Size;


            //窗口置顶
            //this.TopMost = true;

            //系统托盘图标相关设置
            this.notifyIconTask.Icon = Icons[0];
            this.notifyIconTask.Text = "监视工具";
            this.notifyIconTask.Visible = true;

            this.notifyIconTask.BalloonTipTitle = "提示";
            this.notifyIconTask.BalloonTipText = "我在这里";
            this.notifyIconTask.BalloonTipIcon = ToolTipIcon.Info;

            this.notifyIconTask.MouseDoubleClick += new MouseEventHandler(notifyIconTask_MouseDoubleClick);

        }

     

        //#region WIN32API

        //private const uint LIST_MODULES_32BIT = 0x01;
        //private const uint LIST_MODULES_64BIT = 0x02;
        //private const uint LIST_MODULES_ALL = 0x03;
        //private const uint PROCESS_VM_READ = (0x0010);
        //private const uint PROCESS_QUERY_INFORMATION = (0x0400);
        //private const uint PROCESS_QUERY_LIMITED_INFORMATION = (0x1000);
        //private const uint PROCESS_ALL_ACCESS = (0x000F0000) | (0x00100000) | (0xFFFF);

        //[DllImport("Kernel32.dll")]
        //private static extern IntPtr OpenProcess(uint dwDesiredAccess, uint bInheritHandle, int dwProcessId);

        //[DllImport("Kernel32.dll")]
        //private static extern int CloseHandle(IntPtr hObject);

        //[DllImport("Psapi.dll")]
        //private static extern int EnumProcessModulesEx(IntPtr hProcess, IntPtr[] lphModule, int cb, ref int lpcbNeeded, uint dwFilterFlag);


        //[DllImport("Psapi.dll")]
        //private static extern uint GetModuleFileNameExA(IntPtr hProcess, IntPtr hModule, StringBuilder sb, uint nSize);

        //[DllImport("Kernel32.dll")]
        //private static extern int QueryFullProcessImageNameA(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

        //[DllImport("Kernel32.dll")]
        //private static extern uint GetLastError();

        //#endregion

        #region 进程相关接口

        /// <summary>
        /// 获取系统总物理内存
        /// </summary>
        /// <returns></returns>
        private long TotalMemory()
        {
            //获得物理内存
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    return long.Parse(mo["TotalPhysicalMemory"].ToString());
                }
            }

            return 0;
        }

        ///// <summary>
        ///// 获取进程的路径
        ///// </summary>
        ///// <param name="processId"></param>
        ///// <param name="processPath"></param>
        ///// <returns></returns>
        //private bool GetProcessPath(int processId, out string processPath)
        //{
        //    processPath = null;
        //    bool ret = false;

        //    IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, 0, processId);
        //    if (processHandle == IntPtr.Zero)
        //    {
        //        return false;
        //    }

        //    StringBuilder pathSBuilder = new StringBuilder(1024);

        //    //尝试采用以下方式获取程序路径
        //    pathSBuilder.Clear();
        //    uint lpdwSize = 1024;

        //    if (QueryFullProcessImageNameA(processHandle, 0, pathSBuilder, ref lpdwSize) != 0)//获取路径成功
        //    {
        //        ret = true;
        //        processPath = pathSBuilder.ToString();
        //        goto EXIT;
        //    }

        //    //尝试采用以下方式再次获取路径

        //    IntPtr[] lphModule = new IntPtr[1024];
        //    int lpcbNeeded = 0;

        //    if (EnumProcessModulesEx(processHandle, lphModule, IntPtr.Size * lphModule.Length, ref lpcbNeeded, LIST_MODULES_ALL) == 0)//枚举模块失败
        //    {
        //        //uint er = GetLastError();
        //        goto EXIT;
        //    }

        //    if (GetModuleFileNameExA(processHandle, lphModule[0], pathSBuilder, 1024) != 0) //获取成功
        //    {
        //        ret = true;
        //        processPath = pathSBuilder.ToString();
        //    }

        //EXIT:
        //    CloseHandle(processHandle);
        //    return ret;
        //}


        ///// <summary>
        ///// 获取进程的路径
        ///// </summary>
        ///// <param name="processId"></param>
        ///// <param name="processPath"></param>
        ///// <returns></returns>
        //private string GetProcessPath(int processId)
        //{
        //    string processPath = null;

        //    IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, 0, processId);
        //    if (processHandle == IntPtr.Zero)
        //    {
        //        return processPath;
        //    }

        //    StringBuilder pathSBuilder = new StringBuilder(1024);

        //    //尝试采用以下方式获取程序路径
        //    pathSBuilder.Clear();
        //    uint lpdwSize = 1024;
        //    if (QueryFullProcessImageNameA(processHandle, 0, pathSBuilder, ref lpdwSize) != 0)//获取路径成功
        //    {
        //        processPath = pathSBuilder.ToString();
        //        goto EXIT;
        //    }

        //    //尝试采用以下方式再次获取路径

        //    IntPtr[] lphModule = new IntPtr[1024];
        //    int lpcbNeeded = 0;

        //    if (EnumProcessModulesEx(processHandle, lphModule, IntPtr.Size * lphModule.Length, ref lpcbNeeded, LIST_MODULES_ALL) == 0)//枚举模块失败
        //    {
        //        //uint er = GetLastError();
        //        goto EXIT;
        //    }

        //    if (GetModuleFileNameExA(processHandle, lphModule[0], pathSBuilder, 1024) != 0) //获取成功
        //    {
        //        processPath = pathSBuilder.ToString();
        //    }

        //EXIT:
        //    CloseHandle(processHandle);
        //    return processPath;
        //}

        /// <summary>
        /// 获取进程的名称
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        private string GetProcessName(int processId)
        {
            string processName = string.Empty;
            try
            {
                Process process = Process.GetProcessById(processId);
                if (process != null)
                {
                    processName = process.ProcessName;
                    return processName;
                }
                process.Close();//是否需要关闭
            }
            catch (Exception ex)
            {

            }
            return processName;
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="exePath"></param>
        private void StartProcess(string exePath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = exePath;
                psi.WorkingDirectory = System.IO.Path.GetDirectoryName(exePath);

                Process process = Process.Start(psi);

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        private bool KillProcessById(int processId)
        {
            bool ret = false;
            try
            {
                Process process = Process.GetProcessById(processId);
                if (process != null)
                {
                    process.Kill();
                }
                process.Close();//是否需要关闭

                ret = true;
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        #endregion

      
        #region 闹钟辅助封装类

        /// <summary>
        /// 闹钟处理参数
        /// </summary>
        public sealed class AlarmProcessParams
        {
            /// <summary>
            /// 进程ID
            /// </summary>
            public int mProcessId;

            /// <summary>
            /// 闹钟ID
            /// </summary>
            public int mAlarmId;
            /// <summary>
            /// 已投递次数
            /// </summary>
            public int mPostCount;
        }

        #endregion

        #region 私有字段定义

        private long mTotalPhysicalMemory = 0;//总物理内存
        private MonitorItemCollections mMonitorItemCollections = null;//监视项集合
        private HeartBeatServer mHeartBeatServer = null;//心跳服务
        private AlarmTools.Alarm mAlarm = new AlarmTools.Alarm();//闹钟对象

        //选择文件对话框
        private OpenFileDialog mFileDialog = new OpenFileDialog();


        #endregion

        #region IHeartBeatNote接口实现

        /// <summary>
        /// IHeartBeatNote接口实现
        /// 心跳包通知
        /// </summary>
        /// <param name="data"></param>
        public void HeartBeatNote(HeartBeatData data)
        {
            string exePath=TollAssistComm.Helper.GetProcessPath(data.ProcessId);
            if (string.IsNullOrWhiteSpace(exePath))
                return;
            //查询是否在监视列表中
            MonitorItem monitor = this.mMonitorItemCollections.mMonitorItem.Find(item => item.ProgramPath == exePath);
            if (monitor == null) //不在监视列表中则丢弃
                return;

            if (data.Flag == HeartBeatFlag.Suspend) //挂起
            {
                //将进程路径添加到临时挂起队列中
                this.cancelMonitorPaths[exePath] = DateTime.Now;

                DeleteHeartBeatItemFromListView(data.ProcessId);
                return;
            }

            if (this.cancelMonitorPaths.ContainsKey(exePath)) //取消挂起
            {
                DateTime dt;
                this.cancelMonitorPaths.TryRemove(exePath, out dt);
            }

            int alarmId = 0;
            //查看定时器是否存在
            if (mAlarmOfProcessID.ContainsKey(data.ProcessId))
            {
                ////重置相关值
                //mAlarmOfProcessID[data.ProcessId].mPostCount = 0;
                ////重置闹钟
                //if (!mAlarm.ResetAlarm(mAlarmOfProcessID[data.ProcessId].mAlarmId, data.HeartBeatInterval*1000))
                //{
                   
                //    //闹钟已被删除
                //    //创建新的闹钟
                //    mAlarm.AddAlarmEvent(data.HeartBeatInterval * 1000, DoAlarmProcess, mAlarmOfProcessID[data.ProcessId], out alarmId);
                //    mAlarmOfProcessID[data.ProcessId].mAlarmId = alarmId;
                //}

                //重置相关值
                mAlarmOfProcessID[data.ProcessId].mPostCount = 0;
                //重置闹钟
                mAlarm.ResetAlarm(mAlarmOfProcessID[data.ProcessId].mAlarmId, data.HeartBeatInterval * 1000);
                

            }
            else
            {
                //创建新的闹钟
                AlarmProcessParams alarmProcessParams = new AlarmProcessParams() { mPostCount = 0, mProcessId = data.ProcessId };
                if (mAlarm.AddAlarmEvent(data.HeartBeatInterval * 1000, DoAlarmProcess, alarmProcessParams, out alarmId))
                {
                    alarmProcessParams.mAlarmId = alarmId;
                    mAlarmOfProcessID[data.ProcessId] = alarmProcessParams;
                }
            }

            //将此对象发送到UI监视项界面中
            AddHeartBeatDataToListView(data);

        }

        #endregion

        #region 闹钟函数处理实现

        //闹钟与进程ID的关联
        //key:进程ID
        //value:闹钟处理参数
        private ConcurrentDictionary<int, AlarmProcessParams> mAlarmOfProcessID = new ConcurrentDictionary<int, AlarmProcessParams>();

        //时钟处理函数
        //param:用户参数
        //alarmId://闹钟Id
        //返回值:如果返回true则继续关注此时钟，如果返回false，取消时钟关注
        private bool DoAlarmProcess(Object param, int alarmId)
        {
            AlarmProcessParams alarmProcessParams = param as AlarmProcessParams;
            if (alarmProcessParams == null)
                return false;   

            if (alarmProcessParams.mPostCount > this.mMonitorItemCollections.mMaxWaitCount)//超过指定次数
            {
                //处理异常
                string exePath = TollAssistComm.Helper.GetProcessPath(alarmProcessParams.mProcessId);
                if (this.mMonitorItemCollections.mMonitorItem.Count(item=>item.ProgramPath==exePath)>0) //查询进程是否还在监视项中
                {
                    if (!this.cancelMonitorPaths.ContainsKey(exePath)) //不在临时取消监视列表中
                    {
                        //终止进程
                        KillProcessById(alarmProcessParams.mProcessId);
                    }

                    
                }
                this.mAlarmOfProcessID.TryRemove(alarmProcessParams.mProcessId, out alarmProcessParams);//将此进程的注册信息删除

                DeleteHeartBeatItemFromListView(alarmProcessParams.mProcessId);//从详细列表中删除
                return false;
            }
            else
            {
                alarmProcessParams.mPostCount++;
                //继续关注
                return true;
            }

        }


        #endregion

        #region 辅助函数

        private Action<HeartBeatData> mHeartBeatInvokeRequired = null;//心跳数据添加到UI界面委托

        private void AddHeartBeatDataToListView(HeartBeatData data)
        {
            if (this.lstViewHeartBeat.InvokeRequired)
            {
                this.lstViewHeartBeat.Invoke(mHeartBeatInvokeRequired, data);
            }
            else
            {
                ListViewItem[] searchRet = this.lstViewHeartBeat.Items.Find(data.ProcessId.ToString(), false);
                if (searchRet == null || searchRet.Length == 0)
                {
                    //添加新项
                    ListViewItem newListViewItem = new ListViewItem(data.ProcessId.ToString());
                    newListViewItem.Name = data.ProcessId.ToString();
                    newListViewItem.SubItems.AddRange(new string[] { GetProcessName(data.ProcessId), string.Format("{0}%", data.CpuUsageRate.ToString("F02"))
                    ,string.Format("{0}%",data.MemoryUsageRate.ToString("F02")),DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")});

                    this.lstViewHeartBeat.Items.Add(newListViewItem);
                }
                else
                {
                    //修改现有项
                    ListViewItem tagListViewItem = searchRet[0];
                    tagListViewItem.Text = data.ProcessId.ToString();
                    tagListViewItem.Name = data.ProcessId.ToString();
                    tagListViewItem.SubItems[1].Text = GetProcessName(data.ProcessId);
                    tagListViewItem.SubItems[2].Text = string.Format("{0}%", data.CpuUsageRate.ToString("F02"));
                    tagListViewItem.SubItems[3].Text = string.Format("{0}%", data.MemoryUsageRate.ToString("F02"));
                    tagListViewItem.SubItems[4].Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                }
            }
        }


        //从文件读取监视项集合并反序列化为对象
        private MonitorItemCollections ReadMonitorItemsFromFile()
        {
            MonitorItemCollections monitorItems = null;
            //从文件读取监视项集合并反序列化为对象
            XmlSerializer mySerializer =
            new XmlSerializer(typeof(MonitorItemCollections));

            if (!System.IO.File.Exists("MonitorItems.xml"))
            {
                monitorItems = new MonitorItemCollections() { mListenPort = 12580, mMaxWaitCount=3 };
                return monitorItems;
            }

            using (FileStream myFileStream = new FileStream("MonitorItems.xml", FileMode.Open))
            {
                monitorItems = (MonitorItemCollections)
                mySerializer.Deserialize(myFileStream);
            }

            return monitorItems;
        }

        private void SaveMonitorItemsToFile(MonitorItemCollections monitorItems)
        {
            if (monitorItems == null)
                return;
            XmlSerializer mySerializer =
           new XmlSerializer(typeof(MonitorItemCollections));
            // To read the file, create a FileStream.
            using (FileStream myFileStream = new FileStream("MonitorItems.xml", FileMode.Create, FileAccess.Write))
            {
                mySerializer.Serialize(myFileStream, monitorItems);
            }
        }

        private void AddMonitorItemToListView(string programPath)
        {
            if (string.IsNullOrWhiteSpace(programPath))
                return;
            //编号
            ListViewItem codeListViewItem = new ListViewItem((this.lstViewMonitorItems.Items.Count + 1).ToString());
            //程序路径
            codeListViewItem.SubItems.Add(programPath);
            this.lstViewMonitorItems.Items.Add(codeListViewItem);

        }

        /// <summary>
        /// 更新详细列表视图
        /// </summary>
        private void UpdateListViewDetails() 
        {

            this.lstViewDetails.Items.Clear();
            string path = this.lstViewDetails.Tag as string;
            if (path == null) 
                return;

            
            //遍历进程列表找出与path相匹配的进程对象进行显示
            Process[] processes = Process.GetProcesses();
            var tagProcesses = from process in processes where TollAssistComm.Helper.GetProcessPath(process.Id) == path select process;
            foreach (Process item in tagProcesses)
            {
                ListViewItem processIdListViewItem = new ListViewItem(item.Id.ToString());
                processIdListViewItem.SubItems.Add(item.ProcessName);
                processIdListViewItem.SubItems.Add(((int)(((double)item.WorkingSet64 / this.mTotalPhysicalMemory) * 100)).ToString("D02") + "%");
                processIdListViewItem.SubItems.Add((item.WorkingSet64 / 1024.0).ToString("F02") + "KB");

                this.lstViewDetails.Items.Add(processIdListViewItem);
            }

            //ListViewItem item = null;
            //string strProcessId = null;
            //int processId = 0;
            //Process p = null;
            //for(int i=0;i< this.lstViewDetails.Items.Count;++i)
            //{
            //    item = this.lstViewDetails.Items[i];
            //    strProcessId = item.Text;
            //    processId = int.Parse(strProcessId);
            //    try
            //    {
            //       p = Process.GetProcessById(processId);
            //    }
            //    catch (ArgumentException ex)
            //    {
            //        //此进程已经不存在则将其删除
            //        item.Remove();
            //        i--;
            //        continue;
            //    }
            //    item.SubItems[2].Text = (((int)(((double)p.WorkingSet64 / this.mTotalPhysicalMemory) * 100)).ToString("D02") + "%");
            //    item.SubItems[3].Text = ((p.WorkingSet64 / 1024.0).ToString("F02") + "KB");
                
            //    p.Close();
            //}
        }

        /// <summary>
        /// 更新监视列表视图中的行编号
        /// </summary>
        private void UpdateListViewMonitorCode() 
        {
            for (int i = 0; i < this.lstViewMonitorItems.Items.Count;++i )
            {
                this.lstViewMonitorItems.Items[i].Text = (i + 1).ToString();
            }
        }

        /// <summary>
        /// 删除心跳列表中的项
        /// </summary>
        /// <param name="processId"></param>
        private void DeleteHeartBeatItemFromListView(int processId) 
        {
            if (this.lstViewHeartBeat.InvokeRequired)
            {
                this.lstViewHeartBeat.Invoke(new Action<int>(DeleteHeartBeatItemFromListView), processId);
            }
            else 
            {
                string strProcessId = processId.ToString();
                for (int i = 0; i < this.lstViewHeartBeat.Items.Count; ++i)
                {
                    if (this.lstViewHeartBeat.Items[i].Text == strProcessId)
                    {
                        this.lstViewHeartBeat.Items[i].Remove();
                        i--;
                    }
                }
            }

          
        }

        /// <summary>
        /// 为指定的程序创建快捷方式,并返回创建的快捷方式的完整路径
        /// </summary>
        /// <param name="path">可执行文件的完整路径</param>
        /// <param name="shortcutName">快捷方式的名称</param>
        /// <param name="shortcutPath">快捷方式的完整路径</param>
        /// <returns>如果快捷方式已经存在或者输入参数无效则返回false</returns>
        private bool CreateShortcut(string path,string shortcutName,out string shortcutPath) 
        {
            shortcutPath = null;
            if (string.IsNullOrWhiteSpace(path))
                return false;

            if (!System.IO.File.Exists(path))
                return false;
            string extensionName=System.IO.Path.GetExtension(path);
            if(string.IsNullOrWhiteSpace(extensionName)||extensionName!=".exe")
                return false;
            string exeName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrWhiteSpace(exeName))
                return false;

            if (string.IsNullOrWhiteSpace(shortcutName)) 
            {
                shortcutName = exeName;
            }

            string exeDir = System.IO.Path.GetDirectoryName(path);
            string shortcutFullPath = System.IO.Path.Combine(exeDir, shortcutName+".lnk");

            //if (System.IO.File.Exists(shortcutFullPath))
            //    return false;


            int windowStyle = 1;//1:普通 3:最大化 7:最小化

            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutFullPath);
            shortcut.TargetPath = path;
            shortcut.WorkingDirectory = exeDir;
            shortcut.WindowStyle = windowStyle;
            shortcut.Description = "Auto Create";
            shortcut.Save();

            shortcutPath = shortcutFullPath;

            return true;
        }

        /// <summary>
        /// 将程序设置为自动启动
        /// 备注：如果已经存在该项不会添加，如果不存在则添加
        /// </summary>
        /// <param name="keyName">注册表项名称(程序名称)</param>
        /// <param name="path">要自动启动的程序的完整路径</param>
        private void SetAutoStart(string keyName,string path) 
        {
            if (string.IsNullOrWhiteSpace(keyName) || string.IsNullOrWhiteSpace(path))
                return;
            string regeditItemPath = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run";
            try
            {
                Object objPath = Registry.GetValue(regeditItemPath, keyName, null);
                if (objPath== null)
                {
                    //添加到注册表中
                    Registry.SetValue(regeditItemPath, keyName, path, RegistryValueKind.String);
                }
                else 
                {
                    string strPath = objPath as string;
                    if (strPath != path)
                    {
                        //更新注册表
                        Registry.SetValue(regeditItemPath, keyName, path, RegistryValueKind.String);
                    }
                }
            }
            catch (Exception ex)
            {
                
                
            }
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
            if (CreateShortcut(path, keyName, out shortcutPath)) 
            {
                SetAutoStart(keyName, shortcutPath);
            }

        }

        #endregion


        #region UI相关事件实现

        private void FrmMonitor_Load(object sender, EventArgs e)
        {
            //心跳数据添加到UI界面委托
            mHeartBeatInvokeRequired = new Action<HeartBeatData>(AddHeartBeatDataToListView);

            //获取系统物理内存
            mTotalPhysicalMemory = this.TotalMemory();

            //获取监视项
            mMonitorItemCollections = ReadMonitorItemsFromFile();
            //绑定集合到显示界面
            foreach (MonitorItem item in this.mMonitorItemCollections.mMonitorItem)
            {
                AddMonitorItemToListView(item.ProgramPath);
            }

            //启动心跳服务
            string error;
            this.mHeartBeatServer = new HeartBeatServer(this.mMonitorItemCollections.mListenPort, this);
            this.mHeartBeatServer.Start(out error);

            //启动监视timer
            this.timer1.Enabled = true;


            //将程序添加到启动项中
            SetAutoStartup("AssistMonitorTools", TollAssistComm.Helper.GetProcessPath(Process.GetCurrentProcess().Id));

            //string path;
            //GetProcessPath(336, out path);

            this.timerStartHide.Enabled = true;

        }
       
        /// <summary>
        /// 添加监视
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlmsAddMonitor_Click(object sender, EventArgs e)
        {
            this.mFileDialog.Filter = "可执行文件(*.exe)|*.exe";
            if (this.mFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (ListViewItem item in this.lstViewMonitorItems.Items)
                {
                    if (item.SubItems[1].Text == this.mFileDialog.FileName)
                    {
                        MessageBox.Show("选择的程序已经在监视列表中", "提示");
                        return;
                    }
                }

                string selfProcessPath = TollAssistComm.Helper.GetProcessPath(Process.GetCurrentProcess().Id);
                if (this.mFileDialog.FileName == selfProcessPath) 
                {
                    MessageBox.Show("不能将自身进程添加到监视中", "提示");
                    return;
                }


                //不存在则添加到列表中
                AddMonitorItemToListView(this.mFileDialog.FileName);
                UpdateListViewMonitorCode();//更新行编号


                this.mMonitorItemCollections.mMonitorItem.Add(new MonitorItem() { ProgramPath = this.mFileDialog.FileName });
                //保存到文件
                SaveMonitorItemsToFile(this.mMonitorItemCollections);

            }
        }

        //删除监视
        private void tlmsReomveMonitor_Click(object sender, EventArgs e)
        {
            if (this.lstViewMonitorItems.SelectedItems == null || this.lstViewMonitorItems.SelectedItems.Count == 0)
                return;

            //移除选定项
            ListViewItem selectItem = this.lstViewMonitorItems.SelectedItems[0];
            if (selectItem.SubItems == null || selectItem.SubItems.Count == 0)
                return;
            string path = selectItem.SubItems[1].Text;
            if (string.IsNullOrWhiteSpace(path))
                return;

            //从界面控件移除
            selectItem.Remove();
            UpdateListViewMonitorCode();//更新行编号

            ////将详细列表中包含的进程信息从心跳列表中移除
            //foreach (ListViewItem item in this.lstViewDetails.Items)
            //{
            //    int processId = int.Parse(item.Text);
            //    //心跳列表中移除
            //    DeleteHeartBeatItemFromListView(processId);

            //    //删除相关注册信息 
            //    AlarmProcessParams param = null;
            //    this.mAlarmOfProcessID.TryRemove(processId,out param);//将此进程的注册信息删除
            //}


            //移除详细列表
            this.lstViewDetails.Tag = null;


            //从集合中移除
            int index = this.mMonitorItemCollections.mMonitorItem.FindIndex(item => item.ProgramPath == path);
            this.mMonitorItemCollections.mMonitorItem.RemoveAt(index);

            //保存到文件
            SaveMonitorItemsToFile(this.mMonitorItemCollections);

        }

        private void lstViewMonitorItems_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.lstViewMonitorItems.SelectedItems == null || this.lstViewMonitorItems.SelectedItems.Count == 0)
                return;
            //获取选定项路径信息
            ListViewItem selectItem = this.lstViewMonitorItems.SelectedItems[0];
            if (selectItem.SubItems == null || selectItem.SubItems.Count == 0)
                return;
            string path = selectItem.SubItems[1].Text;//选定项路径信息
            if (string.IsNullOrWhiteSpace(path))
                return;

            this.lstViewDetails.Tag = path;//将选择项路径绑定到详细列表视图中

            //this.lstViewDetails.Items.Clear();

            ////遍历进程列表找出与path相匹配的进程对象进行显示
            //Process[] processes = Process.GetProcesses();
            //var tagProcesses = from process in processes where GetProcessPath(process.Id) == path select process;
            //foreach (Process item in tagProcesses)
            //{
            //    ListViewItem processIdListViewItem = new ListViewItem(item.Id.ToString());
            //    processIdListViewItem.SubItems.Add(item.ProcessName);
            //    processIdListViewItem.SubItems.Add(((int)(((double)item.WorkingSet64 / this.mTotalPhysicalMemory) * 100)).ToString("D02")+"%");
            //    processIdListViewItem.SubItems.Add((item.WorkingSet64 / 1024.0).ToString("F02") + "KB");

            //    this.lstViewDetails.Items.Add(processIdListViewItem);
            //}

        }

        private const int MaxSuspendOfMinutes = 10;//最大挂起时间(分钟)
        private ConcurrentDictionary<string, DateTime> cancelMonitorPaths = new ConcurrentDictionary<string, DateTime>();//临时取消监视的路径

        private List<string> monitorPaths = new List<string>();//监视路径
        private void timer1_Tick(object sender, EventArgs e)
        {

            //更新详细列表视图
            UpdateListViewDetails();

            //检查进程运行与否

            monitorPaths.Clear();
            string path = string.Empty;
            foreach (ListViewItem item in this.lstViewMonitorItems.Items)
            {
                //MSDN:ListViewItem.ListViewSubItemCollection 中的第一个子项总是具有子项的项。对集合中的子项执行操作时，务必引用索引位置 1（而不是 0）来更改第一个子项。
                if (item.SubItems.Count >= 2)
                {
                    path = item.SubItems[1].Text;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        this.monitorPaths.Add(path);
                    }
                }
            }

            //遍历当前计算机上所有运行的进程的路径
            var allProcessPath = Process.GetProcesses().Select(process =>
            {
                string procPath = TollAssistComm.Helper.GetProcessPath(process.Id);
                //GetProcessPath(process.Id, out procPath);
                return procPath;
            });

            //筛选出未在运行列表中的路径
            var retPath = monitorPaths.Where(p =>
            {
                return !allProcessPath.Contains(p);
            });

            //启动相关进程
            foreach (var item in retPath)
            {
                if(!this.cancelMonitorPaths.ContainsKey(item))//不在临时取消监视路径中
                    StartProcess(item);
            }

            //检查挂起的监视是否已经超时，超时则删除挂起
            var keys = from item in cancelMonitorPaths where ((DateTime.Now - item.Value).TotalMinutes > MaxSuspendOfMinutes) select item.Key;
            DateTime dt;
            foreach (var item in keys)
            {
                cancelMonitorPaths.TryRemove(item, out dt);
            }
           
        }

        /// <summary>
        /// 关闭按钮被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //默认让窗体最小化到系统托盘
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                   
            }
            else 
            {
                //释放资源
                this.notifyIconTask.Dispose();
                FreeResource();
            }
            
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private void FreeResource()
        {
            string error;
            if (this.mHeartBeatServer != null)
                this.mHeartBeatServer.Stop(out error);

            this.mAlarm.Dispose();

            this.timer1.Enabled = false;
        }

        //系统托盘图标动画
        private Icon[] Icons = { global::TollAssistMonitor.Properties.Resources.anieye01, global::TollAssistMonitor.Properties.Resources.anieye02, global::TollAssistMonitor.Properties.Resources.anieye03,
                               global::TollAssistMonitor.Properties.Resources.anieye04,global::TollAssistMonitor.Properties.Resources.anieye05,global::TollAssistMonitor.Properties.Resources.anieye06,
                               global::TollAssistMonitor.Properties.Resources.anieye07,global::TollAssistMonitor.Properties.Resources.anieye08,global::TollAssistMonitor.Properties.Resources.anieye09,
                               global::TollAssistMonitor.Properties.Resources.anieye10,global::TollAssistMonitor.Properties.Resources.anieye11};
        private int iconIndex = 0;
        private void timerNotifyIcon_Tick(object sender, EventArgs e)
        {
            this.notifyIconTask.Icon = Icons[iconIndex++];
            if (iconIndex == Icons.Length)
                iconIndex = 0;
        }

        /// <summary>
        /// 最大化最小化按钮被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMonitor_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
               
                this.Hide();
                this.notifyIconTask.ShowBalloonTip(2000);
            }
            
            
        }

        /// <summary>
        /// 窗体默认大小
        /// </summary>
        private Size mDefaultSize ;
 

        /// <summary>
        /// 托盘图标被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void notifyIconTask_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)//已经在显示中则返回
                return;

            this.Size = this.mDefaultSize;
            this.SetDesktopLocation((Screen.PrimaryScreen.WorkingArea.Width - this.mDefaultSize.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - this.mDefaultSize.Height) / 2);
            this.Show();
            if (this.WindowState == FormWindowState.Minimized) 
            {
                this.WindowState = FormWindowState.Normal;
            }
           
          
        }

        //显示主界面
        private void tsmlaShowMainForm_Click(object sender, EventArgs e)
        {
            this.notifyIconTask_MouseDoubleClick(null, null);
        }

        //退出程序
        private void tsmlExit_Click(object sender, EventArgs e)
        {
           
            if (MessageBox.Show(this,"确定退出监视吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 控制当窗体起动后最小化到系统托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerStartHide_Tick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.timerStartHide.Enabled = false;
        }

        #endregion

       


      
    }


    /// <summary>
    /// 监视项集合
    /// </summary>
    [Serializable]
    public class MonitorItemCollections
    {
        /// <summary>
        /// 监听端口
        /// </summary>
        public int mListenPort;

        /// <summary>
        /// 闹钟的最大超时等待次数,如果超时次数大于mMaxWaitCount则触发异常机制
        /// </summary>
        public int mMaxWaitCount;

        /// <summary>
        /// 监视项集合
        /// </summary>
        public List<MonitorItem> mMonitorItem = new List<MonitorItem>();
    }

    /// <summary>
    /// 监视项
    /// </summary>
    [Serializable]
    public class MonitorItem
    {
        /// <summary>
        /// 程序路径
        /// </summary>
        public string ProgramPath { get; set; }
    }

   

   

}
