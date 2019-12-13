using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommHandler;
using TollAssistComm;
using Ice_Servant_Factory;
using IDbHandler;

namespace TollAssistUI
{
    /// <summary>
    /// 站点信息更新通知
    /// </summary>
    public interface IStationInfoUpdate 
    {
        void StationInfoUpdate();
    }

    /// <summary>
    /// 站点表同步线程
    /// </summary>
    public sealed class StationTableSyncThread
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">执行同步操作之间的时间间隔，以秒为单位</param>
        public StationTableSyncThread(int interval,IStationInfoUpdate note)
        {
            this.mIntervalOfSecond = interval;
            this.mIStationInfoUpdate = note;
        }

        /// <summary>
        /// 同步操作间隔时间，以秒为单位
        /// </summary>
        private int mIntervalOfSecond;
        private IStationInfoUpdate mIStationInfoUpdate;


        private bool mIsStop = true;

        private void ThreadWorker(object stat) 
        {

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "StationTableSyncThread::ThreadWorker()=>获取SysConfig实例失败,站点表同步线程停止工作");
                return;
            }

            if (!sysConfig.mAutoSyncStationTab)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "StationTableSyncThread::ThreadWorker()=>未开启站点表同步功能,站点表同步线程停止工作");
                return;
            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "StationTableSyncThread::ThreadWorker()=>站点表同步线程开始工作");
           

            int count=100;//单次从服务端获取的条数
            ASSISTICE.Station[] lst=null;
            string error;
            bool sendNote = false;
            while (!this.mIsStop) 
            {

                ClientProxyWrapper<ASSISTICE.ICarQueryPrx> proxy = proxy = SysComponents.GetComponent("ASSISTICE.ICarQueryPrx") as ClientProxyWrapper<ASSISTICE.ICarQueryPrx>;
                if (proxy == null)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "StationTableSyncThread::ThreadWorker()=>获取代理ICarQueryPrx失败");

                }
                else
                {
                    //获取操作代理包装对象
                    ICarQueryProxyWrapper wrapper = new ICarQueryProxyWrapper(proxy.prx);

                    sendNote = false;
                    for (int i = 0;; i+=count)
			        {
                        //获取服务端站点信息
                        if (!wrapper.QueryStations(i, count, out lst, out error)) 
                        {
                            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "StationTableSyncThread::ThreadWorker()=>调用QueryStations接口异常:{0}", error);
                            break;
                        }
                        //不存在站点信息
                        if (lst == null || lst.Length == 0) 
                        {
                            break;
                        }
                        //与本地站点表进行比较并更新本地库
                        if (StationTabSync(lst))
                        {
                            sendNote = true;
                        }
                       
			        }

                    //向外部发送站点信息更新通知
                    if (sendNote&&this.mIStationInfoUpdate != null)
                        this.mIStationInfoUpdate.StationInfoUpdate();
                    
                }

                for (int i = 0; i < this.mIntervalOfSecond && (!this.mIsStop); i++)
                {
                    System.Threading.Thread.Sleep(1000);
                }
               
            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "StationTableSyncThread::ThreadWorker()=>站点表同步线程停止工作");
        }

        /// <summary>
        /// 站点表同步
        /// </summary>
        /// <param name="lst"></param>
        private bool StationTabSync(ASSISTICE.Station[] lst) 
        {
            bool isSync = false;
            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "StationTableSyncThread::StationTabSync()=>获取SqliteHandler实例失败");
                return false;
            }

            StringBuilder sb = new StringBuilder(256);
            List<ASSISTICE.Station> addStations = new List<ASSISTICE.Station>();
            string error;
            int ret = 0;
            foreach (ASSISTICE.Station item in lst)
            {
                sb.Clear();
                sb.AppendFormat("select count(*) from station where mc='{0}'", item.mc);
                ret = sqliteHandler.Collect(sb.ToString(), out error);
                if (ret == -1) 
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "StationTableSyncThread::StationTabSync()=>调用Collect接口异常:{0}",error);
                    break;
                }
                if (ret == 0) 
                {
                    //记录不存在则添加
                    addStations.Add(item);
                }
            }

            if (addStations.Count > 0) 
            {
                //添加新增站点到本地库
                if (!sqliteHandler.BatchAddStationRecord(addStations, out error))
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "StationTableSyncThread::StationTabSync()=>调用BatchAddStationRecord接口异常:{0}", error);
                }
                else 
                {
                    isSync = true;
                }
            }

            //方便GC回收
            sb.Clear();
            sb = null;
            addStations.Clear();
            addStations = null;

            return isSync;
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
                thd.Name = "StationTableSyncThread";
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
    }
}
