using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using ASSISTICE;
using CommHandler;
using TollAssistComm;
using Ice_Servant_Factory;

namespace TollAssistUI
{
    /// <summary>
    /// 客户端消费记录上传线程
    /// 调用方式:Start()=>AddCustomRecord()=>Stop()
    /// </summary>
    public class CustomRecordUploadThread
    {
        public CustomRecordUploadThread()
        {

        }

        private ConcurrentQueue<CustomRecord> mCustomRecords = new ConcurrentQueue<CustomRecord>();//存放消费记录的队列

        private bool mIsStop = true;

        private const int MAXREFRESHTIME = 5;//最大刷新时间(秒)，当超过该时间则刷新缓冲区数据到库中
        private const int MINREQUESTOBJSCNT = 10;//批量插入的最小对象数，当大于等于此值时则刷新缓冲区数据到库中

        private void ThreadWorker(Object stat)
        {

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadThread::ThreadWorker()=>获取SysConfig实例失败,消费记录上传线程停止工作");
                return;
            }

            if (!sysConfig.mAutoUploadCustomRecord)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadThread::ThreadWorker()=>未开启自动上传消费记录功能,消费记录上传线程停止工作");
                return;
            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "CustomRecordUploadThread::ThreadWorker()=>消费记录上传线程开始工作");
            List<CustomRecord> records = new List<CustomRecord>();



            string error;
            int ticks = 0;//计时器
            CustomRecord record;
            while (!this.mIsStop)
            {
                if (mCustomRecords.Count >= MINREQUESTOBJSCNT || ticks >= MAXREFRESHTIME) //当缓冲区数据达到指定值或超时
                {
                    if (mCustomRecords.Count > 0)
                    {
                        //拷贝需要入库的对象实体到集合
                        records.Clear();
                        while (mCustomRecords.TryDequeue(out record)) 
                        {
                            records.Add(record);
                        }

                        ClientProxyWrapper<ASSISTICE.ICarQueryPrx> proxy = proxy = SysComponents.GetComponent("ASSISTICE.ICarQueryPrx") as ClientProxyWrapper<ASSISTICE.ICarQueryPrx>;
                        if (proxy == null)
                        {
                            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadThread::ThreadWorker()=>获取代理ICarQueryPrx失败");

                        }
                        else 
                        {
                            //执行上传操作
                            ICarQueryProxyWrapper wrapper = new ICarQueryProxyWrapper(proxy.prx);
                            if (!wrapper.BatchUpload(records.ToArray(), out error)) 
                            {
                                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadThread::ThreadWorker()=>调用BatchUpload失败:{0}",error);
                            }
                        }
                        records.Clear();
                    }

                    ticks = 0;//计时器归零
                }
                else
                {
                    ticks++;
                }

                System.Threading.Thread.Sleep(1000);//间隔1s进行一次操作
            }
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "CustomRecordUploadThread::ThreadWorker()=>消费记录上传线程停止工作");
        }

        /// <summary>
        /// 添加消费记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool AddCustomRecord(CustomRecord record) 
        {
            if (record == null) return false;

            this.mCustomRecords.Enqueue(record);

            return true;
        }

        /// <summary>
        /// 开始线程工作
        /// </summary>
        public void Start()
        {
            if (this.mIsStop)
            {
                this.mIsStop = false;
                System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.ThreadWorker));
                thd.Name = "CustomRecordUploadThread";
                thd.IsBackground = true;
                thd.Start(null);
            }
        }

        /// <summary>
        /// 停止线程工作
        /// </summary>
        public void Stop()
        {
            if (!this.mIsStop)
                this.mIsStop = true;
        }

        ///// <summary>
        ///// 删除90天以前的索引
        ///// </summary>
        ///// <param name="stat"></param>
        //private void DeleteSince90Index(object stat)
        //{
        //    DateTime from = new DateTime(DateTime.Now.Year, 1, 1);
        //    while (!this.StopDeleteChecked)
        //    {
        //        DateTime now = DateTime.Now;

        //        if (now.Hour != 0) //指定在每天0点执行一次
        //        {
        //            System.Threading.Thread.Sleep(1000 * 300);
        //            continue;
        //        }

        //        //DateTime del_time = now.Subtract(new TimeSpan(90, 0, 0, 0)); //90天以前的日期
        //        DateTime del_time = now.Subtract(new TimeSpan(7, 0, 0, 0)); //7天以前的日期--lq修改

        //        //查询出90天以前的索引数据，然后删除=>（只保留最近90天的数据）
        //        //TODO
        //        this.index.DeleteIndex(from, del_time);//删除指定天数以前的文档
        //        this.index.CommitIndex();
        //        this.index.Optimize();
        //        from = del_time;
        //        System.Threading.Thread.Sleep(1000 * 60 * 60 * 24); //每天执行一次
        //    }
        //}


    }
}
