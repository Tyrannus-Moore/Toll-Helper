using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TollAssistUI
{
    public partial class UCDescInfo : UserControl
    {
        public UCDescInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置显示文本
        /// </summary>
        /// <param name="tips"></param>
        public void SetTips(string tips) 
        {
            this.lblTips.Text = tips;
        }
    }
}
