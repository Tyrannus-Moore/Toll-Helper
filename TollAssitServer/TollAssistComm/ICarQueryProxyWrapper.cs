using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 using System.IO;






namespace TollAssistComm
{
    /// <summary>
    /// 车辆查询代理包装类，此类主要处理ICE相关异常信息
    /// </summary>
    public sealed class ICarQueryProxyWrapper
    {

       //**************************************************************************************
            public static void WriteLog(string strLog)
            {
              string sFilePath=".\\";
              string sFileName = "DaTaoRiZhi"  + ".log";
              sFileName = sFilePath+ "\\"+sFileName; //文件的绝对路径
              if (!Directory.Exists(sFilePath))//验证路径是否存在
              {
                Directory.CreateDirectory(sFilePath);
                //不存在则创建
              }
              FileStream fs;      
              StreamWriter sw;
              if (File.Exists(sFileName))
              //验证文件是否存在，有则追加，无则创建
              {
                fs = new FileStream(sFileName, FileMode.Append, FileAccess.Write);
              }
              else
              {
                fs = new FileStream(sFileName, FileMode.Create, FileAccess.Write);
              }
              sw = new StreamWriter(fs);
              sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   ---   " + strLog);
              sw.Close();
              fs.Close();    
            }
            //**************************************************************************************


        private ASSISTICE.ICarQueryPrx mPrx;

        public ICarQueryProxyWrapper(ASSISTICE.ICarQueryPrx prx)
        {
            this.mPrx = prx;
        }

        //flag:串口传递过来的除车牌号之外的字段信息
        public bool QueryCarRecord(string platte,string flag, ASSISTICE.TollNode node, out ASSISTICE.CarTable[] record, out string error)
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                record = null;
                return false;
            }

            try
            {
                return this.mPrx.QueryCarRecord(platte,flag, node, out record, out error);
            }
            catch (Ice.Exception ex)
            {
                error = ex.Message;
                record = null;
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                record = null;
                return false;
            }

        }

        /// <summary>
        /// 推荐使用
        /// </summary>
        /// <param name="cb__"></param>
        /// <param name="platte"></param>
        /// <param name="flag">串口传递过来的除车牌号之外的字段信息</param>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool QueryCarRecord_async(ASSISTICE.AMI_ICarQuery_QueryCarRecord cb__, string platte,string flag, ASSISTICE.TollNode node)
        {

            //修改时间4-4,vincent
            WriteLog("接受串口消息");



            if (this.mPrx == null)
            {
                return false;
            }

            try
            {
                return this.mPrx.QueryCarRecord_async(cb__, platte,flag, node);
            }
            catch (Ice.Exception ex)
            {

                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool BatchQuery(ASSISTICE.BatchQueryParams[] querys, out string error)
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                querys = null;
                return false;
            }

            try
            {
                return this.mPrx.BatchQuery(querys, out error);
            }
            catch (Ice.Exception ex)
            {
                error = ex.Message;
                querys = null;
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                querys = null;
                return false;
            }
        }

        public bool BatchUpload(ASSISTICE.CustomRecord[] records, out string error)
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                records = null;
                return false;
            }

            try
            {
                return this.mPrx.BatchUpload(records, out error);
            }
            catch (Ice.Exception ex)
            {
                error = ex.Message;
                records = null;
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                records = null;
                return false;
            }
        }

        public bool QueryStations(int from, int count, out ASSISTICE.Station[] lst, out string error)
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                lst = null;
                return false;
            }

            try
            {
                return this.mPrx.QueryStations(from, count, out lst, out error);
            }
            catch (Ice.Exception ex)
            {
                error = ex.Message;
                lst = null;
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                lst = null;
                return false;
            }
        }

        /// <summary>
        /// 上传节点信息
        /// </summary>
        /// <param name="flag">节点唯一标识</param>
        /// <param name="node">节点信息</param>
        /// <returns></returns>
        public bool UploadTollNode(string flag, ASSISTICE.TollNode node,out string error) 
        {
            if (this.mPrx == null)
            {
                error = "代理对象不存在";
                return false;
            }

            try
            {
                return this.mPrx.UploadTollNode(flag, node, out error);
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
