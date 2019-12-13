using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TollAssistComm
{
    /// <summary>
    /// 升级代理包装类
    /// 此类用于包装相关ICE异常信息
    /// </summary>
    public sealed class IUpdateProxyWrapper
    {
        private ASSISTUPDATEMODULEICE.IUpdatePrx mPrx;

        public IUpdateProxyWrapper(ASSISTUPDATEMODULEICE.IUpdatePrx prx)
        {
            this.mPrx = prx;
        }

        public bool CheckUpdate(long localSerialNumber, ASSISTUPDATEMODULEICE.UpdateType type, out ASSISTUPDATEMODULEICE.ResourceItem[] list, out long newSerialNumber, out string error) 
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                list = null;
                newSerialNumber = 0;
                return false;
            }

            try
            {
                return this.mPrx.CheckUpdate(localSerialNumber, type, out list, out newSerialNumber, out error);
            }
            catch (Ice.Exception ex) 
            {
                error = ex.Message;
                list = null;
                newSerialNumber = 0;
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                list = null;
                newSerialNumber = 0;
                return false;
              
            }
        }


        public bool QueryResource(ASSISTUPDATEMODULEICE.UpdateType type, ASSISTUPDATEMODULEICE.ResourceItem item, out long queryId, out string error) 
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                queryId = 0;
                return false;
            }

            try
            {
                return this.mPrx.QueryResource(type, item, out queryId, out error);
            }
            catch (Ice.Exception ex)
            {

                error = ex.Message;
                queryId = 0;
                return false;
            }
            catch (Exception ex) 
            {
                error = ex.Message;
                queryId = 0;
                return false;
            }
           
        }


        public bool GetResource(long queryId, int from, int count, out byte[] data, out string error) 
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                data = null;
                return false;
            }
            try
            {
              return  this.mPrx.GetResource(queryId, from, count, out data, out error);
            }
            catch (Ice.Exception ex)
            {
                error = ex.Message;
                data = null;
                return false;

            }
            catch (Exception ex) 
            {
                error = ex.Message;
                data = null;
                return false;
            }
        }


        public bool CloseResource(long queryId, out string error) 
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                
                return false;
            }

            try
            {
                return this.mPrx.CloseResource(queryId, out error);
            }
            catch (Ice.Exception ex)
            {
                error = ex.Message;

                return false;
            }
            catch (Exception ex) 
            {
                error = ex.Message;

                return false;
            }


        }
    }

    
}
