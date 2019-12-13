using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using ASSISTUPDATEMODULEICE;
using CommHandler;
using TollAssistComm;


namespace UpdateModule
{
    /// <summary>
    /// 当前调用方式
    /// </summary>
    public enum InvokeMethod 
    {
        /// <summary>
        /// 检查更新接口调用
        /// </summary>
        CheckUpdate=0,
        /// <summary>
        /// 查询资源接口调用
        /// </summary>
        QueryResource=1,
        /// <summary>
        /// 获取资源接口调用
        /// </summary>
        GetResource=2,
        /// <summary>
        /// 关闭资源接口调用
        /// </summary>
        CloseResource=3,
    }

    /// <summary>
    /// 调用参数，根据调用方式的不同参数有效个数不同
    /// </summary>
    public sealed class InvokeParams
    {
        public UpdateType Arg0 { get; set; }
        /// <summary>
        /// 查询id或者本地版本号
        /// </summary>
        public long Arg1{get;set;}
        public ResourceItem Arg2 { get; set; }

        /// <summary>
        /// GetResource接口的from参数
        /// </summary>
        public int Arg3 { get; set; }

        /// <summary>
        /// GetResource接口的count参数
        /// </summary>
        public int Arg4 { get; set; }

        
    }

    /// <summary>
    /// 接口调用
    /// </summary>
    public sealed class InterfaceInvoke 
    {
        public InvokeMethod Method { get; set; }
        public InvokeParams Params { get; set; }
    }


    /// <summary>
    /// 更新接口操作完成通知
    /// </summary>
    public interface IUpdateCompled
    {
        void UpdateCallCompled(IUpdateCallProcessEntity processEntity);

    }

    /// <summary>
    /// 具体的处理实现
    /// </summary>
    public sealed class IUpdateCallProcessEntity 
    {
        private InterfaceInvoke mInterfaceInvoke=null;
        private Object mResponse;//响应对象
        public IUpdateCallProcessEntity()
        {
            
        }

        public void SetParams(InvokeMethod method,UpdateType type, long arg1, ResourceItem arg2,int from,int count,Object response) 
        {
            if (this.mInterfaceInvoke == null) 
            {
                this.mInterfaceInvoke = new InterfaceInvoke();
                this.mInterfaceInvoke.Params = new InvokeParams();
            }
            this.mInterfaceInvoke.Method = method;
            this.mInterfaceInvoke.Params.Arg0 = type;
            this.mInterfaceInvoke.Params.Arg1 = arg1;
            this.mInterfaceInvoke.Params.Arg2 = arg2;
            this.mInterfaceInvoke.Params.Arg3 = from;
            this.mInterfaceInvoke.Params.Arg4 = count;
            this.mResponse = response;
            
        }

        public void CallProcess() 
        {

            if (this.mInterfaceInvoke == null) 
                return;

            //具体处理
            switch (this.mInterfaceInvoke.Method)
            {
                case InvokeMethod.CheckUpdate: //检查更新
                    {
                        CheckUpdate();
                        break;
                    }
                case InvokeMethod.QueryResource: //查询资源
                    {
                        QueryResource();
                        break;
                    }
                case InvokeMethod.GetResource: //获取资源
                    {
                        GetResource();
                        break;
                    }
                case InvokeMethod.CloseResource: //关闭资源
                    {
                        CloseResource();
                        break;
                    }
            }
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private void CheckUpdate() 
        {

            if (this.mResponse == null)
                return;
            if (!(this.mResponse is ASSISTUPDATEMODULEICE.AMD_IUpdate_CheckUpdate))
                return;

            ASSISTUPDATEMODULEICE.AMD_IUpdate_CheckUpdate response = this.mResponse as ASSISTUPDATEMODULEICE.AMD_IUpdate_CheckUpdate;

            if (this.mInterfaceInvoke == null || this.mInterfaceInvoke.Params == null)
            {
                ResponseCheckUpdate(response,false, null, 0, "参数无效");
                return;
            }
            if (this.mInterfaceInvoke.Method != InvokeMethod.CheckUpdate)
            {
                
                ResponseCheckUpdate(response, false, null, 0, "参数无效");
                return;
            }

            uint localSerialNumber = (uint)this.mInterfaceInvoke.Params.Arg1;//获取客户端本地版本号

            //组件名称
            string componentName = this.mInterfaceInvoke.Params.Arg0 == UpdateType.Client ? "ShareFileUpdateHandler" : "ShareFileUpdateHandler_Server";
            ShareFileUpdateHandler updateHandler = SysComponents.GetComponent(componentName) as ShareFileUpdateHandler;
            if (updateHandler == null)
            {
                LogerPrintWrapper.Print( LOGCS.LogLevel.ERROR,"IUpdateCallProcessEntity::CheckUpdate()=>获取查询组件{0}失败", componentName);
                ResponseCheckUpdate(response, false, null, 0, string.Format("获取查询组件{0}失败", componentName));
                return;
            }

            List<PublishRecord> records;//服务器上的发布记录
            string error;

            //1.获取服务器上的所有发布记录
            if (!updateHandler.GetPublishRecords(out records, out error)) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::CheckUpdate()调用GetPublishRecords()失败=>{0}", error);
                ResponseCheckUpdate(response, false, null, 0, error);
                return;
            }
            List<PublishRecord> updatePublishList;
            PublishRecord newRecord;
            //构造本地发布记录对象
            PublishRecord localPublishRecord = new PublishRecord() { mSerialNumber = localSerialNumber };

            //2.将服务器上的发布记录与本地的发布记录进行比较生成需要更新的发布记录集合
            if (!updateHandler.GetPublishRecordList(records, localPublishRecord, out updatePublishList, out newRecord, out error)) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::CheckUpdate()调用GetPublishRecordList()失败=>{0}", error);
               
                ResponseCheckUpdate(response, false, null, 0, error);

                return;
            }

            //没有要更新的版本信息
            if (updatePublishList == null || updatePublishList.Count == 0) 
            {
               
                ResponseCheckUpdate(response, false, null, 0, "没有要更新的版本信息");
                return;
            }

            //3.将发布记录与资源清单一一对应
            Dictionary<PublishRecord, DetailList> relationResult;
            if (!updateHandler.RelationRecordAnddDetailList(updatePublishList, out relationResult, out error)) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::CheckUpdate()调用RelationRecordAnddDetailList()失败=>{0}", error);
               
                ResponseCheckUpdate(response, false, null, 0, error);

                return;
            }

            //4.合并所有资源清单生成一个最新的资源清单
            List<MergeResourcItem> updateList;
            if (!updateHandler.MergeResourceList(relationResult, out updateList, out error)) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::CheckUpdate()调用MergeResourceList()失败=>{0}", error);
                
                ResponseCheckUpdate(response, false, null, 0, error);

                return;
            }

            //包装清单对象
            ASSISTUPDATEMODULEICE.ResourceItem[] list=new ResourceItem[updateList.Count];
            for (int i = 0; i < list.Length; i++)
			{
			    list[i]=new ResourceItem(){ resourceFolderName=updateList[i].mResourceFolderName, resourcePath=updateList[i].mResourcePath};
			}
            //整合资源清单结果并将其返回到调用端
            ResponseCheckUpdate(response, true, list, newRecord.mSerialNumber, error);
        }

        /// <summary>
        /// 响应CheckUpdate接口调用请求
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ret__"></param>
        /// <param name="list"></param>
        /// <param name="newSerialNumber"></param>
        /// <param name="error"></param>
        private void ResponseCheckUpdate(ASSISTUPDATEMODULEICE.AMD_IUpdate_CheckUpdate response,bool ret__, ASSISTUPDATEMODULEICE.ResourceItem[] list, long newSerialNumber, string error) 
        {
            if (response != null) 
            {
                response.ice_response(ret__, list, newSerialNumber, error);
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateCallProcessEntity::ResponseCheckUpdate()=>响应对CheckUpdate()接口的调用请求,处理结果:{0},错误消息:{1}", ret__.ToString(), error != null ? error : "无");
            }
        }

        /// <summary>
        /// 查询资源
        /// </summary>
        private void QueryResource() 
        {

            if (this.mResponse == null)
                return;
            if (!(this.mResponse is ASSISTUPDATEMODULEICE.AMD_IUpdate_QueryResource))
                return;

            ASSISTUPDATEMODULEICE.AMD_IUpdate_QueryResource response = this.mResponse as ASSISTUPDATEMODULEICE.AMD_IUpdate_QueryResource;

            if (this.mInterfaceInvoke == null || this.mInterfaceInvoke.Params == null)
            {
                ResponseQueryResource(response, false, 0, "参数无效");
                return;
            }
            if (this.mInterfaceInvoke.Method != InvokeMethod.QueryResource)
            {
                
                ResponseQueryResource(response, false, 0, "参数无效");

                return;
            }
           
            ResourceItem requestResourceItem = this.mInterfaceInvoke.Params.Arg2;//获取客户端请求资源参数


            //组件名称
            string componentName = this.mInterfaceInvoke.Params.Arg0 == UpdateType.Client ? "ShareFileUpdateHandler" : "ShareFileUpdateHandler_Server";
            ShareFileUpdateHandler updateHandler = SysComponents.GetComponent(componentName) as ShareFileUpdateHandler;
            if (updateHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::QueryResource()=>获取查询组件{0}失败", componentName);
                ResponseQueryResource(response, false, 0, string.Format("获取查询组件{0}失败", componentName));
                return;
            }

            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null || string.IsNullOrWhiteSpace(sysConfig.mDownloadTmpDir))
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::QueryResource()=>获取查询组件SysConfig相关信息失败");

                ResponseQueryResource(response, false, 0, "获取查询组件SysConfig相关信息失败");
                return;
            }

            
            //生成系统临时文件
            string fileDir = string.Empty;
            long id = Helper.GetID();
            string fileName = string.Empty;
            string error;
            //组合路径
            fileName = id.ToString() + ".dat";
            fileDir = System.IO.Path.Combine(sysConfig.mDownloadTmpDir, fileName);

            //下载资源
            MergeResourcItem item=new MergeResourcItem(){ mResourceFolderName=requestResourceItem.resourceFolderName, mResourcePath=requestResourceItem.resourcePath};
            if (!updateHandler.DownloadResource(fileDir, item, out error)) 
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::QueryResource()下载资源失败=>{0}", error);
                ResponseQueryResource(response, false, 0, error);
                return;
            }

            //响应客户端
            ResponseQueryResource(response, true, id, error);

        }

        /// <summary>
        /// 响应QueryResource接口调用请求
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ret__"></param>
        /// <param name="queryId"></param>
        /// <param name="error"></param>
        private void ResponseQueryResource(ASSISTUPDATEMODULEICE.AMD_IUpdate_QueryResource response,bool ret__, long queryId, string error)
        {
            if(response!=null)
            {
                response.ice_response(ret__,queryId,error);
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateCallProcessEntity::ResponseQueryResource()=>响应对QueryResource()接口的调用请求,处理结果:{0},错误消息:{1}", ret__.ToString(), error != null ? error : "无");
            }
        }


        private const int MAXUNITLEN = 1024 * 1024 * 10;//10MB
        /// <summary>
        /// 获取资源
        /// </summary>
        private void GetResource()
        {
            if (this.mResponse == null)
                return;
            if (!(this.mResponse is ASSISTUPDATEMODULEICE.AMD_IUpdate_GetResource))
                return;
            ASSISTUPDATEMODULEICE.AMD_IUpdate_GetResource response = this.mResponse as ASSISTUPDATEMODULEICE.AMD_IUpdate_GetResource;


            if (this.mInterfaceInvoke == null || this.mInterfaceInvoke.Params == null)
            {
               
                ResponseGetResource(response, false, null, "参数无效");
                return;
            }
            if (this.mInterfaceInvoke.Method != InvokeMethod.GetResource)
            {
                
                ResponseGetResource(response, false, null, "参数无效");
                return;
            }

            long queryId = this.mInterfaceInvoke.Params.Arg1;//获取客户端请求资源参数
            int from = this.mInterfaceInvoke.Params.Arg3;//获取客户端请求资源参数
            int count = this.mInterfaceInvoke.Params.Arg4;//获取客户端请求资源参数

            if(count>MAXUNITLEN)
            {
              
                ResponseGetResource(response, false, null, "请求的文件过长，单次不能超过10MB");

                return;
            }


            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null || string.IsNullOrWhiteSpace(sysConfig.mDownloadTmpDir))
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::GetResource()=>获取查询组件SysConfig相关信息失败");

                ResponseGetResource(response, false, null, "获取查询组件SysConfig相关信息失败");
                return;
            }

            string fileDir = string.Empty;
            string fileName = string.Empty;
            string error;
            //组合路径
            fileName = queryId.ToString() + ".dat";
            fileDir = System.IO.Path.Combine(sysConfig.mDownloadTmpDir, fileName);

            if (!System.IO.File.Exists(fileDir)) 
            {
                
                ResponseGetResource(response,false, null, "指定的文件不存在");

                return;
            }

            byte[] buffer = null;
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(fileDir, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    if (fs.Length < from)
                    {
                        buffer = new byte[0];
                    }
                    else 
                    {
                        if ((fs.Length - from) > count)
                        {
                            buffer = new byte[count];
                        }
                        else
                        {
                            buffer = new byte[fs.Length - from];
                        }
                        fs.Position = from;
                        int ret_count = fs.Read(buffer, 0, buffer.Length);
                    }
                  

                }
            }
            catch (Exception ex)
            {
                buffer = null;
                error = ex.Message;
                
                ResponseGetResource(response, false, buffer, error);

                return;
            }

            ResponseGetResource(response, true, buffer, null);
        }

        /// <summary>
        /// 响应对GetResource接口的调用请求
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ret__"></param>
        /// <param name="data"></param>
        /// <param name="error"></param>
        private void ResponseGetResource(ASSISTUPDATEMODULEICE.AMD_IUpdate_GetResource response, bool ret__, byte[] data, string error) 
        {
            if (response != null)
            {
                response.ice_response(ret__, data, error);
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateCallProcessEntity::ResponseGetResource()=>响应对GetResource()接口的调用请求,处理结果:{0},错误消息:{1}", ret__.ToString(), error != null ? error : "无");
            }
        }

        /// <summary>
        /// 关闭资源
        /// </summary>
        private void CloseResource()
        {
            if (this.mResponse == null)
                return;
            if (!(this.mResponse is ASSISTUPDATEMODULEICE.AMD_IUpdate_CloseResource))
                return;

            ASSISTUPDATEMODULEICE.AMD_IUpdate_CloseResource response = this.mResponse as ASSISTUPDATEMODULEICE.AMD_IUpdate_CloseResource;

            if (this.mInterfaceInvoke == null || this.mInterfaceInvoke.Params == null)
            {
                
                ResponseCloseResource(response, false, "参数无效");
                return;
            }
            if (this.mInterfaceInvoke.Method != InvokeMethod.CloseResource)
            {
                
                ResponseCloseResource(response, false, "参数无效");
                return;
            }

            long queryId = this.mInterfaceInvoke.Params.Arg1;//获取客户端请求资源参数

            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null || string.IsNullOrWhiteSpace(sysConfig.mDownloadTmpDir))
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateCallProcessEntity::GetResource()=>获取查询组件SysConfig相关信息失败");

                
                ResponseCloseResource(response, false, "获取查询组件SysConfig相关信息失败");
                return;
            }

            string fileDir = string.Empty;
            string fileName = string.Empty;
            string error;
            //组合路径
            fileName = queryId.ToString() + ".dat";
            fileDir = System.IO.Path.Combine(sysConfig.mDownloadTmpDir, fileName);

            if (!System.IO.File.Exists(fileDir))
            {
                
                ResponseCloseResource(response, false, "指定的文件不存在");
                return;
            }
            try
            {
                System.IO.File.Delete(fileDir);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                
                ResponseCloseResource(response, false, error);
                return;
            }
            
            ResponseCloseResource(response, true, null);
        }


        /// <summary>
        /// 响应对CloseResource接口的调用请求
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ret__"></param>
        /// <param name="data"></param>
        /// <param name="error"></param>
        private void ResponseCloseResource(ASSISTUPDATEMODULEICE.AMD_IUpdate_CloseResource response, bool ret__, string error)
        {
            if (response != null)
            {
                response.ice_response(ret__, error);
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateCallProcessEntity::ResponseCloseResource()=>响应对CloseResource()接口的调用请求,处理结果:{0},错误消息:{1}", ret__.ToString(), error != null ? error : "无");
            }
        }

    }

    /// <summary>
    /// 调用处理工作者
    /// [线程类]
    /// </summary>
    public sealed class IUpdateCallProcessWorker
    {
        private ConcurrentQueue<IUpdateCallProcessEntity> mProcessEntitys = new ConcurrentQueue<IUpdateCallProcessEntity>();//更新调用处理实体对象集合
        private bool mIsStop = true;//停止线程工作
        private int mThreadNumber;//线程编号
        private IUpdateCompled mNotify;//完成通知

        public IUpdateCallProcessWorker(int thdCode, IUpdateCompled notify)
        {
            this.mThreadNumber = thdCode;
            this.mNotify = notify;
        }

        private void Worker(Object state) 
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateCallProcessWorker::Worker()=>{0}号查询线程开始工作", mThreadNumber);
            IUpdateCallProcessEntity processEntity;
            while (!this.mIsStop)
            {
                while (this.mProcessEntitys.TryDequeue(out processEntity) && (!this.mIsStop))
                {
                    if (processEntity != null)
                    {
                       //具体处理
                        processEntity.CallProcess();
                        //发送完成通知
                        if (this.mNotify != null)
                            this.mNotify.UpdateCallCompled(processEntity);
                    }
                }

                System.Threading.Thread.Sleep(10);//间隔10ms进行一次操作
            }
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateCallProcessWorker::Worker()=>{0}号查询线程停止工作", mThreadNumber);
        }



        /// <summary>
        /// 添加IUpdateCallProcessEntity到处理队列中
        /// </summary>
        /// <param name="invoke"></param>
        /// <returns></returns>
        public bool AddIUpdateCallProcessEntity(IUpdateCallProcessEntity processEntity)
        {
            if (processEntity == null) return false;

            this.mProcessEntitys.Enqueue(processEntity);

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
                thd.Name = "IUpdateCallProcessWorker";
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
    /// 更新调用处理线程池 
    /// </summary>
    public sealed class IUpdateCallProcessWorkerPool
    {

        private const int DEFAULTTHREADNUMS = 10;//默认线程数
        private const int MAXTHREADNUMS = 20;//最大线程数
        private int mThreadNums;//线程数
        private IUpdateCompled mNotify;//完成通知对象
        private List<IUpdateCallProcessWorker> mWorkers = new List<IUpdateCallProcessWorker>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadNums">需要创建的IUpdateCallProcessWorker个数</param>
        /// <param name="notify">完成通知对象</param>
        public IUpdateCallProcessWorkerPool(int threadNums, IUpdateCompled notify)
        {
            if (threadNums < 0 || threadNums > MAXTHREADNUMS)
                threadNums = DEFAULTTHREADNUMS;

            this.mThreadNums = threadNums;
            this.mNotify = notify;
            for (int i = 0; i < threadNums; i++)
            {
                mWorkers.Add(new IUpdateCallProcessWorker(i + 1, this.mNotify));
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
        /// <param name="threadId">IUpdateCallProcessWorker对应的线程ID</param>
        /// <returns>具有threadId的IUpdateCallProcessWorker,无效返回NULL</returns>
        public IUpdateCallProcessWorker GetWorker(int threadId)
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
            foreach (IUpdateCallProcessWorker item in mWorkers)
            {
                item.Start();
            }
        }

        /// <summary>
        /// 关闭查询线程池
        /// </summary>
        public void Stop()
        {
            foreach (IUpdateCallProcessWorker item in mWorkers)
            {
                item.Stop();
            }
        }

    }
}
