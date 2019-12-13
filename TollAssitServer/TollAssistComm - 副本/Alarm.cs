using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlarmTools
{

    //时钟处理函数
    //param:用户参数
    //alarmId://闹钟Id
    //返回值:如果返回true则继续关注此时钟，如果返回false，取消时钟关注
    public delegate bool AlarmProcess(Object param, int alarmId);

    //时钟内容
    public class AlarmContent
    {
        public AlarmContent()
        {
            times = 0;
            actionTimes = 0;
            aEvent = null;
            param = null;
            alarmId = 0;
        }

        public int times;//时钟，以毫秒为单位
        public int actionTimes;//活动时间
        public AlarmProcess aEvent;//闹钟事件
        public Object param;//用户参数
        public int alarmId;//闹钟Id
    }

    ///<summary>
    ///闹钟(通常只需要使用一个该类的实例)
    ///该类使用内部线程计数器维护多个闹钟时间
    ///注意:闹钟内部计时精度大概为10ms一个单位
    ///</summary>
    public sealed class Alarm
    {
        public Alarm()
        {
            this.mNewThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.AlarmActionFunc));
            this.mNewThread.Name = "Alarm_Thread";
            this.mNewThread.IsBackground = true;
            this.mNewThread.Start(this);
        }

        /// <summary>
        /// 释放闹钟资源
        /// </summary>
        public void Dispose()
        {
            if (!this.isStop)
            {
                this.isStop = true;

                this.mNewThread.Join();
                //WaitForSingleObject(this->mNewThread, INFINITE);
            }
        }

        /// <summary>
        /// 添加闹钟事件
        /// </summary>
        /// <param name="time">以毫秒为单位的闹钟时间</param>
        /// <param name="aEvent">当时间到达时触发的事件</param>
        /// <param name="param">用户参数</param>
        /// <param name="alarmId">输出,作为闹钟标识</param>
        /// <returns></returns>
        public bool AddAlarmEvent(int time, AlarmProcess aEvent, Object param, out int alarmId)
        {
            alarmId = 0;
            if (time == 0 || aEvent == null)
                return false;

            AlarmContent content = new AlarmContent();
            content.times = time;
            content.actionTimes = Environment.TickCount;
            content.aEvent = aEvent;
            content.param = param;
            lock (this.syncLockObj)
            {
                alarmId = this.MakeIndex();
                for (; this.mEvents.ContainsKey(alarmId); alarmId = this.MakeIndex()) ;//寻找一个可用的id
                content.alarmId = alarmId;
                this.mEvents[alarmId] = (content);
            }

            return true;
        }

        /// <summary>
        /// 删除指定的闹钟
        /// 备注:如果指定的时钟已经被触发并且AlarmEvent返回false则闹钟已被删除，此操作将返回失败因为不存在指定的闹钟
        /// </summary>
        /// <param name="alarmId">由AddAlarmEvent返回的值</param>
        /// <returns>成功返回true</returns>
        public bool RemoveAlarm(int alarmId)
        {
            bool rs = false;
            lock (this.syncLockObj)
            {
                if (this.mEvents.ContainsKey(alarmId))
                {
                    rs = this.mEvents.Remove(alarmId);

                }
            }



            return rs;
        }

        /// <summary>
        /// 重置闹钟时间
        /// 备注:如果指定的时钟已经被触发并且AlarmEvent返回false则闹钟已被删除，此操作将返回失败因为不存在指定的闹钟成功返回true
        /// </summary>
        /// <param name="alarmId">将alarmId对应的闹钟时间值重置</param>
        /// <param name="times">闹钟值，如果times==int.MaxValue则闹钟时间值重置为调用AddAlarmEvent函数时传递的time值</param>
        /// <returns>成功返回true</returns>
        public bool ResetAlarm(int alarmId,int times)
        {
            bool rs = false;
            lock (this.syncLockObj)
            {
                if (this.mEvents.ContainsKey(alarmId))
                {
                    if (times != int.MaxValue) 
                    {
                        this.mEvents[alarmId].times = times;
                    }

                    this.mEvents[alarmId].actionTimes = Environment.TickCount;
                    rs = true;
                }
            }

            return rs;
        }

        private static int index = 0;
      
        /// <summary>
        /// 生成标识
        /// </summary>
        /// <returns></returns>
        private int MakeIndex()
        {
            return System.Threading.Interlocked.Increment(ref index);
        }
        private Dictionary<int, AlarmContent> mEvents = new Dictionary<int, AlarmContent>();
        private Object syncLockObj = new object();
        private bool isStop = false;
        private System.Threading.Thread mNewThread;

        
        /// <summary>
        /// 时钟活动函数
        /// </summary>
        /// <param name="arg"></param>
        private void AlarmActionFunc(Object arg)
        {

            int ticks = 0;
            bool isPost = false;
            List<int> removes = new List<int>();
            while (!this.isStop)
            {
                System.Threading.Thread.Sleep(1);//休眠1ms
                removes.Clear();
                lock (this.syncLockObj)
                {
                    foreach (var item in this.mEvents)
                    {
                        ticks = (int)(((Environment.TickCount - item.Value.actionTimes) + 0xFFFFFFFF) % 0xFFFFFFFF);//计算时间差
                        if (ticks >= item.Value.times)//超时
                        {
                            isPost = item.Value.aEvent(item.Value.param, item.Value.alarmId);
                            if (!isPost)//不再关注
                            {
                                removes.Add(item.Key);//添加到待删除
                            }
                            else
                            {
                                item.Value.actionTimes = Environment.TickCount;//继续投递
                            }
                        }
                    }

                    //删除
                    foreach (int key in removes)
                    {
                        this.mEvents.Remove(key);
                    }

                }

            }

        }


    }
}
