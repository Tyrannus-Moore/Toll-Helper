using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.IO.Ports;


namespace CommHandler
{
    [Serializable]
    public class SysConfig
    {

        public SysConfig()
        {
            this.mLocalHostIPAddress = TollAssistComm.Helper.GetHostIPOfFrist();
        }

        #region 站点设置

        /// <summary>
        /// 高速名称
        /// eg:[001成南高速]
        /// </summary>
        public string mCompanyCode = string.Empty;

        /// <summary>
        /// 高速收费站点名称
        /// eg:[003成雅新津东站]
        /// </summary>
        public string mPlazCode = string.Empty;

        /// <summary>
        /// 出入口
        /// </summary>
        public string mLanName = string.Empty;

        /// <summary>
        /// 车道编号
        /// </summary>
        public int mLanNum=1;

        /// <summary>
        /// 本机IP地址
        /// </summary>
        public string mLocalHostIPAddress = string.Empty;

         /// <summary>
        /// 调度服务所在IP地址
        /// </summary>
        public string mServerIPAddress = string.Empty;

        #endregion

        #region 车牌设置

        /// <summary>
        /// 收费软件窗口标题
        /// </summary>
        public string mTollFormTitel = string.Empty;

        /// <summary>
        /// 收费软件程序路径
        /// </summary>
        public string mTollSoftPath = string.Empty;

        /// <summary>
        /// 车牌控件ID
        /// </summary>
        public string mPlatteControlID = string.Empty;

        /// <summary>
        /// 车牌控件坐标
        /// </summary>
        public System.Drawing.Point mPlatteControlPos;

        /// <summary>
        /// 车型控件ID
        /// </summary>
        public string mCarTypeControlID = string.Empty;

        /// <summary>
        /// 车型控件坐标
        /// </summary>
        public System.Drawing.Point mCarTypeControlPos;
 
        #endregion

        #region 提示信息设置

        /// <summary>
        /// 提示信息字体大小
        /// </summary>
        public int mTipsFontSize=18;

        /// <summary>
        /// 提示信息坐标
        /// </summary>
        public System.Drawing.Point mTipsFormPos = new System.Drawing.Point(800, 100);

        /// <summary>
        /// 控制资源窗口(气泡窗口)的可见性
        /// </summary>
        public bool mResFormVisible=true;

        /// <summary>
        /// 系统启动时的欢迎界面是否显示,默认显示
        /// </summary>
        public bool mWelcomeLogoVisible = true;

        #endregion

        #region 超载提示设置

        /// <summary>
        /// 正常显示的时候的超载提示文字
        /// </summary>
        public string mOverloadTips = "超载率：0.0%";

        /// <summary>
        /// 超载提示控件坐标
        /// </summary>
        public System.Drawing.Point mOverloadControlPos;

        #endregion

        /// <summary>
        /// 号码查询服务ICE地址
        /// eg:[FWYXHJ:tcp -h 127.0.0.1 -p 8888];
        /// 默认:QueryPlateServer:tcp -h 127.0.0.1 -p 12350
        /// 注意:如果mServerIPAddress不为空或NULL则IP地址将被替换为mServerIPAddress
        /// </summary>
        public string mIQueryPlateServerIceProxy = "QueryPlateServer:tcp -h 127.0.0.1 -p 12350";

        /// <summary>
        /// 号码查询ICE代理配置文件名称
        /// 默认:ice_props\\Ice_QueryPlate_Proxy.props
        /// </summary>
        public string mIQueryPlateServerIceProxyProp = "ice_props\\Ice_QueryPlate_Proxy.props";

        /// <summary>
        /// 升级服务ICE地址
        /// eg:[FWYXHJ:default -p 8888];
        /// 默认:UpdateServer:tcp -h 127.0.0.1 -p 12360
        /// 注意:如果mServerIPAddress不为空或NULL则IP地址将被替换为mServerIPAddress
        /// </summary>
        public string mUpdateServerIceProxy="UpdateServer:tcp -h 127.0.0.1 -p 12360";

        /// <summary>
        /// 升级ICE服务代理配置文件名称
        /// 默认:ice_props\\Ice_UpdateServer_Proxy.props
        /// </summary>
        public string mUpdateServerIceServantProp = "ice_props\\Ice_UpdateServer_Proxy.props";

        /// <summary>
        /// 本地sqlite缓存数据库的名称
        /// 默认:TollAssist.db
        /// </summary>
        public string mLocalSqliteDBName = "TollAssist.db";


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
        /// 是否自动上传消费记录
        /// </summary>
        public bool mAutoUploadCustomRecord=true;

        /// <summary>
        /// 是否自动同步站点表记录
        /// </summary>
        public bool mAutoSyncStationTab = true;

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
        /// 升级文件夹名称(用于升级过程中存放下载下来的文件)
        /// 【此名称相对于程序执行路径】
        /// </summary>
        public string mUpdateFolderName = "update";

        /// <summary>
        /// 串口名称
        /// </summary>
        public string mPortName = "COM3";

        /// <summary>
        /// 波特率
        /// </summary>
        public int mbaudRate = 9600;

        /// <summary>
        /// 奇偶校验位
        /// </summary>
        public Parity mParity = Parity.None;

        /// <summary>
        /// 每个字节的标准数据位长度
        /// </summary>
        public int mDataBits = 8;

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits mStopBits = StopBits.One;

        /// <summary>
        /// 告警服务websocket地址
        /// </summary>
        public string mAlarmPublishServerUrl = "ws://echo.websocket.org";

        /// <summary>
        /// 告警信息滚动频率
        /// 备注：单位毫秒
        /// </summary>
        public int mAlarmInfoScrollFrequency = 200;

        /// <summary>
        /// 车辆信息滚动频率
        /// 备注：单位毫秒
        /// </summary>
        public int mCarInfoScrollFrequency = 200;

        #region 2018-3-12新增字体选择
        /// <summary>
        /// 普通车辆颜色设置
        /// 备注：int
        /// </summary>
        public int mCommonCarR = 255;
        public int mCommonCarG = 255;
        public int mCommonCarB = 0;
        /// <summary>
        /// 涉嫌车辆颜色设置
        /// 备注：int
        /// </summary>
        public int mSuspectCarR = 255;
        public int mSuspectCarG = 0;
        public int mSuspectCarB = 0;

        /// <summary>
        /// 无效车辆颜色设置
        /// 备注：int
        /// </summary>
        public int mInvalidCarR = 255;
        public int mInvalidCarG = 255;
        public int mInvalidCarB = 0;

        /// <summary>
        /// 保畅车辆颜色设置
        /// 备注：int
        /// </summary>
        public int mUnblockedCarR = 255;
        public int mUnblockedCarG = 0;
        public int mUnblockedCarB = 0;

        public string minformationFont = "宋体";

        public int fontOutline=2;


        public string otherApplacationName = "HskDDNS";
        public string otherApplacationPath = "C:\\Program Files (x86)\\Oray\\HskDDNS\\HskDDNS.exe";
        public int refreshTime = 5000;


        #endregion


        #region 保留字段

        public string mStrRemark1 = string.Empty;//方便序列化;
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

        /// <summary>
        /// 将输入字符串中的IP进行替换(TODO 目前问题已修复待验证20171228)
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="replacement">需要替换的字符</param>
        /// <returns>返回替换后的字符串</returns>
        public static string ReplaceIP(string input,string replacement) 
        {
            string pattern = @"(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\b";//20171228 update
            //string tmp = System.Text.RegularExpressions.Regex.Replace(input, pattern, replacement);
            return System.Text.RegularExpressions.Regex.Replace(input, pattern, replacement);//20171228 update
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
