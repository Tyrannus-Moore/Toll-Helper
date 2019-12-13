using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommHandler;

namespace TollAssistUI
{
    public partial class UCTipInfoSet : UserControl
    {
        public UCTipInfoSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 提示信息坐标填写控件被选择
        /// </summary>
        public event Action TipsPosSelected;


        /// <summary>
        /// 其他控件被选择
        /// </summary>
        public event Action OtherControlSelectedEvent;

        /// <summary>
        /// 选择控件的描述文本
        /// </summary>
        public event Action<string> SelectControlDescEvent;

        /// <summary>
        /// 资源窗口的可见性发生改变
        /// </summary>
        public event Action<bool> ResFormVisibleEvent;

        /// <summary>
        /// 字体大小变更通知
        /// </summary>
        public event Action<int> TipsFontSizeChangedEvent;

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

        public void SetTipsPostion(Point pos) 
        {
            if (pos.IsEmpty)
                return;
            this.txtTipsPostion.Text = pos.ToString();
            this.txtTipsPostion.Tag = pos;
        }

        /// <summary>
        /// 将此控件中的所有参数保存到系统配置中
        /// </summary>
        public void SaveToSysConfig(SysConfig sysConfig)
        {


            if (!string.IsNullOrWhiteSpace(this.numTipsFontSize.Value.ToString()))
                sysConfig.mTipsFontSize = (int)this.numTipsFontSize.Value;

            if (!string.IsNullOrWhiteSpace(this.txtTipsPostion.Text))
                sysConfig.mTipsFormPos = (Point)this.txtTipsPostion.Tag;

            sysConfig.mResFormVisible = this.cboResFormVisible.SelectedIndex == 0;

            sysConfig.mWelcomeLogoVisible = this.cboStartLogoShow.SelectedIndex == 0;
        }

         /// <summary>
        /// 从配置文件读取信息并显示到UI中
        /// </summary>
        /// <param name="sysConfig"></param>
        public void LoadBySysConfig(SysConfig sysConfig) 
        {
            this.numTipsFontSize.Value = sysConfig.mTipsFontSize;
            this.txtTipsPostion.Text = sysConfig.mTipsFormPos.ToString();
            this.txtTipsPostion.Tag = sysConfig.mTipsFormPos;
            this.cboResFormVisible.SelectedIndex = sysConfig.mResFormVisible ? 0 : 1;
            this.cboStartLogoShow.SelectedIndex = sysConfig.mWelcomeLogoVisible ? 0 : 1;

            //当前版本号
            long currentVersion = SoftwareUpdateThread.GetLocalSerialNumber(System.IO.Path.Combine(TollAssistComm.Helper.GetCurrentProcessPath(), "PublishRecord.txt"));
            this.lblCurrVersion.Text = string.Format("当前版本:{0}", currentVersion);
            if (currentVersion > 0)
            {
                //转换当前版本号到日期时间格式
                DateTime lastUpdateTime = TollAssistComm.Helper.GetDateTime(currentVersion * 1000);
                this.lblLastUpdateTime.Text = string.Format("更新时间:{0}", lastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
           
        }


        private void txtTipsPostion_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.TipsPosSelected != null) 
            {
                this.TipsPosSelected();
            }

            SendSelectedItemDesc(sender);
        }

        private void numTipsFontSize_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.OtherControlSelectedEvent != null)
            {
                this.OtherControlSelectedEvent();
            }

            SendSelectedItemDesc(sender);
        }

        private void cboResFormVisible_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ResFormVisibleEvent != null)
            {
                this.ResFormVisibleEvent(this.cboResFormVisible.SelectedIndex == 0);
            }
            
            //TODO

        }

        //字体大小变更
        private void numTipsFontSize_ValueChanged(object sender, EventArgs e)
        {
            if (this.TipsFontSizeChangedEvent!=null)
                TipsFontSizeChangedEvent((int)this.numTipsFontSize.Value);

            
        }


    }
}
