using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Net;
using System.Runtime.InteropServices;

namespace HeartBeat
{
    /// <summary>
    /// 心跳服务
    /// </summary>
    public sealed class HeartBeatServer
    {

        public HeartBeatServer(int listenPort, IHeartBeatNote note)
        {
            this.mListenPort = listenPort;
            this.mIHeartBeatNote = note;
        }

        //private const int BUFFSIZE=1024*512;
        //private byte[] mBuffer = new byte[BUFFSIZE];

        private IHeartBeatNote mIHeartBeatNote;//心跳通知
        private int mListenPort;//本地监听端口
        private UdpClient mServer = null;


        private System.Threading.Thread mThread = null;
        private bool mIsStop = false;

        private ConcurrentQueue<HeartBeatData> mHeartBeatDataList = new ConcurrentQueue<HeartBeatData>();//心跳数据结构
        private void CallBackThread(Object stat)
        {
            HeartBeatData hbData;
            while (!this.mIsStop)
            {
                while (this.mHeartBeatDataList.TryDequeue(out hbData) && (!this.mIsStop))
                {
                    //发送通知
                    if (this.mIHeartBeatNote != null)
                        this.mIHeartBeatNote.HeartBeatNote(hbData);
                }

                System.Threading.Thread.Sleep(10);//间隔10ms进行一次操作
            }
        }

        private void Worker(object stat)
        {
            if (this.mServer == null)
                return;

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);//远程主机
            byte[] recvBuff = null;

            int heartBeatDataSz = Marshal.SizeOf(typeof(HeartBeatData));
            HeartBeatData data;//心跳数据结构
            while (!this.mIsStop)
            {
                try
                {
                    recvBuff = this.mServer.Receive(ref RemoteIpEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HeartBeatServer::Worker()=>Receive异常:{0}", ex.Message);
                    break;
                }
                if (recvBuff == null || recvBuff.Length == 0)
                {
                    Console.WriteLine("HeartBeatServer::Worker()=>HeartBeatServer关闭");
                    break;
                }
                if (recvBuff.Length != heartBeatDataSz)
                {
                    Console.WriteLine("HeartBeatServer::Worker()=>收到的数据长度异常，此包丢弃");
                    continue;
                }
                //解析数据包
                if (!StructConvert.ByteArrayToObjectEx(recvBuff, 0, heartBeatDataSz, out data))
                {
                    Console.WriteLine("HeartBeatServer::Worker()=>解析数据失败，此包丢弃");
                    continue;
                }
                //将数据放入后台回调处理线程
                this.mHeartBeatDataList.Enqueue(data);
            }
        }

        /// <summary>
        /// 启动心跳接收服务
        /// </summary>
        /// <returns></returns>
        public bool Start(out string error)
        {
            error = string.Empty;
            if (this.mServer != null)
            {
                error = "服务已经在使用";
                return false;
            }

            try
            {
                this.mServer = new UdpClient(this.mListenPort);
            }
            catch (Exception ex)
            {

                error = ex.Message;
                this.mServer = null;
                return false;
            }

            //启动服务
            this.mThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.Worker));
            this.mThread.IsBackground = true;
            this.mThread.Name = "HeartBeatServer_Worker";
            this.mThread.Start(null);

            //启动心跳回调处理线程
            var thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.CallBackThread));
            thread.IsBackground = true;
            thread.Name = "HeartBeatServer_CallBackThread";
            thread.Start(null);

            return true;

        }

        /// <summary>
        /// 关闭心跳接收服务
        /// </summary>
        /// <returns></returns>
        public bool Stop(out string error)
        {
            error = string.Empty;
            this.mIsStop = true;
            if (this.mServer != null)
            {
                try
                {
                    this.mServer.Close();
                    this.mThread.Join();
                    return true;
                }
                catch (SocketException ex)
                {
                    error = ex.Message;

                }
            }
            return false;
        }

    }
}
