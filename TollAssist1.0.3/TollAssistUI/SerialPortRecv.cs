using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using TollAssistComm;

namespace TollAssistUI
{
    /// <summary>
    /// 20171218 PM add
    /// 串口接收类
    /// 备注：该类用于接收串口发送过来的车牌号和卡号
    /// </summary>
    public  class SerialPortRecv
    {

        private SerialPort mSerialPort = null;
        private bool mIsStop = false;

        /// <summary>
        /// 使用指定的端口名称、波特率、奇偶校验位、数据位和停止位初始化 SerialPortRecv 类的新实例。
        /// </summary>
        /// <param name="portName">端口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">奇偶校验位</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        public SerialPortRecv(string portName,
                                int baudRate,
                                Parity parity,
                                int dataBits,
         StopBits stopBits)
        {
            this.mSerialPort = new SerialPort(portName, baudRate, parity, dataBits);

           // this.mSerialPort.Encoding = System.Text.Encoding.Default;//采用读取字符串方式

            this.mSerialPort.ReadTimeout = 500;
        }

        
        public bool Start() 
        {
            System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.RecvThreadFunc));
            thd.IsBackground = true;
            thd.Name = "串口接收线程";
            thd.Start(null);
            return true;
        }

        public bool Stop() 
        {
            try
            {
                this.mSerialPort.Close();

                this.mIsStop = true;

            }
            catch (Exception ex)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "SerialPortRecv::Stop()=>关闭串口发生异常:{0},关闭失败", ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 接收串口数据的事件
        /// 第一个参数为车牌号，第二个卡号
        /// </summary>
        public event Action<string,string> RecvEvent;

        private void RecvThreadFunc(Object stat) 
        {
            while (!this.mIsStop)
            {
                try
                {
                    string[] portNames = SerialPort.GetPortNames();
                    if (portNames != null) 
                    {
                        int cnt = portNames.Count(name => { return name == this.mSerialPort.PortName; });
                        if (cnt!=1) 
                        {
                            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "SerialPortRecv::RecvThreadFunc()=>未找到串口名称为{0}的设备,系统将在5秒后重试", this.mSerialPort.PortName);
                            //portNames = null;
                            //GC.Collect();
                            System.Threading.Thread.Sleep(5000);
                            continue;
                        }

                        try

                        {
                            this.mSerialPort.Open();
                        }
                        catch (Exception ex)
                        {
                            LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "SerialPortRecv::RecvThreadFunc()=>打开串口发生异常:{0},系统将在30秒后重试", ex.Message);
                            System.Threading.Thread.Sleep(30000);
                            continue;
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "SerialPortRecv::RecvThreadFunc()=>枚举串口名称异常:{0},系统将在5秒后重试", ex.Message);
                }

                System.Threading.Thread.Sleep(5000);
            }

            //try
            //{
            //    this.mSerialPort.Open();
            //}
            //catch (Exception ex)
            //{
            //    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "SerialPortRecv::RecvThreadFunc()=>打开串口发生异常:{0},启动失败", ex.Message);
            //    return;
            //}

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "SerialPortRecv::RecvThreadFunc()=>串口接收线程启动");
            byte[] buffer = new byte[256];
            int len = 0;
            //string context = null;

            

            while (!this.mIsStop) 
            {
                try
                {
                    //context = null;
                    len = 0;
                    len = this.mSerialPort.Read(buffer, 0, buffer.Length);
                    //context=this.mSerialPort.ReadLine();
                }
                catch (TimeoutException)
                {
                    
                }catch(Exception ex)
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "SerialPortRecv::RecvThreadFunc()=>从串口获取数据发生异常:{0},接收线程终止",ex.Message);
                    break;
                }
                //if (!string.IsNullOrWhiteSpace(context) && this.RecvEvent != null)
                //{
                //    this.RecvEvent(context);
                //}
                if (len > 0)
                {
                    //string context = System.Text.Encoding.UTF8.GetString(buffer, 0, len);
                    //if (!string.IsNullOrWhiteSpace(context) && this.RecvEvent != null)
                    //{
                    //    this.RecvEvent(context);
                    //}
                    CopyData(buffer, 0, len);
                }
            }

            LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "SerialPortRecv::RecvThreadFunc()=>串口接收线程停止");
        }

        //串口数据格式:{卡号-车牌号}
        //串口数据解析相关
        private byte[] mBuffer = new byte[2048];//存放串口发送过来的数据
        private int index = 0;//写mBuffer的开始位置
        private int leftFlag = -1;//查询mBuffer中左大括号('{')的位置

        /// <summary>
        /// 拷贝发送过来的数据到缓冲区,并进行数据解析
        /// </summary>
        /// <param name="tmpBuff">当前接收到的数据</param>
        /// <param name="begin">tmpBuff中数据需要拷贝的开始位置</param>
        /// <param name="end">tmpBuff中数据需要拷贝的结束位置的下一个位置</param>
        private void CopyData(byte[] tmpBuff, int begin, int end) 
        {
            if (begin < 0 || end < 0 || begin >= tmpBuff.Length || end >= tmpBuff.Length || end < begin)
                return;
            if ((end - begin) > this.mBuffer.Length - index) //缓冲区不足
            {
                //清空当前缓冲区的数据并将此次数据丢弃
                this.index = 0;
                this.leftFlag = -1;
                return;
            }

            //拷贝数据
            Array.Copy(tmpBuff, begin, this.mBuffer, this.index, end - begin);
            this.index += (end - begin);

            if (this.leftFlag == -1) //当左大括号未找到时发起找左大括号位置
            {
                this.leftFlag = Array.FindIndex(this.mBuffer, 0,index, p => p == '{');
            }
            int rightFlag = -1;//右大括号位置
            if (this.leftFlag != -1) //左大括号找到后开始找右大括号
            {
                rightFlag = Array.FindIndex(this.mBuffer, this.leftFlag + 1, index - (this.leftFlag + 1), p => p == '}');
            }
            if (this.leftFlag != -1 && rightFlag != -1) //左右括号都找到后则进行数据提取
            {
                //提取数据
                string content = System.Text.Encoding.Default.GetString(this.mBuffer, this.leftFlag, rightFlag - this.leftFlag+1);
                string number,cardId;
                if (ParserNumber(content, out number, out cardId)) 
                {
                    if (this.RecvEvent != null) 
                    {
                        this.RecvEvent(number, cardId);
                    }
                }
                //清空当前缓冲区的数据并将此次数据丢弃
                this.index = 0;
                this.leftFlag = -1;
            }
        }

        /// <summary>
        /// 解析串口返回的卡号和车牌号
        /// </summary>
        /// <param name="inputString">串口数据格式:{卡号-车牌号}</param>
        /// <param name="number">车牌号</param>
        /// <param name="cardId">卡号</param>
        /// <returns>成功返回true</returns>
        private bool ParserNumber(string inputString, out string number, out string cardId) 
        {
            number = null;
            cardId = null;
            if (string.IsNullOrWhiteSpace(inputString))
                return false;

            //串口数据格式:{卡号-车牌号}
            int begin = inputString.IndexOf("{");
            int end = inputString.LastIndexOf("}");
            int flag = inputString.LastIndexOf("-");
            if (begin == -1 || end == -1 || flag ==-1|| begin > end) 
            {
                return false;
            }
            string tmpString = inputString.Substring(begin + 1, end - begin-1);
            string[] strInfo = tmpString.Split('-');
            if (strInfo.Length != 2)
                return false;
            cardId = strInfo[0];
            number = strInfo[1];

            return true;
        }
    }

   
}
