using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDbHandler;
using TollAssistComm;
using System.Data;
using ASSISTICE;


namespace CommHandler
{
    /// <summary>
    /// 消费记录上传工作者
    /// </summary>
    public class CustomRecordUploadWorker
    {
        public CustomRecordUploadWorker()
        {


        }

        /// <summary>
        /// 导出并上传消费记录到中心服务器
        /// </summary>
        /// <param name="begin">需要上传的消费记录的开始时间(包括该值)</param>
        /// <param name="end">需要上传的消费记录的结束时间(包括该值)</param>
        public void Worker(DateTime begin,DateTime end) 
        {

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadWorker::Worker()=>获取SysConfig实例失败");
                return;
            }

            //获取sqlite查询实例
            SqliteHandler sqliteHandler=SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadWorker::Worker()=>获取SqliteHandler实例失败");
                return;
            }

            //文件的末尾名称
            string endName=UploadFileNameOfEnd(sysConfig.mUploadCustomRecordFrequency);

            //生成导出临时文本文件名称
            string customRecordName = string.Format("{0}_{1}_{2}_({3}).txt","消费",begin.ToString("yyyyMMddHHmmss"), end.ToString("yyyyMMddHHmmss"), endName);

            string customRecordPath = System.IO.Path.Combine(sysConfig.mUploadTmpDir, customRecordName);//消费记录文件完整路径
            if (!System.IO.Directory.Exists(sysConfig.mUploadTmpDir)) 
            {
                System.IO.Directory.CreateDirectory(sysConfig.mUploadTmpDir);
            }

            //执行相关查询语句
            string error;
            int count = 1000;
            string sql;
            string strObj;
            for (int from = 0; ; from+=count)
            {
                sql = string.Format("select * from CustomRecord where (dtime>=datetime('{0}') and dtime<=datetime('{1}')) LIMIT {2} OFFSET {3}"
               , begin.ToString("yyyy-MM-dd HH:mm:ss"), end.ToString("yyyy-MM-dd HH:mm:ss"), count, from);

               List<ASSISTICE.CustomRecord> customRecords= sqliteHandler.SearcherCustomRecord(sql, out error);

               if (customRecords == null) 
               {
                   LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadWorker::Worker()=>执行sql:{0}发生异常:{1}", sql, error);
                   break;
               }

                //没有记录则跳出分页
               if (customRecords.Count == 0)
                   break;

               if (!System.IO.File.Exists(customRecordPath)) //新文件在文件开始位置添加列表头
               {
                   string columnHeader = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}", "id", "number", "color", "brand", "type", "flag", "monLevel", "dtime", "operator", "customamount",
                       "companycode", "plazcode", "lanname", "lannum");
                   BuildColumnHeader(columnHeader, customRecordPath);
               }

                //导出查询到的记录到文本中
               using (System.IO.FileStream fs = new System.IO.FileStream(customRecordPath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
               {
                   using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.Default))
                   {
                       foreach (ASSISTICE.CustomRecord customRecord in customRecords)
                       {
                           strObj = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}", customRecord.id, customRecord.number,
                               customRecord.color,customRecord.brand,customRecord.type,customRecord.flag,customRecord.monLevel,
                               customRecord.dtime,customRecord.@operator,customRecord.customamount,
                               customRecord.node.companycode,customRecord.node.plazcode,customRecord.node.lanname,customRecord.node.lannum);
                           sw.WriteLine(strObj);
                       }
                   }
               }

            }

            //查询指定的文件是否存在
            if (!System.IO.File.Exists(customRecordPath))
                return;
           
            //打开网络共享
            bool ret=NetworkShareFileHelper.OpenConnection(sysConfig.mCenterShareFolderAddress, sysConfig.mCenterShareFolderUser, sysConfig.mCenterShareFolderPassword);
            if (!ret) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadWorker::Worker()=>执行打开远程共享文件失败,消费记录上传取消");
                return;
            }
            //消费记录文件夹路径
            string customRecordFolderDir = System.IO.Path.Combine(sysConfig.mCenterShareFolderAddress,sysConfig.mCenterUploadCustomRecordFolderName, sysConfig.mCompanyCode, sysConfig.mPlazCode);
            if (!System.IO.Directory.Exists(customRecordFolderDir)) 
            {
                System.IO.Directory.CreateDirectory(customRecordFolderDir);
            }

            string remoteCustomRecordFile=System.IO.Path.Combine(customRecordFolderDir,customRecordName);

            //上传消费记录
            System.IO.File.Copy(customRecordPath, remoteCustomRecordFile, true);

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "CustomRecordUploadWorker::Worker()=>上传时间点为[{0} TO {1}]的消费记录:{2}成功", begin.ToString("yyyy-MM-dd HH:mm:ss"), end.ToString("yyyy-MM-dd HH:mm:ss"), customRecordName);

            ret = NetworkShareFileHelper.DisConnection(sysConfig.mCenterShareFolderAddress);

            //是否删除临时记录文件
            System.IO.File.Delete(customRecordPath);

        }


        /// <summary>
        /// 删除N天以前消费记录
        /// </summary>
        /// <param name="days"></param>
        public void DeleteCustomRecord(int days)
        {
            if (days <= 0)
                return;

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadWorker::DeleteCustomRecord()=>获取SysConfig实例失败");
                return;
            }

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadWorker::DeleteCustomRecord()=>获取SqliteHandler实例失败");
                return;
            }

            //得到days天以前的日期
            DateTime endTime = DateTime.Now.Subtract(new TimeSpan(days, 0, 0, 0));

            //生成删除语句
            string sql = string.Format("delete from CustomRecord where dtime<datetime('{0}')",endTime.ToString("yyyy-MM-dd HH:mm:ss"));

            string error;
            if (sqliteHandler.ExecuteNonQuery(sql, out error) == -1)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadWorker::DeleteCustomRecord()=>执行删除时间在{0}之前的数据的SQL语句失败:{1}", endTime.ToString("yyyy-MM-dd HH:mm:ss"), error);
            }
            else 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "CustomRecordUploadWorker::DeleteCustomRecord()=>执行删除时间在{0}之前的数据的SQL语句成功", endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            
            
        }

        /// <summary>
        /// 生成上传文件名的末尾频率信息字符
        /// </summary>
        /// <param name="freq">上传频率</param>
        /// <returns></returns>
        public static string UploadFileNameOfEnd(AutoUploadFrequency freq)
        {
            string strFileEndName = string.Empty;
            switch (freq)
            {
                case AutoUploadFrequency.Day:
                    {
                        strFileEndName = "天";
                        break;
                    }
                case AutoUploadFrequency.Week:
                    {
                        strFileEndName = "周";
                        break;
                    }
                case AutoUploadFrequency.Month:
                    {
                        strFileEndName = "月";
                        break;
                    }
            }

            return strFileEndName;
        }

        /// <summary>
        /// 生成列表头
        /// </summary>
        /// <param name="ColumnHeader">列表头文本</param>
        /// <param name="filePath">完整的文件路径</param>
        public static void BuildColumnHeader(string ColumnHeader, string filePath) 
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.Default))
                {
                    sw.WriteLine(ColumnHeader);
                }
            }
        }
    }

    /// <summary>
    /// 消费记录上传线程
    /// 调用方式:Start()=>Stop()
    /// </summary>
    public class CustomRecordUploadThread 
    {
        public CustomRecordUploadThread()
        {

        }

        private bool mIsStop = true;

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

            CustomRecordUploadWorker worker = new CustomRecordUploadWorker();

            DateTime beginTime=DateTime.Now,endTime=DateTime.Now;
            bool isUpload = false;//是否需要上传
            while (!this.mIsStop) 
            {
                if (DateTime.Now.Hour == sysConfig.mUploadCustomRecordTime) //自动上传时间点匹配
                {
                    isUpload = CustomRecordUploadThread.MakeTimeSpanByFrequency(sysConfig.mUploadCustomRecordFrequency, out beginTime, out endTime);

                    if (isUpload) //需要上传
                    {
                        try
                        {
                            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "CustomRecordUploadThread::ThreadWorker()=>执行消费记录信息上传工作,时间段为:[{0} TO {1}]",
                                beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            worker.Worker(beginTime, endTime);
                        }
                        catch (Exception ex)
                        {
                            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CustomRecordUploadThread::ThreadWorker()=>调用上传方法发生异常:{0}",ex.Message);
                            
                        }

                    }
                    //删除N天前的数据
                    worker.DeleteCustomRecord(sysConfig.mCustomRecordSaveDays);

                    for (int i = 0; i < 3600&&(!this.mIsStop); i++)
                    {
                        System.Threading.Thread.Sleep(1000);//跳过当前时间点3600*1000
                    }
                   
                }

                //每5分钟检查一次
                for (int i = 0; i < 300&&(!this.mIsStop); i++)
                {
                    System.Threading.Thread.Sleep(1000);//300*1000
                }
                
            }   
        }

        /// <summary>
        /// 根据自动上传频率生成上传时间段
        /// </summary>
        /// <param name="frequency">上传频率</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间(包含)</param>
        /// <returns>是否成功</returns>
        public static bool MakeTimeSpanByFrequency(AutoUploadFrequency frequency, out DateTime beginTime, out DateTime endTime) 
        {
            beginTime = DateTime.Now;
            endTime = DateTime.Now;
            bool ret = false;
            //根据频率生成上传时间段
            switch (frequency)
            {
                case AutoUploadFrequency.Day:
                    {
                        beginTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                        beginTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day, 0, 0, 0);
                        endTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day, 23, 59, 59);
                        ret = true;
                        break;
                    }
                case AutoUploadFrequency.Week:
                    {
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                        {
                            beginTime = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0));
                            beginTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day, 0, 0, 0);
                            endTime = beginTime.Add(new TimeSpan(6, 23, 59, 59));// new DateTime(beginTime.Year, beginTime.Month, beginTime.Day, 23, 59, 59);
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    }
                case AutoUploadFrequency.Month:
                    {
                        if (DateTime.Now.Day == 1)
                        {
                            beginTime = DateTime.Now.AddMonths(-1);
                            beginTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day, 0, 0, 0);
                            DateTime tmpTime = DateTime.Now.AddDays(-1);
                            endTime = new DateTime(tmpTime.Year, tmpTime.Month, tmpTime.Day, 23, 59, 59);
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    }
            }

            return ret;
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



    /// <summary>
    /// 新记录上传工作者
    /// </summary>
    public class CarRecordUploadWorker
    {
        public CarRecordUploadWorker()
        {


        }

        /// <summary>
        /// 导出并上传新车牌记录到中心服务器
        /// </summary>
        /// <param name="begin">需要上传的新车牌记录的开始时间(包括该值)</param>
        /// <param name="end">需要上传的新车牌记录的结束时间(包括该值)</param>
        public void Worker(DateTime begin, DateTime end,string monlevel)
        {

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CarRecordUploadWorker::Worker()=>获取SysConfig实例失败");
                return;
            }

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CarRecordUploadWorker::Worker()=>获取SqliteHandler实例失败");
                return;
            }

            //文件的末尾名称
            string endName =CustomRecordUploadWorker.UploadFileNameOfEnd(sysConfig.mUploadCarRecordFrequency);

            //生成导出临时文本文件名称
            string recordName = string.Format("{0}_{1}_{2}_({3}).txt", monlevel+"记录", begin.ToString("yyyyMMddHHmmss"), end.ToString("yyyyMMddHHmmss"), endName);

            string recordPath = System.IO.Path.Combine(sysConfig.mUploadTmpDir, recordName);//车记录文件完整路径
            if (!System.IO.Directory.Exists(sysConfig.mUploadTmpDir))
            {
                System.IO.Directory.CreateDirectory(sysConfig.mUploadTmpDir);
            }

            //执行相关查询语句
            string error;
            int count = 1000;
            string sql;
            int iMonlevel = -1;
            if (IDbHandler.MySqlHandler.GetMonLevel().ContainsKey(monlevel))
                iMonlevel = IDbHandler.MySqlHandler.GetMonLevel()[monlevel];

            for (int from = 0; ; from += count)
            {
                sql = string.Format("select * from NewPlatte where (dtime>=datetime('{0}') and dtime<=datetime('{1}')) and monlevel={4} LIMIT {2} OFFSET {3}"
               , begin.ToString("yyyy-MM-dd HH:mm:ss"), end.ToString("yyyy-MM-dd HH:mm:ss"), count, from,iMonlevel);

                List<ASSISTICE.CarRecord> records = sqliteHandler.SearcherCarRecord(sql, out error);

                if (records == null)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CarRecordUploadWorker::Worker()=>执行sql:{0}发生异常:{1}", sql, error);
                    break;
                }

                if (records.Count == 0) //不存在车辆记录 
                {
                    break;
                }

                if (!ExportCarRecordToFile(records, recordPath, out error)) 
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CarRecordUploadWorker::Worker()=>执行ExportCarRecordToFile()发生异常:{0}", error);
                    break;
                }
               
            }

            //查询指定的文件是否存在
            if (!System.IO.File.Exists(recordPath))
                return;

            //打开网络共享
            bool ret = NetworkShareFileHelper.OpenConnection(sysConfig.mCenterShareFolderAddress, sysConfig.mCenterShareFolderUser, sysConfig.mCenterShareFolderPassword);
            if (!ret)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CarRecordUploadWorker::Worker()=>执行打开远程共享文件失败,{0}记录上传取消",monlevel);
                return;
            }
            //车辆记录文件夹路径
            string recordFolderDir = System.IO.Path.Combine(sysConfig.mCenterShareFolderAddress,sysConfig.mCenterUploadUnkownPlateFolderName, sysConfig.mCompanyCode, sysConfig.mPlazCode);
            if (!System.IO.Directory.Exists(recordFolderDir))
            {
                System.IO.Directory.CreateDirectory(recordFolderDir);
            }

            string remoteCarRecordFile = System.IO.Path.Combine(recordFolderDir, recordName);

            //上传车辆记录
            System.IO.File.Copy(recordPath, remoteCarRecordFile, true);

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "CarRecordUploadWorker::Worker()=>上传时间点为[{0} TO {1}]的{3}记录:{2}成功", begin.ToString("yyyy-MM-dd HH:mm:ss"), end.ToString("yyyy-MM-dd HH:mm:ss"), recordName,monlevel);

            ret = NetworkShareFileHelper.DisConnection(sysConfig.mCenterShareFolderAddress);


            //是否需要删除临时文件??
            System.IO.File.Delete(recordPath);
        }


        /// <summary>
        /// 删除N天以前记录
        /// </summary>
        /// <param name="days"></param>
        public void DeleteCarRecord(int days)
        {
            if (days <= 0)
                return;

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "NewPlatteUploadWorker::DeleteCarRecord()=>获取SysConfig实例失败");
                return;
            }

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "NewPlatteUploadWorker::DeleteCarRecord()=>获取SqliteHandler实例失败");
                return;
            }

            //得到days天以前的日期
            DateTime endTime = DateTime.Now.Subtract(new TimeSpan(days, 0, 0, 0));

            //生成删除语句
            string sql = string.Format("delete from NewPlatte where dtime<datetime('{0}')", endTime.ToString("yyyy-MM-dd HH:mm:ss"));

            string error;
            if (sqliteHandler.ExecuteNonQuery(sql, out error) == -1)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "NewPlatteUploadWorker::DeleteCarRecord()=>执行删除时间在{0}之前的数据的SQL语句失败:{1}", endTime.ToString("yyyy-MM-dd HH:mm:ss"), error);
            }
            else 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "NewPlatteUploadWorker::DeleteCarRecord()=>执行删除时间在{0}之前的数据的SQL语句成功", endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }


        }

        /// <summary>
        /// 导出车记录到指定的文件中
        /// </summary>
        /// <param name="newPlatteRecords">新车辆记录</param>
        /// <param name="newPlateRecordPath">导出文件包含文件名称</param>
        /// <param name="error">错误消息</param>
        /// <returns></returns>
        public static bool ExportCarRecordToFile(List<ASSISTICE.CarRecord> carRecords,string carRecordPath,out string error)
        {
            error = string.Empty;
            bool ret = false;
            //没有记录则跳出分页
            if (carRecords.Count == 0)
            {
                error = "不存在需要导出的记录";
                return ret;
            }

            string strObj = null;
            //导出查询到的记录到文本中
            try
            {

                if (!System.IO.File.Exists(carRecordPath)) //新文件在文件开始位置添加列表头
                {
                    string columnHeader = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", "number","color","cardId", "companycode", "plazcode", "lanname", "lannum", "dtime");
                    CustomRecordUploadWorker.BuildColumnHeader(columnHeader, carRecordPath);
                }

                using (System.IO.FileStream fs = new System.IO.FileStream(carRecordPath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.Default))
                    {
                        foreach (ASSISTICE.CarRecord record in carRecords)
                        {
                            strObj = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", record.number,record.color,record.flag,
                                record.companycode, record.plazcode, record.lanname, record.lannum, record.dtime);
                            sw.WriteLine(strObj);
                        }
                    }
                }

                ret = true;
            }
            catch (Exception ex)
            {
                error=ex.Message;
            }

            return ret;
        }

    }


    /// <summary>
    /// 车辆记录上传线程
    /// 调用方式:Start()=>Stop()
    /// </summary>
    public class CarRecordUploadThread
    {
        public CarRecordUploadThread()
        {

        }

        private bool mIsStop = true;

        private void ThreadWorker(Object stat)
        {

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CarRecordUploadThread::ThreadWorker()=>获取SysConfig实例失败,车辆记录上传线程停止工作");
                return;
            }

            if (!sysConfig.mAutoUploadCustomRecord)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CarRecordUploadThread::ThreadWorker()=>未开启自动上传新车牌功能,车辆记录上传线程停止工作");
                return;
            }

            CarRecordUploadWorker worker = new CarRecordUploadWorker();

            DateTime beginTime = DateTime.Now, endTime = DateTime.Now;
            bool isUpload = false;//是否需要上传
            Dictionary<string, int> monLevels = IDbHandler.MySqlHandler.GetMonLevel();
            while (!this.mIsStop)
            {
                if (DateTime.Now.Hour == sysConfig.mUploadCustomRecordTime) //自动上传时间点匹配
                {
                    isUpload = CustomRecordUploadThread.MakeTimeSpanByFrequency(sysConfig.mUploadCarRecordFrequency, out beginTime, out endTime);
                    if (isUpload) //需要上传
                    {
                        foreach (var item in monLevels)
                        {
                            try
                            {
                                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "CarRecordUploadThread::ThreadWorker()=>执行{2}记录上传工作,时间段为:[{0} TO {1}]",
                                     beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"),item.Key);
                                worker.Worker(beginTime, endTime,item.Key);
                            }
                            catch (Exception ex)
                            {
                                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "CarRecordUploadThread::ThreadWorker()=>调用上传方法发生异常:{0}", ex.Message);
                            }
                        }
                       
                    }
                    //删除N天前的数据
                    worker.DeleteCarRecord(sysConfig.mCarRecordSaveDays);

                    for (int i = 0; i < 3600&&(!this.mIsStop); i++)
                    {
                        System.Threading.Thread.Sleep(1000);//跳过当前时间点3600*1000
                    }
                    
                }

                for (int i = 0; i < 300&&(!this.mIsStop); i++)
                {
                    System.Threading.Thread.Sleep(1000);//每5分钟检查一次
                }
               
            }
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
                thd.Name = "NewPlatteRecordUploadThread";
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

    /// <summary>
    /// 备注：不能通过此方式下载远端数据库数据到本地库(20171216 PM)
    /// 车辆表下载线程
    /// [该线程类用于从MSSQL库cartable表的数据下载到本地SQLite库cartable表中]
    /// </summary>
    //public class DownloadCarTableThread 
    //{

    //    public DownloadCarTableThread()
    //    {
                
    //    }

    //    private bool mIsStop = true;

    //    private void ThreadWorker(Object stat)
    //    {

    //        //获取系统配置实例
    //        SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
    //        if (sysConfig == null)
    //        {
    //            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "DownloadCarTableThread::ThreadWorker()=>获取SysConfig实例失败,车辆表下载线程停止工作");
    //            return;
    //        }

    //        LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "DownloadCarTableThread::ThreadWorker()=>车辆表下载线程开始工作");
    //        string sqliteQuery = "select max(DTime) from cartable";//查询本地最大的时间字段
    //        Object objDTime=null;
    //        const string defaultTime=("2015-12-01 00:00:00");
    //        string strDTime;
    //        string error;
    //        string strSql;
    //        DataTable carTables = null;
    //        string strOperate;
    //        while (!this.mIsStop)
    //        {
    //            for (int i = 0; i < 300 && (!this.mIsStop); i++)
    //            {
    //                System.Threading.Thread.Sleep(1000);//每5分钟检查一次
    //            }
    //            if (this.mIsStop)
    //                break;

    //            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
    //            if (sqliteHandler == null)
    //            {
    //                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "DownloadCarTableThread::ThreadWorker()=>获取查询组件sqliteHandler失败");
    //                continue;
    //            }

    //            if (!sqliteHandler.ExecuteScalar(sqliteQuery, out objDTime, out error)) 
    //            {
    //                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "DownloadCarTableThread::ThreadWorker()=>调用ExecuteScalar接口失败:{0}",error);
    //                continue;
    //            }
    //            //时间赋值
    //            if (objDTime != null && objDTime.ToString().Length > 10)
    //            {
    //                strDTime = objDTime.ToString();
    //            }
    //            else 
    //            {
    //                strDTime = defaultTime;
    //            }
    //            strDTime = strDTime.Replace('/', '-');//如果时间格式为2015/12/01 00:00:00则替换为2015-12-01 00:00:00

    //            //取出远程数据库中大于本地最后更新记录时间，并且不是初始数据（标志为A）的所有记录
    //            strSql = string.Format("select * from cartable where DTime>'{0}' and operate<>'A' order by DTime asc",strDTime);

    //            //向MySQL发起查询
    //            MySqlHandler MySqlHandler = SysComponents.GetComponent("MySqlHandler") as MySqlHandler;
    //            if (MySqlHandler == null)
    //            {
    //                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "DownloadCarTableThread::ThreadWorker()=>获取查询组件MySqlHandler失败");
    //                continue;
    //            }
    //            carTables = MySqlHandler.SearcherCarTableOfTable(strSql, out error);
    //            if (carTables == null)//发生异常
    //            {
    //                error = string.Format("执行MySqlHandler.SearcherCarTableOfTable()失败,SQL：{0} \r\n错误信息：{1}", strSql, error);
    //                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "DownloadCarTableThread::ThreadWorker()=>{0}", error);
                   
    //            }
    //            else
    //            {
    //                if (carTables.Rows == null)
    //                    continue;
                    
    //                foreach (DataRow row in carTables.Rows)
    //                {
    //                    strOperate = row[10].ToString();
    //                    CarTable carTable = MySqlHandler.ConvertDataRowToCarTable(row);
    //                    if (carTable == null)
    //                    {
    //                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "DownloadCarTableThread::ThreadWorker()=>调用MySqlHandler.ConvertDataRowToCarTable接口发生异常!!!");
    //                        continue;
    //                    }
    //                    //插入或更新特殊车辆数据
    //                    if (strOperate=="i")
    //                    {
    //                        //strSql.Format(_T("insert into cartable values('%s','%s',%s,'%s','%s','%s',%s,%s,'%s','%s')"),\
    //                        //    strPlatte,strCarClass,strNum,strRemark,strUnit,strMaster,strMonLevel,strSMS,strComment,strDTime);

    //                       //进行后台数据插入
    //                       PlatteWoker.PostCarTablesToInsertThread(carTable);

    //                    }
    //                    else
    //                    {
    //                        strSql=string.Format("update cartable set carclass='{0}',num={1},remark='{2}',unit='{3}',master='{4}',monlevel={5},smsreport={6},comment='{7}',DTime='{8}' where platte='{9}'",
    //                            carTable.carclass,carTable.num,carTable.remark,carTable.unit,carTable.master,carTable.monlevel,carTable.smsreport,carTable.comment,carTable.dtime,carTable.platte);

    //                        //进行后台数据更新
    //                        if (sqliteHandler.ExecuteNonQuery(strSql, out error) < 0) 
    //                        {
    //                            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "DownloadCarTableThread::ThreadWorker()=>调用ExecuteNonQuery接口发生异常:{0}", error);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }


    //    /// <summary>
    //    /// 开始线程工作
    //    /// </summary>
    //    public void Start()
    //    {
    //        if (this.mIsStop)
    //        {
    //            this.mIsStop = false;
    //            System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.ThreadWorker));
    //            thd.Name = "DownloadCarTableThread";
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

    //    //private void PostCarTablesToInsertThread(CarTable table)
    //    //{
    //    //    InsertCarInfoWorker worker = SysComponents.GetComponent("InsertCarInfoWorker") as InsertCarInfoWorker;
    //    //    if (worker == null)
    //    //    {
    //    //        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToInsertThread()=>获取组件InsertCarInfoWorker失败");
    //    //        return;
    //    //    }

    //    //    int id = worker.AllocCarObjectID();
    //    //    if (id != 0)
    //    //    {
    //    //        if (!worker.CopyCarObjectToBuffer(table, id))
    //    //        {
    //    //            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToInsertThread()=>调用CopyCarObjectToBuffer()失败");
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "PlatteWoker::PostCarTablesToInsertThread()=>调用AllocCarObjectID()失败，没有可用的资源");
    //    //    }

    //    //}
    //}
}
