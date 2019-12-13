using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using TollAssistComm;
using System.Collections.Concurrent;
using IDbHandler;
using System.Threading;
using JSONTool;
using System.Net;
using System.Diagnostics;

namespace TollAssistUI
{
    /// <summary>
    /// 20171221 PM add
    /// 告警信息接收类
    /// 备注;1.此类通过websocket协议方式从告警服务端获取告警信息;2.将告警信息写入到本地库或更新本地库告警信息
    /// 使用方式;1.构造此类的实例;2.注册OnWebSocketRecvice事件(可选步骤);3.调用Start()方法启动服务
    /// </summary>
    public class AlarmInfoRecvice
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">告警服务websocket地址</param>
        public AlarmInfoRecvice(string url)
        {
            this.mUrl = url;
        }

        private string mUrl;

       // private WebSocket mWebSocket = null;

        private bool mIsStop = false;
        public bool Start() 
        {
            if (string.IsNullOrWhiteSpace(this.mUrl))
                return false;


            this.mAlarmInfoWriter.Start();



            System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InitThreadFunc));
            thd.IsBackground = true;
            thd.Name = "WEBSOCKET—告警信息订阅线程";
            thd.Start(null);

            return true;

        }

        public bool Stop()
        {
            this.mIsStop = true;

            this.mAlarmInfoWriter.Stop();


            return true;

        }

        private void InitThreadFunc(object stat)
        {
            string clientId = GetClientId().ToString();

            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "WebSokcet当前客户端Id={0}",clientId);

            string url = string.Format("{0}?clientId={1}", this.mUrl, clientId);
            while (!this.mIsStop)
            {
                using (WebSocket client = new WebSocket(url))
                {
                    client.OnOpen += new EventHandler(mWebSocket_OnOpen);
                    client.OnClose += new EventHandler<CloseEventArgs>(mWebSocket_OnClose);
                    client.OnError += new EventHandler<ErrorEventArgs>(mWebSocket_OnError);
                    client.OnMessage += new EventHandler<MessageEventArgs>(mWebSocket_OnMessage);
                    client.Log.Level = LogLevel.Error;
                    client.Log.Output = LogOutput;
                    try
                    {
                        client.Connect();
                    }
                    catch (InvalidOperationException ex)
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoRecvice::LogOutput()=>执行到【{0}】的websocket连接异常:{1}\r\n系统将在30s后重新开启连接",this.mUrl, ex.Message);

                        System.Threading.Thread.Sleep(30000);//30s
                        continue;
                    }
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "AlarmInfoRecvice::LogOutput()=>执行【{0}】的连接{1}", this.mUrl, client.ReadyState == WebSocketState.Open?"成功":"失败");
                    if (client.ReadyState == WebSocketState.Open)
                    {
                        while (!this.mIsStop)
                        {
                            //这里需要考虑服务端是否开启了Ping功能
                            //启用ping测试
                            if (client.Ping())
                            {
                                System.Threading.Thread.Sleep(30000);//30s
                            }
                            else
                            {
                                //关闭当前连接并重新发起新的连接
                                try
                                {
                                    client.Close();
                                }
                                catch (Exception ex)
                                {

                                }
                                //退出ping测试
                                break;
                            }
                        }
                    }
                    else 
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "AlarmInfoRecvice::LogOutput()=>执行【{0}】的连接{1}\r\n系统将在30s后重试连接", this.mUrl, client.ReadyState == WebSocketState.Open ? "成功" : "失败");
                        System.Threading.Thread.Sleep(30000);//30s
                    }

                }
            }
        }


        /// <summary>
        /// 返回客户端ID(每个登录客户端应该有一个唯一的ID)
        /// 备注：客户端ID采用IP(4byte)+进程id(4byte)的方式进行组合生成;
        /// </summary>
        /// <returns></returns>
        private ulong GetClientId() 
        {
            IPAddress hostIp = IPAddress.Loopback;
            string hostName=Dns.GetHostName();
            if (!string.IsNullOrWhiteSpace(hostName)) 
            {
                IPAddress[] ipList = Dns.GetHostAddresses(hostName);
                if (ipList != null && ipList.Length > 0) 
                {
                    foreach (IPAddress item in ipList)
                    {
                        if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) 
                        {
                            hostIp = item;
                            break;
                        }
                    }
                   
                }
            }
            byte[] hostIpOfBytes = hostIp.GetAddressBytes();
            byte[] currentProcessId =BitConverter.GetBytes(Process.GetCurrentProcess().Id);
            byte[] buffer = new byte[sizeof(ulong)];
            Array.Copy(currentProcessId, buffer, currentProcessId.Length);
            Array.Copy(hostIpOfBytes, 0, buffer, currentProcessId.Length, buffer.Length - currentProcessId.Length);

            ulong clientId = BitConverter.ToUInt64(buffer, 0);

            return clientId;
        }


       

        /// <summary>
        /// 接收消息
        /// </summary>
        public event Action<AlarmInfo> OnWebSocketRecvice;

        void LogOutput(LogData ld, string text)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "AlarmInfoRecvice::LogOutput()=>{0}",text);
        }

        
        //收到消息
        void mWebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {

                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "AlarmInfoWriter::mWebSocket_OnMessage()=>收到告警文本:\r\n{0}",e.Data);
                AlarmInfo info = null;
               
                if (this.ParserInfoAndPost(e.Data,out info)&&this.OnWebSocketRecvice != null)
                {
                    this.OnWebSocketRecvice(info);
                }
            }
        }

        void mWebSocket_OnError(object sender, ErrorEventArgs e)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoRecvice::mWebSocket_OnError()=>{0}", e.Message);
        }

        void mWebSocket_OnClose(object sender, CloseEventArgs e)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "AlarmInfoRecvice::mWebSocket_OnClose()=>{0}", e.Reason);
        }

        void mWebSocket_OnOpen(object sender, EventArgs e)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "AlarmInfoRecvice::mWebSocket_OnOpen()=>打开websocket");
        }

        /// <summary>
        /// 解析发布端发送过来的告警消息文本并对告警信息进行入库处理
        /// </summary>
        /// <param name="msg"></param>
        private bool ParserInfoAndPost(string msg,out AlarmInfo outInfo) 
        {
            outInfo = null;
            if (string.IsNullOrWhiteSpace(msg))
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoWriter::ParserInfoAndPost()=>告警信息msg参数不能为空");
                return false;
            }
            //发送过来数据格式为数组，其中第0个元素为字符串表示当前操作类型:撤销(revoke)或发布(publish);第一个元素为AlarmInfo对象
            //["",{}]
            if (msg.IndexOf("[") != 0||msg.Length<5) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoWriter::ParserInfoAndPost()=>告警信息msg参数无效");
                return false;
            }
            int dhFlag = msg.IndexOf(',', 1);//逗号标记
            if (dhFlag == -1) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoWriter::ParserInfoAndPost()=>告警信息msg参数无效");
                return false;
            }
            //获取标记
            string action = msg.Substring(2, dhFlag - 2 - 1);
            int revoke = 0;//是否撤销
            if (action == "publish") 
            {
                revoke = 0;
            }
            else if (action == "revoke")
            {
                revoke = 1;//撤销
            }
            else 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoWriter::ParserInfoAndPost()=>告警信息msg参数无效,操作类型不是撤销(revoke)或发布(publish)");
                return false;
            }

            string jsonString=msg.Substring(dhFlag+1,msg.Length-1-(dhFlag+1));
            if (string.IsNullOrWhiteSpace(jsonString)) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoWriter::ParserInfoAndPost()=>告警信息msg参数无效,获取告警信息JSON字符串出错");
                return false;
            }
            //解析AlarmInfo对象
            string error = null;
            AlarmInfo info=null;
            if (!Object2JSON.JSONStringToObject<AlarmInfo>(jsonString, out info, out error)) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoWriter::ParserInfoAndPost()=>告警信息msg参数无效,解析告警信息JSON字符串出错：{0}",error);
                return false;
            }

            info.Revoke = revoke;
            //投递到后台入库
            this.mAlarmInfoWriter.PostAlarmInfo(info);

            outInfo = info;

            return true;
        }

        private AlarmInfoWriter mAlarmInfoWriter = new AlarmInfoWriter();

    }

    /// <summary>
    /// 告警信息写入和删除类
    /// 备注:内部通过线程进行控制
    /// </summary>
    public class AlarmInfoWriter 
    {

        public void Start() 
        {

            System.Threading.Thread addAlarmInfoThread = new System.Threading.Thread(new ParameterizedThreadStart(WriteThreadFunc));
            addAlarmInfoThread.IsBackground = true;
            addAlarmInfoThread.Name = "addAlarmInfoThread";
            addAlarmInfoThread.Start(null);

            System.Threading.Thread deleteAlarmInfoThread = new System.Threading.Thread(new ParameterizedThreadStart(AlarmInfoDeleteThreadFunc));
            deleteAlarmInfoThread.IsBackground = true;
            deleteAlarmInfoThread.Name = "deleteAlarmInfoThread";
            deleteAlarmInfoThread.Start(null);
        }

        public void Stop() 
        {
            this.mIsStop = true;
        }

        /// <summary>
        /// 投递告警信息到后台入库队列中
        /// </summary>
        /// <param name="info"></param>
        public void PostAlarmInfo(AlarmInfo info) 
        {
            this.mWriteQueue.Enqueue(info);
        }




        private bool mIsStop = false;

        //告警信息写入队列
        private ConcurrentQueue<AlarmInfo> mWriteQueue = new ConcurrentQueue<AlarmInfo>();



        /// <summary>
        /// 告警信息添加线程
        /// </summary>
        /// <param name="stat"></param>
        private void WriteThreadFunc(Object stat) 
        {

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "AlarmInfoWriter::WriteThreadFunc()=>获取SqliteHandler实例失败");
                return;
            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "AlarmInfoWriter::AlarmInfoDeleteThreadFunc()=>告警信息添加线程启动");

            AlarmInfo info=null;
            string error;
            while (!this.mIsStop) 
            {
                if ((!this.mWriteQueue.IsEmpty) && this.mWriteQueue.TryDequeue(out info)) 
                {
                    if (!sqliteHandler.AddAlarmInfoRecord(info, out error)) 
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "AlarmInfoWriter::WriteThreadFunc()=>添加或更新告警信息到数据库失败:{0}",error);
                    }
                }

                System.Threading.Thread.Sleep(100);
            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "AlarmInfoWriter::AlarmInfoDeleteThreadFunc()=>告警信息添加线程停止");
        }

        /// <summary>
        /// 告警信息删除线程（已撤销信息）
        /// </summary>
        /// <param name="stat"></param>
        private void AlarmInfoDeleteThreadFunc(Object stat)
        {

            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "AlarmInfoWriter::AlarmInfoDeleteThreadFunc()=>执行删除60天以前被撤销的告警信息动作的线程启动");
            while (!this.mIsStop)
            {
                if (DateTime.Now.Hour == 0) //0点时刻
                {
                    
                    //删除N天前的数据
                    this.DeleteAlarmInfoRecord(60);

                    for (int i = 0; i < 3600 && (!this.mIsStop); i++)
                    {
                        System.Threading.Thread.Sleep(1000);//跳过当前时间点3600*1000
                    }

                }

                //每5分钟检查一次
                for (int i = 0; i < 300 && (!this.mIsStop); i++)
                {
                    System.Threading.Thread.Sleep(1000);//300*1000
                }

            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "AlarmInfoWriter::AlarmInfoDeleteThreadFunc()=>执行删除60天以前被撤销的告警信息动作的线程停止");
        }

        /// <summary>
        /// 删除N天以前记录
        /// </summary>
        /// <param name="days"></param>
        private void DeleteAlarmInfoRecord(int days)
        {
            if (days <= 0)
                return;

            ////获取系统配置实例
            //SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            //if (sysConfig == null)
            //{
            //    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadWorker::DeleteCustomRecord()=>获取SysConfig实例失败");
            //    return;
            //}

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoWriter::DeleteAlarmInfoRecord()=>获取SqliteHandler实例失败");
                return;
            }

            //得到days天以前的日期
            DateTime endTime = DateTime.Now.Subtract(new TimeSpan(days, 0, 0, 0));

            //生成删除语句
            string sql = string.Format("delete from AlarmInfo where revokeTime<datetime('{0}') and revoke=1", endTime.ToString("yyyy-MM-dd HH:mm:ss"));

            string error;
            if (sqliteHandler.ExecuteNonQuery(sql, out error) == -1)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoWriter::DeleteAlarmInfoRecord()=>执行删除时间在{0}之前的数据的SQL语句失败:{1}", endTime.ToString("yyyy-MM-dd HH:mm:ss"), error);
            }
            else
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "AlarmInfoWriter::DeleteAlarmInfoRecord()=>执行删除时间在{0}之前的数据的SQL语句成功", endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }


        }
    }

    /// <summary>
    /// 告警信息读取类
    /// 备注：此类从数据库读取告警信息;
    /// 使用方法:1.注册ReaderEvent事件；2.当有告警信息到来时调用PostNewAlarmInfo方法添加最新告警信息；3.Start() ；4.Stop()
    /// </summary>
    public class AlarmInfoReader 
    {
        public void Start() 
        {
            System.Threading.Thread readThread = new System.Threading.Thread(new ParameterizedThreadStart(MessageReadThreadFunc));
            readThread.IsBackground = true;
            readThread.Name = "AlarmInfoReadThread";
            readThread.Start(null);
        }

        public void Stop() 
        {
            this.mIsStop = true;
        }

        /// <summary>
        /// 投递新告警消息对象到新告警消息队列中
        /// </summary>
        /// <param name="info"></param>
        public void PostNewAlarmInfo(AlarmInfo info) 
        {
            if (info == null)
                return;
            if (this.mNewAlarmInfoQueue.Count >= NEWMSGQUEUECOUNT) 
            {
                AlarmInfo deleteInfo;
                this.mNewAlarmInfoQueue.TryDequeue(out deleteInfo);
            }

            this.mNewAlarmInfoQueue.Enqueue(info);
        
        }

        /// <summary>
        /// 读取告警消息记录
        /// int:告警消息编号
        /// AlarmInfo:告警消息
        /// bool:是否为最新告警消息，true表示最新告警消息
        /// </summary>
        public event Action<int, AlarmInfo, bool> ReaderEvent;

        /// <summary>
        /// mNewAlarmInfoQueue中存储最新告警消息的个数
        /// </summary>
        private const int NEWMSGQUEUECOUNT = 3;
        /// <summary>
        /// 最新消息队列
        /// </summary>
        private ConcurrentQueue<AlarmInfo> mNewAlarmInfoQueue = new ConcurrentQueue<AlarmInfo>();
        /// <summary>
        /// 历史消息队列
        /// </summary>
       // private ConcurrentQueue<AlarmInfo> mHistoryAlarmInfoQueue = new ConcurrentQueue<AlarmInfo>();

        /// <summary>
        /// 每次取历史消息的个数
        /// </summary>
        private const int HISTORYMSGPAGECOUNT = 100;

        private bool mIsStop = false;

        /// <summary>
        /// 消息读取线程函数
        /// </summary>
        /// <param name="obj"></param>
        private void MessageReadThreadFunc(Object obj) 
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "AlarmInfoReader::MessageReadThreadFunc()=>告警消息读取线程启动");
            while (!this.mIsStop) 
            {
                DateTime nowTime=DateTime.Now;//获取查询视图时间点
                int index = 0;//记录编号
                for (int from = 0; (!this.mIsStop); from += HISTORYMSGPAGECOUNT) //执行分页操作
                {
                    string querySql = string.Format("select * from AlarmInfo where startTime<=datetime('{0}') and endTime>=datetime('{0}') and revoke=0 order by id LIMIT {1} OFFSET {2}",
                        nowTime.ToString("yyyy-MM-dd HH:mm:ss"), HISTORYMSGPAGECOUNT,from);
                    List<AlarmInfo> historyAlarmInfos = Query(querySql);//分页查询

                    if (historyAlarmInfos == null || historyAlarmInfos.Count == 0) //异常或者没有数据则结束分页查询
                        break;

                    //处理历史告警消息
                    foreach (AlarmInfo item in historyAlarmInfos)
                    {
                        if (this.mIsStop)
                            break;

                        //首先查看最新消息队列中是否有记录，有记录则先处理最新消息队列中的记录
                        while (!this.mNewAlarmInfoQueue.IsEmpty && (!this.mIsStop))
                        {
                            AlarmInfo outInfo = null;
                            if (this.mNewAlarmInfoQueue.TryDequeue(out outInfo))
                            {
                                if (this.ReaderEvent != null)
                                {
                                    this.ReaderEvent(index, outInfo, true);//最新告警消息
                                }
                            }
                        }

                        if (this.ReaderEvent != null)
                        {
                            this.ReaderEvent(index++, item, false);//历史告警消息
                        }

                    }

                }

                System.Threading.Thread.Sleep(1000);
            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "AlarmInfoReader::MessageReadThreadFunc()=>告警消息读取线程停止");
        }

        /// <summary>
        /// 查询告警信息
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>异常返回null</returns>
        private List<AlarmInfo> Query(string sql) 
        {
            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoReader::Query()=>获取SqliteHandler实例失败");
                return null;
            }

            string error;
            List<AlarmInfo> rv=sqliteHandler.SearcherAlarmInfo(sql,out error);
            if (rv == null) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "AlarmInfoReader::Query()=>执行SearcherAlarmInfo操作异常:{0}",error);
                return null;
            }

            return rv;

        }

    }

}
