using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASSISTICE;
using System.Data.SQLite;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using TollAssistComm;
using MySql.Data.MySqlClient;

namespace IDbHandler
{
    /// <summary>
    /// SQLite相关操作
    /// </summary>
    public sealed class SqliteHandler
    {

        //实现单例
        private static SqliteHandler gSQLiteDataHandler = null;
        private static object instanceObjLock = new object();

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <param name="storePath"></param>
        /// <returns></returns>
        public static SqliteHandler Instance(string storePath)
        {
            if (gSQLiteDataHandler == null)
            {
                lock (instanceObjLock)
                {
                    if (gSQLiteDataHandler == null)
                    {
                        gSQLiteDataHandler = new SqliteHandler(storePath);
                    }
                }

            }
            return gSQLiteDataHandler;
        }

       

        #region 私有函数

        private SqliteHandler(string storePath)
        {
            string error;
            if (!Init(storePath, out error))
            {
                throw new Exception(error);
            }
        }

        private SQLiteConnection conn = null;
        private bool Init(string _dbName, out string error)
        {
            error = string.Empty;
            if (conn != null) return false;
            try
            {
                conn = new SQLiteConnection("Data Source=" + _dbName);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                error = (ex.Message);

            }

            return false;
        }

        private CarTable ConvertDataRowToCarTable(DataRow item)
        {
            try
            {
                CarTable info = new CarTable();
                info.id = item["id"].ToString();//车辆信息id
                info.color = item["color"].ToString();//车牌颜色
                info.number = item["number"].ToString();//车牌号
                info.brand = item["brand"].ToString();//车辆品牌
                info.type = item["type"].ToString();//车辆类型
                info.axleNum = int.Parse(item["axle_num"].ToString());//轴数
                info.maxPassenger = int.Parse(item["max_passenger"].ToString());//核定载客
                info.maxLoad = double.Parse(item["max_load "].ToString());//核定载重量kg
                info.weight = double.Parse(item["weight"].ToString());//整备质量kg
                info.creator = item["creator"].ToString();// 创建人
                info.company = item["company"].ToString();//创建单位
                info.monLevel = int.Parse(item["mon_level"].ToString());//监视级别
                info.esType = item["es_type"].ToString();//逃费类型(来自于es_car_info表)
                info.esRemark = item["es_remark"].ToString();//备注(来自于es_car_info表)
                info.dtime = item["input_time"].ToString();//创建时间
                return info;
            }
            catch (Exception ex)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR,ex.Message);
            }
            return null;
        }

        private CustomRecord ConvertDataRowToCustomRecord(DataRow item)
        {
            try
            {
                CustomRecord info = new CustomRecord();
                info.id = item["id"].ToString();//车辆信息id
                info.number = item["number"].ToString();//车牌号
                info.color = item["color"].ToString();//车牌颜色
                info.brand = item["brand"].ToString();//车辆品牌
                info.type = item["type"].ToString();//车辆类型
                info.flag = item["flag"].ToString();//串口传递过来的除车牌号之外的字段信息
                info.monLevel = int.Parse(item["mon_level"].ToString());   //监视级别          
                info.dtime = item["dtime"].ToString();//消费时间
                info.@operator = item["operator"].ToString();//操作员
                info.customamount = double.Parse(item["customamount"].ToString());//消费总计

                //站点信息
                info.node = new TollNode();
                info.node.companycode = item["companycode"].ToString();
                info.node.plazcode = item["plazcode"].ToString();
                info.node.lanname = item["lanname"].ToString();
                info.node.lannum = int.Parse(item["lannum"].ToString());

                return info;
            }
            catch (Exception ex)
            {
                LogerPrintWrapper.Print( LOGCS.LogLevel.ERROR,ex.Message);
            }
            return null;
        }


        private AlarmInfo ConvertDataRowToAlarmInfo(DataRow item)
        {
            try
            {
                AlarmInfo info = new AlarmInfo();
                info.Id =long.Parse(item["id"].ToString());
                info.Color = item["color"].ToString();
                info.Content = item["content"].ToString();
                info.CreateTime = item["createTime"].ToString();
                info.CreateOrg = item["createOrg"].ToString();
                info.Creator = item["creator"].ToString();
                info.EndTime = item["endTime"].ToString();
                info.Number = item["number"].ToString();
                info.PubOrg = item["pubOrg"].ToString();
                info.RevoCation = item["revocation"].ToString();
                info.RevokeOrg = item["revokeOrg"].ToString();
                info.RevokeTime = item["revokeTime"].ToString();
                info.StartTime = item["startTime"].ToString();
                info.Status = item["status"].ToString();
                info.Revoke = int.Parse(item["revoke"].ToString());

                return info;
            }
            catch (Exception ex)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, ex.Message);
            }
            return null;
        }

        private Station ConvertDataRowToStation(DataRow item)
        {
            try
            {
                Station info = new Station();
                info.bm = item["bm"].ToString();
                info.lb = item["lb"].ToString();
                info.mc = item["mc"].ToString();
                info.lgs = item["lgs"].ToString();

                return info;
            }
            catch (Exception ex)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, ex.Message);
            }
            return null;
        }


        private CarRecord ConvertDataRowToCarRecord(DataRow item)
        {
            try
            {
                CarRecord info = new CarRecord();
                info.number = item["number"].ToString();//车牌号
                info.color = item["color"].ToString();//车辆颜色
                info.monLevel = int.Parse(item["monlevel"].ToString());//监视级别
                info.type = item["type"].ToString();//车辆类型
                info.flag = item["flag"].ToString();//串口传递过来的除车牌号之外的字段信息
                info.companycode = item["companycode"].ToString();
                info.plazcode = item["plazcode"].ToString();
                info.lanname = item["lanname"].ToString();
                info.lannum =int.Parse(item["lannum"].ToString());
                info.dtime = item["dtime"].ToString();

                return info;
            }
            catch (Exception ex)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, ex.Message);
            }
            return null;
        }


        #endregion


        #region 公开数据库调用接口

        /// <summary>
        /// 释放数据连接
        /// </summary>
        /// <returns></returns>
        public bool Dispose()
        {
            if (conn == null) return false;
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Dispose();
            return true;
        }


        private object LockObj = new object();

        /// <summary>
        /// 添加车辆信息
        /// </summary>
        /// <param name="modes">CarTable对象集合</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功与否</returns>
        public bool BatchAddCarTable(List<CarTable> modes, out string error)
        {
            error = string.Empty;
            if (modes == null || modes.Count == 0)
            {
                error = "BatchAddCarTable()=>参数modes无效";
                return false;
            }


            SQLiteCommand command = new SQLiteCommand(conn);
            string insert_sql = string.Format("insert into CarTable(id,color,number,brand,type,axle_num,max_passernger,max_load,weight,creator,company,mon_level,es_type,es_remark,dtime) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)");//{0},'{1}','{2}',{3}
            command.CommandText = insert_sql;

            DbParameter idParam = command.CreateParameter();
            command.Parameters.Add(idParam);
            DbParameter colorParam = command.CreateParameter();
            command.Parameters.Add(colorParam);
            DbParameter numberParam = command.CreateParameter();
            command.Parameters.Add(numberParam);
            DbParameter brandParam = command.CreateParameter();
            command.Parameters.Add(brandParam);
            DbParameter typeParam = command.CreateParameter();
            command.Parameters.Add(typeParam);
            DbParameter axle_numParam = command.CreateParameter();
            command.Parameters.Add(axle_numParam);
            DbParameter max_passerngerParam = command.CreateParameter();
            command.Parameters.Add(max_passerngerParam);
            DbParameter max_loadParam = command.CreateParameter();
            command.Parameters.Add(max_loadParam);
            DbParameter weightParam = command.CreateParameter();
            command.Parameters.Add(weightParam);
            DbParameter creatorParam = command.CreateParameter();
            command.Parameters.Add(creatorParam);
            DbParameter companyParam = command.CreateParameter();
            command.Parameters.Add(companyParam);
            DbParameter mon_levelParam = command.CreateParameter();
            command.Parameters.Add(mon_levelParam);
            DbParameter es_typeParam = command.CreateParameter();
            command.Parameters.Add(es_typeParam);
            DbParameter es_remarkParam = command.CreateParameter();
            command.Parameters.Add(es_remarkParam);
            DbParameter dtimeParam = command.CreateParameter();
            command.Parameters.Add(dtimeParam);

            lock (LockObj)
            {

                DbTransaction dbTran = conn.BeginTransaction();
                try
                {

                    foreach (CarTable item in modes)
                    {
                        idParam.Value = item.id;
                        colorParam.Value = item.color;
                        numberParam.Value = item.number;
                        brandParam.Value = item.brand;
                        typeParam.Value = item.type;
                        axle_numParam.Value = item.axleNum;
                        max_passerngerParam.Value = item.maxPassenger;
                        max_loadParam.Value = item.maxLoad;
                        weightParam.Value = item.weight;
                        creatorParam.Value = item.creator;
                        companyParam.Value = item.company;
                        mon_levelParam.Value = item.monLevel;
                        es_typeParam.Value = item.esType;
                        es_remarkParam.Value = item.esRemark;
                        dtimeParam.Value = item.dtime;
                        command.ExecuteNonQuery();
                    }

                    dbTran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                    // throw ex;
                    dbTran.Rollback();
                }
                return false;
            }
        }

        /// <summary>
        /// 检索车辆信息
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>数据集合,发生异常返回NULL</returns>
        public List<CarTable> SearcherCarTable(string query, out string error)
        {
            error = string.Empty;
            SQLiteCommand command = new SQLiteCommand(conn);
            try
            {
                command.CommandText = query;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables == null || ds.Tables.Count == 0) return null;
                DataTable tab = ds.Tables[0];
                //转换DT到List
                List<CarTable> lst = new List<CarTable>();
                foreach (DataRow item in tab.Rows)
                {
                    CarTable info = ConvertDataRowToCarTable(item);
                    if (info != null)
                        lst.Add(info);
                }
                return lst;
            }
            catch (Exception ex)
            {
                error = (ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 添加消费记录
        /// </summary>
        /// <param name="modes">CustomRecord对象集合</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功与否</returns>
        public bool BatchAddCustomRecord(List<CustomRecord> modes, out string error)
        {
            error = string.Empty;
            if (modes == null || modes.Count == 0)
            {
                error = "BatchAddCustomRecord()=>参数modes无效";
                return false;
            }


            SQLiteCommand command = new SQLiteCommand(conn);
            string insert_sql = string.Format("insert into CustomRecord(id,number,color,brand,type,flag,mon_level,dtime,operator,customamount,companycode,plazcode,lanname,lannum) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)");//{0},'{1}','{2}',{3}
            command.CommandText = insert_sql;
            DbParameter idParam = command.CreateParameter();
            command.Parameters.Add(idParam);
            DbParameter numberParam = command.CreateParameter();
            command.Parameters.Add(numberParam);
            DbParameter colorParam = command.CreateParameter();
            command.Parameters.Add(colorParam);
            DbParameter brandParam = command.CreateParameter();
            command.Parameters.Add(brandParam);
            DbParameter typeParam = command.CreateParameter();
            command.Parameters.Add(typeParam);
            DbParameter flagParam = command.CreateParameter();
            command.Parameters.Add(flagParam);
            DbParameter mon_levelParam = command.CreateParameter();
            command.Parameters.Add(mon_levelParam);
            DbParameter dtimeParam = command.CreateParameter();
            command.Parameters.Add(dtimeParam);
            DbParameter operatorParam = command.CreateParameter();
            command.Parameters.Add(operatorParam);
            DbParameter customamountParam = command.CreateParameter();
            command.Parameters.Add(customamountParam);
            DbParameter companycodeParam = command.CreateParameter();
            command.Parameters.Add(companycodeParam);
            DbParameter plazcodeParam = command.CreateParameter();
            command.Parameters.Add(plazcodeParam);
            DbParameter lannameParam = command.CreateParameter();
            command.Parameters.Add(lannameParam);
            DbParameter lannumParam = command.CreateParameter();
            command.Parameters.Add(lannumParam);

            lock (LockObj)
            {

                DbTransaction dbTran = conn.BeginTransaction();
                try
                {

                    foreach (CustomRecord item in modes)
                    {
                        idParam.Value = item.id;
                        numberParam.Value = item.number;
                        colorParam.Value = item.color;
                        brandParam.Value = item.brand;
                        typeParam.Value = item.type;
                        flagParam.Value = item.flag;
                        mon_levelParam.Value = item.monLevel;
                        dtimeParam.Value = item.dtime;
                        operatorParam.Value = item.@operator;
                        customamountParam.Value = item.customamount;
                        companycodeParam.Value = item.node.companycode;
                        plazcodeParam.Value = item.node.plazcode;
                        lannameParam.Value = item.node.lanname;
                        lannumParam.Value = item.node.lannum;

                        command.ExecuteNonQuery();
                    }

                    dbTran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                    // throw ex;
                    dbTran.Rollback();
                }
                return false;
            }
        }

        /// <summary>
        /// 添加或者更新收费节点记录
        /// </summary>
        /// <param name="flag">唯一标识</param>
        /// <param name="node">节点信息</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功与否</returns>
        public bool AddTollNodeRecord(string flag,TollNode node, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(flag)||node == null)
            {
                error = "AddTollNodeRecord()=>参数无效";
                return false;
            }


            SQLiteCommand command = new SQLiteCommand(conn);
            string insert_sql = string.Format("insert into TollNode(companycode,plazcode,lanname,lannum,dtime,id) VALUES(?,?,?,?,?,?)");//{0},'{1}','{2}',{3}
            string update_sql = string.Format("update TollNode set companycode=?,plazcode=?,lanname=?,lannum=?,dtime=? where id=?");//{0},'{1}','{2}',{3}

            command.CommandText = insert_sql;
            int rvNum = this.Collect(string.Format("select count(*) from TollNode where id='{0}'",flag), out error);
            if (rvNum > 0) //存在记录则执行更新操作
            {
                command.CommandText = update_sql;
            }
   
            DbParameter companycodeParam = command.CreateParameter();
            command.Parameters.Add(companycodeParam);
            DbParameter plazcodeParam = command.CreateParameter();
            command.Parameters.Add(plazcodeParam);
            DbParameter lannameParam = command.CreateParameter();
            command.Parameters.Add(lannameParam);
            DbParameter lannumParam = command.CreateParameter();
            command.Parameters.Add(lannumParam);
            DbParameter dtimeParam = command.CreateParameter();
            command.Parameters.Add(dtimeParam);
            DbParameter idParam = command.CreateParameter();
            command.Parameters.Add(idParam);

            lock (LockObj)
            {

                DbTransaction dbTran = conn.BeginTransaction();
                try
                {

                    companycodeParam.Value = node.companycode;
                    plazcodeParam.Value = node.plazcode;
                    lannameParam.Value = node.lanname;
                    lannumParam.Value = node.lannum;
                    dtimeParam.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    idParam.Value = flag;

                    command.ExecuteNonQuery();

                    dbTran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                    // throw ex;
                    dbTran.Rollback();
                }
                return false;
            }
        }


        /// <summary>
        /// 添加或者更新告警信息
        /// 备注：如果Revoke字段非0则表示更新告警信息为撤销状态
        /// </summary>
        /// <param name="info">告警信息</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功与否</returns>
        public bool AddAlarmInfoRecord(AlarmInfo info, out string error) 
        {
            error=null;
            if (info == null) 
            {
                error = "AddAlarmInfoRecord()=>无效的输入参数";
                return false;
            }
            SQLiteCommand command = new SQLiteCommand(conn);
            string insert_sql = string.Format("insert into AlarmInfo(color,content,createOrg,createTime,creator,endTime,number,pubOrg,revocation,revokeOrg,revokeTime,startTime,status,id,revoke) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,0)");//{0},'{1}','{2}',{3}
            string update_sql = string.Format("update AlarmInfo set color=?,content=?,createOrg=?,createTime=?,creator=?,endTime=?,number=?,pubOrg=?,revocation=?,revokeOrg=?,revokeTime=?,startTime=?,status=?,revoke=1 where id=?");//{0},'{1}','{2}',{3}

            command.CommandText = insert_sql;
            if (info.Revoke != 0) //存在记录则执行更新操作
            {
                command.CommandText = update_sql;
            }

            DbParameter colorParam = command.CreateParameter();
            command.Parameters.Add(colorParam);
            DbParameter contentParam = command.CreateParameter();
            command.Parameters.Add(contentParam);
            DbParameter createOrgParam = command.CreateParameter();
            command.Parameters.Add(createOrgParam);
            DbParameter createTimeParam = command.CreateParameter();
            command.Parameters.Add(createTimeParam);
            DbParameter creatorParam = command.CreateParameter();
            command.Parameters.Add(creatorParam);
            DbParameter endTimeParam = command.CreateParameter();
            command.Parameters.Add(endTimeParam);
            DbParameter numberParam = command.CreateParameter();
            command.Parameters.Add(numberParam);
            DbParameter pubOrgParam = command.CreateParameter();
            command.Parameters.Add(pubOrgParam);
            DbParameter revocationParam = command.CreateParameter();
            command.Parameters.Add(revocationParam);
            DbParameter revokeOrgParam = command.CreateParameter();
            command.Parameters.Add(revokeOrgParam);
            DbParameter revokeTimeParam = command.CreateParameter();
            command.Parameters.Add(revokeTimeParam);
            DbParameter startTimeParam = command.CreateParameter();
            command.Parameters.Add(startTimeParam);
            DbParameter statusParam = command.CreateParameter();
            command.Parameters.Add(statusParam);
            DbParameter idParam = command.CreateParameter();
            command.Parameters.Add(idParam);

            lock (LockObj)
            {

                DbTransaction dbTran = conn.BeginTransaction();
                try
                {

                    colorParam.Value = info.Color;
                    contentParam.Value = info.Content;
                    createOrgParam.Value = info.CreateOrg;
                    createTimeParam.Value = info.CreateTime;
                    creatorParam.Value = info.Creator;
                    endTimeParam.Value = info.EndTime;
                    numberParam.Value = info.Number;
                    pubOrgParam.Value = info.PubOrg;
                    revocationParam.Value = info.RevoCation;
                    revokeOrgParam.Value = info.RevokeOrg;
                    revokeTimeParam.Value = info.RevokeTime;
                    startTimeParam.Value = info.StartTime;
                    statusParam.Value = info.Status;
                    idParam.Value = info.Id;

                    command.ExecuteNonQuery();

                    dbTran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                    // throw ex;
                    dbTran.Rollback();
                }
                return false;
            }

        }


        /// <summary>
        /// 检索告警信息
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>数据集合,发生异常返回NULL</returns>
        public List<AlarmInfo> SearcherAlarmInfo(string query, out string error) 
        {
            error = string.Empty;
            SQLiteCommand command = new SQLiteCommand(conn);
            try
            {
                command.CommandText = query;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables == null || ds.Tables.Count == 0) return null;
                DataTable tab = ds.Tables[0];
                //转换DT到List
                List<AlarmInfo> lst = new List<AlarmInfo>();
                foreach (DataRow item in tab.Rows)
                {
                    AlarmInfo info = ConvertDataRowToAlarmInfo(item);
                    if (info != null)
                        lst.Add(info);
                }
                return lst;
            }
            catch (Exception ex)
            {
                error = (ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 检索消费记录
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>数据集合,发生异常返回NULL</returns>
        public List<CustomRecord> SearcherCustomRecord(string query, out string error)
        {
            error = string.Empty;
            SQLiteCommand command = new SQLiteCommand(conn);
            try
            {
                command.CommandText = query;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables == null || ds.Tables.Count == 0) return null;
                DataTable tab = ds.Tables[0];
                //转换DT到List
                List<CustomRecord> lst = new List<CustomRecord>();
                foreach (DataRow item in tab.Rows)
                {
                    CustomRecord info = ConvertDataRowToCustomRecord(item);
                    if (info != null)
                        lst.Add(info);
                }
                return lst;
            }
            catch (Exception ex)
            {
                error = (ex.Message);
            }

            return null;
        }


        /// <summary>
        /// 添加收费站点记录
        /// </summary>
        /// <param name="modes">Station对象集合</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功与否</returns>
        public bool BatchAddStationRecord(List<Station> modes, out string error)
        {
            error = string.Empty;
            if (modes == null || modes.Count == 0)
            {
                error = "BatchAddStationRecord()=>参数modes无效";
                return false;
            }


            SQLiteCommand command = new SQLiteCommand(conn);
            string insert_sql = string.Format("insert into Station(bm,lb,mc,lgs) VALUES(?,?,?,?)");//{0},'{1}','{2}',{3}
            command.CommandText = insert_sql;
            DbParameter bmParam = command.CreateParameter();
            command.Parameters.Add(bmParam);
            DbParameter lbParam = command.CreateParameter();
            command.Parameters.Add(lbParam);
            DbParameter mcParam = command.CreateParameter();
            command.Parameters.Add(mcParam);
            DbParameter lgsParam = command.CreateParameter();
            command.Parameters.Add(lgsParam);

            lock (LockObj)
            {

                DbTransaction dbTran = conn.BeginTransaction();
                try
                {

                    foreach (Station item in modes)
                    {
                        bmParam.Value = item.bm;
                        lbParam.Value = item.lb;
                        mcParam.Value = item.mc;
                        lgsParam.Value = item.lgs;

                        command.ExecuteNonQuery();
                    }

                    dbTran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                    // throw ex;
                    dbTran.Rollback();
                }
                return false;
            }
        }

        /// <summary>
        /// 检索收费站点信息
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>数据集合,发生异常返回NULL</returns>
        public List<Station> SearcherStation(string query, out string error)
        {
            error = string.Empty;
            SQLiteCommand command = new SQLiteCommand(conn);
            try
            {
                command.CommandText = query;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables == null || ds.Tables.Count == 0) return null;
                DataTable tab = ds.Tables[0];
                //转换DT到List
                List<Station> lst = new List<Station>();
                foreach (DataRow item in tab.Rows)
                {
                    Station info = ConvertDataRowToStation(item);
                    if (info != null)
                        lst.Add(info);
                }
                return lst;
            }
            catch (Exception ex)
            {
                error = (ex.Message);
            }

            return null;
        }


        /// <summary>
        /// 添加车记录(外地车辆?)
        /// </summary>
        /// <param name="modes">CarRecord对象集合</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功与否</returns>
        public bool BatchAddNewPlatte(List<CarRecord> modes, out string error)
        {
            error = string.Empty;
            if (modes == null || modes.Count == 0)
            {
                error = "BatchAddNewPlatte()=>参数modes无效";
                return false;
            }


            SQLiteCommand command = new SQLiteCommand(conn);
            string insert_sql = string.Format("insert into NewPlatte(number,type,flag,companycode,plazCode,lanname,lannum,dtime,monlevel,color) VALUES(?,?,?,?,?,?,?,?,?,?)");//{0},'{1}','{2}',{3}
            command.CommandText = insert_sql;
            DbParameter numberParam = command.CreateParameter();
            command.Parameters.Add(numberParam);
            DbParameter typeParam = command.CreateParameter();
            command.Parameters.Add(typeParam);
            DbParameter flagParam = command.CreateParameter();
            command.Parameters.Add(flagParam);
            DbParameter companycodeParam = command.CreateParameter();
            command.Parameters.Add(companycodeParam);
            DbParameter plazCodeParam = command.CreateParameter();
            command.Parameters.Add(plazCodeParam);
            DbParameter lannameParam = command.CreateParameter();
            command.Parameters.Add(lannameParam);
            DbParameter lannumParam = command.CreateParameter();
            command.Parameters.Add(lannumParam);
            DbParameter dtimeParam = command.CreateParameter();
            command.Parameters.Add(dtimeParam);

            DbParameter monlevelParam = command.CreateParameter();
            command.Parameters.Add(monlevelParam);
            DbParameter colorParam = command.CreateParameter();
            command.Parameters.Add(colorParam);

            lock (LockObj)
            {

                DbTransaction dbTran = conn.BeginTransaction();
                try
                {

                    foreach (CarRecord item in modes)
                    {
                        numberParam.Value = item.number;
                        typeParam.Value = item.type;
                        flagParam.Value = item.flag;
                        companycodeParam.Value = item.companycode;
                        plazCodeParam.Value = item.plazcode;
                        lannameParam.Value = item.lanname;
                        lannumParam.Value = item.lannum;
                        dtimeParam.Value = item.dtime;
                        monlevelParam.Value = item.monLevel;
                        colorParam.Value = item.color;


                        command.ExecuteNonQuery();
                    }

                    dbTran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                    // throw ex;
                    dbTran.Rollback();
                }
                return false;
            }
        }


        /// <summary>
        /// 检索车记录
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>数据集合,发生异常返回NULL</returns>
        public List<CarRecord> SearcherCarRecord(string query, out string error)
        {
            error = string.Empty;
            SQLiteCommand command = new SQLiteCommand(conn);
            try
            {
                command.CommandText = query;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables == null || ds.Tables.Count == 0) return null;
                DataTable tab = ds.Tables[0];
                //转换DT到List
                List<CarRecord> lst = new List<CarRecord>();
                foreach (DataRow item in tab.Rows)
                {
                    CarRecord info = ConvertDataRowToCarRecord(item);
                    if (info != null)
                        lst.Add(info);
                }
                return lst;
            }
            catch (Exception ex)
            {
                error = (ex.Message);
            }

            return null;
        }



        /// <summary>
        /// 执行语句，返回受影响的行数（增、删、改）
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        /// <param name="error">错误消息</param>
        /// <returns>受影响的行数,发生异常返回-1</returns>
        public int ExecuteNonQuery(string sql, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrEmpty(sql))
            {
                error = "ExecuteNonQuery()=>参数ExecuteNonQuery无效";
                return -1;
            }


            SQLiteCommand command = new SQLiteCommand(conn);

            command.CommandText = sql;

            try
            {

                lock (LockObj)
                {

                    return command.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                error = (ex.Message);
                return -1;
            }

        }

        /// <summary>
        /// 汇总查询结果，该接口用于查询记录数汇总
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>检索到的总记录数,发生异常返回-1</returns>
        public int Collect(string query, out string error)
        {
            error = string.Empty;
            SQLiteCommand command = new SQLiteCommand(conn);
            try
            {
                command.CommandText = query;
                SQLiteDataReader reader = command.ExecuteReader();

                if (reader == null || !reader.Read()) return 0;

                return reader.GetInt32(0);

            }
            catch (Exception ex)
            {
                error = (ex.Message);
            }

            return -1;
        }


       /// <summary>
       /// 执行查询返回首行首列的值
       /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
       /// <param name="result">返回结果</param>
       /// <param name="error">错误消息</param>
       /// <returns>失败返回false</returns>
        public bool ExecuteScalar(string query,out Object result, out string error)
        {
            bool ret = true;
            error = string.Empty;
            result = null;
            SQLiteCommand command = new SQLiteCommand(conn);
            try
            {
                command.CommandText = query;
                result = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                error = (ex.Message);
                ret = false;
            }

            return ret;
        }

        #endregion
    }

    /// <summary>
    /// MySQL相关操作
    /// </summary>
    public sealed class MySqlHandler
    {
        //实现单例
        private static MySqlHandler gSQDataHandler = null;
        private static object instanceObjLock = new object();

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="dbName"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static MySqlHandler Instance(string ip, string dbName, string user, string password)
        {
            if (gSQDataHandler == null)
            {
                lock (instanceObjLock)
                {
                    if (gSQDataHandler == null)
                    {
                        gSQDataHandler = new MySqlHandler(ip, dbName, user, password);
                    }
                }

            }
            return gSQDataHandler;
        }

        #region 公开数据库调用接口

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool TestConnection(out string error)
        {
            error = string.Empty;
            using (MySqlConnection connection = new MySqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    return false;
                }
            }

        }

        /// <summary>
        /// 重置连接信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="_dbName"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ResetConnection(string ip, string dbName, string user, string password)
        {
            return this.Init(ip, dbName, user, password);
        }

        /// <summary>
        /// 检索车辆信息
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>数据集合,发生异常返回NULL</returns>
        public List<CarTable> SearcherCarTable(string query, out string error)
        {
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(builder.ConnectionString))
            {
                try
                {
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables == null || ds.Tables.Count == 0) return null;
                    DataTable tab = ds.Tables[0];
                    //转换DT到List
                    List<CarTable> lst = new List<CarTable>();
                    foreach (DataRow item in tab.Rows)
                    {
                        CarTable info = ConvertDataRowToCarTable(item);
                        if (info != null)
                            lst.Add(info);
                    }
                    return lst;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                }
                return null;
            }


        }


        /// <summary>
        /// 检索逃费车辆信息
        /// 2018-01-03 add
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>数据集合,发生异常返回NULL</returns>
        public List<CarTable> SearcherESCarTable(string query, out string error)
        {
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(builder.ConnectionString))
            {
                try
                {
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables == null || ds.Tables.Count == 0) return null;
                    DataTable tab = ds.Tables[0];
                    Dictionary<string, CarTable> records = new Dictionary<string, CarTable>();
                    //转换DT到List
                    List<CarTable> lst = new List<CarTable>();

                    //逃费类型统计
                    foreach (DataRow item in tab.Rows)
                    {
                        string key = string.Format("{0}_{1}",item["number"].ToString(),item["color"].ToString());
                        CarTable info = null;
                        if (!records.ContainsKey(key))
                        {
                            info = ConvertDataRowToCarTable(item);
                            records[key] = info;
                        }
                        else 
                        {
                            info = records[key];
                        }
                        info.esRemark += string.Format("{0}:{1}:{2}次;\r\n",item["dtime"],item["es_type"].ToString(), item["cnt"].ToString());
                        //if (info != null)
                        //    lst.Add(info);
                    }

                    foreach (var item in records)
                    {
                        lst.Add(item.Value);
                    }

                    return lst;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                }
                return null;
            }


        }

        /// <summary>
        /// 检索车辆信息
        /// </summary>
        /// <param name="query">检索条件，根据不同引擎而定</param>
        /// <param name="error">错误消息</param>
        /// <returns>数据集合,发生异常返回NULL</returns>
        public DataTable SearcherCarTableOfTable(string query, out string error)
        {
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(builder.ConnectionString))
            {
                try
                {
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables == null || ds.Tables.Count == 0) return null;
                    DataTable tab = ds.Tables[0];
                    return tab;
                }
                catch (Exception ex)
                {
                    error = (ex.Message);
                }
                return null;
            }


        }

        #endregion

        #region 私有函数

        private MySqlHandler(string ip, string dbName, string user, string password)
        {
            if (!Init(ip, dbName, user, password))
            {
                throw new Exception("MySqlHandler()=>构造函数初始化异常");
            }
        }

        private DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
        private bool Init(string ip, string _dbName, string user, string password)
        {
            builder.Clear();
            builder.Add("Server", ip);
            //builder.Add("Server", ip);
            builder.Add("Database", _dbName);
            builder.Add("User ID", user);
            builder.Add("Password", password);
            builder.Add("CharSet", "utf8");
            builder.Add("pooling", "true");
           // builder.Add("integrated security", true);

            return true;
        }


        public CarTable ConvertDataRowToCarTable(DataRow item)
        {
            try
            {
                CarTable info = new CarTable();
                info.id = item["id"].ToString();//车辆信息id
                info.color = item["color"].ToString();//车牌颜色
                info.number = item["number"].ToString();//车牌号
                info.brand = item["brand"].ToString();//车辆品牌
                info.type = item["type"].ToString();//车辆类型
                info.axleNum = int.Parse(item["axle_num"].ToString());//轴数
                info.maxPassenger = int.Parse(item["max_passenger"].ToString());//核定载客数
                info.maxLoad = double.Parse(item["max_load"].ToString());//核定载重量KG
                info.weight = double.Parse(item["weight"].ToString());//整备质量KG
                info.creator = item["creator"].ToString();//创建人      
                info.company = item["company"].ToString();//创建单位

                info.monLevel = ConvertMonLevel(item["mon_level"].ToString());//监视级别
                info.esType = item["es_type"].ToString();//逃费类型(来自于es_car_info表)
                info.esRemark = item["remark"].ToString();//备注(来自于es_car_info表)
                info.dtime = item["input_time"].ToString();//创建时间
                                                 
                return info;
            }
            catch (Exception ex)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, ex.Message);
            }
            return null;
        }

        private static Dictionary<string, int> mMonLevel = new Dictionary<string, int>();
        static MySqlHandler ()
	    {

            mMonLevel["一般车辆"] = 0;
            mMonLevel["涉嫌"] = 1;
            mMonLevel["保障"] = 2;
            mMonLevel["无效"] = 3;
            mMonLevel["未知"] = -1;
	    }
        /// <summary>
        /// 获取车辆监视级别
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetMonLevel() 
        {
            return mMonLevel;
        }

        private int ConvertMonLevel(string mon_level) 
        {
            int rv = -1;//未知
            if (mMonLevel.ContainsKey(mon_level))
                return mMonLevel[mon_level];
            //switch (mon_level) 
            //{
            //    case "一般车辆":
            //        rv = 0;
            //        break;
            //    case "涉嫌":
            //        rv = 1;
            //        break;
            //    case "保障":
            //        rv = 2;
            //        break;
            //    case "无效":
            //        rv = 3;
            //        break;
            //}
            return rv;
        }

        #endregion

      
    }
}
