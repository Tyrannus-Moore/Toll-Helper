using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HeartBeat
{
    /// <summary>
    /// 心跳标记
    /// </summary>
    public enum HeartBeatFlag 
    {
        /// <summary>
        /// 默认方式，使用心跳
        /// </summary>
        None=0,
        /// <summary>
        /// 挂起监视(暂时不监视),挂起的时间由服务端决定
        /// </summary>
        Suspend=1,
        /// <summary>
        /// 恢复监视(继续监视)
        /// </summary>
        Resume=2,
    }

    /// <summary>
    /// 心跳交换数据结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HeartBeatData
    {
        /// <summary>
        /// 进程ID
        /// </summary>
        public int ProcessId;
        /// <summary>
        /// 内存使用量(百分比)
        /// </summary>
        public double MemoryUsageRate;

        /// <summary>
        /// CPU使用量(百分比)
        /// </summary>
        public double CpuUsageRate;

        /// <summary>
        /// 心跳间隔时间(单位秒)
        /// </summary>
        public int HeartBeatInterval;

        /// <summary>
        /// 心跳标记
        /// </summary>
        public HeartBeatFlag Flag;
    }


    /// <summary>
    /// 心跳通知接口
    /// </summary>
    public interface IHeartBeatNote
    {
        /// <summary>
        /// 心跳通知
        /// </summary>
        /// <param name="data"></param>
        void HeartBeatNote(HeartBeatData data);
    }
}
