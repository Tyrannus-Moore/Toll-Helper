using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TollAssistComm
{
    /// <summary>
    /// 日志打印包装类
    /// </summary>
    public static class LogerPrintWrapper
    {
        private static LOGCS.CLoger mCLoger = null;
        private static bool mClosePrint = true;


        public static void BindCLoger(LOGCS.CLoger cLoger) 
        {
            mCLoger = cLoger;
            mClosePrint = false;
        }
        /// <summary>
        /// 日志打印
        /// </summary>
        /// <param name="level">需要打印的日志的级别</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">参数</param>
        public static void Print(LOGCS.LogLevel level,string format,params object[] args)
        {
            if (mCLoger == null || mClosePrint)
                return;

            mCLoger.Loger(level, format, args);
        }

        /// <summary>
        /// 关闭日志打印
        /// </summary>
        public static void ClosePrint() 
        {
            mClosePrint = true;
        }


    }
}
