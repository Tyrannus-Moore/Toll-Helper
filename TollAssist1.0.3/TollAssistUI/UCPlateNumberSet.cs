using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommHandler;
using TollAssistComm;

namespace TollAssistUI
{
    public partial class UCPlateNumberSet : UserControl
    {
        public UCPlateNumberSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 号牌控件位置填写文本框被选择
        /// </summary>
        public event Action PlatteNumberPosSelected;

        /// <summary>
        /// 车型控件位置填写文本框被选择
        /// </summary>
        public event Action CarTypePosSelected;



        /// <summary>
        /// 其他控件被选择
        /// </summary>
        public event Action OtherControlSelectedEvent;

        /// <summary>
        /// 选择控件的描述文本
        /// </summary>
        public event Action<string> SelectControlDescEvent;

        public void SetPlattePos(Point pos)
        {
            if (pos.IsEmpty)
                return;

            this.txtSuspectCar.Text = pos.ToString();
            this.txtSuspectCar.Tag = pos;

            IntPtr platteHandle = Win32ControlInfoFetchHelper.GetControlByPos(pos);
            if (platteHandle == IntPtr.Zero)
                return;
            this.txtCommonCar.Text = platteHandle.ToString();
            this.txtFontSelect.Text= Win32ControlInfoFetchHelper.GetProcessPathByHwnd(platteHandle);

        }

        public void SetCarTypePos(Point pos)
        {
            if (pos.IsEmpty)
                return;

            this.txtInvalidCar.Text = pos.ToString();
            this.txtInvalidCar.Tag = pos;

            IntPtr carTypeHandle = Win32ControlInfoFetchHelper.GetControlByPos(pos);
            if (carTypeHandle == IntPtr.Zero)
                return;
            this.txtUnblockedCar.Text = carTypeHandle.ToString();

        }

        /*
        /// <summary>
        /// 将此控件中的所有参数保存到系统配置中
        /// </summary>
        public void SaveToSysConfig(SysConfig sysConfig)
        {
            if (!string.IsNullOrWhiteSpace(this.txtTollSoftTitel.Text))
                sysConfig.mTollFormTitel = this.txtTollSoftTitel.Text;

            if (!string.IsNullOrWhiteSpace(this.txtFontSelect.Text))
                sysConfig.mTollSoftPath = this.txtFontSelect.Text;

            if (!string.IsNullOrWhiteSpace(this.txtCommonCar.Text))
                sysConfig.mPlatteControlID = this.txtCommonCar.Text;

            if (!string.IsNullOrWhiteSpace(this.txtSuspectCar.Text))
                sysConfig.mPlatteControlPos = (Point)this.txtSuspectCar.Tag;

            if (!string.IsNullOrWhiteSpace(this.txtUnblockedCar.Text))
                sysConfig.mCarTypeControlID = this.txtUnblockedCar.Text;

            if (!string.IsNullOrWhiteSpace(this.txtInvalidCar.Text))
                sysConfig.mCarTypeControlPos = (Point)this.txtInvalidCar.Tag;
        }

         /// <summary>
        /// 从配置文件读取信息并显示到UI中
        /// </summary>
        /// <param name="sysConfig"></param>
        public void LoadBySysConfig(SysConfig sysConfig) 
        {
            this.txtTollSoftTitel.Text = sysConfig.mTollFormTitel;
            this.txtFontSelect.Text = sysConfig.mTollSoftPath;
            this.txtCommonCar.Text = sysConfig.mPlatteControlID;
            this.txtSuspectCar.Text = sysConfig.mPlatteControlPos.ToString();
            this.txtSuspectCar.Tag = sysConfig.mPlatteControlPos;
            this.txtUnblockedCar.Text = sysConfig.mCarTypeControlID;
            this.txtInvalidCar.Text = sysConfig.mCarTypeControlPos.ToString();
            this.txtInvalidCar.Tag = sysConfig.mCarTypeControlPos;

        }
        */
        private void txtPlatteNumberPos_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.PlatteNumberPosSelected != null)
                this.PlatteNumberPosSelected();

            SendSelectedItemDesc(sender);
        }

        private void txtCarTypePos_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.CarTypePosSelected != null)
                this.CarTypePosSelected();

            SendSelectedItemDesc(sender);
        }

        private void txtTollSoftTitel_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.OtherControlSelectedEvent != null)
                this.OtherControlSelectedEvent();

            SendSelectedItemDesc(sender);
        }

        /// <summary>
        /// 将选择控件的描述信息发送到注册事件中
        /// </summary>
        /// <param name="sender"></param>
        private void SendSelectedItemDesc(object sender)
        {
            if (this.SelectControlDescEvent != null)
            {
                Control ctl = sender as Control;
                if (ctl != null && ctl.Tag != null)
                    this.SelectControlDescEvent(ctl.Tag as string);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtCarTypeID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPlatteNumberPos_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPlatteNumberID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTollSoftPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtCarTypePos_TextChanged(object sender, EventArgs e)
        {

        }

        private void UCPlateNumberSet_Load(object sender, EventArgs e)
        {

        }
 
        /// <summary>
        /// 将此控件中的所有参数保存到系统配置中
        /// </summary>
        public void SaveToSysConfig(SysConfig sysConfig)
        {
            if (!string.IsNullOrWhiteSpace(this.txtFontSelect.Text))
                sysConfig.minformationFont = this.txtFontSelect.Text;
            if(!string.IsNullOrWhiteSpace(this.txtCommonCar.Text))
            {
                GetRGB(this.txtCommonCar.Text,ref sysConfig.mCommonCarR,ref sysConfig.mCommonCarG,ref sysConfig.mCommonCarB);
            }
            if (!string.IsNullOrWhiteSpace(this.txtSuspectCar.Text))
            {
                GetRGB(this.txtSuspectCar.Text, ref sysConfig.mSuspectCarR, ref sysConfig.mSuspectCarG, ref sysConfig.mSuspectCarB);
            }
            if (!string.IsNullOrWhiteSpace(this.txtUnblockedCar.Text))
            {
                GetRGB(this.txtUnblockedCar.Text, ref sysConfig.mUnblockedCarR, ref sysConfig.mUnblockedCarG, ref sysConfig.mUnblockedCarB);
            }
            if (!string.IsNullOrWhiteSpace(this.txtInvalidCar.Text))
            {
                GetRGB(this.txtInvalidCar.Text, ref sysConfig.mInvalidCarR, ref sysConfig.mInvalidCarG, ref sysConfig.mInvalidCarB);
            }

        }

        /// <summary>
        /// 从配置文件读取信息并显示到UI中
        /// </summary>
        /// <param name="sysConfig"></param>
        public void LoadBySysConfig(SysConfig sysConfig)
        {
            this.txtFontSelect.Text = sysConfig.minformationFont;
            this.txtCommonCar.Text="R:"+sysConfig.mCommonCarR+"\tG:"+sysConfig.mCommonCarG+"\tB:"+sysConfig.mCommonCarB;
            this.txtSuspectCar.Text="R:"+sysConfig.mSuspectCarR+"\tG:"+sysConfig.mSuspectCarG+"\tB:"+sysConfig.mSuspectCarB;
            this.txtInvalidCar.Text = "R:" + sysConfig.mInvalidCarR + "\tG:" + sysConfig.mInvalidCarG + "\tB:" + sysConfig.mInvalidCarB;
            this.txtUnblockedCar.Text = "R:" + sysConfig.mUnblockedCarR + "\tG:" + sysConfig.mUnblockedCarG + "\tB:" + sysConfig.mUnblockedCarB;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() != DialogResult.Cancel)
            {
                Font temp;
                temp = fontDialog.Font;//将当前选定的文字改变字体  
                this.txtFontSelect.Text = temp.Name;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if(colordialog.ShowDialog()!=DialogResult.Cancel)
            {
                Color temp;
                temp = colordialog.Color;
                this.txtCommonCar.Text = "R:" + temp.R + "\tG:" + temp.G + "\tB:" + temp.B;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() != DialogResult.Cancel)
            {
                Color temp;
                temp = colordialog.Color;
                this.txtSuspectCar.Text = "R:" + temp.R + "\tG:" + temp.G + "\tB:" + temp.B;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() != DialogResult.Cancel)
            {
                Color temp;
                temp = colordialog.Color;
                this.txtUnblockedCar.Text = "R:" + temp.R + "\tG:" + temp.G + "\tB:" + temp.B;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() != DialogResult.Cancel)
            {
                Color temp;
                temp = colordialog.Color;
                this.txtInvalidCar.Text = "R:" + temp.R + "\tG:" + temp.G + "\tB:" + temp.B;
            }
        }

        private void GetRGB(string temp,ref int R ,ref int G,ref int B)
        {
            string[] s = temp.Split(new char[] { ':','\t' });
            R = Convert.ToInt32(s[1]);
            G = Convert.ToInt32(s[3]);
            B = Convert.ToInt32(s[5]);
        }
    }
}
