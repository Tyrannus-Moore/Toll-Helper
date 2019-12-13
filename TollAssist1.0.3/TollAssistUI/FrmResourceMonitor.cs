using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using CSharpPerformance;
using System.Threading;
using System.Runtime.InteropServices;
using TollAssistComm;

namespace TollAssistUI
{
    public partial class FrmResourceMonitor : Form
    {
        public FrmResourceMonitor()
        {
            InitializeComponent();
        }

         [DllImport("user32.dll")]
        private static extern int GetWindowLongA(IntPtr hWnd,int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLongA(IntPtr hWnd,int nIndex,int dwNewLong);
        private const int GWL_EXSTYLE = (-20);
        private const int WS_EX_LAYERED = 0x00080000;

        [DllImport("user32.dll")]
        private static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags); 

        private const uint LWA_COLORKEY=0x00000001;
        private const uint LWA_ALPHA = 0x00000002;

        private void FrmResourceMonitor_Load(object sender, EventArgs e)
        {

            InitFormPostion();

            this.ShowInTaskbar = false;
            this.TopLevel = true;
            this.TopMost = true;

            this.ClientSize = new Size(139, 60);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.TransparencyKey = System.Drawing.SystemColors.Control;//控制透明
            this.backImage =global::TollAssistUI.Properties.Resources.fuk_2x;


           

            this.DoRefreshShow = DoShow;

        }

        //private uint RGB(byte r,byte g,byte b)
        //{ 
        //   return ((uint)(((byte)(r)|((ushort)((byte)(g))<<8))|(((uint)(byte)(b))<<16)));
        //}

        //设置窗体初始位置
        private void InitFormPostion() 
        {
            //this.ClientSize = new Size(300, 200);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - 150, Screen.PrimaryScreen.WorkingArea.Height-70);
        }


        //以下接口均用于解决不规则窗口毛边问题
        //使用此接口请注意不要设置窗体的TransparencyKey属性和窗体的背景图片属性
        //另外使用了此接口窗体将不再响应ONPONT事件

        #region 重写窗体的 CreateParams 属性
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000;  //  WS_EX_LAYERED 扩展样式
                ////无边框任务栏窗口最小化
                //const int WS_MINIMIZEBOX = 0x00020000;  // Winuser.h中定义
                ////CreateParams cp = base.CreateParams;
                //cp.Style = cp.Style | WS_MINIMIZEBOX;   // 允许最小化操作
                return cp;
            }
        }
        #endregion

        #region API调用

        /// <summary>
        /// 设置不规则图片背景窗体并消除毛边问题
        /// 【注意：使用此接口窗体将不再处理WM_POINT消息，即ONPOINT事件失效】
        /// </summary>
        /// <param name="bitmap"></param>
        public void SetBits(Bitmap bitmap)//调用UpdateLayeredWindow（）方法。this.BackgroundImage为你事先准备的带透明图片。
        {
            //if (!haveHandle) return;

            if (bitmap == null) return;

            if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
                throw new ApplicationException("图片必须是32位带Alhpa通道的图片。");

            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

            try
            {
                Win32.Point topLoc = new Win32.Point(Left, Top);
                Win32.Size bitMapSize = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
                Win32.Point srcLoc = new Win32.Point(0, 0);

                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));//这里这个背景色一定要和Graphics.Clear的背景色一致
                oldBits = Win32.SelectObject(memDc, hBitmap);

                blendFunc.BlendOp = Win32.AC_SRC_OVER;
                blendFunc.SourceConstantAlpha = 255;
                blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
                blendFunc.BlendFlags = 0;

                Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBits);
                    Win32.DeleteObject(hBitmap);
                }
                Win32.ReleaseDC(IntPtr.Zero, screenDC);
                Win32.DeleteDC(memDc);
            }
        }
        #endregion
        


        #region 显示绘制

        //进程资源占用情况参数--占用比[0~1]之间的值
        private double mSysCPURate, mSysMemRate, mProcessCPURate, mProcessMemRate;
        //刷新显示委托
        private Action DoRefreshShow;

        private Font processRateFont = new Font("微软雅黑", 7, GraphicsUnit.Point);
        private Font sysMemRateFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        private Brush brush=new SolidBrush(Color.FromArgb(55,166,240));

        private Bitmap backImage = null;

        /// <summary>
        /// 绘制显示信息
        /// </summary>
        private void DrawInfo() 
        {

            if (this.backImage == null)
                return;

            Bitmap tmpBitmap = new Bitmap(this.backImage.Width, this.backImage.Height);


            Graphics g = Graphics.FromImage(tmpBitmap);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//控制抗锯齿
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            g.Clear(Color.FromArgb(0));//控制透明，此处颜色一定要和SetBits()中设置的颜色一致
            g.DrawImage(this.backImage, 0, 0);

            //显示相关信息

            string strCpu = string.Format("CPU:{0}%", (this.mProcessCPURate * 100).ToString("F0"));
            string strMem = string.Format("MEM:{0}%", (this.mProcessMemRate * 100).ToString("F0"));
            //string strSysMem = string.Format("{0}%", (this.mSysMemRate*100).ToString("F0"));


            //g.DrawString(strSysMem, sysMemRateFont, Brushes.WhiteSmoke, new PointF(15, 20));
            g.DrawString(strCpu, processRateFont, Brushes.Black, new PointF(70, 18));
            g.DrawString(strMem, processRateFont, Brushes.Black, new PointF(70, 30));


            double res_total = this.mProcessCPURate + this.mProcessMemRate;//总的资源占用情况:内存+CPU
            if (res_total > 1.0)
                res_total = 1.0f;
            string strResTotal = string.Format("{0}%", (res_total * 100).ToString("F0"));


            ////以下为显示系统内存占用情况
            //int top = 8;//绘制的完整的圆的Y坐标
            //int ellipse_h = 36;//绘制的完整的圆的半径
            ////double hRate = this.mSysUsedMemInBytes / (double)mSystemInfo.PhysicalMemory;//计算系统内存占用情况
            //int h = (int)(ellipse_h * this.mSysMemRate);//计算应该显示的圆的高度

            //以下为显示系统资源占用情况
            int top = 3;//绘制的完整的圆的Y坐标
            int ellipse_h = 52;//绘制的完整的圆的半径
            //double hRate = this.mSysUsedMemInBytes / (double)mSystemInfo.PhysicalMemory;//计算系统内存占用情况
            int h = (int)(ellipse_h * res_total);//计算应该显示的圆的高度

            g.Clip = new System.Drawing.Region(new Rectangle(4, top + ellipse_h - h, ellipse_h, h));//使用裁剪的方式让其只显示需要的部分(注意：此对象可以在外面一次性定义好)
            g.FillEllipse(brush, 4, top, ellipse_h, ellipse_h);//完整的绘制一个圆，因为使用了裁剪故此圆只有被Clip定义的区域会被显示出来

            //还原裁剪区域，显示系统内存占用情况字符串，如果不还原裁剪区域则可能导致以下字符串被裁剪无法显示
            g.Clip = new Region(this.ClientRectangle);
            //g.DrawString(strSysMem, sysMemRateFont, Brushes.WhiteSmoke, new PointF(15, 20));
            g.DrawString(strResTotal, sysMemRateFont, Brushes.WhiteSmoke, new PointF(20, 22));

            SetBits(tmpBitmap);
            tmpBitmap.Dispose();
            g.Dispose();
        }

        private void FrmResourceMonitor_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;

            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//控制抗锯齿
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;


            //g.Clear(System.Drawing.SystemColors.Control);//控制透明
            //g.DrawImage(this.backImage, 0,0);

            ////显示相关信息

            //string strCpu = string.Format("CPU:{0}%",(this.mProcessCPURate*100).ToString("F0"));
            //string strMem = string.Format("MEM:{0}%", (this.mProcessMemRate*100).ToString("F0"));
            ////string strSysMem = string.Format("{0}%", (this.mSysMemRate*100).ToString("F0"));


            ////g.DrawString(strSysMem, sysMemRateFont, Brushes.WhiteSmoke, new PointF(15, 20));
            //g.DrawString(strCpu, processRateFont, Brushes.Black, new PointF(70, 18));
            //g.DrawString(strMem, processRateFont, Brushes.Black, new PointF(70, 30));


            //double res_total = 0.50;//this.mProcessCPURate + this.mProcessMemRate;//总的资源占用情况:内存+CPU
            //if (res_total > 1.0)
            //    res_total = 1.0f;
            //string strResTotal = string.Format("{0}%", (res_total * 100).ToString("F0"));


            //////以下为显示系统内存占用情况
            ////int top = 8;//绘制的完整的圆的Y坐标
            ////int ellipse_h = 36;//绘制的完整的圆的半径
            //////double hRate = this.mSysUsedMemInBytes / (double)mSystemInfo.PhysicalMemory;//计算系统内存占用情况
            ////int h = (int)(ellipse_h * this.mSysMemRate);//计算应该显示的圆的高度

            ////以下为显示系统资源占用情况
            //int top = 3;//绘制的完整的圆的Y坐标
            //int ellipse_h = 52;//绘制的完整的圆的半径
            ////double hRate = this.mSysUsedMemInBytes / (double)mSystemInfo.PhysicalMemory;//计算系统内存占用情况
            //int h = (int)(ellipse_h * res_total);//计算应该显示的圆的高度

            //g.Clip = new System.Drawing.Region(new Rectangle(4, top + ellipse_h - h, ellipse_h, h));//使用裁剪的方式让其只显示需要的部分(注意：此对象可以在外面一次性定义好)
            //g.FillEllipse(brush, 4, top, ellipse_h, ellipse_h);//完整的绘制一个圆，因为使用了裁剪故此圆只有被Clip定义的区域会被显示出来

            ////还原裁剪区域，显示系统内存占用情况字符串，如果不还原裁剪区域则可能导致以下字符串被裁剪无法显示
            //g.Clip = new Region(this.ClientRectangle);
            ////g.DrawString(strSysMem, sysMemRateFont, Brushes.WhiteSmoke, new PointF(15, 20));
            //g.DrawString(strResTotal, sysMemRateFont, Brushes.WhiteSmoke, new PointF(20, 22));
        }

        /// <summary>
        /// 更新显示参数
        /// </summary>
        /// <param name="sysCPURate">系统CPU占用比[0~1]之间的值</param>
        /// <param name="sysMemRate">系统内存占用比[0~1]之间的值</param>
        /// <param name="processCPURate">当前进程CPU占用比[0~1]之间的值</param>
        /// <param name="processMemRate">当前进程内存占用比[0~1]之间的值</param>
        public void RefreshShow(double sysCPURate, double sysMemRate, double processCPURate, double processMemRate) 
        {
            this.mSysCPURate = sysCPURate;
            this.mSysMemRate = sysMemRate;

            this.mProcessCPURate = processCPURate;
            this.mProcessMemRate = processMemRate;

            //刷新显示
            DoShow();

        }

        //刷新显示
        private void DoShow() 
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(this.DoRefreshShow);
            }
            else
            {
                this.DrawInfo();
                //this.Refresh();
            }
        }

        #endregion

        #region 窗口移动相关

        private bool isMove = false;
        private Point? last = null;//上次的位置
        private Point formPoint = new Point();

        private void FrmResourceMonitor_MouseDown(object sender, MouseEventArgs e)
        {
            isMove = (e.Button == System.Windows.Forms.MouseButtons.Left);
            if (isMove)
            {
                formPoint = this.Location;
                last = Cursor.Position;
            }
        }

        private void FrmResourceMonitor_MouseUp(object sender, MouseEventArgs e)
        {
            isMove = false;
        }

        private void FrmResourceMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMove)
            {
                Point p = Cursor.Position;
                formPoint.X += p.X - last.Value.X;
                formPoint.Y += p.Y - last.Value.Y;
                this.Location = formPoint;
                last = p;
            }
        }

        #endregion
    }


   

}
