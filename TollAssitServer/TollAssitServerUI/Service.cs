using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateModule;
using IDbHandler;
using System.Net.Sockets;
using HeartBeat;
using System.Runtime.InteropServices;
using System.Diagnostics;
using TollAssistComm;
using Ice_Servant_Factory;

namespace CommHandler
{
    public static class Service
    {

        private static bool isDbInstance = false;//判断数据库是否已经实例化完成

        public static bool StartService(LOGCS.CallBackLoger callback, ASSISTUPDATEMODULEICE.UpdateType type, IUpdateSotfwareNote note)
        {
            bool rs = false;

            //初始化系统日志服务并注册到系统组件中
            LOGCS.CLoger cLoger=LOGCS.CLoger.GetInstance(callback);
            if (cLoger == null)
                return false;
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
                System.Threading.Thread.Sleep(1000);//等待数据库实例初始化完成
            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "数据库实例化:{0}",isDbInstance?"完成":"未完成(已超时)");
            

            //访问中心文件系统实例初始化
            if (!string.IsNullOrWhiteSpace(sysConfig.mCenterShareFolderAddress) && (!string.IsNullOrWhiteSpace(sysConfig.mCenterClientUpdateFolderName)))
            {
                string clientUpdateFolderAddr = System.IO.Path.Combine(sysConfig.mCenterShareFolderAddress, sysConfig.mCenterClientUpdateFolderName);
                ShareFileUpdateHandler client_ShareFileUpdateHandler = new ShareFileUpdateHandler(clientUpdateFolderAddr, sysConfig.mCenterShareFolderUser, sysConfig.mCenterShareFolderPassword);
                //注册到系统组件中
                SysComponents.RegComponent("ShareFileUpdateHandler", client_ShareFileUpdateHandler);

                //开始预热链路
                client_ShareFileUpdateHandler.PreheatShare();

            }

            if (!string.IsNullOrWhiteSpace(sysConfig.mCenterShareFolderAddress) && (!string.IsNullOrWhiteSpace(sysConfig.mCenterServerUpdateFolderName)))
            {
                string serverUpdateFolderAddr = System.IO.Path.Combine(sysConfig.mCenterShareFolderAddress, sysConfig.mCenterServerUpdateFolderName);
                ShareFileUpdateHandler server_ShareFileUpdateHandler = new ShareFileUpdateHandler(serverUpdateFolderAddr, sysConfig.mCenterShareFolderUser, sysConfig.mCenterShareFolderPassword);
                //注册到系统组件中
                SysComponents.RegComponent("ShareFileUpdateHandler_Server", server_ShareFileUpdateHandler);

                //开始预热链路
                server_ShareFileUpdateHandler.PreheatShare();
            }

            //本地不再缓存车辆信息 20171216 PM
            ////实例化车辆信息下载线程
            //DownloadCarTableThread downloadCarTableThread = new DownloadCarTableThread();
            //downloadCarTableThread.Start();
            ////注册到系统组件中
            //SysComponents.RegComponent("DownloadCarTableThread", downloadCarTableThread);

            //实例化查询服务实体对象
            int queryObjsNums = sysConfig.mMaxQueryEntityObjCount;//缓存多少个查询实体对象
            ICarQueryImpl iCarQueryImpl = new ICarQueryImpl(queryObjsNums);
            //注册到系统组件中
            SysComponents.RegComponent("ICarQueryImpl", iCarQueryImpl);

            //本地不再缓存车辆信息 20171216 PM
            ////实例化车辆信息插入线程
            //int carObjNums = sysConfig.mMaxCacalCarEntityObjCount;//缓存多少个车辆信息对象
            //InsertCarInfoWorker insertCarInfoWorker = new InsertCarInfoWorker(carObjNums);
            ////注册到系统组件中
            //SysComponents.RegComponent("InsertCarInfoWorker", insertCarInfoWorker);
            ////启动线程
            //insertCarInfoWorker.Start();

            //实例化车辆(车辆)信息插入线程
            int newCarObjNums = sysConfig.mMaxUnkownCarEntityObjCount;//缓存多少个新车辆信息对象
            InsertCarRecordWorker insertCarReccordWorker = new InsertCarRecordWorker(newCarObjNums);
            //注册到系统组件中
            SysComponents.RegComponent("InsertCarRecordWorker", insertCarReccordWorker);
            //启动线程
            insertCarReccordWorker.Start();

            //2017-12-17 PM 消费记录功能取消
            ////实例化消费记录插入线程
            //InsertCustomRecordWorker insertCustomRecordWorker = new InsertCustomRecordWorker();
            ////注册到系统组件中
            //SysComponents.RegComponent("InsertCustomRecordWorker", insertCustomRecordWorker);
            ////启动线程
            //insertCustomRecordWorker.Start();


            //初始化查询线程池
            int thdNums = sysConfig.mQueryThreadWorkerPoolSize;//线程个数
            QueryThreadWorkerPool queryThreadPool = new QueryThreadWorkerPool(thdNums, iCarQueryImpl);
            //注册到系统组件中
            SysComponents.RegComponent("QueryThreadWorkerPool", queryThreadPool);
            //启动线程池
            queryThreadPool.Start();


            //实例化升级服务实体对象
            int updateCallProcessEntityNums = sysConfig.mMaxUpdateCallEntityObjCount;//缓存多少个实体对象
            IUpdateImpl updateImpl = new IUpdateImpl(updateCallProcessEntityNums);
            //注册到系统组件中
            SysComponents.RegComponent("IUpdateImpl", updateImpl);

            //初始化升级工作线程池
            int updateThdNums = sysConfig.mUpdateCallProcessWorkerPoolSize;//线程个数
            IUpdateCallProcessWorkerPool updateThreadPool = new IUpdateCallProcessWorkerPool(updateThdNums, updateImpl);
            //注册到系统组件中
            SysComponents.RegComponent("IUpdateCallProcessWorkerPool", updateThreadPool);
            //启动线程池
            updateThreadPool.Start();


            //启动ICE查询服务
            System.Threading.Thread queryServerStartThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(StartICEQueryServerThreadFunc));
            queryServerStartThread.IsBackground = true;
            queryServerStartThread.Name = "QueryServerStartThread";
            queryServerStartThread.Start(sysConfig);


            //启动ICE升级服务
            System.Threading.Thread clientUpdateServerStartThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(StartICEUpdateServerThreadFunc));
            clientUpdateServerStartThread.IsBackground = true;
            clientUpdateServerStartThread.Name = "ClientUpdateServerStartThread";
            clientUpdateServerStartThread.Start(sysConfig);

            //2017-12-17 PM 消费记录功能取消
            ////自动上传消费记录线程
            //CustomRecordUploadThread customRecordUploadThread = new CustomRecordUploadThread();
            //customRecordUploadThread.Start();
            ////注册到系统组件中
            //SysComponents.RegComponent("CustomRecordUploadThread", customRecordUploadThread);

            //自动上传车辆记录线程
            CarRecordUploadThread carRecordUploadThread = new CarRecordUploadThread();
            carRecordUploadThread.Start();
            //注册到系统组件中
            SysComponents.RegComponent("CarRecordUploadThread", carRecordUploadThread);

            ////心跳服务
            //System.Threading.Thread heartBaetThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SendHeartBeatThreadFunc));
            //heartBaetThread.IsBackground = true;
            //heartBaetThread.Name = "HeartBaetThread";
            //heartBaetThread.Start(sysConfig);

            //升级代理初始化线程
            System.Threading.Thread initUpdateServerProxyThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InitUpdateServerProxyThreadFunc));
            initUpdateServerProxyThread.IsBackground = true;
            initUpdateServerProxyThread.Name = "InitUpdateServerProxyThreadFunc";
            initUpdateServerProxyThread.Start(sysConfig);

            //自动升级检查
            SoftwareUpdateThread softwareUpdateThread = new SoftwareUpdateThread(type, note);
            softwareUpdateThread.Start();
            //注册到系统组件中
            SysComponents.RegComponent("SoftwareUpdateThread", softwareUpdateThread);


            rs = true;
            return rs;
        }


        public static bool StopService()
        {

            //关闭自动升级
            SoftwareUpdateThread softwareUpdateThread = SysComponents.GetComponent("SoftwareUpdateThread") as SoftwareUpdateThread;
            if (softwareUpdateThread != null)
            {
                softwareUpdateThread.Stop();
                SysComponents.UnRegComponent("SoftwareUpdateThread");
            }

            //关闭自动上传新车辆记录线程
            CarRecordUploadThread carRecordUploadThread = SysComponents.GetComponent("CarRecordUploadThread") as CarRecordUploadThread;
            if (carRecordUploadThread != null)
            {
                carRecordUploadThread.Stop();
                SysComponents.UnRegComponent("CarRecordUploadThread");
            }

            ////2017-12-17 PM 消费记录功能取消
            //关闭自动上传消费记录线程
            //CustomRecordUploadThread customRecordUploadThread = SysComponents.GetComponent("CustomRecordUploadThread") as CustomRecordUploadThread;
            //if (customRecordUploadThread != null)
            //{
            //    customRecordUploadThread.Stop();
            //    SysComponents.UnRegComponent("CustomRecordUploadThread");
            //}

            //关闭ICE升级服务
            Ice_Servant_Factory.BaseICEObjectWrapper IUpdateImpl_Wrapper = SysComponents.GetComponent("IUpdateImpl_Wrapper") as Ice_Servant_Factory.BaseICEObjectWrapper;
            if (IUpdateImpl_Wrapper != null)
            {
                IUpdateImpl_Wrapper.Shutdown();
                SysComponents.UnRegComponent("IUpdateImpl_Wrapper");
            }

            //关闭ICE查询服务
            Ice_Servant_Factory.BaseICEObjectWrapper ICarQueryImpl_Wrapper = SysComponents.GetComponent("ICarQueryImpl_Wrapper") as Ice_Servant_Factory.BaseICEObjectWrapper;
            if (ICarQueryImpl_Wrapper != null)
            {
                ICarQueryImpl_Wrapper.Shutdown();
                SysComponents.UnRegComponent("ICarQueryImpl_Wrapper");
            }

            //关闭升级工作线程池
            IUpdateCallProcessWorkerPool updateThreadPool = SysComponents.GetComponent("IUpdateCallProcessWorkerPool") as IUpdateCallProcessWorkerPool;
            if (updateThreadPool != null)
            {
                updateThreadPool.Stop();
                SysComponents.UnRegComponent("IUpdateCallProcessWorkerPool");
            }

            //关闭查询线程池
            QueryThreadWorkerPool queryThreadPool = SysComponents.GetComponent("QueryThreadWorkerPool") as QueryThreadWorkerPool;
            if (queryThreadPool != null)
            {
                queryThreadPool.Stop();
                SysComponents.UnRegComponent("QueryThreadWorkerPool");
            }

            //2017-12-17 PM 消费记录功能取消
            //关闭消费记录插入线程 
            //InsertCustomRecordWorker insertCustomRecordWorker = SysComponents.GetComponent("InsertCustomRecordWorker") as InsertCustomRecordWorker;
            //if (insertCustomRecordWorker != null)
            //{
            //    insertCustomRecordWorker.Stop();
            //    SysComponents.UnRegComponent("InsertCustomRecordWorker");
            //}

            //关闭车辆插入线程
            InsertCarRecordWorker insertCarRecordWorker = SysComponents.GetComponent("InsertCarRecordWorker") as InsertCarRecordWorker;
            if (insertCarRecordWorker != null)
            {
                insertCarRecordWorker.Stop();
                SysComponents.UnRegComponent("InsertCarRecordWorker");
            }

            //本地不再缓存车辆信息 20171216 PM
            ////关闭车辆本地插入线程
            //InsertCarInfoWorker insertCarInfoWorker = SysComponents.GetComponent("InsertCarInfoWorker") as InsertCarInfoWorker;
            //if (insertCarInfoWorker != null)
            //{
            //    insertCarInfoWorker.Stop();
            //    SysComponents.UnRegComponent("InsertCarInfoWorker");
            //}

            ////关闭车辆下载线程
            //DownloadCarTableThread downloadCarTableThread = SysComponents.GetComponent("DownloadCarTableThread") as DownloadCarTableThread;
            //if (downloadCarTableThread != null) 
            //{
            //    downloadCarTableThread.Stop();
            //    SysComponents.UnRegComponent("DownloadCarTableThread");
            //}

            //日志相关资源释放
            LogerPrintWrapper.ClosePrint();
            //
            LOGCS.CLoger cloger = SysComponents.GetComponent("CLoger") as LOGCS.CLoger;
            if (cloger != null)
                cloger.Dispose();

            return true;
        }

        /// <summary>
        /// 启动数据库实例加载操作
        /// 【线程函数】
        /// </summary>
        /// <param name="stat">SysConfig对象</param>
        private static void StartDBInstanceLoadThreadFunc(object stat) 
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

            //实例化中心MSSQL数据操作对象并注册到系统组件中
            MySqlHandler MySqlHandler = MySqlHandler.Instance(sysConfig.mCenterMSSQLAddress, sysConfig.mCenterMSSQLInitialCatalog, sysConfig.mCenterMSSQLUser, sysConfig.mCenterMSSQLPassword);
            if (MySqlHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "中心MSSQL数据库实例加载失败,请检查相关参数是否异常");
            }
            else 
            {
                //连接测试
                string error;
                while (true)
                {
                    if (!MySqlHandler.TestConnection(out error))
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "中心MSSQL数据库实例连接失败:{0}\r\n系统将在30秒后重试", error);
                    }
                    else
                    {
                        SysComponents.RegComponent("MySqlHandler", MySqlHandler);//注册组件
                        LogerPrintWrapper.Print(LOGCS.LogLevel.INFO, "中心MSSQL数据库实例连接测试成功!");
                        break;
                    }

                    System.Threading.Thread.Sleep(30000);
                }
            }

            isDbInstance = true;
        }

        /// <summary>
        /// 启动ICE查询服务
        /// 【线程函数】
        /// </summary>
        /// <param name="stat">SysConfig对象</param>
        private static void StartICEQueryServerThreadFunc(object stat) 
        {
            SysConfig sysConfig = stat as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "启动ICE查询服务失败,系统配置对象为NULL");
                return;
            }

            //获取车辆查询实现对象
            ICarQueryImpl iCarQueryImpl = SysComponents.GetComponent("ICarQueryImpl") as ICarQueryImpl;
            if (iCarQueryImpl == null) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "启动ICE查询服务失败,ICarQueryImpl对象为NULL");
                return;
            }

            //初始化服务端代码
            Ice_Servant_Factory.BaseICEObjectWrapper wrapper = new Ice_Servant_Factory.BaseICEObjectWrapper(iCarQueryImpl);
            SysComponents.RegComponent("ICarQueryImpl_Wrapper", wrapper);

            string error;
            if (!Ice_Servant_Factory.ICEServantFactory.BuildService(sysConfig.mIQueryPlateServerIceServantProp, sysConfig.mIQueryPlateServerIceServant, ref wrapper, out error)) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "启动ICE查询服务失败:{0}",error);
            }



        }

        /// <summary>
        /// 启动ICE客户端升级服务
        /// 【该服务用于客户端升级请求】
        /// 【线程函数】
        /// </summary>
        /// <param name="stat">SysConfig对象</param>
        private static void StartICEUpdateServerThreadFunc(object stat) 
        {
            SysConfig sysConfig = stat as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "启动ICE客户端升级服务失败,系统配置对象为NULL");
                return;
            }

            //获取ICE客户端升级服务实现对象
            IUpdateImpl iUpdateImpl = SysComponents.GetComponent("IUpdateImpl") as IUpdateImpl;
            if (iUpdateImpl == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "启动ICE客户端升级服务失败,IUpdateImpl对象为NULL");
                return;
            }

            //初始化服务端代码
            Ice_Servant_Factory.BaseICEObjectWrapper wrapper = new Ice_Servant_Factory.BaseICEObjectWrapper(iUpdateImpl);
            SysComponents.RegComponent("IUpdateImpl_Wrapper", wrapper);

            string error;
            if (!Ice_Servant_Factory.ICEServantFactory.BuildService(sysConfig.mUpdateServerIceServantProp, sysConfig.mUpdateServerIceServant, ref wrapper, out error))
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "启动ICE客户端升级服务失败:{0}", error);
            }

        }


        ///// <summary>
        ///// 心跳发送线程函数
        ///// 【线程函数】
        ///// </summary>
        ///// <param name="stat">SysConfig对象</param>
        //private static void SendHeartBeatThreadFunc(object stat)
        //{
        //    SysConfig sysConfig = stat as SysConfig;
        //    if (sysConfig == null)
        //    {
        //        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "心跳发送线程失败,系统配置对象为NULL");
        //        return;
        //    }

        //    //解析IP地址和端口
        //    string remoteIP=sysConfig.mMonitorServerEndPoint.Substring(0,sysConfig.mMonitorServerEndPoint.IndexOf(':'));
        //    int port=int.Parse(sysConfig.mMonitorServerEndPoint.Substring(sysConfig.mMonitorServerEndPoint.IndexOf(':')+1));
        //    System.Net.IPEndPoint remoteHost=new System.Net.IPEndPoint(System.Net.IPAddress.Parse(remoteIP),port);

        //    UdpClient client = new UdpClient();
            

        //    //TODO考虑是否需要CPU和内存使用情况统计
        //    //
        //    HeartBeatData data=new HeartBeatData(){ ProcessId=Process.GetCurrentProcess().Id, HeartBeatInterval=sysConfig.mHeartBeatIntervalOfSecond};
        //    int heartBeatDataSz = Marshal.SizeOf(typeof(HeartBeatData));
        //    byte[] buffer=new byte[heartBeatDataSz];

        //    while(true)
        //    {
        //        if(StructConvert.ObjectToBytesEx(ref data,buffer,0,heartBeatDataSz))
        //        {
        //            try
        //            {
        //                client.Send(buffer, buffer.Length, remoteHost);
        //            }
        //            catch (Exception ex)
        //            {
        //                 LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "心跳包发送失败:{0}",ex.Message);
        //            }
        //        }
        //        System.Threading.Thread.Sleep(sysConfig.mHeartBeatIntervalOfSecond * 1000);
        //    }

            

        //}


        /// <summary>
        /// 升级服务代理初始化线程函数
        /// 【线程函数】
        /// </summary>
        /// <param name="stat">SysConfig对象</param>
        private static void InitUpdateServerProxyThreadFunc(object stat) 
        {
            SysConfig sysConfig = stat as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "InitUpdateServerProxyThreadFunc()=>系统配置对象为NULL");
                return;
            }

            string error;
            ClientProxyWrapper<ASSISTUPDATEMODULEICE.IUpdatePrx> proxy = new ClientProxyWrapper<ASSISTUPDATEMODULEICE.IUpdatePrx>();
            while (true) 
            {

                if (!ICEServantFactory.GetProxy(sysConfig.mUpdateServerPorxyProp, sysConfig.mUpdateServerIceAddress, ref proxy, out error))
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
    }
}
