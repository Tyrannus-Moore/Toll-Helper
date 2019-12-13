using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ice_Servant_Factory;

using TollAssistComm;

namespace CommHandler
{
    /// <summary>
    /// 更新通知接口
    /// </summary>
    public interface IUpdateSotfwareNote
    {

        /// <summary>
        /// 更新通知
        /// </summary>
        /// <param name="updateResult">升级检查下载结果</param>
        /// <param name="updateDir">下载的数据的路径</param>
        /// <param name="error">错误消息</param>
        void UpdateSotfwareNote(UpdateDownloadResult updateResult, string updateDir, string error);
    }

    /// <summary>
    /// 升级检查下载结果
    /// </summary>
    public enum UpdateDownloadResult
    {
        /// <summary>
        /// 更新或者下载存在错误
        /// </summary>
        Error = 0,
        /// <summary>
        /// 没有可以用的下载信息(不存在新版本)
        /// </summary>
        NoDownload = 1,
        /// <summary>
        /// 下载完成
        /// </summary>
        Downloaded = 2,
    }

    /// <summary>
    /// 软件升级模块
    /// </summary>
    public sealed class SoftwareUpdate
    {

        private ASSISTUPDATEMODULEICE.UpdateType mUpdateType;//升级类型
        public SoftwareUpdate(ASSISTUPDATEMODULEICE.UpdateType type)
        {
            this.mUpdateType = type;
        }


        /// <summary>
        /// 执行升级检查并下载所有资源到指定目录下
        /// </summary>
        /// <param name="localSerialNumber">本地软件的当前版本号，从PublishRecord.txt中读取</param>
        /// <param name="localDownloadDir">临时存放下载的文件的主目录</param>
        /// <param name="folderPath">下载的文件夹完整路径</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public UpdateDownloadResult UpdateDownload(long localSerialNumber, string localDownloadDir, out string folderPath, out string error)
        {
            //获取客户端代理对象
            ClientProxyWrapper<ASSISTUPDATEMODULEICE.IUpdatePrx> proxy = SysComponents.GetComponent("ASSISTUPDATEMODULEICE.IUpdatePrx") as ClientProxyWrapper<ASSISTUPDATEMODULEICE.IUpdatePrx>;
            if (proxy == null)
            {
                error = "获取代理IUpdatePrx失败";
                folderPath = null;
                return UpdateDownloadResult.Error;
            }

            IUpdateProxyWrapper wrapper = new IUpdateProxyWrapper(proxy.prx);

            //检查更新
            ASSISTUPDATEMODULEICE.ResourceItem[] list = null;
            long newLocalSerialNumber = 0;
            if (!wrapper.CheckUpdate(localSerialNumber, this.mUpdateType, out list, out newLocalSerialNumber, out error))
            {
                error = string.Format("调用CheckUpdate失败:{0}", error);
                folderPath = null;
                return UpdateDownloadResult.Error;
            }
            if (list == null || list.Length == 0)
            {
                error = null;
                folderPath = null;
                return UpdateDownloadResult.NoDownload;
            }


            //创建下载的目录
            folderPath = System.IO.Path.Combine(localDownloadDir, string.Format("{0}_{1}", newLocalSerialNumber, Helper.GetID()));
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            //下载更新
            long queryId = 0;
            byte[] fileBin = null;
            int from = 0;
            int count = 1024 * 1024 * 1;//每次最大1MB
            string filePath;
            string fileDir;
            foreach (ASSISTUPDATEMODULEICE.ResourceItem item in list)
            {
                if (!wrapper.QueryResource(this.mUpdateType, item, out queryId, out error))
                {
                    error = string.Format("调用QueryResource()失败:{0}", error);
                    return UpdateDownloadResult.Error;
                }
                filePath = System.IO.Path.Combine(folderPath, item.resourcePath);//构造资源文件完整路径
                //下载文件
                for (from = 0; ; from += count)
                {
                    if (!wrapper.GetResource(queryId, from, count, out fileBin, out error))
                    {
                        error = string.Format("调用GetResource()失败:{0}", error);
                        wrapper.CloseResource(queryId, out error);//强行关闭资源
                        return UpdateDownloadResult.Error;
                    }

                    if (fileBin.Length == 0)
                    {
                        //当前文件的内容已经取完
                        wrapper.CloseResource(queryId, out error);//关闭资源
                        break;
                    }

                    //文件本地路径
                    fileDir = System.IO.Path.GetDirectoryName(filePath);
                    if (!System.IO.Directory.Exists(fileDir))
                        System.IO.Directory.CreateDirectory(fileDir);

                    //写入数据到磁盘中
                    using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                    {
                        fs.Write(fileBin, 0, fileBin.Length);
                        fs.Flush();
                    }
                }

            }

            //生成本地版本号文件并保存到指定目录中
            filePath = System.IO.Path.Combine(folderPath, "PublishRecord.txt");
            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.Default))
                {
                    sw.WriteLine("{0} {1}", newLocalSerialNumber, "");
                }
            }

            return UpdateDownloadResult.Downloaded;
        }
    }

    /// <summary>
    /// 软件更新线程
    /// 适用于：自动或定时更新
    /// </summary>
    public sealed class SoftwareUpdateThread
    {
        private IUpdateSotfwareNote mIUpdateSotfwareNote;//通知接口
        private SoftwareUpdate mSoftwareUpdate;
        public SoftwareUpdateThread(ASSISTUPDATEMODULEICE.UpdateType type, IUpdateSotfwareNote note)
        {
            this.mSoftwareUpdate = new SoftwareUpdate(type);
            this.mIUpdateSotfwareNote = note;
        }

        private bool mIsStop = true;

        private void ThreadWorker(Object stat)
        {

            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "SoftwareUpdateThread::ThreadWorker()=>获取SysConfig实例失败,软件升级线程停止工作");
                return;
            }

            if (sysConfig.mUpdateWay == SoftwareUpdateWay.Manual)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "SoftwareUpdateThread::ThreadWorker()=>当前升级方式为手动,软件升级线程停止工作");
                return;
            }

            //检查频率(秒)
            int checkFreauqency = (sysConfig.mCheckUpdateFrequency > 0 && sysConfig.mCheckUpdateFrequency < 3600) ? sysConfig.mCheckUpdateFrequency : 300;

            //本地版本号
            uint localSerialNumber = GetLocalSerialNumber(System.IO.Path.Combine(Helper.GetCurrentProcessPath(), "PublishRecord.txt"));
            //定时更新时间点
            int checkUpdateDateTime = (sysConfig.mCheckUpdateDateTime >= 0 && sysConfig.mCheckUpdateDateTime <= 23) ? sysConfig.mCheckUpdateDateTime : 2;

            //本地下载路径
            string localDownDir = System.IO.Path.Combine(Helper.GetCurrentProcessPath(), sysConfig.mUpdateFolderName);
            if (!System.IO.Directory.Exists(localDownDir))
                System.IO.Directory.CreateDirectory(localDownDir);


            DateTime beginTime = DateTime.Now, endTime = DateTime.Now;
            string error;
            string folderPath;
            UpdateDownloadResult downResult;
            while (!this.mIsStop)
            {
                switch (sysConfig.mUpdateWay)
                {
                    case SoftwareUpdateWay.Auto: //自动升级
                    {

                        //本地版本号
                        localSerialNumber = GetLocalSerialNumber(System.IO.Path.Combine(Helper.GetCurrentProcessPath(), "PublishRecord.txt"));
                        //升级下载
                        downResult = DoUpdateDownload(localSerialNumber, localDownDir, out folderPath, out error);

                        for (int i = 0; i < checkFreauqency && (!this.mIsStop); i++)
                        {
                            System.Threading.Thread.Sleep(1000);
                        }

                        break;
                    }
                    case SoftwareUpdateWay.FixTime: //定时升级
                    {
                        if (DateTime.Now.Hour == checkUpdateDateTime) //时间匹配
                        {
                            //本地版本号
                            localSerialNumber = GetLocalSerialNumber(System.IO.Path.Combine(Helper.GetCurrentProcessPath(), "PublishRecord.txt"));
                            //升级下载
                            downResult = DoUpdateDownload(localSerialNumber, localDownDir, out folderPath, out error);

                            for (int i = 0; i < 3600 && (!this.mIsStop); i++)
                            {
                                System.Threading.Thread.Sleep(1000);//跳过该时刻
                            }

                        }
                        break;
                    }
                }

                for (int i = 0; i < 300 && (!this.mIsStop); i++)
                {
                    System.Threading.Thread.Sleep(1000);//每5分钟检查一次
                }


            }
        }

        /// <summary>
        /// 升级下载
        /// </summary>
        /// <param name="localSerialNumber"></param>
        /// <param name="localDownDir"></param>
        /// <param name="folderPath"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private UpdateDownloadResult DoUpdateDownload(uint localSerialNumber, string localDownDir, out string folderPath, out string error)
        {
            UpdateDownloadResult downResult;
            error = null;
            folderPath = null;
            try
            {
                //检测更新并下载资源
                downResult = this.mSoftwareUpdate.UpdateDownload(localSerialNumber, localDownDir, out folderPath, out error);
                if (this.mIUpdateSotfwareNote != null)
                {
                    this.mIUpdateSotfwareNote.UpdateSotfwareNote(downResult, folderPath, error);
                }
            }
            catch (Exception ex)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "SoftwareUpdateThread::调用UpdateDownload接口异常:{0}", ex.Message);
                downResult = UpdateDownloadResult.Error;
            }
            return downResult;
        }

        /// <summary>
        /// 获取本地版本号
        /// </summary>
        /// <param name="serialNumberFilePath">PublishRecord.txt文件的完整路径</param>
        /// <returns></returns>
        public static uint GetLocalSerialNumber(string serialNumberFilePath)
        {
            //读取本地版本号
            uint localSerialNumber = 0;
            if (System.IO.File.Exists(serialNumberFilePath))
            {
                try
                {
                    string fileContent = System.IO.File.ReadAllText(serialNumberFilePath);
                    if (!string.IsNullOrWhiteSpace(fileContent))
                    {
                        localSerialNumber = uint.Parse(fileContent.Substring(0, fileContent.IndexOf(' ')));
                    }
                }
                catch (Exception ex)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "SoftwareUpdateThread::GetLocalSerialNumber()异常:{0}", ex.Message);
                }
            }

            return localSerialNumber;
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
                thd.Name = "SoftwareUpdateThread";
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
