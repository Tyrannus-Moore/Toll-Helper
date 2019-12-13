using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CSharpPerformance;

namespace TollAssistComm
{
    /// <summary>
    /// 进程资源监视通知接口
    /// </summary>
    public interface IProcessResourceMonitor 
    {
        /// <summary>
        /// 进程资源占用情况
        /// </summary>
        /// <param name="sysCPURate">系统CPU占用比[0~1]之间的值</param>
        /// <param name="sysMemRate">系统内存占用比[0~1]之间的值</param>
        /// <param name="processCPURate">当前进程CPU占用比[0~1]之间的值</param>
        /// <param name="processMemRate">当前进程内存占用比[0~1]之间的值</param>
        void ProcessResource(double sysCPURate,double sysMemRate,double processCPURate,double processMemRate);
    }

    /// <summary>
    /// 进程资源情况监视类
    /// </summary>
    public sealed class ProcessResourceMonitorThread
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">监视间隔时间，以秒为单位</param>
        /// <param name="note">监视通知对象</param>
        public ProcessResourceMonitorThread(int interval, IProcessResourceMonitor note)
        {
            if (interval <= 0)
                interval = 1;

            this.mIntervalOfSecond = interval;
            this.mIProcessResourceMonitor = note;
        }



        /// <summary>
        /// 开始线程工作
        /// </summary>
        public void Start()
        {
            if (this.mIsStop)
            {
                this.mIsStop = false;
                thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.ThreadWorker));
                thd.Name = "ProcessResourceMonitorThread";
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

        /// <summary>
        /// 等待线程退出
        /// 超时时间毫秒
        /// </summary>
        public void WaitThreadExit(int timeout) 
        {
            this.thd.Join(timeout);
        }


        private int mIntervalOfSecond;
        private IProcessResourceMonitor mIProcessResourceMonitor;

        private System.Threading.Thread thd = null;

        private bool mIsStop = true;//是否停止监视

        private void ThreadWorker(Object stat) 
        {

            //获取当前进程对象
            Process cur = Process.GetCurrentProcess();

           // PerformanceCounter curpcp = new PerformanceCounter("Process", "Working Set - Private", cur.ProcessName);
            PerformanceCounter curpc = new PerformanceCounter("Process", "Working Set", cur.ProcessName);
           // PerformanceCounter curtime = new PerformanceCounter("Process", "% Processor Time", cur.ProcessName);

            //上次记录CPU的时间
            TimeSpan prevCpuTime = TimeSpan.Zero;
           
            PerformanceCounter totalcpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            SystemInfo sys = new SystemInfo();

            int interval = this.mIntervalOfSecond;//时间间隔(秒)
            int interval_count = 0;//时间计数器

            double processCPURate;//进程CPU使用率 
            double processMemRate;//进程的内存占用率

            double sysCPURate;//系统CPU占用率
            double sysMemRate;//系统内存占用率

            while (!this.mIsStop) 
            {

                System.Threading.Thread.Sleep(1000);
                interval_count++;
                if (interval_count == interval) //时间超时
                {
                    interval_count = 0;//计数器归0

                    //当前时间
                    TimeSpan curCpuTime = cur.TotalProcessorTime;
                    //计算当前进程的CPU使用情况
                    processCPURate = (curCpuTime - prevCpuTime).TotalMilliseconds / (interval*1000) / Environment.ProcessorCount;
                    prevCpuTime = curCpuTime;

                    //计算进程的内存占用率
                    processMemRate = curpc.NextValue() / sys.PhysicalMemory;

                    //计算系统CPU占用率
                    sysCPURate = totalcpu.NextValue();

                    //计算系统内存使用率
                    sysMemRate = (sys.PhysicalMemory - sys.MemoryAvailable) / (double)sys.PhysicalMemory;

                    //发送通知
                    if (this.mIProcessResourceMonitor != null)
                        this.mIProcessResourceMonitor.ProcessResource(sysCPURate, sysMemRate, processCPURate, processMemRate);
                }
            }

        }
    }
}
