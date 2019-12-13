using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace HeartBeat
{
    /// <summary>
    /// 心跳客户端
    /// </summary>
    public sealed class HeartBeatClient
    {
        /// <summary>
        /// 心跳客户端
        /// </summary>
        /// <param name="serverIP">心跳服务端IP地址</param>
        /// <param name="port">心跳服务端端口</param>
        public HeartBeatClient(string serverIP,int port)
        {
            this.mServerIP = serverIP;
            this.mPort = port;

            this.mRemoteHost = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(this.mServerIP), this.mPort);
        }

        private string mServerIP;
        private int mPort;
        System.Net.IPEndPoint mRemoteHost;

        private bool mIsStop = true;

        private UdpClient mClient = new UdpClient();


        //private void ThreadWorker(Object stat) 
        //{
        //    while (this.mIsStop) 
        //    {

        //        System.Threading.Thread.Sleep(this.mIntervalOfSecond*1000);
        //    }
        //}

        /// <summary>
        /// 发送心跳数据
        /// </summary>
        /// <param name="hbd"></param>
        public bool SendHeartBeat(HeartBeatData hbd) 
        {
            bool ret = false;
            int heartBeatDataSz = Marshal.SizeOf(typeof(HeartBeatData));
            byte[] buffer = new byte[heartBeatDataSz];

            if (StructConvert.ObjectToBytesEx(ref hbd, buffer, 0, heartBeatDataSz))
            {
                try
                {
                    this.mClient.Send(buffer, buffer.Length, this.mRemoteHost);
                    ret = true;
                }
                catch (Exception ex)
                {
                   // LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "心跳包发送失败:{0}", ex.Message);
                }
            }

           
            buffer = null;

            return ret;
        }

        ///// <summary>
        ///// 开始线程工作
        ///// </summary>
        //public void Start()
        //{
        //    if (this.mIsStop)
        //    {
        //        this.mIsStop = false;
        //        System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.ThreadWorker));
        //        thd.Name = "HeartBeatClientThread";
        //        thd.IsBackground = true;
        //        thd.Start(null);
        //    }
        //}

        ///// <summary>
        ///// 停止线程工作
        ///// </summary>
        //public void Stop()
        //{
        //    if (!this.mIsStop)
        //        this.mIsStop = true;
        //}
    }
}
