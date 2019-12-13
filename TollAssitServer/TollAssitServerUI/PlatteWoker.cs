using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASSISTICE;
using IDbHandler;
using CommHandler;
using System.Collections.Concurrent;
using TollAssistComm;


namespace CommHandler
{

    /// <summary>
    /// 完成通知
    /// </summary>
   public interface IQueryCompled 
    {
        void QueryCompled(PlatteWoker worker);
        
    }

    /// <summary>
    /// 车辆信息查询线程池 
    /// </summary>
    public sealed class QueryThreadWorkerPool 
    {

        private const int DEFAULTTHREADNUMS = 10;//默认线程数
        private const int MAXTHREADNUMS = 20;//最大线程数
        private int mThreadNums;//线程数
        private IQueryCompled mNotify;//完成通知对象
        private List<QueryThreadWorker> mWorkers = new List<QueryThreadWorker>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadNums">需要创建的QueryThreadWorker个数</param>
        /// <param name="notify">完成通知对象</param>
        public QueryThreadWorkerPool(int threadNums,IQueryCompled notify)
        {
            if (threadNums < 0 || threadNums > MAXTHREADNUMS)
                threadNums = DEFAULTTHREADNUMS;

            this.mThreadNums = threadNums;
            this.mNotify = notify;
            for (int i = 0; i < threadNums; i++)
            {
                mWorkers.Add(new QueryThreadWorker(i + 1, this.mNotify));
            }
        }

        /// <summary>
        /// 返回查询工作者线程个数
        /// </summary>
        /// <returns></returns>
        public int GetWorkerCount() 
        {
            return this.mThreadNums;
        }

        /// <summary>
        /// 获取工作者线程对象
        /// </summary>
        /// <param name="threadId">QueryThreadWorker对应的线程ID</param>
        /// <returns>具有threadId的QueryThreadWorker,无效返回NULL</returns>
        public QueryThreadWorker GetWorker(int threadId) 
        {
            if (threadId < 0 || threadId > (this.mWorkers.Count - 1))
                return null;

            return this.mWorkers[threadId];
        }

        /// <summary>
        /// 启动查询线程池
        /// </summary>
        public void Start()
        {
            foreach (QueryThreadWorker item in mWorkers)
            {
                item.Start();
            }
        }

        /// <summary>
        /// 关闭查询线程池
        /// </summary>
        public void Stop() 
        {
            foreach (QueryThreadWorker item in mWorkers)
            {
                item.Stop();
            }
        }

    }


    /// <summary>
    /// 查询线程
    /// 本类的使用方式：外部先初始化其实例，之后调用Start()=>[AddPlatteWoker()]=>Stop()
    /// </summary>
    public sealed class QueryThreadWorker 
    {
        private ConcurrentQueue<PlatteWoker> mPlatteWoker = new ConcurrentQueue<PlatteWoker>();//存放号码查询对象的队列
        private bool mIsStop = true;//停止线程工作
        private int mThreadNumber;//线程编号
        private IQueryCompled mNotify;//完成通知



        private void Worker(object state) 
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "QueryThreadWorker::Worker()=>{0}号查询线程开始工作", mThreadNumber);
            PlatteWoker worker;
            while (!this.mIsStop)
            {
                while (this.mPlatteWoker.TryDequeue(out worker) && (!this.mIsStop))
                {
                    if (worker != null) 
                    {
                        worker.Worker(null);

                        //发送完成通知
                        if (this.mNotify != null)
                            this.mNotify.QueryCompled(worker);
                    }
                }

                System.Threading.Thread.Sleep(10);//间隔10ms进行一次操作
            }
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "QueryThreadWorker::Worker()=>{0}号查询线程停止工作", mThreadNumber);
        }

        /// <summary>
        /// 查询线程工作者
        /// </summary>
        /// <param name="thdNumber">线程编号</param>
        /// <param name="notify">完成通知</param>
        public QueryThreadWorker(int thdCode, IQueryCompled notify)
        {
            this.mThreadNumber = thdCode;
            this.mNotify = notify;
        }

        /// <summary>
        /// 添加PlatteWoker处理队列中
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public bool AddPlatteWoker(PlatteWoker worker)
        {
            if (worker == null) return false;

            this.mPlatteWoker.Enqueue(worker);

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
                System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.Worker));
                thd.Name = "QueryThreadWorker";
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



    /// <summary>
    /// 号码查询工作者
    /// </summary>
    public sealed class PlatteWoker
    {
        private string mPlatte;//号码
        private string mFlag;//车辆信息标记
        private TollNode mNode;//收费站点信息
        private ASSISTICE.AMD_ICarQuery_QueryCarRecord mResponse;//响应对象

        public PlatteWoker()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platte"></param>
        /// <param name="node"></param>
        /// <param name="response">响应对象，如何该参数为NULL则不需要返回客户端信息</param>
        public PlatteWoker(string platte,TollNode node,ASSISTICE.AMD_ICarQuery_QueryCarRecord response)
        {
            this.mPlatte = platte;
            this.mNode = node;
            this.mResponse = response;
        }

        /// <summary>
        /// 设置相关参数
        /// 注意：此函数在调用Worker接口之前调用有效，否则行为未定义
        /// [非线程安全]
        /// </summary>
        /// <param name="platte"></param>
        /// <param name="carType"></param>
        /// <param name="node"></param>
        /// <param name="response"></param>
        public void SetParams(string platte,string flag, TollNode node, ASSISTICE.AMD_ICarQuery_QueryCarRecord response) 
        {
            this.mPlatte = platte;
            this.mFlag = flag;
            this.mNode = node;
            this.mResponse = response;
        }

        /// <summary>
        /// 执行相关查询
        /// 具体步骤:
        /// 1.首先查询本地缓冲数据(不再需要此步骤20171216PM);2.不存在记录则查询远程MS数据库;3.将结果返回调用方
        /// </summary>
        /// <param name="state"></param>
        public void Worker(object state)
        {
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::Worker()=>获取查询组件sqliteHandler失败");
                this.ResponseResult(false, null, "获取查询组件sqliteHandler失败");
                return;
            }

            if (string.IsNullOrEmpty(this.mPlatte)||this.mPlatte.Length<=6||this.mPlatte.Length>=10) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::Worker()=>无效的Platte");
                this.ResponseResult(false, null, "无效的Platte");
                return;
            }

            string error;

            //20171224 PM 取消只对川籍车辆进行查询限制功能           
            //条件：与最后一次比对车牌不一致、含有“川”、长度在7-10的车牌
            //bool boolSICHUAN = this.mPlatte.IndexOf("川") == 0;
            //if (!boolSICHUAN)
            //{
            //    //将此车辆信息添加到新车辆表中
            //    //CarTable newCarEntity = new CarTable();
            //    //newCarEntity.platte = this.mPlatte;
            //    //newCarEntity.cartype = this.mCarType;
            //    //PostCarTablesToNewCarInsertThread(newCarEntity, this.mNode);//非川籍车辆是否需要添加到新车辆表中？？
            //    //返回空记录
            //    this.ResponseResult(false, null, "非川籍车辆,信息无法获取");
            //    return;
            //}

            //备注：本地不再缓存车辆信息 20171216 PM
            ////生成sqlite语法形式的sql语句
            //string sqlite_query = string.Format("select * from cartable where platte='{0}' limit 1",this.mPlatte);
            ////1.首先查询本地缓冲数据
            List<CarTable>  carTables=null;//sqliteHandler.SearcherCarTable(sqlite_query, out error);
            //if (carTables == null) //发生异常
            //{
            //    error = string.Format("执行sqliteHandler.SearcherCarTable()失败,SQL：{0} \r\n错误信息：{1}", sqlite_query,error);
            //    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::Worker()=>{0}", error);
            //    this.ResponseResult(false, null, error);
            //    return;
            //}
            //if (carTables.Count == 0) //没有查询到数据
            {
                //2.向路公司的MYSQL数据库发起查询
                MySqlHandler MySqlHandler = SysComponents.GetComponent("MySqlHandler") as MySqlHandler;
                if (MySqlHandler == null)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::Worker()=>获取查询组件MySqlHandler失败");
                    this.ResponseResult(false, null, "获取查询组件MySqlHandler失败");
                    return;
                }
                //向路公司的MYSQL数据库发起查询
    
                StringBuilder mysql_query = new StringBuilder();

                //车辆基本信息查询
                mysql_query.Clear();
                mysql_query.Append("select car_info.remark,car_info.id,car_info.color,car_info.number,car_info.brand,car_info.type,");
                mysql_query.Append("car_info.axle_num,car_info.max_passenger,car_info.max_load,car_info.weight,car_info.creator,");
                mysql_query.Append("car_info.company,car_info.car_level as mon_level,'' as es_type,car_info.time ");
                mysql_query.Append("from car_info ");
                mysql_query.AppendFormat(" where car_info.number='{0}' LIMIT 2", this.mPlatte);
                string sql_query = mysql_query.ToString();
                List<CarTable>  totalCarTables = MySqlHandler.SearcherCarTable(sql_query, out error);
                if (totalCarTables == null)//发生异常
                {
                    error = string.Format("执行MySqlHandler.SearcherCarTable()失败,SQL：{0} \r\n错误信息：{1}", sql_query, error);
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::Worker()=>{0}", error);
                    this.ResponseResult(false, null, error);
                    return;
                }
                else if (totalCarTables.Count==0) //没有查询到车辆记录，作为新车
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "PlatteWoker::Worker()=>未从MySQL数据库命中信息");
                    //将此车辆信息添加到新车辆表中
                    CarTable newCarEntity = new CarTable();
                    newCarEntity.number = this.mPlatte;
                    newCarEntity.monLevel=IDbHandler.MySqlHandler.GetMonLevel()["未知"];//新车辆信息
                    //newCarEntity. = this.mCarType;
                    PostCarTablesToCarInsertThread(newCarEntity, this.mNode,this.mFlag);

                    //返回空记录
                    this.ResponseResult(false, null, "未找到需要的车辆信息");
                    return;
                }

                //以下为逃费信息相关查询

                ////根据车牌号和车牌号颜色作为唯一条件查询车辆表和逃费车辆表，找出最近一次未处理的逃费记录和车辆基本信息，最多两条(同号牌车牌颜色不同的车型)
                ////备注：由于查询条件中只提供车牌号故查询结果可能产生两条车辆基本信息(同号牌车牌颜色不同的车型)
                //mysql_query.Clear();
                //mysql_query.Append("select car_info.id,car_info.color,car_info.number,car_info.brand,car_info.type,");
                //mysql_query.Append("car_info.axle_num,car_info.max_passenger,car_info.max_load,car_info.weight,car_info.creator,");
                //mysql_query.Append("car_info.company,car_info.car_level as mon_level,b.type as es_type,b.remark,car_info.time ");
                //mysql_query.Append("from car_info LEFT JOIN ");
                //mysql_query.Append("(select * from es_car_info GROUP BY es_car_info.color,es_car_info.number HAVING MAX(es_car_info.input_time)) b ");
                //mysql_query.Append("on car_info.color= b.color and car_info.number=b.number");
                //mysql_query.AppendFormat(" where car_info.number='{0}' and (b.is_handle='未处理' or b.is_handle is NULL)  LIMIT 2", this.mPlatte); 
                //sql_query = mysql_query.ToString();//string.Format("select  top 1, * from cartable where platte='{0}'", this.mPlatte);
                //List<CarTable>  selectCarTables = MySqlHandler.SearcherCarTable(sql_query, out error);//此语句只能查询逃费表中存在未处理的记录或者逃费表中不存在的记录
                
                //2018-01-03 update
                //根据车牌号和车牌号颜色作为唯一条件查询车辆表和逃费车辆表，找出所有的逃费记录和车辆基本信息，
                //备注：由于查询条件中只提供车牌号故查询结果可能产生两条车辆基本信息(同号牌车牌颜色不同的车型)
                mysql_query.Clear();
                mysql_query.Append("select car_info.remark as CRemark,car_info.id,car_info.color,car_info.number,car_info.brand,car_info.type,");
                mysql_query.Append("car_info.axle_num,car_info.max_passenger,car_info.max_load,car_info.weight,car_info.creator,");
                mysql_query.Append("car_info.company,car_info.car_level as mon_level,b.type as es_type,car_info.remark as remark,b.cnt,b.input_time,car_info.time ");
                mysql_query.Append("from car_info INNER JOIN ");
                mysql_query.AppendFormat("(select count(*) as cnt,es_car_info.color,es_car_info.number,es_car_info.type,es_car_info.input_time from es_car_info where es_car_info.number='{0}' GROUP BY es_car_info.color,es_car_info.number,es_car_info.type ) b ", this.mPlatte);
                mysql_query.Append("on car_info.color= b.color and car_info.number=b.number");
                mysql_query.AppendFormat(" where car_info.number='{0}' ", this.mPlatte);
                sql_query = mysql_query.ToString();//string.Format("select  top 1, * from cartable where platte='{0}'", this.mPlatte);  20180303修改增加remark和input_time
                List<CarTable> selectCarTables = MySqlHandler.SearcherESCarTable(sql_query, out error);//此语句只能查询逃费表中存在未处理的记录或者逃费表中不存在的记录
                if (selectCarTables == null)//发生异常
                {
                    error = string.Format("执行MySqlHandler.SearcherCarTable()失败_逃费信息查询,SQL：{0} \r\n错误信息：{1}", sql_query, error);
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::Worker()=>{0}", error);
                    //返回车辆基本信息
                    this.ResponseResult(true, totalCarTables.ToArray(), error);
                }
                else 
                {
                    if (selectCarTables.Count != 0) //存在部分车辆逃费信息
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "PlatteWoker::Worker()=>从MySQL数据库命中部分逃费信息");

                        carTables = new List<CarTable>();
                        carTables.AddRange(selectCarTables);//将有逃费信息的车辆添加到结果集合中
                        //筛选逃费集合中没有但是总的车辆信息集合中存在的记录，并将其添加到结果集合中
                        foreach (CarTable item in totalCarTables)
                        {
                            bool exists=selectCarTables.Exists(car => car.number == item.number && car.color == item.color);
                            if (!exists) 
                            {
                                carTables.Add(item);
                            }
                        }

                        //将命中记录缓存到本地SQLite数据库中方便下次查询
                        //PostCarTablesToInsertThread(carTables[0]);



                        //返回记录
                        this.ResponseResult(true, carTables.ToArray(), string.Empty);
                    }
                    else 
                    {
                        //没有记录只能说有可能逃费表中存在已处理记录，只需要把车辆基本信息查询出来即可
                       


                        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "PlatteWoker::Worker()=>找到车辆记录单未从MySQL数据库命中逃费信息");
                        ////将此车辆信息添加到新车辆表中
                        //CarTable newCarEntity = new CarTable();
                        //newCarEntity.number = this.mPlatte;
                        ////newCarEntity. = this.mCarType;
                        //PostCarTablesToNewCarInsertThread(newCarEntity, this.mNode);

                        //返回空记录
                        //this.ResponseResult(false, null, "未找到需要的车辆信息");

                        //返回记录
                        this.ResponseResult(true, totalCarTables.ToArray(), string.Empty);

                    }
                }
            }
            //备注：本地不再缓存车辆信息 20171216 PM
            //else 
            //{ 
            //    //缓存命中直接返回
            //    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "PlatteWoker::Worker()=>从缓存数据库命中信息=>车牌号为:{0}", this.mPlatte);
            //    this.ResponseResult(true, carTables[0], string.Empty);
            //}
            
        }

        /// <summary>
        /// 向请求端返回结果
        /// </summary>
        /// <param name="ret">函数返回值</param>
        /// <param name="carTable">输出参数</param>
        /// <param name="error">错误信息</param>
        private void ResponseResult(bool ret,CarTable[] carTable,string error) 
        {
            //车辆记录到本地库 2018-01-02 PM
            if (ret && carTable != null) 
            {
                foreach (var item in carTable)
                {
                    PostCarTablesToCarInsertThread(item, this.mNode,this.mFlag);
                }
            }

            if (this.mResponse != null) 
            {
                this.mResponse.ice_response(ret, carTable, error);//这里是否会发生ICE异常?
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "PlatteWoker::ResponseResult()=>响应对号码:{0} 的查询请求,处理结果:{1},错误消息:{2}",this.mPlatte,ret.ToString(),error!=null?error:"无");
            }
           

        }

        /// <summary>
        /// 备注：本地不再缓存车辆信息 20171216 PM
        /// 投递车辆信息集合到后台入库线程进行入库操作
        /// </summary>
        //public static void PostCarTablesToInsertThread(CarTable table) 
        //{
        //    InsertCarInfoWorker worker = SysComponents.GetComponent("InsertCarInfoWorker") as InsertCarInfoWorker;
        //    if (worker == null)
        //    {
        //        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToInsertThread()=>获取组件InsertCarInfoWorker失败");
        //        return;
        //    }

        //   int id=worker.AllocCarObjectID();
        //   if (id != 0)
        //   {
        //       if (!worker.CopyCarObjectToBuffer(table, id)) 
        //       {
        //           LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToInsertThread()=>调用CopyCarObjectToBuffer()失败");
        //       }
        //   }
        //   else 
        //   {
        //       LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToInsertThread()=>调用AllocCarObjectID()失败，没有可用的资源");
        //   }

        //}

        /// <summary>
        /// 投递车辆信息集合到后台入库线程进行新车辆(车辆)入库操作
        /// </summary>
        private void PostCarTablesToCarInsertThread(CarTable table,TollNode node,string cardId)
        {
            InsertCarRecordWorker worker = SysComponents.GetComponent("InsertCarRecordWorker") as InsertCarRecordWorker;
            if (worker == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToCarInsertThread()=>获取组件InsertCarRecordWorker失败");
                return;
            }

            int id = worker.AllocCarObjectID();
            if (id != 0)
            {
                if (!worker.CopyCarObjectToBuffer(table,node,cardId, id))
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToCarInsertThread()=>调用CopyCarObjectToBuffer()失败");
                }
            }
            else
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToCarInsertThread()=>调用AllocCarObjectID()失败，没有可用的资源");
            }

        }

       
    }

    /// <summary>
    /// 备注：本地不再缓存车辆信息 20171216 PM
    /// 将车辆信息插入到本地缓冲数据库的CarTable表中
    /// [线程类]
    /// 本类的使用方式：外部先初始化其实例，之后调用Start()=>[AllocCarObjectID()=>CopyCarObjectToBuffer()]=>Stop()
    /// </summary>
    //public sealed class InsertCarInfoWorker 
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="carObjNums">carObjNums需要缓存的CarTable对象的个数</param>
    //    public InsertCarInfoWorker(int carObjNums)
    //    {
    //        if (carObjNums < 0 || carObjNums > MAXCAROBJNUMS)
    //            carObjNums = DEFAULTCAROBJNUMS;

    //        //初始化缓冲区
    //        this.mCarObjNums = carObjNums;
    //        for (int i = 0; i < carObjNums; i++)
    //        {
    //            this.mCarObjBuffer.Add(i + 1, new CarTableWrapper());
    //        }

    //    }

    //    /// <summary>
    //    /// 车辆信息包装类
    //    /// </summary>
    //    class CarTableWrapper
    //    {
    //        public bool used=false;//标识当前对象是否在使用中
    //        public bool locked = true;//标识当前对象是否可用,锁定的对象不能获取其值
    //        public CarTable obj=new CarTable();
    //    }

    //    private const int DEFAULTCAROBJNUMS = 1000;//默认缓冲区中对象个数
    //    private const int MAXCAROBJNUMS=100000;//最大缓冲区对象个数
    //    private const int MAXREFRESHTIME = 5;//最大刷新时间(秒)，当超过该时间则刷新缓冲区数据到库中
    //    private const int MINREQUESTOBJSCNT = 10;//批量插入的最小对象数，当大于等于此值时则刷新缓冲区数据到库中
    //    private int mCarObjNums;//需要缓存的CarTable对象的个数,方便内部循环使用
    //    private Dictionary<int, CarTableWrapper> mCarObjBuffer = new Dictionary<int, CarTableWrapper>();
    //    private bool mIsStop = true;//停止线程工作
    //    private object resLockObj = new object();

    //    /// <summary>
    //    /// 执行批量插入
    //    /// </summary>
    //    /// <param name="cars"></param>
    //    /// <param name="error"></param>
    //    /// <returns></returns>
    //    private bool BatchInsert(List<CarTable> cars, out string error) 
    //    {
           
    //        SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
    //        if (sqliteHandler == null)
    //        {
    //            error="InsertCarInfoWorker::BatchInsert()=>获取查询组件sqliteHandler失败";
    //            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InsertCarInfoWorker::BatchInsert()=>获取查询组件sqliteHandler失败");
    //            return false;
    //        }
    //        return sqliteHandler.BatchAddCarTable(cars, out error);

    //    }

    //    /// <summary>
    //    /// 线程函数:执行数据库插入操作
    //    /// </summary>
    //    /// <param name="stat"></param>
    //    private void Worker(object stat) 
    //    {
    //        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCarInfoWorker::Worker()=>数据库插入线程开始工作");
    //        List<CarTableWrapper> carTableWrappers = new List<CarTableWrapper>();
    //        List<CarTable> carTabs = new List<CarTable>();
    //        string error;
    //        int ticks=0;//计时器
    //        while (!this.mIsStop) 
    //        {
    //            carTableWrappers.Clear();
    //            foreach (var item in this.mCarObjBuffer)
    //            {
    //                if (item.Value.used && (!item.Value.locked)) 
    //                {
    //                    //将此对象添加到插入列表中
    //                    carTableWrappers.Add(item.Value);
    //                }
    //            }
    //            if (carTableWrappers.Count >= MINREQUESTOBJSCNT || ticks >= MAXREFRESHTIME) //当缓冲区数据达到指定值或超时
    //            {
    //                if (carTableWrappers.Count >0) 
    //                {
    //                    //拷贝需要入库的对象实体到集合
    //                    carTabs.Clear();
    //                    foreach (var item in carTableWrappers)
    //                    {
    //                        carTabs.Add(item.obj);
    //                    }
    //                    //执行入库操作
    //                    if (!this.BatchInsert(carTabs, out error))
    //                    {
    //                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InsertCarInfoWorker::Worker()=>执行车辆信息入库失败:{0}", error);
    //                    }
    //                    else
    //                    {
    //                        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCarInfoWorker::Worker()=>执行车辆信息入库成功:{0}条", carTabs.Count);
    //                    }

    //                    //循环对每个对象解锁
    //                    foreach (CarTableWrapper item in carTableWrappers)
    //                    {
    //                        //注意顺序
    //                        item.locked = true;//还原锁定
    //                        item.used = false;//还原使用状态
    //                    }
    //                }

    //                ticks = 0;//计时器归零
    //            }
    //            else 
    //            {
    //                ticks++;
    //            }

    //            System.Threading.Thread.Sleep(1000);//间隔1s进行一次操作
    //        }
    //        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCarInfoWorker::Worker()=>数据库插入线程已停止工作");
    //    }

    //    /// <summary>
    //    /// 获取可用的资源ID
    //    /// </summary>
    //    /// <returns>成功返回资源ID，失败返回0</returns>
    //    public int AllocCarObjectID() 
    //    {
    //        int id = 0;
    //        lock (resLockObj) 
    //        {
    //            foreach (var item in this.mCarObjBuffer)
    //            {
    //                if (!item.Value.used) 
    //                {
    //                    id = item.Key;
    //                    item.Value.used = true;//标记当前缓冲区为使用中
    //                    break;
    //                }
    //            }
    //        }
    //        return id;
    //    }

    //    ///// <summary>
    //    ///// 释放CarObject
    //    ///// [非线程安全,是否应该提供该接口???]
    //    ///// </summary>
    //    ///// <param name="id">调用AllocCarObjectID返回的有效ID</param>
    //    //public void FreeCarObjectByID(int id) 
    //    //{
    //    //    if (this.mCarObjBuffer.ContainsKey(id)) 
    //    //    {
    //    //        if (this.mCarObjBuffer[id].used&&this.mCarObjBuffer[id].locked) 
    //    //        {
    //    //            this.mCarObjBuffer[id].used = false;
    //    //        }
    //    //    }
    //    //}

    //    /// <summary>
    //    /// 拷贝对象到分配的空间中
    //    /// </summary>
    //    /// <param name="table"></param>
    //    /// <param name="destId">调用AllocCarObjectID返回的有效ID</param>
    //    /// <returns></returns>
    //    public bool CopyCarObjectToBuffer(CarTable table, int destId) 
    //    {
    //        if (!this.mCarObjBuffer.ContainsKey(destId))
    //            return false;

    //        if (this.mCarObjBuffer[destId].used && this.mCarObjBuffer[destId].locked)
    //        {
    //            CarTable obj = this.mCarObjBuffer[destId].obj;
    //            obj.platte = table.platte;
    //            obj.carclass = table.carclass;
    //            obj.cartype = table.cartype;
    //            obj.num = table.num;
    //            obj.remark = table.remark;
    //            obj.unit = table.unit;
    //            obj.master = table.master;
    //            obj.monlevel = table.monlevel;
    //            obj.smsreport = table.smsreport;
    //            obj.comment = table.comment;
    //            obj.dtime = table.dtime;

    //            //标记当前对象可以被使用(解锁)=>使入库线程可以获取该对象的值
    //            this.mCarObjBuffer[destId].locked = false;

    //            return true;
    //        }

    //        return false;

    //    }

    //    /// <summary>
    //    /// 开始线程工作
    //    /// </summary>
    //    public void Start() 
    //    {
    //        if (this.mIsStop) 
    //        {
    //            this.mIsStop = false;
    //            System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.Worker));
    //            thd.Name = "InsertCarInfoWorker";
    //            thd.IsBackground = true;
    //            thd.Start(null);
    //        }
    //    }

    //    /// <summary>
    //    /// 停止线程工作
    //    /// </summary>
    //    public void Stop() 
    //    {
    //        if (!this.mIsStop)
    //            this.mIsStop = true;
    //    }
    //}


    /// <summary>
    /// 将车辆信息插入到本地缓冲数据库的newPlatte表中
    /// [线程类]
    /// 本类的使用方式：外部先初始化其实例，之后调用Start()=>[AllocCarObjectID()=>CopyCarObjectToBuffer()]=>Stop()
    /// </summary>
    public sealed class InsertCarRecordWorker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCarObjNums">carObjNums需要缓存的NewPlatte对象的个数</param>
        public InsertCarRecordWorker(int newCarObjNums)
        {
            if (newCarObjNums < 0 || newCarObjNums > MAXCAROBJNUMS)
                newCarObjNums = DEFAULTCAROBJNUMS;

            //初始化缓冲区
            this.mNewCarObjNums = newCarObjNums;
            for (int i = 0; i < newCarObjNums; i++)
            {
                this.mCarObjBuffer.Add(i + 1, new CarRecordWrapper());
            }

        }

        /// <summary>
        /// 新车辆信息包装类
        /// </summary>
        class CarRecordWrapper
        {
            public bool used = false;//标识当前对象是否在使用中
            public bool locked = true;//标识当前对象是否可用,锁定的对象不能获取其值
            public CarRecord obj = new CarRecord();
        }

        private const int DEFAULTCAROBJNUMS = 1000;//默认缓冲区中对象个数
        private const int MAXCAROBJNUMS = 100000;//最大缓冲区对象个数
        private const int MAXREFRESHTIME = 5;//最大刷新时间(秒)，当超过该时间则刷新缓冲区数据到库中
        private const int MINREQUESTOBJSCNT = 10;//批量插入的最小对象数，当大于等于此值时则刷新缓冲区数据到库中
        private int mNewCarObjNums;//需要缓存的NewPlatte对象的个数,方便内部循环使用
        private Dictionary<int, CarRecordWrapper> mCarObjBuffer = new Dictionary<int, CarRecordWrapper>();
        private bool mIsStop = true;//停止线程工作
        private object resLockObj = new object();

        /// <summary>
        /// 执行批量插入
        /// </summary>
        /// <param name="cars"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool BatchInsert(List<CarRecord> cars, out string error)
        {

            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                error = "InsertNewPlatteWorker::BatchInsert()=>获取查询组件sqliteHandler失败";
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InsertCarRecordWorker::BatchInsert()=>获取查询组件sqliteHandler失败");
                return false;
            }
            return sqliteHandler.BatchAddNewPlatte(cars, out error);

        }

        /// <summary>
        /// 线程函数:执行数据库插入操作
        /// </summary>
        /// <param name="stat"></param>
        private void Worker(object stat)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCarRecordWorker::Worker()=>(未识别车辆)数据库插入线程开始工作");
            List<CarRecordWrapper> newCarWrappers = new List<CarRecordWrapper>();
            List<CarRecord> carTabs = new List<CarRecord>();
            string error;
            int ticks = 0;//计时器
            while (!this.mIsStop)
            {
                newCarWrappers.Clear();
                foreach (var item in this.mCarObjBuffer)
                {
                    if (item.Value.used && (!item.Value.locked))
                    {
                        //将此对象添加到插入列表中
                        newCarWrappers.Add(item.Value);
                    }
                }
                if (newCarWrappers.Count >= MINREQUESTOBJSCNT || ticks >= MAXREFRESHTIME) //当缓冲区数据达到指定值或超时
                {
                    if (newCarWrappers.Count > 0) 
                    {
                        //拷贝需要入库的对象实体到集合
                        carTabs.Clear();
                        foreach (var item in newCarWrappers)
                        {
                            carTabs.Add(item.obj);
                        }
                        //执行入库操作
                        if (!this.BatchInsert(carTabs, out error))
                        {
                            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InsertCarRecordWorker::Worker()=>执行车辆信息入库失败:{0}", error);
                        }
                        else
                        {
                            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCarRecordWorker::Worker()=>执行车辆信息入库成功:{0}条", carTabs.Count);
                        }

                        //循环对每个对象解锁
                        foreach (CarRecordWrapper item in newCarWrappers)
                        {
                            //注意顺序
                            item.locked = true;//还原锁定
                            item.used = false;//还原使用状态
                        }
                    }

                    ticks = 0;//计时器归零
                }
                else
                {
                    ticks++;
                }

                System.Threading.Thread.Sleep(1000);//间隔1s进行一次操作
            }
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCarRecordWorker::Worker()=>(车辆记录)数据库插入线程已停止工作");
        }

        /// <summary>
        /// 获取可用的资源ID
        /// </summary>
        /// <returns>成功返回资源ID，失败返回0</returns>
        public int AllocCarObjectID()
        {
            int id = 0;
            lock (resLockObj)
            {
                foreach (var item in this.mCarObjBuffer)
                {
                    if (!item.Value.used)
                    {
                        id = item.Key;
                        item.Value.used = true;//标记当前缓冲区为使用中
                        break;
                    }
                }
            }
            return id;
        }

        ///// <summary>
        ///// 释放CarObject
        ///// [非线程安全,是否应该提供该接口???]
        ///// </summary>
        ///// <param name="id">调用AllocCarObjectID返回的有效ID</param>
        //public void FreeCarObjectByID(int id) 
        //{
        //    if (this.mCarObjBuffer.ContainsKey(id)) 
        //    {
        //        if (this.mCarObjBuffer[id].used&&this.mCarObjBuffer[id].locked) 
        //        {
        //            this.mCarObjBuffer[id].used = false;
        //        }
        //    }
        //}

        /// <summary>
        /// 拷贝对象到分配的空间中
        /// </summary>
        /// <param name="table"></param>
        /// <param name="node"></param>
        /// <param name="destId">调用AllocCarObjectID返回的有效ID</param>
        /// <returns></returns>
        public bool CopyCarObjectToBuffer(CarTable table,TollNode node,string cardId, int destId)
        {
            if (!this.mCarObjBuffer.ContainsKey(destId))
                return false;

            if (this.mCarObjBuffer[destId].used && this.mCarObjBuffer[destId].locked)
            {
                CarRecord obj = this.mCarObjBuffer[destId].obj;
                obj.number = table.number;
                obj.color = table.color;
                obj.monLevel = table.monLevel;
                obj.type = table.type;
                obj.flag = cardId;
                obj.companycode = node.companycode;
                obj.plazcode = node.plazcode;
                obj.lanname= node.lanname;
                obj.lannum = node.lannum;
                obj.dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//手动打上时间信息
               

                //标记当前对象可以被使用(解锁)=>使入库线程可以获取该对象的值
                this.mCarObjBuffer[destId].locked = false;

                return true;
            }

            return false;

        }

        /// <summary>
        /// 开始线程工作
        /// </summary>
        public void Start()
        {
            if (this.mIsStop)
            {
                this.mIsStop = false;
                System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.Worker));
                thd.Name = "InsertCarRecordWorker";
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

    /// <summary>
    /// 将消费记录插入本地消费记录表中
    /// [线程类]
    /// 本类的使用方式：外部先初始化其实例，之后调用Start()=>[AddCustomRecords()]=>Stop()
    /// </summary>
    public sealed class InsertCustomRecordWorker 
    {
        private ConcurrentQueue<CustomRecord[]> mCustomRecords = new ConcurrentQueue<CustomRecord[]>();//存放消费记录的队列
        private bool mIsStop = true;//停止线程工作
        private const int MAXQUEUECOUNT = 1000;//mCustomRecords允许的最大大小

        /// <summary>
        /// 执行批量插入
        /// </summary>
        /// <param name="cars"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool BatchInsert(List<CustomRecord> customRecords, out string error)
        {

            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                error = "InsertCustomRecordWorker::BatchInsert()=>获取查询组件sqliteHandler失败";
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InsertCustomRecordWorker::BatchInsert()=>获取查询组件sqliteHandler失败");
                return false;
            }
            return sqliteHandler.BatchAddCustomRecord(customRecords, out error);

        }

        /// <summary>
        /// 线程函数:执行数据库插入操作
        /// </summary>
        /// <param name="stat"></param>
        private void Worker(object stat) 
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCustomRecordWorker::Worker()=>消费记录_数据库插入线程开始工作");
            List<CustomRecord> customRecords = new List<CustomRecord>();
            string error;
            CustomRecord[] result;
            while (!this.mIsStop)
            {
                while (this.mCustomRecords.TryDequeue(out result)&&(!this.mIsStop)) 
                {
                    customRecords.Clear();
                    customRecords.AddRange(result);//添加消费记录到列表
                    //执行入库操作
                    if (!this.BatchInsert(customRecords, out error))
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InsertCustomRecordWorker::Worker()=>执行消费记录信息入库失败:{0}", error);
                    }
                    else
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCustomRecordWorker::Worker()=>执行消费记录信息入库成功:{0}条", customRecords.Count);
                    }
                    result = null;
                    
                }

                System.Threading.Thread.Sleep(1000);//间隔1s进行一次操作
            }
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "InsertCustomRecordWorker::Worker()=>消费记录_数据库插入线程已停止工作");
        }

        /// <summary>
        /// 添加消费记录到处理队列中
        /// </summary>
        /// <param name="records"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool AddCustomRecords(CustomRecord[] records,out string error) 
        {
            error = string.Empty;
            if (records == null)
            {
                error = "参数records无效";
                return false;
            }

            if (this.mCustomRecords.Count > MAXQUEUECOUNT) 
            {
                error = "消费记录处理队列已满，请稍后再试";
                return false;
            }

            this.mCustomRecords.Enqueue(records);

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
                System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.Worker));
                thd.Name = "InsertCustomRecordWorker";
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
