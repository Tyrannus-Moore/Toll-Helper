using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommTools
{
    public static class Helper
    {
        private static int loopVale = 0;
        private static DateTime utcTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 利用时间值+回滚值生成long值
        /// </summary>
        /// <returns></returns>
        public static long GetID()
        {
            int sinceSeconds = (int)((DateTime.Now - utcTime).TotalSeconds);
            int incrementVar = System.Threading.Interlocked.Increment(ref loopVale);
            byte[] mem_value = new byte[sizeof(long)];
            byte[] sinceSeconds_buff = BitConverter.GetBytes(sinceSeconds);
            byte[] incrementVar_buff = BitConverter.GetBytes(incrementVar);
            Array.Copy(sinceSeconds_buff, 0, mem_value, 0, sinceSeconds_buff.Length);
            Array.Copy(incrementVar_buff, 0, mem_value, sinceSeconds_buff.Length, incrementVar_buff.Length);

            return BitConverter.ToInt64(mem_value, 0);
        }

        /// <summary>
        /// 返回从1970-1-1起开始经过的时间毫秒值(UTC)
        /// </summary>
        /// <param name="currTime"></param>
        /// <returns></returns>
        public static long GetDateValue(DateTime currTime)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            TimeSpan span = currTime.Subtract(new TimeSpan(8, 0, 0)) - dt;
            return (long)span.TotalMilliseconds;
        }

        /// <summary>
        /// 返回东八区表示的时间日期对象
        /// </summary>
        /// <param name="currTime">从1970-1-1起开始经过的时间毫秒值(UTC)</param>
        /// <returns></returns>
        public static DateTime GetDateTime(long currTime)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            TimeSpan span = TimeSpan.FromMilliseconds(currTime);
            TimeSpan china_span = new TimeSpan(8, 0, 0);
            TimeSpan total_span = span.Add(china_span);

            return dt.Add(total_span);

        }

        /// <summary>
        /// 获取当前进程的执行路径不包括进程名称
        /// </summary>
        /// <returns>进程路径</returns>
        public static string GetCurrentProcessPath() 
        {

            string fileName=System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            fileName = fileName.Substring(0, fileName.LastIndexOf('\\'));

            return fileName;

        }


     
    }
}
