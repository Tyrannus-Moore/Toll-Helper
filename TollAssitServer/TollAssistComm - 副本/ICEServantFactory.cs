using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
//using Ice;
namespace Ice_Servant_Factory
{

    /// <summary>
    /// ICE服务端对象包装类;
    /// 对于每一个实现服务的对象都要求使用该包装类
    /// </summary>
    public class BaseICEObjectWrapper
    {
        /// <summary>
        /// ICE服务端对象实例基类
        /// </summary>
        /// <param name="_object">实际的服务端对象</param>
        public BaseICEObjectWrapper(Ice.Object _object)
        {
            this.mCommPtr = null;
            this.mObject = _object;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Destroy()
        {
            if (this.mCommPtr != null)
            {
                try
                {
                    if (!this.mCommPtr.isShutdown())
                        this.mCommPtr.shutdown();

                    this.mCommPtr.destroy();

                }
                catch (Exception)
                {

                }
                mCommPtr = null;
            }
        }
        //inline virtual ~IICEObject()
        //{
        //    if(mCommPtr)
        //    {
        //        try
        //        {
        //            if(mCommPtr)
        //            {
        //                if(!mCommPtr->isShutdown())
        //                    mCommPtr->shutdown();
        //                mCommPtr->destroy();
        //            }

        //        }catch(...)
        //        {

        //        }
        //        mCommPtr=NULL;
        //    }
        //}

        /// <summary>
        /// 关闭服务端servant
        /// </summary>
        public virtual void Shutdown()
        {
            if (this.mCommPtr != null)
            {
                this.mCommPtr.shutdown();
            }
        }
        public virtual Ice.Communicator mCommPtr { get; set; }
        public virtual Ice.Object mObject { get; set; }

    }

    /// <summary>
    /// 客户端代理对象包装类
    /// </summary>
    public sealed class ClientProxyWrapper<T>
    {
        public Ice.Communicator mCommPtr { get; set; }
        /// <summary>
        /// 客户端代理对象
        /// </summary>
        public T prx { get; set; } //Ice.ObjectPrx
    }

    /// <summary>
    /// 负责生成ICE服务端和客户端
    /// </summary>
    public static class ICEServantFactory
    {
        /// <summary>
        /// 该函数调用为阻塞模式,直到impl释放为止,通常在一个新启动的线程中调用该方法
        /// </summary>
        /// <param name="propFile">ICE相关属性配置文件(全路径),要求路径字符串为UTF8编码且文件内容也必须为UTF8-BOM编码</param>
        /// <param name="servant">对外提供的服务信息(eg:HW_ICE_SERVICE:tcp -h 192.168.31.4 -p 1234)</param>
        /// <param name="impl">服务端实现类</param>
        /// <param name="error">错误信息</param>
        /// <returns>成功返回true</returns>
        public static bool BuildService(string propFile, string servant, ref Ice_Servant_Factory.BaseICEObjectWrapper impl, out string error)
        {
            error = string.Empty;
            try
            {
                Ice.Properties props = Ice.Util.createProperties();
                props.load(propFile);
                Ice.InitializationData id = new Ice.InitializationData();
                id.properties = props;
                impl.mCommPtr = Ice.Util.initialize(id);

                int index = servant.IndexOf(':');
                if (index < 1)
                {
                    error = ("servant格式错误");
                    return false;
                }
                string serName = servant.Substring(0, index);
                string prot = servant.Substring(index + 1);
                Ice.ObjectAdapter adapter = impl.mCommPtr.createObjectAdapterWithEndpoints(serName + "Adapter", prot);
                Ice.Object _object = impl.mObject;//new DeviceServiceHandler(this);
                adapter.add(_object, impl.mCommPtr.stringToIdentity(serName));

                adapter.activate();
                impl.mCommPtr.waitForShutdown();
                return true;

            }
            catch (Ice.Exception e)
            {
                error += ("ICEServantFactory::StartService异常:");
                error += (e.Message);
                return false;
            }
        }

        /// <summary>
        /// 该函数用于获取客户端代理对象包装类
        /// T 表示客户端代理对象
        /// </summary>
        /// <param name="propFile">ICE相关属性配置文件(全路径),要求路径字符串为UTF8编码且文件内容也必须为UTF8-BOM编码</param>
        /// <param name="servant">对外提供的服务信息(eg:HW_ICE_SERVICE:tcp -h 192.168.31.4 -p 1234)</param>
        /// <param name="proxy">客户端代理对象包装类</param>
        /// <param name="error">错误信息</param>
        /// <returns>成功返回true</returns>
        public static bool GetProxy<T>(string propFile, string servant, ref Ice_Servant_Factory.ClientProxyWrapper<T> proxy, out string error)
        {
            Ice.Communicator comptr = null;
            error = string.Empty;
            try
            {
                Ice.Properties props = Ice.Util.createProperties();
                props.load(propFile);
                Ice.InitializationData id = new Ice.InitializationData();
                id.properties = props;
                comptr = Ice.Util.initialize(id);
                Ice.ObjectPrx b = comptr.stringToProxy(servant);

                Type proxy_type=typeof(T);
                string prxHelper;
                string fullName=proxy_type.FullName;
                //找出xxPrxHelper
                int index=fullName.LastIndexOf(".");
                prxHelper = fullName.Substring(0, index) + fullName.Substring(index)+"Helper";
                Type prxHelper_type= Type.GetType(prxHelper);
                MethodInfo methodInfo = prxHelper_type.GetMethod("checkedCast", new[] { typeof(Ice.ObjectPrx) });
                if (methodInfo != null) 
                {
                    proxy.prx = (T)methodInfo.Invoke(null, new object[] {b});
                } 
               //T.checkedCast(b);
                if (proxy.prx != null)
                {
                    proxy.mCommPtr = comptr;
                }
                return proxy.prx != null;

            }
			catch (TargetInvocationException e) 
            {
                error += ("ICEServantFactory::GetProxy异常:");
                error += (e.InnerException.InnerException.Message);
                return false;
            }
            catch (Ice.Exception e)
            {
                error += ("ICEServantFactory::GetProxy异常:");
                error += (e.Message);
                try
                {
                    if (comptr != null)
                        comptr.destroy();
                }
                catch (Exception ex)
                {

                }
                return false;
            }
        }


    }


}
