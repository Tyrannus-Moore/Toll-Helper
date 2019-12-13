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
    public partial class UCOverLoadSet : UserControl
    {
        public UCOverLoadSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 超载提示信息坐标填写控件被选择
        /// </summary>
        public event Action OverloadPostionSelectedEvent;

        /// <summary>
        /// 其他控件被选择
        /// </summary>
        public event Action OtherControlSelectedEvent;

        /// <summary>
        /// 选择控件的描述文本
        /// </summary>
        public event Action<string> SelectControlDescEvent;


        public void SetOverloadPos(Point pos) 
        {
            if (pos.IsEmpty)
                return;

            this.txtOverloadPostion.Text = pos.ToString();
            this.txtOverloadPostion.Tag = pos;
        }

        /// <summary>
        /// 将此控件中的所有参数保存到系统配置中
        /// </summary>
        public void SaveToSysConfig(SysConfig sysConfig) 
        {
            if (!string.IsNullOrWhiteSpace(this.txtOverloadRate.Text))
                sysConfig.mOverloadTips = this.txtOverloadRate.Text;

            if (!string.IsNullOrWhiteSpace(this.txtOverloadPostion.Text))
                sysConfig.mOverloadControlPos = (Point)this.txtOverloadPostion.Tag;
        }

        /// <summary>
        /// 从配置文件读取信息并显示到UI中
        /// </summary>
        /// <param name="sysConfig"></param>
        public void LoadBySysConfig(SysConfig sysConfig) 
        {
            this.txtOverloadRate.Text = sysConfig.mOverloadTips;
            this.txtOverloadPostion.Text = sysConfig.mOverloadControlPos.ToString();
            this.txtOverloadPostion.Tag = sysConfig.mOverloadControlPos;
        }

        private void txtOverloadPostion_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.OverloadPostionSelectedEvent != null)
                this.OverloadPostionSelectedEvent();

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

        private void txtOverloadRate_MouseClick(object sender, MouseEventArgs e)
        {
            
            if (this.OtherControlSelectedEvent != null)
                this.OtherControlSelectedEvent();


            SendSelectedItemDesc(sender);
        }
    }
}
