using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace CommHandler
{
    [Serializable]
    public class SysConfig
    {

        #region 用户参数
        /// <summary>
        /// 高速名称
        /// eg:[001成雅高速]
        /// </summary>
        public string mCompanyCode;

        /// <summary>
        /// 高速收费站点名称
        /// eg:[003成雅新津东站]
        /// </summary>
        public string mPlazCode;

        #endregion

        /// <summary>
        /// 号码查询服务ICE地址
        /// eg:[FWYXHJ:tcp -h 127.0.0.1 -p 8888];
        /// 默认:QueryPlateServer:default -p 12350
        /// </summary>
        public string mIQueryPlateServerIceServant = "QueryPlateServer:default -p 12350";

        /// <summary>
        /// 号码查询ICE服务配置文件名称
        /// 默认:ice_props\\Ice_QueryPlate.props
        /// </summary>
        public string mIQueryPlateServerIceServantProp = "ice_props\\Ice_QueryPlate.props";

        /// <summary>
        /// 升级服务ICE地址
        /// eg:[FWYXHJ:default -p 8888];
        /// 默认:UpdateServer:default -p 12360
        /// </summary>
        public string mUpdateServerIceServant="UpdateServer:default -p 12360";

        /// <summary>
        /// 升级ICE服务配置文件名称
        /// 默认:ice_props\\Ice_UpdateServer.props
        /// </summary>
        public string mUpdateServerIceServantProp = "ice_props\\Ice_UpdateServer.props";


        /// <summary>
        /// 升级服务所在的地址
        /// 默认:UpdateServer:tcp -h 127.0.0.1 -p 12360
        /// 【ICE客户端调用需要】
        /// </summary>
        public string mUpdateServerIceAddress = "UpdateServer:tcp -h 127.0.0.1 -p 12360";

        /// <summary>
        /// 升级ICE服务客户端配置文件名称
        ///  默认:ice_props\\Ice_UpdateServer_Proxy.props
        /// </summary>
        public string mUpdateServerPorxyProp = "ice_props\\Ice_UpdateServer_Proxy.props";

        /// <summary>
        /// 最大允许的查询实体个数，此参数将影响客户端的查询请求数，当超出mMaxQueryEntityObjCount后，客户端查询将失败;
        /// 默认1000
        /// </summary>
        public int mMaxQueryEntityObjCount=1000;

        /// <summary>
        /// 最大允许缓存的车辆信息实体对象个数，此参数用于本地入库之前的内存缓存;
        /// 默认1000
        /// </summary>
        public int mMaxCacalCarEntityObjCount=1000;

        /// <summary>
        ///  最大允许缓存的未知车辆信息实体对象个数，此参数用于本地入库之前的内存缓存;
        ///  默认1000
        /// </summary>
        public int mMaxUnkownCarEntityObjCount = 1000;

        /// <summary>
        /// 车辆信息查询线程池中用于查询的线程个数
        /// 默认10个查询线程
        /// </summary>
        public int mQueryThreadWorkerPoolSize = 10;

        /// <summary>
        /// 最大允许的升级实体个数，此参数将影响客户端的升级请求数，当超过mMaxUpdateCallEntityObjCount后，客户端的升级请求将失败
        /// 默认1000
        /// </summary>
        public int mMaxUpdateCallEntityObjCount = 1000;

        /// <summary>
        /// 升级调用线程池中用于升级相关处理的线程个数
        /// 默认10个线程
        /// </summary>
        public int mUpdateCallProcessWorkerPoolSize = 10;

        /// <summary>
        /// 本地sqlite缓存数据库的名称
        /// 默认:TollAssist.db
        /// </summary>
        public string mLocalSqliteDBName = "TollAssist.db";

        /// <summary>
        /// 中心ms数据库的连接地址
        /// </summary>
        public string mCenterMSSQLAddress = "116.62.22.247";//方便序列化

        /// <summary>
        /// 中心ms数据库的实例名称
        /// </summary>
        public string mCenterMSSQLInitialCatalog = "highspeed";

        /// <summary>
        /// 中心ms数据库的用户名
        /// </summary>
        public string mCenterMSSQLUser = "root";

        /// <summary>
        /// 中心ms数据库的密码
        /// </summary>
        public string mCenterMSSQLPassword = "localhost";

        /// <summary>
        /// 监视服务的终结点信息,用于定时向监视服务发送心跳包
        /// 默认:127.0.0.1:12370
        /// </summary>
        public string mMonitorServerEndPoint = "127.0.0.1:12370";

        /// <summary>
        /// 是否启用心跳服务
        /// </summary>
        public bool mEnableHeartBeat = true;

        /// <summary>
        /// 心跳包的发送间隔时间，以秒为单位，默认5秒
        /// </summary>
        public int mHeartBeatIntervalOfSecond = 5;

        /// <summary>
        /// 中心共享文件夹地址
        /// eg:[\\192.168.0.55\share]
        /// </summary>
        public string mCenterShareFolderAddress=@"\\192.168.100.55\share";

        /// <summary>
        /// 用于访问中心共享文件夹的用户名
        /// </summary>
        public string mCenterShareFolderUser="lsh";

        /// <summary>
        /// 用于访问中心共享文件夹的密码
        /// </summary>
        public string mCenterShareFolderPassword="lsh";

        /// <summary>
        /// 中心存放客户端(收费助手)升级模块的文件夹名称
        /// </summary>
        public string mCenterClientUpdateFolderName = "Client";

        /// <summary>
        /// 中心存放服务端升级模块的文件夹名称
        /// </summary>
        public string mCenterServerUpdateFolderName="Server";

        /// <summary>
        /// 中心用于上传车辆记录的文件夹名称
        /// </summary>
        public string mCenterUploadUnkownPlateFolderName = "车辆记录";

        /// <summary>
        /// 中心用于上传消费记录的文件夹名称
        /// </summary>
        public string mCenterUploadCustomRecordFolderName="消费记录";

        /// <summary>
        /// 是否自动上传消费记录
        /// </summary>
        public bool mAutoUploadCustomRecord=true;

        /// <summary>
        /// 上传消费记录的频率(天、周、月)
        /// 默认按周上传
        /// </summary>
        public AutoUploadFrequency mUploadCustomRecordFrequency= AutoUploadFrequency.Week;

        /// <summary>
        /// 执行上传消费记录的触发时间点([0~23]中的一个小时值)
        /// 默认:早上1点
        /// </summary>
        public int mUploadCustomRecordTime=1;

        /// <summary>
        /// 是否自动上传车辆记录
        /// </summary>
        public bool mAutoUploadCarRecord=true;

        /// <summary>
        /// 上传车辆的频率(天、周、月)
        /// 默认按周上传 
        /// </summary>
        public AutoUploadFrequency mUploadCarRecordFrequency = AutoUploadFrequency.Week;

        /// <summary>
        /// 执行上传车辆记录的触发时间点([0~23]中的一个小时值)
        /// 默认:早上1点
        /// </summary>
        public int mUploadCarRecordTime = 1;

        /// <summary>
        /// 软件升级方式(自动升级、定时升级、手动升级)
        /// 默认:定时升级
        /// </summary>
        public SoftwareUpdateWay mUpdateWay= SoftwareUpdateWay.FixTime;

        /// <summary>
        /// 检查是否存在新版本的频率(时间秒)---自动升级需要此参数
        /// 默认:300秒
        /// </summary>
        public int mCheckUpdateFrequency=300;

        /// <summary>
        /// 定时升级触发时间点([0~23]中的一个小时值)---定时升级需要此参数
        /// 默认:早上2点
        /// </summary>
        public int mCheckUpdateDateTime = 2;

        /// <summary>
        /// 消费记录保存天数,当超过该天数后数据库中的消费记录被清空
        /// 默认40天
        /// </summary>
        public int mCustomRecordSaveDays = 40;

        /// <summary>
        /// 车辆记录保存天数，当超过该天数后数据库中的车辆记录被清空
        /// 默认40天
        /// </summary>
        public int mCarRecordSaveDays = 40;

       



        /// <summary>
        /// 下载临时目录
        /// 默认:[D:\\TollAssistServerDownload\\Tmp\\Download]
        /// </summary>
        public string mDownloadTmpDir = "D:\\TollAssistServerDownload\\Tmp\\Download";

        /// <summary>
        /// 上传临时目录
        /// 默认:[D:\\TollAssistServerDownload\\Tmp\\Upload]
        /// </summary>
        public string mUploadTmpDir = "D:\\TollAssistServerDownload\\Tmp\\Upload";

        /// <summary>
        /// 升级文件夹名称(用于升级过程中存放下载下来的文件)
        /// 【此名称相对于程序执行路径】
        /// </summary>
        public string mUpdateFolderName = "update";

        #region 保留字段

        public string mStrRemark1 = string.Empty;
        public string mStrRemark2 = string.Empty;
        public string mStrRemark3 = string.Empty;
        public string mStrRemark4 = string.Empty;
        public string mStrRemark5 = string.Empty;
        public string mStrRemark6 = string.Empty;
        public string mStrRemark7 = string.Empty;
        public string mStrRemark8 = string.Empty;
        public string mStrRemark9 = string.Empty;
        public string mStrRemark10 = string.Empty;

        public int mIntRemark1;
        public int mIntRemark2;
        public int mIntRemark3;
        public int mIntRemark4;

        public long mLongRemark1;
        public long mLongRemark2;
        public long mLongRemark3;
        public long mLongRemark4;

        public bool mBoolRemark1;
        public bool mBoolRemark2;
        public bool mBoolRemark3;
        public bool mBoolRemark4;

        #endregion



        /// <summary>
        /// 从SysConfig.xml文件读取并反序列化为对象
        /// </summary>
        /// <returns></returns>
        public static SysConfig LoadObjectFromFile()
        {
            SysConfig obj = null;
            //从文件读取监视项集合并反序列化为对象
            XmlSerializer mySerializer =
            new XmlSerializer(typeof(SysConfig));

            if (!System.IO.File.Exists("SysConfig.xml"))
            {
                obj = new SysConfig() ;

                //保存到本地文件
                SaveObjectToFile(obj);

                return obj;
            }

            using (FileStream myFileStream = new FileStream("SysConfig.xml", FileMode.Open))
            {
                obj = (SysConfig)
                mySerializer.Deserialize(myFileStream);
            }

            return obj;
        }

        /// <summary>
        /// 将SysConfig对象保存到文件SysConfig.xml中
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveObjectToFile(SysConfig obj)
        {
            if (obj == null)
                return;
            XmlSerializer mySerializer =
           new XmlSerializer(typeof(SysConfig));
            // To read the file, create a FileStream.
            using (FileStream myFileStream = new FileStream("SysConfig.xml", FileMode.Create, FileAccess.Write))
            {
                mySerializer.Serialize(myFileStream, obj);
            }
        }

    }

   
    /// <summary>
    /// 自动上传频率
    /// </summary>
    public enum AutoUploadFrequency 
    {
        /// <summary>
        /// 按天上传
        /// </summary>
        Day=0,

        /// <summary>
        /// 按周上传
        /// </summary>
        Week=1,

        /// <summary>
        /// 按月上传
        /// </summary>
        Month=2,
    }

    /// <summary>
    /// 软件升级方式
    /// </summary>
    public enum SoftwareUpdateWay
    {
        /// <summary>
        /// 自动升级
        /// </summary>
        Auto=0,
        /// <summary>
        /// 手动升级
        /// </summary>
        Manual=1,

        /// <summary>
        /// 定时升级
        /// </summary>
        FixTime=2,
    }
}
