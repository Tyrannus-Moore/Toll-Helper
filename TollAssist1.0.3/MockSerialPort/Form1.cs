using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace MockSerialPort
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();
            if (portNames != null&&portNames.Length>0) 
            {
                this.cboPortNames.Items.AddRange(portNames);
                this.cboPortNames.SelectedIndex = 0;
            }
            

            this.btnOpen.Enabled = true;
            this.btnClose.Enabled = false;
            this.btnSend.Enabled = false;
        }

        private SerialPort mSerialPort = null;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.cboPortNames.Text)) 
            {
                AppendTextToLog("串口设备不能为空!");
                return;
            }
            if (this.mSerialPort == null) 
            {
                try
                {
                    this.mSerialPort = new SerialPort(this.cboPortNames.Text, 9600, Parity.None, 8, StopBits.One);
                   // this.mSerialPort.Encoding = System.Text.Encoding.UTF8;//采用发送字符串方式
                    this.mSerialPort.Open();
                    string args = string.Format("串口名称:{0} 波特率:{1} 校验位:{2} 数据位:{3} 停止位:{4}", this.cboPortNames.Text, 9600, "无", 8, "一个");
                    AppendTextToLog(string.Format("打开串口设备成功,串口相关参数：{0}", args));

                    this.btnOpen.Enabled = false;
                    this.btnClose.Enabled = true;
                    this.btnSend.Enabled = true;
                }
                catch (Exception ex)
                {
                    AppendTextToLog(string.Format("打开串口设备异常:{0}",ex.Message));
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (this.mSerialPort == null)
                return;
            try
            {
                this.mSerialPort.Close();
                this.btnOpen.Enabled = true;
                this.btnClose.Enabled = false;
                this.btnSend.Enabled = false;
                this.mSerialPort = null;
                AppendTextToLog(string.Format("关闭串口设备成功"));
            }
            catch (Exception ex)
            {
                AppendTextToLog(string.Format("关闭串口设备异常:{0}", ex.Message));
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtNumber.Text))
                return;

            try
            {
                string data = string.Format("{{{0}-{1}}}", "1234567890", this.txtNumber.Text);
                byte[] sendBuff = System.Text.Encoding.Default.GetBytes(data);
                this.mSerialPort.Write(sendBuff, 0, sendBuff.Length);
               // this.mSerialPort.WriteLine(this.txtNumber.Text);
                AppendTextToLog(string.Format("写数据到串口设备成功"));
            }
            catch (Exception ex)
            {
               AppendTextToLog(string.Format("写数据到串口设备异常:{0}", ex.Message));
            }

        }

        private void AppendTextToLog(string txt) 
        {
            this.txtLog.AppendText("\r\n");
            this.txtLog.AppendText(string.Format("[{0}]\r\n",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            this.txtLog.AppendText(txt);
            this.txtLog.Select(this.txtLog.TextLength-1, 0);
            this.txtLog.ScrollToCaret();

        }
    }
}
