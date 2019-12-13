using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TollAssistComm;

namespace TollAssistUI
{
    public partial class FrmReport : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scrollFrequency">文字滚动频率，单位毫秒</param>
        public FrmReport(int scrollFrequency)
        {
            InitializeComponent();
            this.AutoSize = true;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            if (scrollFrequency < 10 || scrollFrequency > 5000) //增加滚动频率设置
            {
                this.mScrollFrequency = 200;
            }
            else
            {
                this.mScrollFrequency = scrollFrequency;
            }



            timer1.Interval = 10000;
            timer1.Enabled = true;
            timer1.Start();
           
        }


        private Font mFont =null;
        private SolidBrush mColor = new SolidBrush(Color.Blue);

        private int mScrollFrequency = 200;//告警信息显示滚动频率

        /// <summary>
        /// 设置显示文本字体和颜色
        /// </summary>
        /// <param name="font"></param>
        /// <param name="color"></param>
        public void SetFont(Font font, Color color)
        {
            mManualResetEvent.Reset();
            System.Threading.Thread.Sleep(10);

            if (font != null)
            {
                this.mFont = font;
            }
            else
            {
                this.mFont = this.Font;
               
            }
            if (color == null)
            {
                return;
            }

            this.mColor.Dispose();
            this.mColor = new SolidBrush(color);

            mManualResetEvent.Set();

            //重新显示内容
            this.SetText(this.mShowText);

        }

        private string mDefaultText = "告警信息";
        private string mShowText = "告警信息";
        private int mTxtOffsetX = 0;//文本开始坐标
        private int mTxtOffsetMax = 0;//最大坐标
        private int mTxtOffsetMin = 0;//最小坐标 20171230 add 采用从右到左方式滚动
        private int mOffset = 0;//当前坐标
        private System.Threading.ManualResetEvent mManualResetEvent = new System.Threading.ManualResetEvent(false);
        private StringBuilder mBufferText = new StringBuilder();//字符缓冲区

        /// <summary>
        /// 重置到默认状态
        /// </summary>
        private void ResetDefaultText() 
        {
            string txt = mDefaultText;

            //20171230取消文字反方向显示
            //this.mBufferText.Clear();
            //for (int i = txt.Length - 1; i >= 0; i--)
            //{
            //    this.mBufferText.Append(txt[i]);
            //}

            int txtWidth = 0;//this.CalcUnitTextPixle() * this.mBufferText.Length;//计算文本占用宽度
            //this.mShowText = this.mBufferText.ToString();20171230 update 文字取消左到右方式滚动
            this.mShowText = txt;//20171230 add

            txtWidth = this.CalcTextPixle(this.mShowText);
           // this.mTxtOffsetX = -txtWidth;//文本初始偏移值 20171230 update 文字取消左到右方式滚动
            //this.mTxtOffsetMax = Screen.PrimaryScreen.Bounds.Width;//文本最大偏移值 20171230 update 文字取消左到右方式滚动
            this.mTxtOffsetX = Screen.PrimaryScreen.Bounds.Width;//文字滚动采用从右到左方式 20171230 add
            this.mTxtOffsetMin = -txtWidth;////文字滚动采用从右到左方式  20171230 add
            this.mOffset = this.mTxtOffsetX;//当前偏移值 
       
        }

        /// <summary>
        /// 设置显示文本
        /// </summary>
        /// <param name="txt"></param>
        public void SetText(string txt) 
        {
            if (string.IsNullOrWhiteSpace(txt))
                return;

            lock (mRWObjLock)
            {
                //20171230取消文字反方向显示
                //this.mBufferText.Clear();
                //for (int i = txt.Length - 1; i >= 0; i--)
                //{
                //    this.mBufferText.Append(txt[i]);
                //}

                int txtWidth = 0;//this.CalcUnitTextPixle() * this.mBufferText.Length;//计算文本占用宽度
                //this.mShowText = this.mBufferText.ToString(); //20171230 update 文字取消左到右方式滚动
                this.mShowText = txt;//20171230 add

                txtWidth = this.CalcTextPixle(this.mShowText);

                //mManualResetEvent.Reset();

                // this.mTxtOffsetX = -txtWidth;//文本初始偏移值 20171230 update 文字取消左到右方式滚动
                //this.mTxtOffsetMax = Screen.PrimaryScreen.Bounds.Width;//文本最大偏移值 20171230 update 文字取消左到右方式滚动
                this.mTxtOffsetX = Screen.PrimaryScreen.Bounds.Width;//文字滚动采用从右到左方式 20171230 add
                this.mTxtOffsetMin = -txtWidth;////文字滚动采用从右到左方式  20171230 add
                this.mOffset = this.mTxtOffsetX;//当前偏移值 

                //mManualResetEvent.Set();

            }
        }

        private Object mRWObjLock = new object();


        /// <summary>
        /// 对一条数据的滚动已完成的事件
        /// string:表示当前滚动的文字信息
        /// bool:返回值:表示是否继续进行当前文字的滚动操作，false：停止对当前文字进行滚动
        /// 用户需要调用SetText()方法设置新的滚动文本，注意：SetText()不能在此调用线程中进行调用，因为SetText()方法属于UI线程而此事件方法非UI线程方法
        /// true：继续对当前文字进行滚动
        /// 注意：如果在此事件中立即调用SetText可能会导致死锁问题，SetText的调用应该使其事件返回后再进行
        /// </summary>
        public event Func<string, bool> ScrollCompledEvent;

        private bool mIsExit = false;//程序是否退出

        private void StopShow() 
        {
            this.mIsExit = true;
            System.Threading.Thread.Sleep(200);
        }

        private void DrawTextThreadFunc(Object stat) 
        {
            while (!this.mIsExit) 
            {
                lock (mRWObjLock) 
                {
                    this.mManualResetEvent.WaitOne();//控制鼠标悬停
                    //this.mOffset += 5;//从左到右方式滚动 
                    this.mOffset -= 5;//从右到左方式滚动 20171230 add


                    //绘制显示
                    DoShow(this.mOffset);
                    //if (this.mOffset > this.mTxtOffsetMax) 20171230 update 取消从左到右方式滚动
                    if (this.mOffset < this.mTxtOffsetMin) //20171230 add 从右到左方式滚动
                    {
                        if (this.ScrollCompledEvent != null)
                        {
                            bool rs = this.ScrollCompledEvent(this.mShowText);
                            if (rs) //用户不需要重新设置内容仅需要继续进行当前文本滚动
                            {
                                this.mOffset = this.mTxtOffsetX;//继续对当前数据进行滚动显示
                              
                            }
                            else //停止当前文本滚动
                            {
                                ResetDefaultText();
                                //this.mManualResetEvent.Reset(); //可能会死锁？？
                            }
                        }
                        else
                        {
                            this.mOffset = this.mTxtOffsetX;//继续对当前数据进行滚动显示
                        }

                    }
                }

                System.Threading.Thread.Sleep(this.mScrollFrequency);
            }
        }

        /// <summary>
        /// 开始显示
        /// </summary>
        /// <param name="offset"></param>
        private void DoShow(int offset) 
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(DoShow), offset);
            }
            else 
            {
                DrawInfo(offset);
            }
        }

        /// <summary>
        /// 绘制信息
        /// </summary>
        /// <param name="offset"></param>
        private void DrawInfo(int offset) 
        {
            Bitmap tmpBitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, 400);
            Graphics g = Graphics.FromImage(tmpBitmap);

            g.Clear(Color.FromArgb(0));//控制透明，此处颜色一定要和SetBits()中设置的颜色一致
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            g.DrawString(this.mShowText, this.mFont, this.mColor, offset,5);

            SetBits(tmpBitmap);
            //font.Dispose();
            tmpBitmap.Dispose();
            g.Dispose();
        }

        private void FrmReport_MouseEnter(object sender, EventArgs e)
        {
            this.mManualResetEvent.Reset();
        }

        private void FrmReport_MouseLeave(object sender, EventArgs e)
        {
            this.mManualResetEvent.Set();
        }


        /// <summary>
        /// 计算单位字符所用像素大小
        /// </summary>
        /// <returns></returns>
        private int CalcUnitTextPixle() 
        {
            Graphics g = this.CreateGraphics();
            g.PageUnit = GraphicsUnit.Pixel;
            int sz =(int)(g.MeasureString("国A", this.mFont).Width / 2);
            g.Dispose();
            return sz;
        }

        /// <summary>
        /// 计算字符所用像素大小
        /// </summary>
        /// <returns></returns>
        private int CalcTextPixle(string txt)
        {
            Graphics g = this.CreateGraphics();
            g.PageUnit = GraphicsUnit.Pixel;
            int sz = (int)g.MeasureString(txt, this.mFont).Width;
            g.Dispose();
            return sz;
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

        private void FrmReport_Load(object sender, EventArgs e)
        {
            this.Left = 0;
            this.Top = 0;

            SetFont(this.Font, Color.Red);
            SetText("告警信息");

            System.Threading.Thread thd = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.DrawTextThreadFunc));
            thd.IsBackground = true;
            thd.Name = "告警信息显示线程";
            thd.Start(null);
        }

        private void FrmReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall) 
            {
                StopShow();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

       



    }
}
