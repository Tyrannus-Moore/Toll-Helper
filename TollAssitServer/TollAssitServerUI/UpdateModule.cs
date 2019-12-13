using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using TollAssistComm;

//升级模块相关
namespace UpdateModule
{
    //中心共享文件夹子目录结构如下:
    //-------share
    //         |-----Server 存放服务端升级文件
    //         |-----Client 存放客户端升级文件
    //         |-----XXXXXX 消费记录和新车辆记录上传目录名字可配置


    //文件发布流程:
    //1.将要发布的内容按照在目标程序中出现的相对位置进行整理，根目录即表示目标程序所在目录;
    //2.在发布程序中发布界面选择当前整理好的文件夹，然后由发布程序将文件夹中的内容拷贝到资源文件夹中，其内部层次不可更改；这里
    //资源文件夹的命名采用时间格式(yyyyMMddHHmmss);可以选择性的填写更新说明(发布时候不做要求)
    //3.将整理好的资源文件夹内的相对路径填写到资源清单列表中，这里相对路径不包含资源文件夹Resource
    //4.填写发布记录表,其中将资源文件夹名称指向2步骤中生成的文件夹名称，同时生成发布序号，发布序号由时间值确认;
    
    //关于如何获取更新问题
    //1.客户端保留已经下载的最后一次的更新记录表项信息，对应到一个PublishRecord对象
    //2.客户端向服务端查询比PublishRecord.mSerialNumber大的所有发布记录
    //3.将所有PublishRecord对象进行并操作得到最终的PublishRecord对象集合
    //4.将PublishRecord对象集合中的所有信息都发送到客户端，注意最后需要向客户端发送一个新的PublishRecord对象
    //5.客户端更新版本
    //6.客户端更新本地记录表项信息


    //相关实体映射
    //PublishRecord：表示一条发布记录
    //ResourceFolderEntity：表示需要更新的内容所在的文件夹对象
    //DetailList：表示ResourceFolderEntity中需要更新的内容清单列表

    //具体映射
    //PublishRecord.mResourceFolderName=>ResourceFolderEntity,其中文件夹名称格式为yyyyMMddHHmmss
    //ResourceFolderEntity里面包含：一个名称为Resource的文件夹其内容为要发布的文件、一个List.txt的资源清单文件，
    //该文件中每行代表Resource中的一个资源(文件或文件夹)、可选的Describe.txt文件

    //文件组织结构如下:
    //---------PublishRecords.txt
    //---------yyyyMMddHHmmss
    //                  |-------Resource
    //                              |--------资源A
    //                              |--------资源B
    //                              |--------资源文件夹C
    //                              |--------资源N
    //                              |--------.....
    //                  |-------List.txt
    //---------yyyyMMddHHmmss
    //---------yyyyMMddHHmmss
    //---------..............

    /// <summary>
    /// 发布记录
    /// 每条记录的格式为:mSerialNumber mResourceFolderName
    /// </summary>
    public sealed class PublishRecord 
    {
        /// <summary>
        /// 发布序号，用于确认发布版本号，由时间值生成
        /// </summary>
        public uint mSerialNumber { get; set; }
        /// <summary>
        /// 资源文件夹名称，格式为yyyyMMddHHmmss
        /// </summary>
        public string mResourceFolderName { get; set; }

    }

    /// <summary>
    /// 清单列表
    /// </summary>
    public sealed class DetailList 
    {
        /// <summary>
        /// 文件列表，对应List.txt中的行集合
        /// </summary>
        public List<string> mFileList { get; set; }
    }

    /// <summary>
    /// 资源文件夹实体
    /// </summary>
    public sealed class ResourceFolderEntity 
    {
        /// <summary>
        /// 资源文件夹，对应名称为Resource
        /// </summary>
        public string mResourceFolder { get; set; }

        /// <summary>
        /// 资料清单列表，列表中的每项内容都必须存在于资源文件夹中
        /// </summary>
        public DetailList mDetailList { get; set; }

        /// <summary>
        /// 更新说明，对应名称为Describe.txt
        /// </summary>
        public string Describe { get; set; }
    }

    /// <summary>
    /// 合并资源项
    /// </summary>
    public sealed class MergeResourcItem 
    {
        public string mResourceFolderName { get; set; }//发布资源文件夹名称yyyyMMddHHmmss
        public string mResourcePath { get; set; }//详细资源路径,这里并不包含上级资源文件夹(Resource)路径

    }

    /// <summary>
    /// 升级模块帮助类
    /// </summary>
    public static class UpdateModuleHelper 
    {
        /// <summary>
        /// 根据发布记录集合与本地保存的发布记录进行比较获取发布记录列表
        /// </summary>
        /// <param name="records">完整的发布记录集合</param>
        /// <param name="localRecord">本地最后更新的发布记录</param>
        /// <param name="updatePublishList">需要更新的发布记录列表</param>
        /// <param name="newRecord">更新后的发布记录</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public static bool GetPublishRecordList(List<PublishRecord> records, PublishRecord localRecord,
            out List<PublishRecord> updatePublishList, out PublishRecord newRecord, out string error) 
        {
            error = string.Empty;
            updatePublishList = null;
            newRecord = null;
            if (records == null || records.Count == 0) 
            {
                error = "参数records无效";
                return false;
            }
            if (localRecord == null) 
            {
                //将本地版本值为最旧版本
                localRecord = new PublishRecord() { mResourceFolderName = string.Empty, mSerialNumber = 0 };
            }
            //筛选出比localRecord大的所有发布记录
            var hitPublishRecords = from record in records where record.mSerialNumber > localRecord.mSerialNumber select record;
            //获取筛选结果
            updatePublishList=hitPublishRecords.ToList();
            newRecord=new PublishRecord(){ mSerialNumber=0};

            //取最大值
            foreach (var item in updatePublishList)
            {
                if (item.mSerialNumber > newRecord.mSerialNumber)
                    newRecord = item;
            }

            return true;

        }

        /// <summary>
        /// 合并多个发布记录中的清单列表，生成一个最新的清单列表
        /// </summary>
        /// <param name="records">多个发布记录</param>
        /// <param name="updateList">合并后的清单列表</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public static bool MergeResourceList(Dictionary<PublishRecord, DetailList> records, out List<MergeResourcItem> updateList, out string error) 
        {
            error = string.Empty;
            updateList = null;
            if (records == null || records.Count == 0) 
            {
                error = "输入参数无效";
                return false;
            }

            //构造一个资源清单map,其中key:具体资源 value:最大的发布序号PublishRecord
            Dictionary<string, PublishRecord> recCount = new Dictionary<string, PublishRecord>();
            foreach (var item in records)
            {
                //归并操作
                foreach (string rec in item.Value.mFileList)
                {
                    string key = rec.Trim();
                    if (recCount.ContainsKey(key))
                    {
                        if (item.Key.mSerialNumber > recCount[key].mSerialNumber)
                        {
                            recCount[key] = item.Key;
                        }
                    }
                    else 
                    {
                        recCount[key] = item.Key;
                    }
                }
            }

            if (recCount.Count == 0) 
            {
                error = "不存在资源清单";
                return false;
            }

            updateList = new List<MergeResourcItem>();
            //整理资源结果
            foreach (var item in recCount)
            {
                updateList.Add(new MergeResourcItem() { mResourcePath=item.Key, mResourceFolderName=item.Value.mResourceFolderName});
            }
            return true;
        }

        /// <summary>
        /// FTP文件下载
        /// </summary>
        /// <param name="user">ftp用户名</param>
        /// <param name="pwd">ftp密码</param>
        /// <param name="url">完整的资源地址</param>
        /// <param name="timeout">下载超时时间，以毫秒为单位</param>
        /// <param name="fOffset">文件偏移位置，将从此位置读取文件</param>
        /// <param name="data">返回文件内容</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public static bool FtpFileDownLoad(string user,string pwd,string url, int timeout,uint fOffset, out byte[] data, out string error) 
        {
            error = string.Empty;
            data = null;
            FtpWebResponse response = null;
            Stream responseStream = null;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(user, pwd);
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Timeout = timeout;
                request.UseBinary = true;
                request.ContentOffset = fOffset;

                response = (FtpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                data = new byte[response.ContentLength];
                responseStream.Read(data, 0, data.Length);
               
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally 
            {
                if (response != null)
                {
                    response.Close();
                }

            }

            return false;

        }

        /// <summary>
        /// 共享文件下载
        /// </summary>
        /// <param name="user">远程主机用户名</param>
        /// <param name="pwd">远程主机密码</param>
        /// <param name="url">下载资源的完整URL</param>
        /// <param name="fOffset">文件的偏移量</param>
        /// <param name="fCount">需要下载的文件的大小,不能超过100MB</param>
        /// <param name="data">下载的数据</param>
        /// <param name="error">错误消息</param>
        /// <returns></returns>
        public static bool ShareFileDownLoad(string user, string pwd, string url,int fOffset,int fCount, out byte[] data, out string error) 
        {
            data = null;
            error = string.Empty;
            bool ret = false;

            string resFolderPath =System.IO.Path.GetDirectoryName(url);
            if (!NetworkShareFileHelper.OpenConnection(resFolderPath, user, pwd)) 
            {
                error =string.Format("网络连接打开失败,请检查URL是否正确:{0}",url);
                return false;
            }


            try
            {
                using (System.IO.FileStream fs = new FileStream(url, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length < fOffset)
                    {
                        data = new byte[0];
                    }
                    else 
                    {
                        if ((fs.Length - fOffset) > fCount)
                        {
                            data = new byte[fCount];
                        }
                        else
                        {
                            data = new byte[fs.Length - fOffset];
                        }
                        fs.Position = fOffset;
                        int ret_count = fs.Read(data, 0, data.Length);
                    }
                    

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                error =string.Format("文件读取异常:{0}",ex.Message);
                data = null;
            }

            //关闭连接
            NetworkShareFileHelper.DisConnection(resFolderPath);
            return ret;
        }


        /// <summary>
        /// 确定一个path指向的是文件还是其他
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsFile(string path) 
        {
            return System.IO.File.Exists(path);
        }

        /// <summary>
        /// 确定一个path指向的是文件夹还是其他
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsDirectory(string path) 
        {
            return System.IO.Directory.Exists(path);
        }

        /// <summary>
        /// 拷贝文件或文件夹下内容到指定的目录下；
        /// srcFolder为目录则拷贝其下文件或目录到destFolder,此时destFolder必须为目录;srcFolder为文件则destFolder可以为目录或者文件，为文件时直接拷贝，为目录则拷贝到其下
        /// </summary>
        /// <param name="srcFolder">拷贝源</param>
        /// <param name="destFolder">拷贝目的地</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool CopyFile(string srcFolder, string destFolder, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(srcFolder) || string.IsNullOrWhiteSpace(destFolder))
            {
                return false;
            }

            //有效性判断=>srcFolder必须存在
            if (!System.IO.Directory.Exists(srcFolder))
            {
                if (!System.IO.File.Exists(srcFolder))
                {
                    error = string.Format("指定的路径:{0}无效", srcFolder);
                    return false;
                }
            }

            if (System.IO.File.Exists(srcFolder)) //确定srcFolder为文件
            {
                if (!IsFile(destFolder))//destFolder为路径
                {
                    string fileName = System.IO.Path.GetFileName(srcFolder);//获取文件名
                    string destFile = System.IO.Path.Combine(destFolder, fileName);

                    if (!System.IO.Directory.Exists(destFolder))
                        System.IO.Directory.CreateDirectory(destFolder);

                    try
                    {
                        System.IO.File.Copy(srcFolder, destFile, true);//拷贝
                        //SendNotify(true, srcFolder, destFile, null);
                    }
                    catch (Exception ex)
                    {
                        //SendNotify(false, srcFolder, destFile, ex.Message);
                    }
                }
                else //destFolder为文件
                {
                    string destDir = System.IO.Path.GetDirectoryName(destFolder);

                    if (!System.IO.Directory.Exists(destDir))
                        System.IO.Directory.CreateDirectory(destDir);

                    try
                    {
                        System.IO.File.Copy(srcFolder, destFolder, true);//拷贝
                        //SendNotify(true, srcFolder, destFolder, null);
                    }
                    catch (Exception ex)
                    {
                        //SendNotify(false, srcFolder, destFolder, ex.Message);
                    }
                }

                return true;
            }
            else //srcFolder为文件夹,此种情况destFolder必须为路径
            {
                if (!IsFile(destFolder))//destFolder为路径
                {

                    if (!System.IO.Directory.Exists(destFolder))
                        System.IO.Directory.CreateDirectory(destFolder);

                    string[] files = System.IO.Directory.GetFiles(srcFolder);
                    foreach (string file in files)
                    {
                        if (!CopyFile(file, destFolder, out error))
                        {
                            return false;
                        }
                    }
                    string[] dirs = System.IO.Directory.GetDirectories(srcFolder);
                    foreach (string dir in dirs)
                    {
                        if (!CopyFile(dir, System.IO.Path.Combine(destFolder, dir.Substring(dir.LastIndexOf("\\") + 1)), out error))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else //destFolder为文件
                {
                    error = string.Format("{0}不是有效的路径", destFolder);
                    return false;
                }
            }

        }

    }

    /// <summary>
    /// 更新操作封装类(FTP方式)
    /// 具体操作步骤:GetPublishRecords()=>GetPublishRecordList()=>RelationRecordAnddDetailList()=>MergeResourceList()=>DownloadAllResource()=>UpdatePublishRecord()=>
    /// 将下载的和更新的文件全部替换到老的文件即完成了更新操作=>重启程序
    /// </summary>
    public sealed class FtpUpdateHandler
    {
        private string mFtpUrl;
        private string mFtpUser;
        private string mFtpPwd;
        private int mTimeout;//下载超时时间,ms
        public FtpUpdateHandler(string ftpUrl,string user,string pwd,int timeout_ms)
        {
            this.mFtpUrl = ftpUrl;
            this.mFtpUser = user;
            this.mFtpPwd = pwd;
            this.mTimeout = timeout_ms;
           
        }

        /// <summary>
        /// 获取发布记录集合对象
        /// </summary>
        /// <param name="records">返回的发布记录集合</param>
        /// <param name="error">错误消息</param>
        /// <returns></returns>
        public bool GetPublishRecords(out List<PublishRecord> records,out string error) 
        {
            bool ret = false;
            records = null;
            error = string.Empty;
            byte[] publishRecordBin = null;
            //首先下载发布记录文件
            string publishRecordUrl=System.IO.Path.Combine(this.mFtpUrl,"PublishRecords.txt");
            ret=UpdateModuleHelper.FtpFileDownLoad(this.mFtpUser, this.mFtpPwd, publishRecordUrl, this.mTimeout, 0,out publishRecordBin, out error);
            if (ret) 
            {
                //将文件中的内容转换为记录对象
                using (System.IO.MemoryStream ms = new MemoryStream(publishRecordBin)) 
                {
                    using (System.IO.StreamReader reader = new StreamReader(ms, System.Text.Encoding.Default)) 
                    {
                        string strRecord = string.Empty;
                        string[] terms = null;//字段集合
                        records = new List<PublishRecord>();
                        while ((strRecord = reader.ReadLine()) != null)
                        {

                            if (string.IsNullOrWhiteSpace(strRecord)) 
                                continue;
                            //解析每行数据,数据格式为:mSerialNumber mResourceFolderName
                            terms = strRecord.Split(' ');
                            if (terms == null)
                                continue;
                            PublishRecord record = new PublishRecord();
                            record.mSerialNumber = uint.Parse(terms[0]);
                            if (terms.Length > 1)
                            {
                                record.mResourceFolderName = terms[1];
                            }

                            records.Add(record);

                        }


                    }
                    
                }
               
            }
            return ret;
        }

        /// <summary>
        /// 获取与发布记录关联的发布清单列表
        /// </summary>
        /// <param name="record">发布记录对象</param>
        /// <param name="detailList">发布清单列表</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public bool GetDetailListByPublishRecord(PublishRecord record, out DetailList detailList, out string error)
        {
            bool ret = false;
            error = string.Empty;
            detailList = null;

            if (record == null || string.IsNullOrWhiteSpace(record.mResourceFolderName)) 
            {
                error = "输入参数无效";
                return ret;
            }

            //资源路径
            string detailListUrl = System.IO.Path.Combine(this.mFtpUrl, record.mResourceFolderName, "List.txt");
            //下载资源
            byte[] detailListBin=null;
            ret = UpdateModuleHelper.FtpFileDownLoad(this.mFtpUser, this.mFtpPwd, this.mFtpUrl, this.mTimeout, 0, out detailListBin, out error);
            if (ret) 
            {
                //将文件中的内容转换为记录对象
                using (System.IO.MemoryStream ms = new MemoryStream(detailListBin)) 
                {
                    using(System.IO.StreamReader reader = new StreamReader(ms, System.Text.Encoding.Default))
                    {
                        string strResource = string.Empty;
                        List<string> recList = new List<string>();
                        while ((strResource = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(strResource))
                                recList.Add(strResource);
                        }
                        if (recList.Count > 0)
                        {
                            detailList = new DetailList() { mFileList = recList };
                        }


                    }
                }       
            }


            return ret;
        }

        /// <summary>
        /// 将发布记录与资源清单进行关联
        /// </summary>
        /// <param name="records">发布记录集合</param>
        /// <param name="relationResult">关联结果</param>
        /// <returns></returns>
        public bool RelationRecordAnddDetailList(List<PublishRecord> records,out Dictionary<PublishRecord, DetailList> relationResult,out string error) 
        {
            bool ret = false;
            error = string.Empty;
            relationResult = null;

            if (records == null) 
            {
                error = "参数records无效";
                return ret;
            }

            DetailList dList=null;
            relationResult = new Dictionary<PublishRecord, DetailList>();
            foreach (PublishRecord item in records)
            {
                ret = GetDetailListByPublishRecord(item, out dList, out error);
                if (!ret) //只要有一条失败则返回失败
                    break;

                relationResult[item] = dList;

            }

            return ret;
        }

        /// <summary>
        /// 合并多个发布记录中的清单列表，生成一个最新的清单列表
        /// </summary>
        /// <param name="records">多个发布记录</param>
        /// <param name="updateList">合并后的清单列表</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public bool MergeResourceList(Dictionary<PublishRecord, DetailList> records, out List<MergeResourcItem> updateList, out string error) 
        {
            return UpdateModuleHelper.MergeResourceList(records, out updateList, out error);
        }

        /// <summary>
        /// 下载所有资源到downloadFolder文件夹下
        /// </summary>
        /// <param name="downloadFolder">下载文件目录</param>
        /// <param name="updateList">需要下载的文件列表</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool DownloadAllResource(string downloadFolder, List<MergeResourcItem> updateList, out string error) 
        {
            bool ret = false;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(downloadFolder) || updateList == null || updateList.Count == 0) 
            {
                error = "输入参数无效";
                return ret;
            }

            string resPath = string.Empty;
            string localRecPath = string.Empty;//本地资源路径
            string localDir = string.Empty;//本地目录
            byte[] recBin=null;
            foreach (MergeResourcItem item in updateList)
            {
                resPath = System.IO.Path.Combine(this.mFtpUrl, item.mResourceFolderName, "Resource", item.mResourcePath);
                ret=UpdateModuleHelper.FtpFileDownLoad(this.mFtpUser, this.mFtpPwd, resPath, this.mTimeout, 0,out recBin, out error);

                if (!ret) //失败一个则认为失败
                     break;

                //下载成功将文件写入指定的文件夹中
                localRecPath = System.IO.Path.Combine(downloadFolder, item.mResourcePath);

                localDir=System.IO.Path.GetDirectoryName(localRecPath);//获取资源文件在本地中的目录
                if (!System.IO.Directory.Exists(localDir)) //目录不存在则创建该目录
                    System.IO.Directory.CreateDirectory(localDir);

                //写入文件到磁盘
                using (System.IO.FileStream fs = new FileStream(localRecPath, FileMode.Create, FileAccess.Write)) 
                {
                    fs.Write(recBin, 0, recBin.Length);
                    fs.Flush();
                }

            }

            return ret;
        }

        /// <summary>
        /// 下载资源为指定文件
        /// </summary>
        /// <param name="downloadFile">下载文件保存地址包括文件的名称</param>
        /// <param name="resourcItem">需要下载文件</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool DownloadResource(string downloadFile, MergeResourcItem resourcItem, out string error)
        {
            bool ret = false;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(downloadFile) || resourcItem == null)
            {
                error = "输入参数无效";
                return ret;
            }

            string resPath = string.Empty;
            string localDir = string.Empty;//本地目录
            byte[] recBin = null;
            resPath = System.IO.Path.Combine(this.mFtpUrl, resourcItem.mResourceFolderName, "Resource", resourcItem.mResourcePath);
            ret = UpdateModuleHelper.FtpFileDownLoad(this.mFtpUser, this.mFtpPwd, resPath, this.mTimeout, 0, out recBin, out error);

            if (!ret) //失败一个则认为失败
                return false;

            //下载成功将文件写入指定的文件中
            
            localDir = System.IO.Path.GetDirectoryName(downloadFile);//获取资源文件在本地中的目录
            if (!System.IO.Directory.Exists(localDir)) //目录不存在则创建该目录
                System.IO.Directory.CreateDirectory(localDir);

            //写入文件到磁盘
            using (System.IO.FileStream fs = new FileStream(downloadFile, FileMode.Create, FileAccess.Write))
            {
                fs.Write(recBin, 0, recBin.Length);
                fs.Flush();
            }

            return ret;
        }


        /// <summary>
        /// 根据发布记录集合与本地保存的发布记录进行比较获取发布记录列表
        /// 备注：如果返回true&&updatePublishList.Count==0则没有新版本
        /// </summary>
        /// <param name="records">完整的发布记录集合</param>
        /// <param name="localRecord">本地最后更新的发布记录,不存在则为null</param>
        /// <param name="updatePublishList">需要更新的发布记录列表</param>
        /// <param name="newRecord">更新后的发布记录</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public bool GetPublishRecordList(List<PublishRecord> records, PublishRecord localRecord,
            out List<PublishRecord> updatePublishList, out PublishRecord newRecord, out string error)
        {
            return UpdateModuleHelper.GetPublishRecordList(records, localRecord,out updatePublishList, out newRecord, out error);
        }

        /// <summary>
        /// 更新或写入发布记录到本地磁盘中
        /// </summary>
        /// <param name="publishRecordPath">发布记录所在的路径</param>
        /// <param name="record">新的发布记录</param>
        /// <param name="error">错误消息</param>
        /// <returns></returns>
        public bool UpdatePublishRecord(string publishRecordPath, PublishRecord record, out string error) 
        {
            bool ret = false;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(publishRecordPath) || record == null) 
            {
                error = "输入参数无效";
                return ret;
            }

            string strRecord = string.Format("{0} {1}",record.mSerialNumber,record.mResourceFolderName);

            string localDir = System.IO.Path.GetDirectoryName(publishRecordPath);//获取本地目录
            if (!System.IO.Directory.Exists(localDir)) //目录不存在则创建该目录
                System.IO.Directory.CreateDirectory(localDir);

            //写入文件到磁盘
            using (System.IO.FileStream fs = new FileStream(publishRecordPath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = System.Text.Encoding.Default.GetBytes(strRecord);
                fs.Write(buffer,0,buffer.Length);
                fs.Flush();
                buffer = null;
                ret = true;
            }

            return ret; 
        }



    }


    /// <summary>
    /// 更新操作封装类(共享文件方式)
    /// 具体操作步骤:GetPublishRecords()=>GetPublishRecordList()=>RelationRecordAnddDetailList()=>MergeResourceList()=>DownloadAllResource()=>UpdatePublishRecord()=>
    /// 将下载的和更新的文件全部替换到老的文件即完成了更新操作=>重启程序
    /// </summary>
    public sealed class ShareFileUpdateHandler
    {

        private const int MAXFILELEN = 1024 * 1024 * 100;//最大文件长度100MB
        private string mRemoteUrl;
        private string mUser;
        private string mPwd;
        public ShareFileUpdateHandler(string remoteHostUrl, string user, string pwd)
        {
            this.mRemoteUrl = remoteHostUrl;
            this.mUser = user;
            this.mPwd = pwd;

        }

        /// <summary>
        /// 获取发布记录集合对象
        /// </summary>
        /// <param name="records">返回的发布记录集合</param>
        /// <param name="error">错误消息</param>
        /// <returns></returns>
        public bool GetPublishRecords(out List<PublishRecord> records, out string error)
        {
            bool ret = false;
            records = null;
            error = string.Empty;
            byte[] publishRecordBin = null;
            //首先下载发布记录文件
            string publishRecordUrl = System.IO.Path.Combine(this.mRemoteUrl, "PublishRecords.txt");
            ret = UpdateModuleHelper.ShareFileDownLoad(this.mUser, this.mPwd, publishRecordUrl, 0,MAXFILELEN, out publishRecordBin, out error);
            if (ret)
            {
                //将文件中的内容转换为记录对象
                using (System.IO.MemoryStream ms = new MemoryStream(publishRecordBin))
                {
                    using (System.IO.StreamReader reader = new StreamReader(ms, System.Text.Encoding.Default))
                    {
                        string strRecord = string.Empty;
                        string[] terms = null;//字段集合
                        records = new List<PublishRecord>();
                        while ((strRecord = reader.ReadLine()) != null)
                        {

                            if (string.IsNullOrWhiteSpace(strRecord))
                                continue;
                            //解析每行数据,数据格式为:mSerialNumber mResourceFolderName
                            terms = strRecord.Split(' ');
                            if (terms == null)
                                continue;
                            PublishRecord record = new PublishRecord();
                            record.mSerialNumber = uint.Parse(terms[0]);
                            if (terms.Length > 1)
                            {
                                record.mResourceFolderName = terms[1];
                            }

                            records.Add(record);

                        }


                    }

                }

            }
            return ret;
        }

        /// <summary>
        /// 获取与发布记录关联的发布清单列表
        /// </summary>
        /// <param name="record">发布记录对象</param>
        /// <param name="detailList">发布清单列表</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public bool GetDetailListByPublishRecord(PublishRecord record, out DetailList detailList, out string error)
        {
            bool ret = false;
            error = string.Empty;
            detailList = null;

            if (record == null || string.IsNullOrWhiteSpace(record.mResourceFolderName))
            {
                error = "输入参数无效";
                return ret;
            }

            //资源路径
            string detailListUrl = System.IO.Path.Combine(this.mRemoteUrl, record.mResourceFolderName, "List.txt");
            //下载资源
            byte[] detailListBin = null;
            ret = UpdateModuleHelper.ShareFileDownLoad(this.mUser, this.mPwd, detailListUrl, 0, MAXFILELEN, out detailListBin, out error);
            if (ret)
            {
                //将文件中的内容转换为记录对象
                using (System.IO.MemoryStream ms = new MemoryStream(detailListBin))
                {
                    using (System.IO.StreamReader reader = new StreamReader(ms, System.Text.Encoding.Default))
                    {
                        string strResource = string.Empty;
                        List<string> recList = new List<string>();
                        while ((strResource = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(strResource))
                                recList.Add(strResource);
                        }
                        if (recList.Count > 0)
                        {
                            detailList = new DetailList() { mFileList = recList };
                        }


                    }
                }
            }


            return ret;
        }

        /// <summary>
        /// 将发布记录与资源清单进行关联
        /// </summary>
        /// <param name="records">发布记录集合</param>
        /// <param name="relationResult">关联结果</param>
        /// <returns></returns>
        public bool RelationRecordAnddDetailList(List<PublishRecord> records, out Dictionary<PublishRecord, DetailList> relationResult, out string error)
        {
            bool ret = false;
            error = string.Empty;
            relationResult = null;

            if (records == null)
            {
                error = "参数records无效";
                return ret;
            }

            DetailList dList = null;
            relationResult = new Dictionary<PublishRecord, DetailList>();
            foreach (PublishRecord item in records)
            {
                ret = GetDetailListByPublishRecord(item, out dList, out error);
                if (!ret) //只要有一条失败则返回失败
                    break;

                relationResult[item] = dList;

            }

            return ret;
        }

        /// <summary>
        /// 合并多个发布记录中的清单列表，生成一个最新的清单列表
        /// </summary>
        /// <param name="records">多个发布记录</param>
        /// <param name="updateList">合并后的清单列表</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public bool MergeResourceList(Dictionary<PublishRecord, DetailList> records, out List<MergeResourcItem> updateList, out string error)
        {
            return UpdateModuleHelper.MergeResourceList(records, out updateList, out error);
        }

        /// <summary>
        /// 下载所有资源到downloadFolder文件夹下
        /// </summary>
        /// <param name="downloadFolder">下载文件目录</param>
        /// <param name="updateList">需要下载的文件列表</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool DownloadAllResource(string downloadFolder, List<MergeResourcItem> updateList, out string error)
        {
            bool ret = false;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(downloadFolder) || updateList == null || updateList.Count == 0)
            {
                error = "输入参数无效";
                return ret;
            }

            string resPath = string.Empty;
            string localRecPath = string.Empty;//本地资源路径
            string localDir = string.Empty;//本地目录
            byte[] recBin = null;
            foreach (MergeResourcItem item in updateList)
            {
                resPath = System.IO.Path.Combine(this.mRemoteUrl, item.mResourceFolderName, "Resource", item.mResourcePath);
                ret = UpdateModuleHelper.ShareFileDownLoad(this.mUser, this.mPwd, resPath, 0,MAXFILELEN, out recBin, out error);

                if (!ret) //失败一个则认为失败
                    break;

                //下载成功将文件写入指定的文件夹中
                localRecPath = System.IO.Path.Combine(downloadFolder, item.mResourcePath);

                localDir = System.IO.Path.GetDirectoryName(localRecPath);//获取资源文件在本地中的目录
                if (!System.IO.Directory.Exists(localDir)) //目录不存在则创建该目录
                    System.IO.Directory.CreateDirectory(localDir);

                //写入文件到磁盘
                using (System.IO.FileStream fs = new FileStream(localRecPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(recBin, 0, recBin.Length);
                    fs.Flush();
                }

            }

            return ret;
        }

        /// <summary>
        /// 下载资源为指定文件
        /// </summary>
        /// <param name="downloadFile">下载文件保存地址包括文件的名称</param>
        /// <param name="resourcItem">需要下载文件</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool DownloadResource(string downloadFile, MergeResourcItem resourcItem, out string error)
        {
            bool ret = false;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(downloadFile) || resourcItem == null)
            {
                error = "输入参数无效";
                return ret;
            }

            string resPath = string.Empty;
            string localDir = string.Empty;//本地目录
            byte[] recBin = null;
            resPath = System.IO.Path.Combine(this.mRemoteUrl, resourcItem.mResourceFolderName, "Resource", resourcItem.mResourcePath);
            ret = UpdateModuleHelper.ShareFileDownLoad(this.mUser, this.mPwd, resPath, 0,MAXFILELEN, out recBin, out error);

            if (!ret) //失败一个则认为失败
                return false;

            //下载成功将文件写入指定的文件中

            localDir = System.IO.Path.GetDirectoryName(downloadFile);//获取资源文件在本地中的目录
            if (!System.IO.Directory.Exists(localDir)) //目录不存在则创建该目录
                System.IO.Directory.CreateDirectory(localDir);

            //写入文件到磁盘
            using (System.IO.FileStream fs = new FileStream(downloadFile, FileMode.Create, FileAccess.Write))
            {
                fs.Write(recBin, 0, recBin.Length);
                fs.Flush();
            }

            return ret;
        }


        /// <summary>
        /// 根据发布记录集合与本地保存的发布记录进行比较获取发布记录列表
        /// 备注：如果返回true且updatePublishList.Count==0则没有新版本
        /// </summary>
        /// <param name="records">完整的发布记录集合</param>
        /// <param name="localRecord">本地最后更新的发布记录</param>
        /// <param name="updatePublishList">需要更新的发布记录列表</param>
        /// <param name="newRecord">更新后的发布记录</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public bool GetPublishRecordList(List<PublishRecord> records, PublishRecord localRecord,
            out List<PublishRecord> updatePublishList, out PublishRecord newRecord, out string error)
        {
            return UpdateModuleHelper.GetPublishRecordList(records, localRecord, out updatePublishList, out newRecord, out error);
        }

        /// <summary>
        /// 更新或写入发布记录到本地磁盘中
        /// </summary>
        /// <param name="publishRecordPath">发布记录所在的路径</param>
        /// <param name="record">新的发布记录</param>
        /// <param name="error">错误消息</param>
        /// <returns></returns>
        public bool UpdatePublishRecord(string publishRecordPath, PublishRecord record, out string error)
        {
            bool ret = false;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(publishRecordPath) || record == null)
            {
                error = "输入参数无效";
                return ret;
            }

            string strRecord = string.Format("{0} {1}", record.mSerialNumber, record.mResourceFolderName);

            string localDir = System.IO.Path.GetDirectoryName(publishRecordPath);//获取本地目录
            if (!System.IO.Directory.Exists(localDir)) //目录不存在则创建该目录
                System.IO.Directory.CreateDirectory(localDir);

            //写入文件到磁盘
            using (System.IO.FileStream fs = new FileStream(publishRecordPath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = System.Text.Encoding.Default.GetBytes(strRecord);
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                buffer = null;
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// 共享资源预热功能
        /// 防止需要访问时发生超时问题
        /// </summary>
        public void PreheatShare() 
        {
            System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(stat =>
            {
                //预热连接
                for (int i = 0; i < 10; i++)
                {
                    if (NetworkShareFileHelper.OpenConnection(this.mRemoteUrl, this.mUser, this.mPwd))
                    {
                        NetworkShareFileHelper.DisConnection(this.mRemoteUrl);
                        break;
                    }
                }



            }));
        }


    }
}
