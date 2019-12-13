using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using TollAssistComm;
using Ice_Servant_Factory;
using System.Threading;
using System.Collections.Concurrent;
using CommHandler;
using System.Net.Sockets;
using HeartBeat;
using System.Diagnostics;
using IDbHandler;
using ASSISTICE;
using System.Drawing.Drawing2D;
using System.Collections;
using System.IO;





namespace TollAssistUI
{
    
    
    public partial class FrmMain : Form, IAMI_ICarQuery_QueryCarRecord, IUpdateSotfwareNote, IProcessResourceMonitor, IStationInfoUpdate, ISysConfigChanged
    {



    #region  //   定时器使用的 “user32.dll” api 引用。
        int vincent_k = 0;
        int vincent_i = 0;
        Mutex vincentMutex = new Mutex();   //定时器 互斥变量
        /********** **********/
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
      
      
        [DllImport("user32.dll")]
        public static extern void ShowWindow(System.IntPtr hWnd,int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(int hWnd, int msg, IntPtr wParam, IntPtr lParam);
       
        public const int WM_ACTIVATE = 0x0006;//WM_ACTIVATE消息

        [DllImport("user32.dll")]
        static extern bool PostMessage(int hwnd, int msg, uint wParam, uint lParam);
       
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(Point Point);
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point lpPoint);
        public static IntPtr GetMouseWindow()
        {
            Point p;
            GetCursorPos(out p);
            return WindowFromPoint(p);
        }
        //获取前台窗口
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 模拟鼠标操作
        /// </summary>
        [DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        //移动鼠标 
        const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public static void MouseMove(int x, int y)
        {
            mouse_event(MOUSEEVENTF_MOVE, x, y, 0, 0);
        }
        public static void MouseClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0); //再复制一份则为双击
        }
        public static void MouseDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }
        public static void MouseUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////


        


        //string processName = "SCLane";
       //string processName = "notepad";

        #endregion
        /********** **********/



        public FrmMain()
        {
            InitializeComponent();

            this.mAMI_ICarQuery_QueryCarRecordWrapper = new AMI_ICarQuery_QueryCarRecordWrapper(this);

            this.mFrmCFG = new FrmCFG(this);
            this.mFrmCFG.VisibleChanged += new EventHandler(mFrmCFG_VisibleChanged);
            this.VisibleChanged += new EventHandler(FrmMain_VisibleChanged);

            this.AutoSize = true;

               
            
            
            timer1.Interval = 1000;  //设置定时器时间为1 秒。  3分钟后，改为 3分钟一次
            
            
          


        }



        void FrmMain_VisibleChanged(object sender, EventArgs e)
        {
            this.mFrmMainShow = this.Visible;

            //lhy
            if (this.mFrmMainShow&&!timer1.Enabled)
            {
                timer1.Start();    //   主界面隐藏时，启动定时器。
            }
        }

        void mFrmCFG_VisibleChanged(object sender, EventArgs e)
        {
            this.mFrmCfgShow = this.mFrmCFG.Visible;

            if (this.mFrmCfgShow == false)
                this.Visible = true;
        }

        #region 系统钩子相关函数

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public uint dwExtraInfo;
        }



        private const int WH_KEYBOARD_LL = 13;//键盘钩子
        private const int WM_KEYUP = 0x0101;
        private const int VK_RETURN = 0x0D;//回车
        private const int VK_F4 = 0x73;//F4

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetModuleHandleA(string lpModuleName);

        // 安装钩子

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸载钩子

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // 继续下一个钩子

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, Int32 wParam, IntPtr lParam);

        // 取得当前线程编号

        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        #endregion

        // private static Semaphore gSemaphore = new Semaphore(0, 255);


        private static IntPtr gKeyHook = IntPtr.Zero;//键盘hook句柄
        private static ConcurrentQueue<Keys> gDoKeys = new ConcurrentQueue<Keys>();//存放键盘动作的队列

        private static bool GetKBDLLHOOKSTRUCTByIntPtr(IntPtr p, out KBDLLHOOKSTRUCT keyStruct)
        {
            keyStruct = new KBDLLHOOKSTRUCT();
            try
            {
                keyStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(p, typeof(KBDLLHOOKSTRUCT));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 全局键盘钩子回调函数
        // 参数: action 标识键盘消息(按下,弹起), keyStruct 包含按键信息
        // 注意：钩子回调函数建立在堆栈内存中，最好不要在函数中分配内存、建立对象、弹出窗口等,容易降低软件运行速度
        // 及造成内存溢出（建议通过PostMessage（窗口，消息，NULL，NULL）在宿主线程或窗体中执行相应操作）。
        private static int KeyboardProc(int nCode, int action, IntPtr lParam)
        {

            // 判断按键动作
            KBDLLHOOKSTRUCT pKeyStruct;
            if (GetKBDLLHOOKSTRUCTByIntPtr(lParam, out pKeyStruct))
            {

                switch (action)
                {
                    case WM_KEYUP:
                        //备注:此功能取消，车牌号获取方式改为串口获取 20171217PM
                        //回车键交由TollAssit处理
                        //if ((pKeyStruct.vkCode == VK_RETURN))
                        //{
                        //    gDoKeys.Enqueue(Keys.Enter);
                        //}
                        //F4键交由“收费软件助手-设置”处理
                        if (pKeyStruct.vkCode == VK_F4)
                        {
                            gDoKeys.Enqueue(Keys.F4);
                        }

                        break;
                }
            }


            // 返回 true 表示继续传递按键消息
            // 返回 false 表示结束按键消息传递
            //return 0;

            return CallNextHookEx(gKeyHook, nCode, action, lParam);

        }

        private GCHandle mHookProc_GCHandle;
        private HookProc mKeyPorc_HookProc;

        /// <summary>
        /// 配置界面是否已经显示
        /// </summary>
        private bool mFrmCfgShow = false;
        /// <summary>
        /// 主界面是否已经显示
        /// </summary>
        private bool mFrmMainShow = false;

        /// <summary>
        /// 键盘消息 线程处理函数
        /// </summary>
        /// <param name="stat"></param>
        private void KeyboardMessageThreadFunc(Object stat)
        {
            while (!this.mAppExit)
            {
                Keys key;
                while (FrmMain.gDoKeys.TryDequeue(out key) && (!this.mAppExit))
                {
                    switch (key)
                    {
                        //备注:此功能取消，车牌号获取方式改为串口获取 20171217PM
                        //case Keys.Enter: //回车
                        //    {
                        //        if (!this.mFrmCfgShow && this.mFrmMainShow)
                        //        {
                        //            //获取车牌控件中的车牌信息
                        //            if (FetchPlatte())
                        //            {
                        //                //查询车辆信息
                        //                DoQuery();
                        //            }

                        //        }
                        //        break;
                        //    }
                        case Keys.F4: //F4
                            {
                                if (!this.mFrmCfgShow && this.mFrmMainShow)
                                {
                                    //隐藏主界面
                                    HideMainForm();
                                    //显示配置界面
                                    ShowCfgForm();



                                }
                                break;
                            }
                    }

                    System.Threading.Thread.Sleep(1);

                }

                System.Threading.Thread.Sleep(10);

            }
        }

        private FrmCFG mFrmCFG = null;//new FrmCFG(this);

        private FrmReport mFrmReport = null;//new FrmReport();//告警界面20171218 PM Add

        /// <summary>
        /// 显示主界面
        /// </summary>
        private void ShowMainForm()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(ShowMainForm));
            }
            else
            {
                this.Show();
            }
        }

        /// <summary>
        /// 隐藏主界面
        /// </summary>
        private void HideMainForm()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(HideMainForm));
            }
            else
            {
                timer1.Stop();    //显示主界面时，定时器关闭
                this.Hide();
            }
        }

        /// <summary>
        /// 显示配置界面
        /// </summary>
        private void ShowCfgForm()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(ShowCfgForm));
            }
            else
            {
                this.mFrmCFG.Show();
            }
        }

        /// <summary>
        /// 隐藏配置界面
        /// </summary>
        private void HideCfgForm()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(HideCfgForm));
            }
            else
            {
                this.mFrmCFG.Hide();
            }
        }


        /// <summary>
        /// 获取车牌号码
        /// </summary>
        //private bool FetchPlatte()
        //{
        //    //获取系统配置实例
        //    SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
        //    if (sysConfig == null)
        //    {
        //        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::FetchPlatte()=>获取SysConfig实例失败");
        //        return false;
        //    }

        //    if (sysConfig.mPlatteControlPos.IsEmpty)
        //        return false;
        //    //获取窗口控件的句柄
        //    IntPtr platteControlHandle = Win32ControlInfoFetchHelper.GetControlByPos(sysConfig.mPlatteControlPos);
        //    //获取进程路径
        //    string tollSoftPath = Win32ControlInfoFetchHelper.GetProcessPathByHwnd(platteControlHandle);
        //    if ((!string.IsNullOrWhiteSpace(sysConfig.mTollSoftPath)) && tollSoftPath != sysConfig.mTollSoftPath)
        //        return false;

        //    //是否需要获取车型控件文本??
        //    IntPtr carTypeHandle;
        //    Win32ControlInfoFetchHelper.GetControlInfo(sysConfig.mCarTypeControlPos, out carTypeHandle, out this.mCurrentCarType);

        //    //是否需要获取超载率文本??
        //    IntPtr overLoadHandle;
        //    Win32ControlInfoFetchHelper.GetControlInfo(sysConfig.mOverloadControlPos, out overLoadHandle, out this.mOverLoadRateInfo);

        //    return Win32ControlInfoFetchHelper.GetControlInfo(sysConfig.mPlatteControlPos, out platteControlHandle, out this.mCurrentPlatteNumber);



        //}

        //logo界面
        private FrmWelcome mFrmWelcome = new FrmWelcome();

        private SerialPortRecv mSerialPortRecv = null;//串口接收类 20171218 PM add
        private AlarmInfoRecvice mAlarmInfoRecvice = null;//告警信息接收类 20171221 PM add
        private AlarmInfoReader mAlarmInfoReader = null;//告警信息读取类 20171227 PM add

        private void FrmMain_Load(object sender, EventArgs e)
        {
            
            this.Visible = false;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            //this.TransparencyKey = Color.White;20170616AM

            
            mKeyPorc_HookProc = FrmMain.KeyboardProc;
            mHookProc_GCHandle = GCHandle.Alloc(mKeyPorc_HookProc, GCHandleType.Normal);

            ////this.StartPosition = FormStartPosition.CenterScreen;

            FrmMain.gKeyHook = SetWindowsHookEx(WH_KEYBOARD_LL, mKeyPorc_HookProc, GetModuleHandleA(null), 0);
            if (FrmMain.gKeyHook == IntPtr.Zero)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::FrmMain_Load()=>注册键盘hook函数失败");
            }

            //开启键盘事件线程
            System.Threading.Thread keyProcThread = new Thread(new ParameterizedThreadStart(KeyboardMessageThreadFunc));
            keyProcThread.IsBackground = true;
            keyProcThread.Name = "KeyProcThread";
            keyProcThread.Start(null);

            //初始化相关环境
            Init();

            //显示内容委托
            this.mDoShowAction = this.DoShow;

            //备注:此功能取消20171217PM
            //显示超载信息委托
            //this.DoShowOverLoadWarnAction = this.ShowOverLoadWarnTips;

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::FrmMain_Load()=>获取SysConfig实例失败");
            }


            //加载logo
            if (sysConfig != null && sysConfig.mWelcomeLogoVisible)
            {
                mFrmWelcome.StartPosition = FormStartPosition.CenterScreen;
                mFrmWelcome.ShowDialog();

            }


            //显示窗体
            this.Visible = true;
            //显示收费助手默认信息
            ShowDefaultTips();

            //显示系统资源占用情况窗体
            if (sysConfig != null && sysConfig.mResFormVisible)
            {
                this.mFrmResourceMonitor.Show();

            }

            //设置车辆信息显示频率
            this.SetScrollFrequency(sysConfig.mCarInfoScrollFrequency);//车辆信息显示滚动频率

            //告警信息界面显示 20171218 PM add
            mFrmReport = new FrmReport(sysConfig.mAlarmInfoScrollFrequency);//告警界面20171218 PM Add
            this.mFrmReport.Show();
            this.mFrmReport.ScrollCompledEvent += new Func<string, bool>(mFrmReport_ScrollCompledEvent);

            //串口接收数据 20171218 PM add
            this.mSerialPortRecv = new SerialPortRecv(sysConfig.mPortName, sysConfig.mbaudRate, sysConfig.mParity, sysConfig.mDataBits, sysConfig.mStopBits);
            this.mSerialPortRecv.RecvEvent += new Action<string,string>(mSerialPortRecv_RecvEvent);
            this.mSerialPortRecv.Start();

            //告警信息接收类 20171221 PM add
            this.mAlarmInfoRecvice = new AlarmInfoRecvice(sysConfig.mAlarmPublishServerUrl);   //vincent：初始化接受信息的地址
            this.mAlarmInfoRecvice.OnWebSocketRecvice += new Action<AlarmInfo>(mAlarmInfoRecvice_OnWebSocketRecvice);
            this.mAlarmInfoRecvice.Start();   //开始接受

            //告警信息读取类 20171227 PM add
            this.mAlarmInfoReader = new AlarmInfoReader();
            this.mAlarmInfoReader.ReaderEvent += new Action<int, AlarmInfo, bool>(mAlarmInfoReader_ReaderEvent);
            this.mAlarmInfoReader.Start();

            //将程序添加到启动项中
            SetAutoStartup("TollAssist", Helper.GetProcessPath(Process.GetCurrentProcess().Id));

        }

        /// <summary>
        /// 控制告警信息滚动
        /// </summary>
        private System.Threading.AutoResetEvent mScrollCtrl = new AutoResetEvent(false);

        /// <summary>
        /// 滚动完成事件;
        /// 备注:用户需要调用SetText()方法设置新的滚动文本，注意：SetText()不能在此调用线程中进行调用，因为SetText()方法属于UI线程而此事件方法非UI线程方法
        /// </summary>
        /// <param name="txt">表示当前滚动的文字信息</param>
        /// <returns>返回值:表示是否继续进行当前文字的滚动操作，false：停止对当前文字进行滚动</returns>
        bool mFrmReport_ScrollCompledEvent(string txt)
        {
            this.mScrollCtrl.Set();
            return false;
        }

        /// <summary>
        /// 被动接收告警信息
        /// </summary>
        /// <param name="number">告警信息编号</param>
        /// <param name="info">告警信息</param>
        /// <param name="newInfo">是否为最新告警消息</param>
        void mAlarmInfoReader_ReaderEvent(int number, AlarmInfo info, bool newInfo)
        {
            if (info == null)
                return;
            string strInfo = string.Format("{0}.{1}",newInfo?"★":number.ToString(),info.ToString());
            if (this.mFrmReport != null) 
            {
                this.mFrmReport.SetText(strInfo);
                this.mScrollCtrl.WaitOne();
               // System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 告警信息接收事件
        /// </summary>
        /// <param name="obj">告警信息</param>
        void mAlarmInfoRecvice_OnWebSocketRecvice(AlarmInfo obj)
        {
            //if (this.InvokeRequired)
            //{
            //    this.mFrmReport.BeginInvoke(new Action<AlarmInfo>(mAlarmInfoRecvice_OnWebSocketRecvice), obj);
            //}
            //else 
            //{
            //    //if (this.mFrmReport != null)
            //    //{
            //    //    this.mFrmReport.SetText(obj);

            //    //}
            //}

            if (this.mAlarmInfoReader != null) //将此消息投递最新消息队列中保存
            {
                if (obj != null && obj.Revoke == 0)//新的告警消息
                    this.mAlarmInfoReader.PostNewAlarmInfo(obj);
            }
            
        }

        /// <summary>
        /// 接收串口发送过来的数据
        /// </summary>
        /// <param name="data"></param>
        void mSerialPortRecv_RecvEvent(string number,string cardId)
        {
            this.DoQuery(number, cardId);
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


        private bool mAppExit = false;//应用程序退出

        private bool isDbInstance = false;//数据库实例化是否完成


        /// <summary>
        /// 初始化相关环境
        /// </summary>
        private void Init()
        {
            //初始化系统日志服务并注册到系统组件中
            LOGCS.CLoger cLoger = LOGCS.CLoger.GetInstance();
            if (cLoger == null)
                return;
            //注册到系统组件中
            SysComponents.RegComponent("CLoger", cLoger);

            //将日志对象绑定到包装类中
            LogerPrintWrapper.BindCLoger(cLoger);

            //初始化系统配置文件并注册到组件中
            SysConfig sysConfig = SysConfig.LoadObjectFromFile();
            //注册到系统组件中
            SysComponents.RegComponent("SysConfig", sysConfig);

            //数据库操作实例注册线程
            System.Threading.Thread dbInstanceLoadThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(StartDBInstanceLoadThreadFunc));
            dbInstanceLoadThread.IsBackground = true;
            dbInstanceLoadThread.Name = "DBInstanceLoadThread";
            dbInstanceLoadThread.Start(sysConfig);
            for (int i = 0; i < 3 && (!isDbInstance); i++)
            {
                System.Threading.Thread.Sleep(1000);//等待Sqlite数据库实例初始化完成
            }
            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "数据库实例化:{0}", isDbInstance ? "完成" : "未完成(已超时)");

            //心跳服务
            this.mHeartBaetThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SendHeartBeatThreadFunc));
            this.mHeartBaetThread.IsBackground = true;
            this.mHeartBaetThread.Name = "HeartBaetThread";
            this.mHeartBaetThread.Start(null);

            //查询代理初始化线程
            System.Threading.Thread initQueryServerProxyThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InitQueryServerProxyThreadFunc));
            initQueryServerProxyThread.IsBackground = true;
            initQueryServerProxyThread.Name = "InitQueryServerProxyThread";
            initQueryServerProxyThread.Start(sysConfig);

            //升级代理初始化线程
            System.Threading.Thread initUpdateServerProxyThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InitUpdateServerProxyThreadFunc));
            initUpdateServerProxyThread.IsBackground = true;
            initUpdateServerProxyThread.Name = "InitUpdateServerProxyThreadFunc";
            initUpdateServerProxyThread.Start(sysConfig);

            System.Threading.Thread.Sleep(1000);

            //连接测试线程
            System.Threading.Thread connectionThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ConntectionTestThreadFunc));
            connectionThread.IsBackground = true;
            connectionThread.Name = "ConntectionTestThreadFunc";
            connectionThread.Start(sysConfig);

            //自动升级检查
            SoftwareUpdateThread softwareUpdateThread = new SoftwareUpdateThread(ASSISTUPDATEMODULEICE.UpdateType.Client, this);
            softwareUpdateThread.Start();
            //注册到系统组件中
            SysComponents.RegComponent("SoftwareUpdateThread", softwareUpdateThread);

            //备注此功能取消20171217 PM
            ////自动上传消费记录线程
            //CustomRecordUploadThread customRecordUploadThread = new CustomRecordUploadThread();
            //customRecordUploadThread.Start();
            ////注册到系统组件中
            //SysComponents.RegComponent("CustomRecordUploadThread", customRecordUploadThread);

            //监视系统的资源占用情况
            this.mProcessResourceMonitorThread = new ProcessResourceMonitorThread(2, this);
            this.mProcessResourceMonitorThread.Start();

            //站点同步线程
            StationTableSyncThread stationTableSyncThread = new StationTableSyncThread(300, this);
            stationTableSyncThread.Start();
            //注册到系统组件中
            SysComponents.RegComponent("StationTableSyncThread", stationTableSyncThread);

            //20171230 updte 不再闪烁
            //提示窗口闪烁控制线程
            //System.Threading.Thread tipsFlashThread = new System.Threading.Thread(new ParameterizedThreadStart(TipsFlashThreadFunc));
            //tipsFlashThread.IsBackground = true;
            //tipsFlashThread.Name = "TipsFlashThread";
            //tipsFlashThread.Start(null);

            //20171230 add
            //车辆信息滚动显示线程
            //ScollDrawThreadFunc
            System.Threading.Thread scollDrawThread = new System.Threading.Thread(new ParameterizedThreadStart(ScollDrawThreadFunc));
            scollDrawThread.IsBackground = true;
            scollDrawThread.Name = "ScollDrawThread";
            scollDrawThread.Start(null);
        }

        /// <summary>
        /// 启动数据库实例加载操作
        /// 【线程函数】
        /// </summary>
        /// <param name="stat">SysConfig对象</param>
        private void StartDBInstanceLoadThreadFunc(object stat)
        {
            SysConfig sysConfig = stat as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "数据库实例加载失败,系统配置对象为NULL");

                isDbInstance = true;

                return;
            }

            //实例化本地Sqlite操作对象并注册到系统组件中
            SqliteHandler sqliteHandler = SqliteHandler.Instance(sysConfig.mLocalSqliteDBName);
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "Sqlite数据库实例加载失败,请检查文件:{0}是否存在", sysConfig.mLocalSqliteDBName);
            }
            else
            {
                SysComponents.RegComponent("SqliteHandler", sqliteHandler);//注册组件
                LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "Sqlite数据库实例加载成功!");
            }

            ////实例化中心MSSQL数据操作对象并注册到系统组件中
            //MSSqlHandler mSSqlHandler = MSSqlHandler.Instance(sysConfig.mCenterMSSQLAddress, sysConfig.mCenterMSSQLInitialCatalog, sysConfig.mCenterMSSQLUser, sysConfig.mCenterMSSQLPassword);
            //if (mSSqlHandler == null)
            //{
            //    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "中心MSSQL数据库实例加载失败,请检查相关参数是否异常");
            //}
            //else
            //{
            //    //连接测试
            //    string error;
            //    if (!mSSqlHandler.TestConnection(out error))
            //    {
            //        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "中心MSSQL数据库实例连接失败:{0}", error);
            //    }
            //    else
            //    {
            //        SysComponents.RegComponent("MSSqlHandler", mSSqlHandler);//注册组件
            //        LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "中心MSSQL数据库实例连接测试成功!");
            //    }
            //}

            isDbInstance = true;

        }

        /// <summary>
        /// 升级服务代理初始化线程函数
        /// 【线程函数】
        /// </summary>
        /// <param name="stat">SysConfig对象</param>
        private void InitUpdateServerProxyThreadFunc(object stat)
        {
            SysConfig sysConfig = stat as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InitUpdateServerProxyThreadFunc()=>系统配置对象为NULL");
                return;
            }

            string error;
            ClientProxyWrapper<ASSISTUPDATEMODULEICE.IUpdatePrx> proxy = new ClientProxyWrapper<ASSISTUPDATEMODULEICE.IUpdatePrx>();

            if (!string.IsNullOrWhiteSpace(sysConfig.mServerIPAddress) && Helper.ValidateIPAddress(sysConfig.mServerIPAddress))
            {
                sysConfig.mUpdateServerIceProxy = SysConfig.ReplaceIP(sysConfig.mUpdateServerIceProxy, sysConfig.mServerIPAddress);
            }

            while (true)
            {

                if (!ICEServantFactory.GetProxy(sysConfig.mUpdateServerIceServantProp, sysConfig.mUpdateServerIceProxy, ref proxy, out error))
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InitUpdateServerProxyThreadFunc()=>获取升级服务代理失败:{0}\r\n系统将在30秒后重试", error);
                }
                else
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InitUpdateServerProxyThreadFunc()=>获取升级服务代理成功", error);
                    //注册到系统组件中
                    SysComponents.RegComponent("ASSISTUPDATEMODULEICE.IUpdatePrx", proxy);
                    break;
                }


                System.Threading.Thread.Sleep(30000);
            }
        }


        /// <summary>
        /// 查询服务代理初始化线程函数
        /// 【线程函数】
        /// </summary>
        /// <param name="stat">SysConfig对象</param>
        private void InitQueryServerProxyThreadFunc(object stat)
        {
            SysConfig sysConfig = stat as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InitUpdateServerProxyThreadFunc()=>系统配置对象为NULL");
                return;
            }

            string error;
            ClientProxyWrapper<ASSISTICE.ICarQueryPrx> proxy = new ClientProxyWrapper<ASSISTICE.ICarQueryPrx>();

            if (!string.IsNullOrWhiteSpace(sysConfig.mServerIPAddress) && Helper.ValidateIPAddress(sysConfig.mServerIPAddress))
            {
                sysConfig.mIQueryPlateServerIceProxy = SysConfig.ReplaceIP(sysConfig.mIQueryPlateServerIceProxy, sysConfig.mServerIPAddress);
            }

            while (true)
            {
                if (!ICEServantFactory.GetProxy(sysConfig.mIQueryPlateServerIceProxyProp, sysConfig.mIQueryPlateServerIceProxy, ref proxy, out error))
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InitUpdateServerProxyThreadFunc()=>获取查询服务代理失败:{0}\r\n系统将在30秒后重试", error);
                }
                else
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InitUpdateServerProxyThreadFunc()=>获取查询服务代理成功", error);
                    //注册到系统组件中
                    SysComponents.RegComponent("ASSISTICE.ICarQueryPrx", proxy);
                    break;
                }


                System.Threading.Thread.Sleep(30000);
            }
        }

        /// <summary>
        /// 连接测试线程
        /// </summary>
        /// <param name="stat"></param>
        private void ConntectionTestThreadFunc(object stat)
        {
            while (!this.mAppExit)
            {
                ClientProxyWrapper<ASSISTICE.ICarQueryPrx> proxy = proxy = SysComponents.GetComponent("ASSISTICE.ICarQueryPrx") as ClientProxyWrapper<ASSISTICE.ICarQueryPrx>;
                if (proxy == null)
                {
                    this.mFrmCFG.ConntectionStatChanged(false);
                    System.Threading.Thread.Sleep(1000);//1s
                    continue;
                }

                try
                {
                    proxy.prx.ice_ping();
                    this.mFrmCFG.ConntectionStatChanged(true);
                }
                catch (Ice.Exception ex)
                {

                    this.mFrmCFG.ConntectionStatChanged(false);
                }
                catch (Exception ex)
                {
                    this.mFrmCFG.ConntectionStatChanged(false);
                }

                for (int i = 0; i < 30 && (!this.mAppExit); i++)
                {
                    System.Threading.Thread.Sleep(1000);//30s
                }


            }


        }


        /// <summary>
        /// 上次车牌号码
        /// </summary>
        private string mLastPlatteNumber;
        /// <summary>
        /// 当前车牌号码
        /// </summary>
        //private string mCurrentPlatteNumber;//20171217 update
        /// <summary>
        /// 当前车型
        /// </summary>
        // private string mCurrentCarType;//20171217 update

        /// <summary>
        /// 超载率文本
        /// </summary>
        // private string mOverLoadRateInfo;//20171217 update

        /// <summary>
        /// 收费站点信息
        /// </summary>
        private ASSISTICE.TollNode mSiteInfo = new ASSISTICE.TollNode();

        /// <summary>
        /// 号码查询回调包装类
        /// </summary>
        private AMI_ICarQuery_QueryCarRecordWrapper mAMI_ICarQuery_QueryCarRecordWrapper;

        //备注：20171217 此功能取消
        ///// <summary>
        ///// 验证车牌号码是否有效(空判断和长度判断)
        ///// </summary>
        ///// <param name="platte">车牌</param>
        ///// <param name="carType">车型</param>
        ///// <returns></returns>
        //private bool IsPlatteValid(string platte,string carType) 
        //{
        //    if (string.IsNullOrWhiteSpace(platte) || string.IsNullOrWhiteSpace(carType))
        //        return false;

        //    int strLen = platte.Length;

        //    if (strLen <= 6 || strLen >= 10) //无效车牌
        //        return false;

        //    return true;
        //}

        /// <summary>
        /// 验证车牌号码是否有效(空判断和长度判断) 20171217 PM add
        /// </summary>
        /// <param name="platte">车牌</param>
        /// <returns></returns>
        private bool IsPlatteValid(string platte)
        {
            if (string.IsNullOrWhiteSpace(platte))
                return false;

            int strLen = platte.Length;

            if (strLen <= 6 || strLen >= 10) //无效车牌
                return false;

            return true;
        }

        #region 超载提示显示相关
        private FrmOverloadWarn mFrmOverloadWarn = new FrmOverloadWarn();
        private Action DoShowOverLoadWarnAction;
        private void ShowOverLoadWarnTips()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(DoShowOverLoadWarnAction);
            }
            else
            {
                mFrmOverloadWarn.DoShow(this.mDefaultFontSize, new Point(this.Location.X, this.Location.Y + this.Height));
            }
        }

        #endregion

        /// <summary>
        /// 执行查询
        /// </summary>
        private void DoQuery(string carNumber, string cardId)
        {
            //if (string.IsNullOrWhiteSpace(mCurrentPlatteNumber) || string.IsNullOrWhiteSpace(this.mCurrentCarType))
            //    return;

            //if (this.mCurrentPlatteNumber == this.mLastPlatteNumber)
            //{
            //    //车牌无变化则直接置顶显示界面
            //    return;
            //}

            //备注：超载率提示功能取消20171217PM
            ////20170623 add超载率显示
            //if (!string.IsNullOrWhiteSpace(this.mOverLoadRateInfo))
            //{
            //    //获取系统配置实例
            //    SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            //    if (sysConfig != null)
            //    {
            //        if (this.mOverLoadRateInfo.IndexOf("超载率") == 0 && this.mOverLoadRateInfo != sysConfig.mOverloadTips)
            //        {
            //            //显示车辆超载信息并报警
            //            ShowOverLoadWarnTips();

            //        }
            //    }

            //}

            //车牌信息查询
            if (this.IsPlatteValid(carNumber))//车牌有效
            {
                if (carNumber == this.mLastPlatteNumber)
                {
                    //车牌无变化则直接置顶显示界面
                    return;
                }

                this.mLastPlatteNumber = carNumber;//记录本次车牌信息

                UpdateSiteInfo(this.mSiteInfo);//更新收费站点信息

                //备注：此功能取消20171217PM
                ////投递消费记录到服务器中
                ////目前存在部分字段未知情况
                //CustomRecord record = new CustomRecord();
                //record.number = this.mCurrentPlatteNumber;
                //record.type = this.mCurrentCarType;
                //record.flag = string.Empty;//TODO
                //record.dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //record.node = this.mSiteInfo;
                //this.PostCustomRecordToServer(record);

                //下发查询请求
                ClientProxyWrapper<ASSISTICE.ICarQueryPrx> proxy = proxy = SysComponents.GetComponent("ASSISTICE.ICarQueryPrx") as ClientProxyWrapper<ASSISTICE.ICarQueryPrx>;
                if (proxy == null)
                {
                    //this.mLastPlatteNumber = this.mCurrentPlatteNumber;
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::DoQuery()=>获取代理ICarQueryPrx失败");
                    //显示收费助手默认信息
                    ShowDefaultTips();
                    return;
                }

                ICarQueryProxyWrapper wrapper = new ICarQueryProxyWrapper(proxy.prx);
                Console.WriteLine("下发请求:{0}", carNumber);
                if (!wrapper.QueryCarRecord_async(this.mAMI_ICarQuery_QueryCarRecordWrapper, carNumber, cardId, this.mSiteInfo))
                {
                    // this.mLastPlatteNumber = this.mCurrentPlatteNumber;

                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::DoQuery()=>调用QueryCarRecord_async接口发生异常");
                    //显示收费助手默认信息
                    ShowDefaultTips();
                }

            }
            else
            {
                //显示收费助手默认信息
                ShowDefaultTips();
            }


            //this.mLastPlatteNumber = this.mCurrentPlatteNumber;


            ////条件：与最后一次比对车牌不一致、含有“川”、长度在7-10的车牌
            //bool boolSICHUAN = mCurrentPlatteNumber.IndexOf("川") == 0;
            //int strLen = mCurrentPlatteNumber.Length;

            //if (strLen > 6 && strLen < 10) //有效车牌
            //{
            //    UpdateSiteInfo(this.mSiteInfo);//更新收费站点信息
            //    //投递消费记录到服务器中
            //    //目前存在部分字段未知情况
            //    CustomRecord record = new CustomRecord();
            //    record.platte = this.mCurrentPlatteNumber;
            //    record.cartype = this.mCurrentCarType;
            //    record.comment = string.Empty;
            //    record.dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    record.node = this.mSiteInfo;
            //    this.PostCustomRecordToServer(record);

            //    if (boolSICHUAN) //在数据库中比对川籍车牌，并显示相关信息
            //    {
            //        ClientProxyWrapper<ASSISTICE.ICarQueryPrx> proxy = proxy = SysComponents.GetComponent("ASSISTICE.ICarQueryPrx") as ClientProxyWrapper<ASSISTICE.ICarQueryPrx>;
            //        if (proxy == null)
            //        {
            //            //this.mLastPlatteNumber = this.mCurrentPlatteNumber;
            //            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::DoQuery()=>获取代理ICarQueryPrx失败");
            //            //显示收费助手版本号
            //            this.mTips = mDefaultTips;
            //            this.mColorTips = this.mDefaultColorTips;
            //            this.DoShow();
            //            return;
            //        }

            //        ICarQueryProxyWrapper wrapper = new ICarQueryProxyWrapper(proxy.prx);
            //        Console.WriteLine("下发请求:{0}", this.mCurrentPlatteNumber);
            //        if (!wrapper.QueryCarRecord_async(this.mAMI_ICarQuery_QueryCarRecordWrapper,this.mCurrentCarType, this.mCurrentPlatteNumber, this.mSiteInfo))
            //        {
            //            // this.mLastPlatteNumber = this.mCurrentPlatteNumber;

            //            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::DoQuery()=>调用QueryCarRecord_async接口发生异常");
            //            //显示收费助手版本号
            //            this.mTips = mDefaultTips;
            //            this.mColorTips = this.mDefaultColorTips;
            //            this.DoShow();
            //        }
            //    }
            //    else //若不是川籍有效车牌，则显示收费助手版本号
            //    {
            //        //this.mLastPlatteNumber = this.mCurrentPlatteNumber;
            //        this.mTips = mDefaultTips;
            //        this.mColorTips = this.mDefaultColorTips;
            //        this.DoShow();

            //    }
            //}
            //else
            //{
            //    //显示收费助手版本号
            //    this.mTips = mDefaultTips;
            //    this.mColorTips = this.mDefaultColorTips;
            //    this.DoShow();
            //}




        }

        //更新站点信息
        private void UpdateSiteInfo(ASSISTICE.TollNode tollNode)
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::UpdateSotfwareNote()=>获取SysConfig实例失败");
                return;
            }
            tollNode.companycode = sysConfig.mCompanyCode;
            tollNode.plazcode = sysConfig.mPlazCode;
            tollNode.lanname = sysConfig.mLanName;
            tollNode.lannum = sysConfig.mLanNum;

        }

        //构造车辆信息
        //private string CarInfo(string Platte, string CarClass, int Num, string Remark, string Unit, string Comment)
        //{
        //    string strValue = string.Empty;
        //    string strNumName = string.Empty;
        //    string strNum = string.Empty;
        //    string strRemark1 = string.Empty;
        //    string strRemark2 = string.Empty;

        //    int pos = 0;
        //    string[] resToken;
        //    string wstrBm;
        //    resToken = Remark.Split(';');
        //    //品牌字段分解（同号牌蓝黄牌多车情况）
        //    if (resToken != null && resToken.Length > 0)
        //    {
        //        strRemark1 = resToken[0];

        //        if (resToken.Length > 1)
        //            strRemark2 = resToken[1];
        //    }
        //    else
        //        strRemark1 = Remark;

        //    strNum = Num.ToString();

        //    if (CarClass == "客")
        //    {
        //        strValue = "客车：";
        //        strNumName = "座位数：";
        //    }
        //    else
        //    {
        //        strValue = "货车：";
        //        strNumName = "轴数：";
        //    }
        //    //车牌
        //    strValue += Platte;
        //    strValue += "\r\n";
        //    //客、货
        //    strValue += strNumName;
        //    //座位数、轴数
        //    strValue += (strNum + strRemark2);
        //    strValue += "\r\n";
        //    //品牌
        //    strValue += "品牌：";
        //    strValue += AddStr(strRemark1);
        //    strValue += "\r\n";
        //    //单位
        //    strValue += "单位：";
        //    strValue += AddStr(Unit);
        //    strValue += "\r\n";
        //    //备注
        //    strValue += "备注：";
        //    strValue += AddStr(Comment);

        //    return strValue;
        //}


        //构造车辆信息
        private string CarInfo(ASSISTICE.CarTable record)
        {

            StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("车牌:{0}\r\n", record.number);
            //sb.AppendFormat("类型:{0}\r\n", record.type);
            //sb.AppendFormat("品牌:{0}\r\n", record.brand);
            //sb.AppendFormat("号牌颜色:{0}\r\n", record.color);
            //sb.AppendFormat("座位数:{0}\r\n", record.maxPassenger);
            //sb.AppendFormat("轴数:{0}\r\n", record.axleNum);
            //sb.AppendFormat("备注:{0}-{1}\r\n", record.esType, record.esRemark);
            /**vincent^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            sb.AppendFormat("车牌:{0} ", record.number);
            sb.AppendFormat("类型:{0}\r\n", record.type);
            sb.AppendFormat("品牌:{0} ", record.brand);
            sb.AppendFormat("号牌颜色:{0}\r\n", record.color);
            sb.AppendFormat("座位数:{0} ", record.maxPassenger);
            sb.AppendFormat("轴数:{0}\r\n", record.axleNum);
            //sb.AppendFormat("备注:{0}-{1}\r\n", record.esType, record.esRemark);//2018-01-02 TODO 需要修改
            sb.AppendFormat("备注:{0}",record.esRemark);
                ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^**/

            ///整合车辆查询信息，显示。

            sb.AppendFormat("{0} ", record.color.Substring(0,1));
            sb.AppendFormat("{0} \r\n", record.number);


            if (record.axleNum >=2)
            {
            sb.AppendFormat("{0}轴 ", record.axleNum);
           
            }
            if(record.maxPassenger >=2)
            { 
                sb.AppendFormat("{0}座 ", record.maxPassenger);
            }
           
        
            
           sb.AppendFormat("{0} ", record.brand);
        

            sb.AppendFormat("{0}", record.type);

           

            sb.AppendFormat("\r\n");
            
          
         
             sb.AppendFormat("备注:{0}", record.esRemark);
             string vincentstring = record.esRemark;
             //MessageBox.Show(vincentstring);
           





            return sb.ToString();
        }

        //转换为多行文本
        private string ToMultiLine(string cstr, int len)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            for (int i = 0; i < cstr.Length; i++)
            {
                if (index == len)
                {
                    sb.Append("\r\n");
                    index = 0;
                }
                sb.Append(cstr[i]);
                index++;
            }

            return sb.ToString();
        }

        //按指定行长度追加字符串并自动换行
        private string AddStr(string strAdd)
        {
            int lineLen = 20;
            if (strAdd.Length > 0)
            {
                return ToMultiLine(strAdd, lineLen);
            }
            return strAdd;
        }

        public void ice_response(bool ret__, ASSISTICE.CarTable[] records, string error)
        {

            //this.mTips = "hellofdsfsdfsdfsdfsdfsd\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123\r\n123";

            //this.DoShow();
            //return;

            if (!ret__)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::DoQuery()=>调用QueryCarRecord_async接口返回失败：{0}", error);
                //return;
            }

            if (records == null || records.Length == 0)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::DoQuery()=>调用QueryCarRecord_async接口成功，但返回车辆信息为空");
                //显示收费助手默认信息
                ShowDefaultTips();
                return;
            }

            if (ret__ && this.mLastPlatteNumber == records[0].number) //当前车牌号码匹配,返回的records集合中每个record的number都相等
            {

                string strTips;//提示信息
                // string m_SlaveTips;//次要提示信息
                Color ColorTips;//提示颜色
                int imgIndex;//提示图片
                bool flash = false;//是否闪烁
                bool doFlash = false;//是否需要闪烁提示

                List<TipInfo> tipInfos = new List<TipInfo>();
                for (int i = 0; i < records.Length && i < 4; i++) //返回的records集合中最多只显示2条记录   //李昊域 update  add  显示4条
                {
                    //生成相关提示信息
                    MakeTipString(records[i], out strTips, out ColorTips);

                    //不再显示图片和不在闪烁提示信息 20171230 update
                    ////生成提示图片
                    ////20171216PM update
                    ////TODO根据车辆类型进行客货分类
                    //switch (records[i].type)
                    //{
                    //    case "小型自动挡汽车":
                    //    case "小型轿车":
                    //    case "小型汽车":
                    //    case "中型客车":
                    //    case "城市公交车":
                    //    case "大型客车":
                    //        {
                    //            //载客类汽车根据荷载人数显示相应车辆图片
                    //            imgIndex = records[i].maxPassenger > 7 ? (records[i].maxPassenger > 30 ? (records[i].maxPassenger > 39 ? 3 : 2) : 1) : 0;
                    //            break;
                    //        }
                    //    default: imgIndex = -1; break;
                    //}
                    ////根据车辆监控级别判断是否需要闪烁
                    //switch (records[i].monLevel)
                    //{
                    //    case 0: //一般车辆
                    //        {
                    //            flash = false;
                    //            break;
                    //        }
                    //    case 1://涉嫌
                    //    case 2://保障
                    //    case 3://无效
                    //        {
                    //            flash = true;
                    //            break;
                    //        }
                    //}

                    //tipInfos.Add(new TipInfo(strTips, ColorTips, imgIndex, flash));

                    tipInfos.Add(new TipInfo(strTips, ColorTips, -1, flash));//20171230 add 直接添加
                }

                //不在闪烁提示信息 20171230 update
                ////判断当前组中是否存在需要闪烁的车辆信息
                //doFlash = tipInfos.Exists(info => { return info.Flash; });



                //if (doFlash) //闪烁方式提示
                //{
                //    DoShow(tipInfos);//显示车辆信息
                //    this.EnableTipsFlash();
                //}
                //else //默认方式提示
                //{
                //    DoShow(tipInfos);//显示车辆信息
                //}

                //20171230 add
                
                this.SetShowInfo(tipInfos);
            }
            else
            {

                //显示收费助手默认信息
                ShowDefaultTips();
            }

            // this.mLastPlatteNumber = record.platte;

        }

        ///// <summary>
        ///// 生成提示文本
        ///// </summary>
        ///// <param name="record">车辆记录</param>
        ///// <param name="strTips">主提示文件</param>
        ///// <param name="m_SlaveTips">次提示文字</param>
        ///// <param name="ColorTips">提示颜色</param>
        //private void MakeTipString(ASSISTICE.CarTable record, out string strTips, out string m_SlaveTips, out Color ColorTips)
        //{

        //    strTips = string.Empty;
        //    m_SlaveTips = string.Empty;
        //    ColorTips = Color.Blue;

        //    string strCarInfo = CarInfo(record.platte, record. CarClass, record.Num, record.Remark,record.Unit, record.Comment);//构造提示信息
        //    // string strTips;//主提示信息
        //    // string m_SlaveTips;//次要提示信息
        //    // Color ColorTips;//提示颜色
        //    switch (record.monLevel)//监控级别（1:涉嫌车辆 3:无效车牌）需要上传。
        //    {
        //        case 0://一般车辆
        //            {
        //                ////记录座位数 2170513 add/update LQ
        //                int ret_num = record.num > 7 ? (record.num > 30 ? (record.num > 39 ? 4 : 3) : 2) : 1;
        //                switch (ret_num)
        //                {
        //                    case 1:
        //                        m_SlaveTips = string.Format("一类客车({0}座)", record.num);
        //                        break;
        //                    case 2:
        //                        m_SlaveTips = string.Format("二类客车({0}座)", record.num);
        //                        break;
        //                    case 3:
        //                        m_SlaveTips = string.Format("三类客车({0}座)", record.num);
        //                        break; ;
        //                    case 4:
        //                        m_SlaveTips = string.Format("四类客车({0}座)", record.num);
        //                        break;
        //                }
        //                strTips = "一般车辆/" + strCarInfo;
        //                ColorTips = Color.FromArgb(255, 0, 0, 255);//蓝色显示
        //                break;
        //            }
        //        case 1://涉嫌车辆
        //            {
        //                m_SlaveTips = "涉嫌车辆";
        //                strTips = "涉嫌车辆/" + strCarInfo;
        //                ColorTips = Color.FromArgb(255, 255, 0, 0);//红色显示
        //                //b_upload=true;
        //                break;
        //            }
        //        case 2://保畅车辆
        //            {
        //                m_SlaveTips = "保畅车辆";
        //                strTips = "保畅车辆/" + strCarInfo;
        //                ColorTips = Color.FromArgb(255, 0, 255, 0);//绿色显示
        //                break;
        //            }
        //        case 3://无效车牌
        //            {
        //                m_SlaveTips = "无效车牌";
        //                strTips = "无效车牌/" + strCarInfo;
        //                ColorTips = Color.FromArgb(255, 0, 0, 0);//黑色显示
        //                //b_upload=true;
        //                break;
        //            }
        //    }
        //}


        /// <summary>
        /// 生成提示文本
        /// </summary>
        /// <param name="record">车辆记录</param>
        /// <param name="strTips">提示文字</param>
        /// <param name="ColorTips">提示颜色</param>
        private void MakeTipString(ASSISTICE.CarTable record, out string strTips, out Color ColorTips)
        {

            strTips = string.Empty;
            ColorTips = Color.Blue;

            //获取系统配置实例
            SysConfig sysConfigVincent = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfigVincent == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmCFG::DoShow()=>获取SysConfig实例失败");
                return;
            }



            string strCarInfo = CarInfo(record);//构造提示信息
            // string strTips;//主提示信息
            // string m_SlaveTips;//次要提示信息
            // Color ColorTips;//提示颜色
            //提示信息，各种类型车辆显示的颜色
            switch (record.monLevel)//监控级别（1:涉嫌车辆 3:无效车牌）需要上传。
            {
                case 0://一般车辆
                    {
                        strTips = "一般车辆\r\n" + strCarInfo;
                        ColorTips = Color.FromArgb(255, sysConfigVincent.mCommonCarR, sysConfigVincent.mCommonCarG, sysConfigVincent.mCommonCarB);//蓝色显示
                        break;
                    }
                case 1://涉嫌车辆
                    {
                        strTips = "涉嫌车辆\r\n" + strCarInfo;
                        ColorTips = Color.FromArgb(255, sysConfigVincent.mSuspectCarR, sysConfigVincent.mSuspectCarG, sysConfigVincent.mSuspectCarB);//红色显示
                        //b_upload=true;
                        break;
                    }
                case 2://保畅车辆
                    {
                        strTips = "保畅车辆\r\n" + strCarInfo;
                        ColorTips = Color.FromArgb(255, sysConfigVincent.mUnblockedCarR, sysConfigVincent.mUnblockedCarG, sysConfigVincent.mUnblockedCarB);//绿色显示
                        break;
                    }
                case 3://无效车牌
                    {
                        strTips = "无效车牌\r\n" + strCarInfo;
                        ColorTips = Color.FromArgb(255, sysConfigVincent.mInvalidCarR, sysConfigVincent.mInvalidCarG, sysConfigVincent.mInvalidCarB);//黑色显示
                        //b_upload=true;
                        break;
                    }
                default:
                    {
                        strTips = strCarInfo;
                        ColorTips = Color.FromArgb(255, 255, 255, 0);//没有车辆类型，则 该车辆 用 黄色显示。
                        break;
                    }

            }
        }


        //20171230 update 此方法不再使用
        //private void DoShow(List<TipInfo> tipInfos)
        //{
        //    if (tipInfos == null)
        //        return;

        //    if (this.InvokeRequired)
        //    {
        //        this.BeginInvoke(this.mDoShowAction, tipInfos);
        //    }
        //    else
        //    {
        //        //获取系统配置实例
        //        SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
        //        if (sysConfig == null)
        //        {
        //            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmCFG::pnlSaveSet_MouseClick()=>获取SysConfig实例失败");
        //            return;
        //        }

        //        this.mDefaultFontSize = sysConfig.mTipsFontSize;
        //        this.Location = sysConfig.mTipsFormPos;

        //        this.DrawInfo(tipInfos);
        //        //this.Refresh();
        //    }
        //}

        public void ice_exception(Ice.Exception ex)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::DoQuery()=>调用QueryCarRecord_async接口返回异常：{0}", ex.Message);

            //显示收费助手版本号
            ShowDefaultTips();

        }

     

        ///// <summary>
        ///// 显示默认提示
        ///// </summary>
        ///// <param name="g"></param>
        ///// <param name="fontOfPix">字体大小，以像素为单位</param>
        ///// <param name="fontColor">字体颜色</param>
        //private void ShowDefaultTips(Graphics g,int fontOfPix,Color fontColor,string defaultStr) 
        //{
        //    Font F = new Font(this.Font.FontFamily, fontOfPix, GraphicsUnit.Pixel);

        //    //this.BackColor = Color.White;
        //    this.TransparencyKey = Color.White;

        //    g.Clear(Color.White);
        //    g.DrawString(defaultStr, F, Brushes.Blue, this.ClientRectangle);


        //}

        #region 车辆信息显示相关

        /// <summary>
        /// 
        /// 默认提示文字
        /// </summary>
        private const string mDefaultTips = "高速公路\r\n收费稽查\r\n管理系统";   
        //默认颜色
        private Color mDefaultColorTips = Color.Blue;
        //默认字体大小
        private int mDefaultFontSize = 23;
        /// <summary>
        /// 提示信息
        /// </summary>
        //private string mTips = "您好:收费助手";
        private Color mColorTips = Color.Blue;
        //private int mTipsImageIndex = -1;//提示图片编号
        private Bitmap[] mCarTypeImages = new Bitmap[] { TollAssistUI.Properties.Resources._1_2x, TollAssistUI.Properties.Resources._2_2x, TollAssistUI.Properties.Resources._3_2x, TollAssistUI.Properties.Resources._4_2x };

        private List<TipInfo> mDefaultTipInfos = null;

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            ////this.TransparencyKey = Color.White;

            //Font font = new System.Drawing.Font("微软雅黑", this.mDefaultFontSize, FontStyle.Bold, GraphicsUnit.Pixel);

            //SolidBrush brush = new SolidBrush(this.mColorTips);

            //Graphics g = e.Graphics;

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            //g.Clear(Color.White);

            ////计算绘制文本需要占用的大小
            //SizeF areaText = g.MeasureString(this.mTips, font);

            //g.DrawString(this.mTips, font, brush, 0, 0);

            //if (this.mTipsImageIndex >= 0 && this.mTipsImageIndex < mCarTypeImages.Length)
            //{
            //    //在文本下方显示提示图片
            //    //TODO
            //    float image_x = 0;
            //    float image_y = areaText.Height + 10;//提示图片的Y坐标


            //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//控制抗锯齿
            //    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            //    //绘制提示图片
            //    g.DrawImage(this.mCarTypeImages[this.mTipsImageIndex], image_x, image_y);
            //}

            //brush.Dispose();
            //font.Dispose();


        }

        //以下为滚动显示车辆信息相关参数

        private List<TipInfo> mLastTipInfo = null;//上次绘制信息
        private int mScrollOffset = 0;//上下滚动偏移值 20171230 add
        private int mScrollMinOffset = 0;//从下到上滚动的最小偏移值 20121230 add
        private int mScrollFrequency = 200;//车辆信息显示滚动频率
        //李昊域 
        private int mScrollcount = 0;//用于滚动时候，开始的一段时间停留， 2018-2-28 add




        private void SetScrollFrequency(int scrollFrequency) 
        {
            if (scrollFrequency < 10 || scrollFrequency > 5000) //滚动频率设置
            {
                this.mScrollFrequency = 200;
            }
            else
            {
                this.mScrollFrequency = scrollFrequency;
            }
        }

        private object scrollLock = new object();//滚动锁
       
        /// <summary>
        /// 设置车辆显示信息
        /// 备注：用户调用 20171230 add
        /// </summary>
        /// <param name="showInfo"></param>
        private void SetShowInfo(List<TipInfo> showInfo) 
        {
            if (showInfo == null || showInfo.Count == 0)
                return;

            int h=CalcDrawTextMaxHeight(showInfo);
            lock (scrollLock) 
            {
                this.mLastTipInfo = showInfo;
                this.mScrollOffset = 0;
                this.mScrollMinOffset = -h;
            }
        }

        /// <summary>
        /// 显示默认提示
        /// 备注：用户调用 20171230 add
        /// </summary>
        private void ShowDefaultTips()
        {
            //this.DisEnableTipsFlash();

            //this.mTips = mDefaultTips;
            //this.mTipsImageIndex = -1;
            //this.mColorTips = this.mDefaultColorTips;   
            // List<TipInfo> mTipInfos=new List<TipInfo>{new TipInfo(mDefaultTips, this.mDefaultColorTips, -1, false)};

            if (this.mDefaultTipInfos == null)
                this.mDefaultTipInfos = new List<TipInfo> { new TipInfo(mDefaultTips, mDefaultColorTips, -1, false) };

            this.SetShowInfo(mDefaultTipInfos);

            // this.DoShow(mTipInfos);
        }

        /// <summary>
        /// 滚动显示车辆信息线程
        /// 20171230 add
        /// </summary>
        /// <param name="stat"></param>
        private void ScollDrawThreadFunc(Object stat) 
        {
            while (!this.mAppExit) 
            {
                lock (scrollLock) 
                {
                    this.DoShow();
                    this.mScrollOffset -= 5;//滚动偏移
                    if (this.mScrollOffset < this.mScrollMinOffset) //滚动已经完成一轮
                    {
                        //重置滚动值继续滚动
                        this.mScrollOffset = 0;
                    }
                }

                System.Threading.Thread.Sleep(this.mScrollFrequency);
            }
        }

     

        /// <summary>
        /// 计算绘制文本占用高度
        /// 20171230 add
        /// </summary>
        /// <param name="drawInfo"></param>
        /// <returns></returns>
        private int CalcDrawTextMaxHeight(List<TipInfo> drawInfo)
        {
            Font font = new System.Drawing.Font("微软雅黑", this.mDefaultFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            Graphics g = this.CreateGraphics();
            g.PageUnit = GraphicsUnit.Pixel;
            float totalHeigth = 0;
           // foreach (TipInfo tipInfo in drawInfo)
            for (int i = 0; i < drawInfo.Count; i++) 
            {
                TipInfo tipInfo = drawInfo[i];
                string textInfo = tipInfo.Text + "-------------------------------------";
                if (i == drawInfo.Count - 1)
                {
                    textInfo = tipInfo.Text;
                }
                //计算绘制文本需要占用的大小
                SizeF areaText = g.MeasureString(textInfo, font);
                totalHeigth += areaText.Height + 5;//间隔距离
            }
              //李昊域 
            this.mScrollcount = 0;//每次有新数据的时候重新归零 20180228 add 



            font.Dispose();
            g.Dispose();
            return (int)totalHeigth;
        }



        private Action mDoShowAction;

        /// <summary>
        /// 20171230 add
        /// 绘制显示信息
        /// 备注：用户不能调用此函数
        /// </summary>
        private void DoShow()
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(this.mDoShowAction);
            }
            else
            {
                //获取系统配置实例
                SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
                if (sysConfig == null)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmCFG::DoShow()=>获取SysConfig实例失败");
                    return;
                }

                this.mDefaultFontSize = sysConfig.mTipsFontSize;
                this.Location = sysConfig.mTipsFormPos;

                this.DrawInfo();
                //this.Refresh();
            }
        }

        /// <summary>
        /// 绘制显示信息
        /// 20171230 update
        /// 备注：用户不能调用此函数
        /// </summary>
        private void DrawInfo()
        {
            if (this.mLastTipInfo == null)
                return;
            Bitmap tmpBitmap = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(tmpBitmap);

            g.Clear(Color.FromArgb(0));//控制透明，此处颜色一定要和SetBits()中设置的颜色一致
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            SysConfig sysConfigVincent = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfigVincent == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmCFG::DoShow()=>获取SysConfig实例失败");
                return;
            }

            string vincentFont=sysConfigVincent.minformationFont;   

            Font font = new System.Drawing.Font(vincentFont, this.mDefaultFontSize, FontStyle.Bold, GraphicsUnit.Pixel);

            float axleY = this.mScrollOffset;//垂直方向上的位置
            string textInfo = null;
            //李昊域
            if (this.mLastTipInfo.Count == 1 || this.mScrollcount <= 10)
            {
                axleY = 0;
                this.mScrollOffset += 5;
            }

            if (this.mScrollcount >= 20)
            {
                this.mScrollcount = 20;
            }
            else
            {
                this.mScrollcount++;
            }//判断当有多条数据的时候，停留10次再滚动 20180228add








            for (int i = 0; i < this.mLastTipInfo.Count; i++) 
            //foreach (TipInfo tipInfo in this.mLastTipInfo)
            {
                //201701-02 多个记录之间增加分割线
                TipInfo tipInfo = this.mLastTipInfo[i];
                textInfo = tipInfo.Text + "-------------------------------------";
                if (i == this.mLastTipInfo.Count - 1) 
                {
                    textInfo = tipInfo.Text;
                }
                
                //计算绘制文本需要占用的大小
                SizeF areaText = g.MeasureString(textInfo, font);

                //SolidBrush brush = new SolidBrush(tipInfo.Flash ? tipInfo.FlashColor : tipInfo.TipColor);//20171230 取消闪烁颜色设置,不再闪烁
                SolidBrush brush = new SolidBrush(tipInfo.TipColor);
                RectangleF rect = new RectangleF(0, axleY, areaText.Width, areaText.Height);
                StringFormat format = StringFormat.GenericTypographic;
                float dpi = g.DpiY;
                //*************************************************
           


                using (GraphicsPath path = GetStringPath(textInfo, dpi, rect, font, format))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;//设置字体质量

                    Pen vincentPen = new Pen(Color.Black, sysConfigVincent.fontOutline);  //  设置字体轮廓的大小画笔。通过配置文件
                    

                    //g.DrawPath(Pens.Black, path);//绘制轮廓（描边）   字体轮廓
                    g.DrawPath(vincentPen, path);//绘制轮廓（描边）   字体轮廓
                    g.FillPath(brush, path);//填充轮廓（填充）
                }
                //g.DrawString(textInfo, font, brush, 0, axleY);

                axleY += areaText.Height + 5;//间隔距离

                //2017-12-30 update 取消图片绘制 
                //if (tipInfo.ImageIndex >= 0 && tipInfo.ImageIndex < mCarTypeImages.Length)
                //{
                //    //在文本下方显示提示图片
                //    //TODO
                //    float image_x = 0;
                //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//控制抗锯齿
                //    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                //    //绘制提示图片
                //    g.DrawImage(this.mCarTypeImages[tipInfo.ImageIndex], image_x, axleY);

                //    axleY += this.mCarTypeImages[tipInfo.ImageIndex].Height + 10;
                //}
                brush.Dispose();

            }

            SetBits(tmpBitmap);
            font.Dispose();
            tmpBitmap.Dispose();
            g.Dispose();
        }
        GraphicsPath GetStringPath(string s, float dpi, RectangleF rect, Font font, StringFormat format)
        {
            GraphicsPath path = new GraphicsPath();
            // Convert font size into appropriate coordinates
            float emSize = dpi * font.SizeInPoints / 72;

            path.AddString(s, font.FontFamily, (int)font.Style, emSize, rect, format);

            return path;
        }

        #endregion


        ////20171230 取消
        ///// <summary>
        ///// 绘制显示信息
        ///// </summary>
        //private void DrawInfo(List<TipInfo> tipInfoObjs)
        //{

        //    this.lastTipInfo = tipInfoObjs;

        //    Bitmap tmpBitmap = new Bitmap(this.Width, this.Height);
        //    Graphics g = Graphics.FromImage(tmpBitmap);

        //    g.Clear(Color.FromArgb(0));//控制透明，此处颜色一定要和SetBits()中设置的颜色一致
        //    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

        //    Font font = new System.Drawing.Font("微软雅黑", this.mDefaultFontSize, FontStyle.Bold, GraphicsUnit.Pixel);

        //    float axleY = 0;//垂直方向上的位置
        //    foreach (TipInfo tipInfo in tipInfoObjs)
        //    {
        //        //计算绘制文本需要占用的大小
        //        SizeF areaText = g.MeasureString(tipInfo.Text, font);

        //        SolidBrush brush = new SolidBrush(tipInfo.Flash ? tipInfo.FlashColor : tipInfo.TipColor);

        //        g.DrawString(tipInfo.Text, font, brush, 0, axleY);

        //        axleY += areaText.Height + 10;//提示图片的Y坐标

        //        if (tipInfo.ImageIndex >= 0 && tipInfo.ImageIndex < mCarTypeImages.Length)
        //        {
        //            //在文本下方显示提示图片
        //            //TODO
        //            float image_x = 0;
        //            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//控制抗锯齿
        //            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        //            //绘制提示图片
        //            g.DrawImage(this.mCarTypeImages[tipInfo.ImageIndex], image_x, axleY);

        //            axleY += this.mCarTypeImages[tipInfo.ImageIndex].Height + 10;
        //        }
        //        brush.Dispose();

        //    }

        //    SetBits(tmpBitmap);
        //    font.Dispose();
        //    tmpBitmap.Dispose();
        //    g.Dispose();
        //}


        ///// <summary>
        ///// 绘制显示信息
        ///// </summary>
        //private void DrawInfo()
        //{    
        //    Bitmap tmpBitmap = new Bitmap(this.Width, this.Height);
        //    Graphics g = Graphics.FromImage(tmpBitmap);

        //    g.Clear(Color.FromArgb(0));//控制透明，此处颜色一定要和SetBits()中设置的颜色一致
        //    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;


        //    Font font = new System.Drawing.Font("微软雅黑", this.mDefaultFontSize, FontStyle.Bold, GraphicsUnit.Pixel);

        //    SolidBrush brush = new SolidBrush(this.mColorTips);

        //    //计算绘制文本需要占用的大小
        //    SizeF areaText = g.MeasureString(this.mTips, font);

        //    g.DrawString(this.mTips, font, brush, 0, 0);

        //    if (this.mTipsImageIndex >= 0 && this.mTipsImageIndex < mCarTypeImages.Length)
        //    {
        //        //在文本下方显示提示图片
        //        //TODO
        //        float image_x = 0;
        //        float image_y = areaText.Height + 10;//提示图片的Y坐标


        //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//控制抗锯齿
        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        //        //绘制提示图片
        //        g.DrawImage(this.mCarTypeImages[this.mTipsImageIndex], image_x, image_y);
        //    }

        //    SetBits(tmpBitmap);

        //    brush.Dispose();
        //    font.Dispose();

        //    tmpBitmap.Dispose();
        //    g.Dispose();
        //}

        //以下接口均用于解决不规则窗口毛边问题
        //使用此接口请注意不要设置窗体的TransparencyKey属性和窗体的背景图片属性
        //另外使用了此接口窗体将不再响应ONPONT事件

        #region 重写窗体的 CreateParams 属性
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000;  //  WS_EX_LAYERED 扩展样式
                ////无边框任务栏窗口最小化
                //const int WS_MINIMIZEBOX = 0x00020000;  // Winuser.h中定义
                ////CreateParams cp = base.CreateParams;
                //cp.Style = cp.Style | WS_MINIMIZEBOX;   // 允许最小化操作
                return cp;
            }
        }
        #endregion

        #region API调用

        /// <summary>
        /// 设置不规则图片背景窗体并消除毛边问题
        /// 【注意：使用此接口窗体将不再处理WM_POINT消息，即ONPOINT事件失效】
        /// </summary>
        /// <param name="bitmap"></param>
        public void SetBits(Bitmap bitmap)//调用UpdateLayeredWindow（）方法。this.BackgroundImage为你事先准备的带透明图片。
        {
            //if (!haveHandle) return;

            if (bitmap == null) return;

            if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
                throw new ApplicationException("图片必须是32位带Alhpa通道的图片。");

            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

            try
            {
                Win32.Point topLoc = new Win32.Point(Left, Top);
                Win32.Size bitMapSize = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
                Win32.Point srcLoc = new Win32.Point(0, 0);

                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));//这里这个背景色一定要和Graphics.Clear的背景色一致
                oldBits = Win32.SelectObject(memDc, hBitmap);

                blendFunc.BlendOp = Win32.AC_SRC_OVER;
                blendFunc.SourceConstantAlpha = 255;
                blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
                blendFunc.BlendFlags = 0;

                Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBits);
                    Win32.DeleteObject(hBitmap);
                }
                Win32.ReleaseDC(IntPtr.Zero, screenDC);
                Win32.DeleteDC(memDc);
            }
        }
        #endregion


        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

           
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                if (FrmMain.gKeyHook != IntPtr.Zero)
                    UnhookWindowsHookEx(FrmMain.gKeyHook);

                //20171218 PM add
                //关闭告警提示
                //if (this.mFrmReport != null)
                //{
                //    //this.mFrmReport.StopShow();
                //    //this.mFrmReport.Close();
                //}

                //20171227 PM add
                if(this.mAlarmInfoRecvice!=null)
                    this.mAlarmInfoRecvice.Stop();
                //20171227 PM add
                if(this.mAlarmInfoReader!=null)
                    this.mAlarmInfoReader.Stop();

                //20171218 PM add
                //串口功能关闭
                if (this.mSerialPortRecv != null) 
                {
                    this.mSerialPortRecv.Stop();
                }

                //关闭系统资源占用
                this.mProcessResourceMonitorThread.Stop();
                this.mProcessResourceMonitorThread.WaitThreadExit(1000);

                //关闭站点同步
                StationTableSyncThread stationTableSyncThread = SysComponents.GetComponent("StationTableSyncThread") as StationTableSyncThread;
                if (stationTableSyncThread != null)
                {
                    stationTableSyncThread.Stop();
                }

                //关闭消费记录上传
                CustomRecordUploadThread customRecordUploadThread = SysComponents.GetComponent("CustomRecordUploadThread") as CustomRecordUploadThread;
                if (customRecordUploadThread != null)
                {
                    customRecordUploadThread.Stop();
                }

                //关闭系统升级
                SoftwareUpdateThread softwareUpdateThread = SysComponents.GetComponent("SoftwareUpdateThread") as SoftwareUpdateThread;
                if (softwareUpdateThread != null)
                {
                    softwareUpdateThread.Stop();
                }




                //设置应用程序退出标记，使其他诸如:连接测试线程、窗口闪烁线程等关闭
                this.mAppExit = true;


                if (mHookProc_GCHandle != null)
                    mHookProc_GCHandle.Free();

                LogerPrintWrapper.ClosePrint();
                LOGCS.CLoger cloger = SysComponents.GetComponent("CLoger") as LOGCS.CLoger;
                if (cloger != null)
                    cloger.Dispose();
            }
        }

        //软件升级
        public void UpdateSotfwareNote(UpdateDownloadResult updateResult, string updateDir, string error)
        {
            if (updateResult == UpdateDownloadResult.Downloaded)
            {
                //获取系统配置实例
                SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
                if (sysConfig == null)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::UpdateSotfwareNote()=>获取SysConfig实例失败");
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
                string showTitel = "收费助手程序升级中...";//升级程序显示标题内容
                string startProcessPath = Helper.GetProcessPath(hbd.ProcessId);//升级程序升级完成后需要启动的进程路径

                string closeForm = bool.TrueString;//指示升级完成后关闭升级程序
                int waitTime = 5000;//5秒,升级程序关闭之前的等待时间

                string args = string.Format("{0} {1} \"{2}\" \"{3}\" {4} \"{5}\" {6}", currentProcessId, showTitel, srcDir, destDir, closeForm, startProcessPath, waitTime);

                string updateExePath = "update.exe";
                //启动升级程序
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(updateExePath, args);

                LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "IUpdateSotfwareNote.UpdateSotfwareNote()=>程序即将升级&&本进程即将退出");

                //TODO清理资源

                //关闭本进程
                Application.Exit();

            }
            else
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateSotfwareNote.UpdateSotfwareNote()=>升级检查失败,错误消息:{0}", error != null ? error : "无");
            }
        }

        /// <summary>
        /// 投递消费记录到服务端
        /// </summary>
        /// <param name="record"></param>
        private void PostCustomRecordToServer(CustomRecord record)
        {
            if (record == null)
                return;
            CustomRecordUploadThread uploadThrad = SysComponents.GetComponent("CustomRecordUploadThread") as CustomRecordUploadThread;
            if (uploadThrad == null)
                return;
            uploadThrad.AddCustomRecord(record);
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
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmMain::SendHeartBeatThreadFunc()=>获取SysConfig实例失败");
                return;
            }

            if (!sysConfig.mEnableHeartBeat)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "FrmMain::SendHeartBeatThreadFunc()=>未启用心跳服务,心跳发送线程退出");
                return;
            }

            //解析IP地址和端口
            string remoteIP = sysConfig.mMonitorServerEndPoint.Substring(0, sysConfig.mMonitorServerEndPoint.IndexOf(':'));
            int port = int.Parse(sysConfig.mMonitorServerEndPoint.Substring(sysConfig.mMonitorServerEndPoint.IndexOf(':') + 1));

            //心跳客户端对象实例化
            this.mHeartBeatClient = new HeartBeatClient(remoteIP, port);

            HeartBeatData hbd = new HeartBeatData();
            hbd.ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
            hbd.HeartBeatInterval = sysConfig.mHeartBeatIntervalOfSecond;


            while (!this.mHeartBaetThreadIsStop)
            {

                hbd.CpuUsageRate = this.mProcessCPURate;
                hbd.MemoryUsageRate = this.mProcessMemRate;
                hbd.Flag = HeartBeatFlag.None;

                mHeartBeatClient.SendHeartBeat(hbd);

                for (int i = 0; i < sysConfig.mHeartBeatIntervalOfSecond && (!this.mHeartBaetThreadIsStop); i++)
                {
                    System.Threading.Thread.Sleep(1000);
                }

            }



        }

        #endregion

        #region 系统资源占用

        //资源占用显示窗口
        private FrmResourceMonitor mFrmResourceMonitor = new FrmResourceMonitor();

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

            //LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "ProcessResource()=>系统CPU:{0},系统内存:{1},进程CPU:{2},进程内存:{3}", this.mSysCPURate, this.mSysMemRate, this.mProcessCPURate, this.mProcessMemRate);
            //System.Threading.Thread.Sleep(30000); 
            mFrmResourceMonitor.RefreshShow(sysCPURate, sysMemRate, processCPURate, processMemRate);
        }

        #endregion


        /// <summary>
        /// 收到站点更新通知
        /// </summary>
        public void StationInfoUpdate()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(StationInfoUpdate));
            }
            else
            {
                this.mFrmCFG.ManualRefreshCompanyCodes();
            }
        }

        /// <summary>
        /// 资源窗口的可见性发生改变
        /// </summary>
        /// <param name="visible"></param>
        public void ResFormVisible(bool visible)
        {
            this.mFrmResourceMonitor.Visible = visible;
        }

        /// <summary>
        /// 用户修改了提示字体大小
        /// </summary>
        /// <param name="fontSize"></param>
        public void TipsFontSizeChanged(int fontSize)
        {

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmCFG::TipsFontSizeChanged()=>获取SysConfig实例失败");
                return;
            }
            sysConfig.mTipsFontSize = fontSize;

            
            this.DoShow();
        }

        #region 卓文君禁用alt+f4
        private void FrmMain_KeyDown(object sender, KeyEventArgs e)///*******************************vincent  disable key f4 +alt
        {
            if (e.KeyCode == Keys.F4 && e.Modifiers == Keys.Alt)   ///识别当两个按键同时按下时。且该窗口属性中 keypreview 值需要为true才能生效
            {
                e.Handled = true;
            }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            
          #region  vincent模拟鼠标事件
            this.TopMost = true;
            vincentMutex.WaitOne();
            
            
            
            if (vincent_k == 180)
            {
                timer1.Interval = 180000;
                vincent_i = 1;  //  表示3分钟后，为1
                // MessageBox.Show("test", "test1", MessageBoxButtons.YesNo);
            }

            if (vincent_k==2)
            {
                mouse_event(MOUSEEVENTF_MOVE, 0, -100, 0, 0);   //鼠标上移  100个像素
            }

            if(vincent_i!=1)    //不是3分钟后，vincentk保持计数
            {
                vincent_k++;

               // mouse_event(MOUSEEVENTF_MOVE, 0, 0, 0, 0);
                MouseDown();    //模拟鼠标一次点击
                MouseUp();

            }

          #endregion


            ////vincent add top other

            ///********** vincent**********/

            //SysConfig vincentSysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            //if (vincentSysConfig == null)
            //{
            //    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::topOtherApplacation()=>获取SysConfig实例失败");
            //    return;
            //}
            
            //Process[] temp = Process.GetProcessesByName(processName);//在所有已启动的进程中查找需要的进程； 
            //if (temp.Length<=0)  
            //{
            //    // processName = Process.Start(vincentSysConfig.otherApplacationPath).ProcessName;//启动软件时获取进程名称    
            //    temp = Process.GetProcessesByName(processName);//在所有已启动的进程中查找需要的进程； 
            //}
            //else
            //{
            //    //processName = Process.Start("C:\\this.txt").ProcessName;
            //    //temp = Process.GetProcessesByName(processName);//在所有已启动的进程中查找需要的进程；    
            //    if (temp.Length > 0)//如果查找到    
            //    {
            //        IntPtr handle = temp[0].MainWindowHandle;
            //       //SwitchToThisWindow(handle, true);    // 激活，显示在最前   
                  
                   

            //    }

            //}
            /********** vincent**********/
            vincentMutex.ReleaseMutex();
        }


        //#region 控制窗口闪烁线程


        //private const int FLASHINTERVAL = 500;//500ms
        //private int mCurrentFlashCount = 0;//当前闪烁次数
        //private ManualResetEvent mManualResetEventFlash = new ManualResetEvent(true);
        //private void TipsFlashThreadFunc(object stat)
        //{
        //    while (!this.mAppExit)
        //    {
        //        if (this.mCurrentFlashCount > 0)
        //        {
        //            this.mManualResetEventFlash.Reset();
        //            this.mCurrentFlashCount--;
        //            this.mManualResetEventFlash.Set();


        //            this.DoShow(this.lastTipInfo);

        //            System.Threading.Thread.Sleep(FLASHINTERVAL);
        //        }
        //        else
        //        {
        //            System.Threading.Thread.Sleep(1);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 启用提示文字闪烁
        ///// </summary>
        //private void EnableTipsFlash()
        //{
        //    this.mManualResetEventFlash.WaitOne();
        //    this.mCurrentFlashCount = TipInfo.FLASHCOUNT;
        //}

        ///// <summary>
        ///// 关闭提示文字闪烁
        ///// </summary>
        //private void DisEnableTipsFlash()
        //{

        //    this.mManualResetEventFlash.WaitOne();
        //    this.mCurrentFlashCount = 0;
        //}




        //#endregion




    }

    /// <summary>
    /// 提示信息对象
    /// </summary>
    public class TipInfo
    {
        public TipInfo(string text, Color color, int img, bool flash)
        {
            this.Text = text;
            this.TipColor = color;
            this.ImageIndex = img;
            this.Flash = flash;
        }
        /// <summary>
        /// 提示文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 提示颜色
        /// </summary>
        public Color TipColor { get; set; }

        /// <summary>
        /// 图片索引信息
        /// </summary>
        public int ImageIndex { get; set; }

        /// <summary>
        /// 是否需要闪烁
        /// </summary>
        public bool Flash { get; set; }

        /// <summary>
        /// 闪烁颜色
        /// </summary>
        public Color FlashColor
        {
            get
            {
                if (flashIndex < FLASHCOUNT && Flash) //控制闪烁颜色
                {
                    Color clr = this.TipColor;
                    if (flashIndex % 2 == 0)
                    {
                        clr = this.TipColor;
                    }
                    else
                    {
                        clr = Color.FromArgb(this.mFlashColorBuilder.Next() % 256, this.mFlashColorBuilder.Next() % 256, this.mFlashColorBuilder.Next() % 256);
                    }

                    flashIndex++;
                    return clr;
                }
                else
                {
                    return this.TipColor;
                }
            }

        }
        private Random mFlashColorBuilder = new Random();//颜色生成器
        private int flashIndex = 0;//闪烁计数
        public const int FLASHCOUNT = 5;//固定闪烁次数

    }

    public interface IAMI_ICarQuery_QueryCarRecord
    {
        void ice_response(bool ret__, ASSISTICE.CarTable[] record, string error);
        void ice_exception(Ice.Exception ex);
    }

    internal class AMI_ICarQuery_QueryCarRecordWrapper : ASSISTICE.AMI_ICarQuery_QueryCarRecord
    {

        private IAMI_ICarQuery_QueryCarRecord mIAMI_ICarQuery_QueryCarRecord;
        public AMI_ICarQuery_QueryCarRecordWrapper(IAMI_ICarQuery_QueryCarRecord cb__)
        {
            this.mIAMI_ICarQuery_QueryCarRecord = cb__;
        }

        public override void ice_response(bool ret__, ASSISTICE.CarTable[] record, string error)   
        {
            if (this.mIAMI_ICarQuery_QueryCarRecord != null)
            {
                this.mIAMI_ICarQuery_QueryCarRecord.ice_response(ret__, record, error);
            }
        }

        public override void ice_exception(Ice.Exception ex)   //车辆信息查询的回调函数
        {
            if (this.mIAMI_ICarQuery_QueryCarRecord != null)
            {
                this.mIAMI_ICarQuery_QueryCarRecord.ice_exception(ex);
            }
        }
    }

}
