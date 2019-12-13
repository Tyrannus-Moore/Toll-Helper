using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASSISTICE;
using System.Collections.Concurrent;
using CommHandler;
using IDbHandler;
using TollAssistComm;

namespace CommHandler
{
    /// <summary>
    /// 查询接口实现类
    /// </summary>
    public sealed class ICarQueryImpl : ICarQueryDisp_,IQueryCompled
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryObjNums">内部需要缓存的PlatteWoker的个数</param>
        public ICarQueryImpl(int queryObjNums)
        {
            if (queryObjNums < 0 || queryObjNums > MAXWORKEROBJNUMS)
                queryObjNums = DEFAULTWORKEROBJNUMS;

            //初始化缓冲区
            for (int i = 0; i < queryObjNums; i++)
            {
                this.mWorkers.Enqueue(new PlatteWoker());
            }
        }


        private ConcurrentQueue<PlatteWoker> mWorkers = new ConcurrentQueue<PlatteWoker>();//存放号码查询对象的队列 
        private const int DEFAULTWORKEROBJNUMS = 1000;//默认缓冲区中对象个数
        private const int MAXWORKEROBJNUMS = 100000;//最大缓冲区对象个数

        /// <summary>
        /// 执行按号码查询车辆信息
        /// </summary>
        /// <param name="cb__"></param>
        /// <param name="platte"></param>
        /// <param name="flag">串口传递过来的除车牌号之外的字段信息</param>
        /// <param name="node"></param>
        /// <param name="current__"></param>
        public override void QueryCarRecord_async(AMD_ICarQuery_QueryCarRecord cb__, string platte,string flag, TollNode node, Ice.Current current__)
        {

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "ICarQueryImpl::QueryCarRecord_async()=>收到查询请求:platte={0},flag={1}", platte,flag);

            if (cb__ == null) return;

            if (string.IsNullOrWhiteSpace(platte)||node==null) 
            {
                cb__.ice_response(false, null, "无效的查询参数");
                return;
            }

            string error;
            PlatteWoker worker = null;
            if (this.mWorkers.TryDequeue(out worker) && worker != null)
            {
                worker.SetParams(platte, flag, node, cb__);
                
                QueryThreadWorkerPool queryThreadWorkerPool = SysComponents.GetComponent("QueryThreadWorkerPool") as QueryThreadWorkerPool;
                if (queryThreadWorkerPool == null)
                {
                    error = "获取查询组件QueryThreadWorkerPool失败";
                    LogerPrintWrapper.Print( LOGCS.LogLevel.ERROR,"ICarQueryImpl::QueryCarRecord_async()=>{0}", error);
                    
                    this.QueryCompled(worker);
                    cb__.ice_response(false, null, error);//立即返回结果
                }
                else
                {
                    //利用hash方式分发到不同的处理线程
                    QueryThreadWorker queryThreadWorker = queryThreadWorkerPool.GetWorker(DateTime.Now.Millisecond% queryThreadWorkerPool.GetWorkerCount());
                    if (!queryThreadWorker.AddPlatteWoker(worker)) 
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR,"ICarQueryImpl::QueryCarRecord_async()=>{0}", "将worker添加到查询线程失败");

                        this.QueryCompled(worker);
                        cb__.ice_response(false, null, "将worker添加到查询线程失败");//立即返回结果

                    }
                }


            }
            else 
            {
                cb__.ice_response(false, null, "没有可用的PlatteWoker对象");
            }
        }

        /// <summary>
        /// 批量查询，此接口相当于缓存预热功能
        /// </summary>
        /// <param name="querys"></param>
        /// <param name="error"></param>
        /// <param name="current__"></param>
        /// <returns></returns>
        public override bool BatchQuery(BatchQueryParams[] querys, out string error, Ice.Current current__)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "ICarQueryImpl::BatchQuery()=>收到查询请求,批量查询参数数组长度为:{0}",querys!=null?querys.Length:0);
            error = "未实现该接口";
            return false;
        }

        /// <summary>
        /// 消费记录批量上传
        /// </summary>
        /// <param name="records"></param>
        /// <param name="error"></param>
        /// <param name="current__"></param>
        /// <returns></returns>
        public override bool BatchUpload(CustomRecord[] records, out string error, Ice.Current current__)
        {

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "ICarQueryImpl::BatchUpload()=>收到批量上传请求,批量上传参数数组长度为:{0}", records != null ? records.Length : 0);

            error = string.Empty;
            InsertCustomRecordWorker insertCustomRecordWorker = SysComponents.GetComponent("InsertCustomRecordWorker") as InsertCustomRecordWorker;
            if (insertCustomRecordWorker == null)
            {
                error = "获取查询组件InsertCustomRecordWorker失败";
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "ICarQueryImpl::BatchUpload()=>{0}", error);
                return false;
            }
            else 
            {
                bool rs=insertCustomRecordWorker.AddCustomRecords(records,out error);
                if (!rs) 
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "ICarQueryImpl::BatchUpload()=>{0}", error);
                }
                return rs;
            }
        }

        /// <summary>
        /// 查询站点信息集合
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <param name="error"></param>
        /// <param name="current__"></param>
        /// <returns></returns>
        public override bool QueryStations(int from, int count, out Station[] lst, out string error, Ice.Current current__)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "ICarQueryImpl::QueryStations()=>收到查询站点请求,from:{0} count:{1}",from,count);

            lst = null;
            error = string.Empty;
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                error = "获取查询组件sqliteHandler失败";
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "ICarQueryImpl::QueryStations()=>{0}", error);
                return false;
            }
            else 
            {
                string query_sql = string.Format("select * from station order by bm limit {0},{1}",from,count);
                List<Station> result = sqliteHandler.SearcherStation(query_sql, out error);
                if (result == null)
                {
                    error = string.Format("SearcherStation()查询发送异常 SQL:{0}\r\n错误消息:{1}",query_sql, error);
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "ICarQueryImpl::QueryStations()=>{0}", error);
                    return false;
                }
                else 
                {
                    lst = result.ToArray();
                    return true;
                }
            }
        }


        //接口实现
        public void QueryCompled(PlatteWoker worker)
        {
            if (worker != null)
                this.mWorkers.Enqueue(worker);
        }

        /// <summary>
        /// 上传站点_节点信息到数据库
        /// </summary>
        /// <param name="flag">节点唯一编号</param>
        /// <param name="node">节点信息</param>
        /// <param name="error">错误信息</param>
        /// <param name="current__"></param>
        /// <returns></returns>
        public override bool UploadTollNode(string flag, TollNode node, out string error, Ice.Current current__)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "ICarQueryImpl::UploadTollNode()=>收到上传站点—节点信息请求");
            if (string.IsNullOrWhiteSpace(flag) || node == null) 
            {
                error = "输入参数无效";
                return false;
            }
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                error = "获取查询组件sqliteHandler失败";
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "ICarQueryImpl::UploadTollNode()=>{0}", error);
                return false;
            }
            else
            {
               return sqliteHandler.AddTollNodeRecord(flag, node, out error);
            }

        }
    }
}
