using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TollAssistUI
{
    public partial class FrmOverloadWarn : Form
    {
        public FrmOverloadWarn()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.TransparencyKey = Color.White;
        }

        private string mOverloadInfo = "超载车辆";

        private int mTipFontSize = 23;//提示字体大小
        private void FrmOverloadWarn_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            g.Clear(Color.White);

            if (this.showCount % 2 == 0)//控制显示与隐藏
            {
                return;
            }

            Font font = new System.Drawing.Font("微软雅黑", this.mTipFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            Pen rectPen=new Pen(Color.Red,4);
            SizeF sz = g.MeasureString(this.mOverloadInfo, font);
            g.DrawRectangle(rectPen, 0, 0, sz.Width, sz.Height);
            g.DrawString(this.mOverloadInfo, font, Brushes.Red, 0, 0);
            rectPen.Dispose();
            font.Dispose();
        }

        private void FrmOverloadWarn_Load(object sender, EventArgs e)
        {
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.TransparencyKey = Color.White;

            this.DoShowAction = DoShow;
            
        }

        private const int SHOWCOUNT = 5;
        private int showCount = SHOWCOUNT;//显示计数，设置为奇数

        private System.Threading.ManualResetEvent mManualResetEvent = new System.Threading.ManualResetEvent(true);
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (showCount <= 0) 
            {
                this.timer1.Enabled = false;
                this.Hide();
                return;
            }  
            this.Refresh();
            mManualResetEvent.Reset();
            this.showCount--;
            mManualResetEvent.Set();

        }


        private Action<int, Point> DoShowAction;

        /// <summary>
        /// 显示超载提示文字
        /// </summary>
        /// <param name="fontSz">字体大小</param>
        /// <param name="showLocation">窗口位置</param>
        public void DoShow(int fontSz, Point showLocation) 
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(this.DoShowAction, fontSz, showLocation);
            }
            else
            {
                if (this.timer1.Enabled)
                    this.timer1.Enabled = false;

                this.mTipFontSize = fontSz;

                mManualResetEvent.WaitOne();
                this.showCount = SHOWCOUNT;
                this.TopMost = true;

                if (!this.Visible)
                    this.Visible = true;
                this.Location = showLocation;
                this.timer1.Enabled = true;
            }
        }

    }
}
