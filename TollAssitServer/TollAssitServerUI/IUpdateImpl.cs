using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASSISTUPDATEMODULEICE;
using System.Collections.Concurrent;
using UpdateModule;
using TollAssistComm;

namespace CommHandler
{
    public sealed class IUpdateImpl : IUpdateDisp_, IUpdateCompled
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParamEntityNums">内部需要缓存的IUpdateCallProcessEntity的个数</param>
        public IUpdateImpl(int ParamEntityNums)
        {
            if (ParamEntityNums < 0 || ParamEntityNums > MAXWORKEROBJNUMS)
                ParamEntityNums = DEFAULTWORKEROBJNUMS;

            //初始化缓冲区
            for (int i = 0; i < ParamEntityNums; i++)
            {
                this.mWorkers.Enqueue(new IUpdateCallProcessEntity());
            }
        }

        private ConcurrentQueue<IUpdateCallProcessEntity> mWorkers = new ConcurrentQueue<IUpdateCallProcessEntity>();//存放号码查询对象的队列 
        private const int DEFAULTWORKEROBJNUMS = 1000;//默认缓冲区中对象个数
        private const int MAXWORKEROBJNUMS = 100000;//最大缓冲区对象个数

        public override void CheckUpdate_async(AMD_IUpdate_CheckUpdate cb__, long localSerialNumber, UpdateType type, Ice.Current current__)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateImpl::CheckUpdate_async()=>收到检查更新请求,localSerialNumber:{0} UpdateType:{1}", localSerialNumber, type.ToString());

            if (cb__ == null) 
                return;

            string error;
            IUpdateCallProcessEntity processEntity = null;
            if (this.mWorkers.TryDequeue(out processEntity) && processEntity != null)
            {
                processEntity.SetParams(InvokeMethod.CheckUpdate,type, localSerialNumber,null,0,0,cb__);

                IUpdateCallProcessWorkerPool iUpdateCallProcessWorkerPool = SysComponents.GetComponent("IUpdateCallProcessWorkerPool") as IUpdateCallProcessWorkerPool;
                if (iUpdateCallProcessWorkerPool == null)
                {
                    error = "获取查询组件IUpdateCallProcessWorkerPool失败";
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateImpl::CheckUpdate_async()=>{0}", error);

                    this.UpdateCallCompled(processEntity);
                    cb__.ice_response(false, null, 0,error);//立即返回结果
                }
                else
                {
                    //利用hash方式分发到不同的处理线程
                    IUpdateCallProcessWorker iUpdateCallProcessWorker = iUpdateCallProcessWorkerPool.GetWorker(DateTime.Now.Millisecond % iUpdateCallProcessWorkerPool.GetWorkerCount());
                    if (!iUpdateCallProcessWorker.AddIUpdateCallProcessEntity(processEntity))
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateImpl::CheckUpdate_async()=>{0}", "将processEntity添加到查询线程失败");

                        this.UpdateCallCompled(processEntity);
                        cb__.ice_response(false, null, 0,"将processEntity添加到查询线程失败");//立即返回结果

                    }
                }


            }
            else
            {
                cb__.ice_response(false, null, 0,"没有可用的IUpdateCallProcessEntity对象");
            }
        }

        public override void QueryResource_async(AMD_IUpdate_QueryResource cb__, UpdateType type, ResourceItem item, Ice.Current current__)
        {

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateImpl::QueryResource_async()=>收到查询资源请求,UpdateType:{0}",type.ToString());

            if (cb__ == null)
                return;
            string error;
            IUpdateCallProcessEntity processEntity = null;
            if (this.mWorkers.TryDequeue(out processEntity) && processEntity != null)
            {
                processEntity.SetParams(InvokeMethod.QueryResource, type, 0,item,0,0, cb__);

                IUpdateCallProcessWorkerPool iUpdateCallProcessWorkerPool = SysComponents.GetComponent("IUpdateCallProcessWorkerPool") as IUpdateCallProcessWorkerPool;
                if (iUpdateCallProcessWorkerPool == null)
                {
                    error = "获取查询组件IUpdateCallProcessWorkerPool失败";
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateImpl::QueryResource_async()=>{0}", error);

                    this.UpdateCallCompled(processEntity);
                    cb__.ice_response(false, 0, error);//立即返回结果
                }
                else
                {
                    //利用hash方式分发到不同的处理线程
                    IUpdateCallProcessWorker iUpdateCallProcessWorker = iUpdateCallProcessWorkerPool.GetWorker(DateTime.Now.Millisecond % iUpdateCallProcessWorkerPool.GetWorkerCount());
                    if (!iUpdateCallProcessWorker.AddIUpdateCallProcessEntity(processEntity))
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateImpl::QueryResource_async()=>{0}", "将processEntity添加到查询线程失败");

                        this.UpdateCallCompled(processEntity);
                        cb__.ice_response(false, 0, "将processEntity添加到查询线程失败");//立即返回结果

                    }
                }


            }
            else
            {
                cb__.ice_response(false, 0, "没有可用的IUpdateCallProcessEntity对象");
            }

        }

        public override void GetResource_async(AMD_IUpdate_GetResource cb__, long queryId, int from, int count, Ice.Current current__)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateImpl::GetResource_async()=>收到获取资源请求,queryId:{0} from:{1} count:{2}", queryId,from,count);

            if (cb__ == null)
                return;
            string error;
            IUpdateCallProcessEntity processEntity = null;
            if (this.mWorkers.TryDequeue(out processEntity) && processEntity != null)
            {
                processEntity.SetParams(InvokeMethod.GetResource, UpdateType.Other, queryId, null,from,count, cb__);

                IUpdateCallProcessWorkerPool iUpdateCallProcessWorkerPool = SysComponents.GetComponent("IUpdateCallProcessWorkerPool") as IUpdateCallProcessWorkerPool;
                if (iUpdateCallProcessWorkerPool == null)
                {
                    error = "获取查询组件IUpdateCallProcessWorkerPool失败";
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateImpl::GetResource_async()=>{0}", error);

                    this.UpdateCallCompled(processEntity);
                    cb__.ice_response(false, null, error);//立即返回结果
                }
                else
                {
                    //利用hash方式分发到不同的处理线程
                    IUpdateCallProcessWorker iUpdateCallProcessWorker = iUpdateCallProcessWorkerPool.GetWorker(DateTime.Now.Millisecond % iUpdateCallProcessWorkerPool.GetWorkerCount());
                    if (!iUpdateCallProcessWorker.AddIUpdateCallProcessEntity(processEntity))
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateImpl::GetResource_async()=>{0}", "将processEntity添加到查询线程失败");

                        this.UpdateCallCompled(processEntity);
                        cb__.ice_response(false, null, "将processEntity添加到查询线程失败");//立即返回结果

                    }
                }


            }
            else
            {
                cb__.ice_response(false, null, "没有可用的IUpdateCallProcessEntity对象");
            }
        }

        public override void CloseResource_async(AMD_IUpdate_CloseResource cb__, long queryId, Ice.Current current__)
        {
            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "IUpdateImpl::CloseResource_async()=>收到关闭资源请求,queryId:{0}", queryId);

            if (cb__ == null)
                return;
            string error;
            IUpdateCallProcessEntity processEntity = null;
            if (this.mWorkers.TryDequeue(out processEntity) && processEntity != null)
            {
                processEntity.SetParams(InvokeMethod.CloseResource, UpdateType.Other, queryId, null, 0, 0, cb__);

                IUpdateCallProcessWorkerPool iUpdateCallProcessWorkerPool = SysComponents.GetComponent("IUpdateCallProcessWorkerPool") as IUpdateCallProcessWorkerPool;
                if (iUpdateCallProcessWorkerPool == null)
                {
                    error = "获取查询组件IUpdateCallProcessWorkerPool失败";
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateImpl::CloseResource_async()=>{0}", error);

                    this.UpdateCallCompled(processEntity);
                    cb__.ice_response(false, error);//立即返回结果
                }
                else
                {
                    //利用hash方式分发到不同的处理线程
                    IUpdateCallProcessWorker iUpdateCallProcessWorker = iUpdateCallProcessWorkerPool.GetWorker(DateTime.Now.Millisecond % iUpdateCallProcessWorkerPool.GetWorkerCount());
                    if (!iUpdateCallProcessWorker.AddIUpdateCallProcessEntity(processEntity))
                    {
                        LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "IUpdateImpl::CloseResource_async()=>{0}", "将processEntity添加到查询线程失败");

                        this.UpdateCallCompled(processEntity);
                        cb__.ice_response(false, "将processEntity添加到查询线程失败");//立即返回结果

                    }
                }


            }
            else
            {
                cb__.ice_response(false, "没有可用的IUpdateCallProcessEntity对象");
            }
        }

        public void UpdateCallCompled(IUpdateCallProcessEntity processEntity)
        {
            if (processEntity != null)
                this.mWorkers.Enqueue(processEntity);
        }
    }
}
