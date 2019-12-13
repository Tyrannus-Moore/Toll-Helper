using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using TollAssistComm;


namespace TollAssistUI
{
    public partial class FrmWelcome : Form
    {
        public FrmWelcome()
        {
            InitializeComponent();



        }

        
        private void FrmWelcome_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //g.Clear(Color.Black);
            //g.DrawImage(global::TollAssistUI.Properties.Resources.QQ拼音截图未命名, this.ClientRectangle);

           

        }

        private void FrmWelcome_Load(object sender, EventArgs e)
        {
            //this.TopMost = false;
            //Win32ControlInfoFetchHelper.SetWindowPos(this.Handle, new IntPtr(Win32ControlInfoFetchHelper.HWND_BOTTOM), 0, 0, 0, 0,
            //    Win32ControlInfoFetchHelper.SWP_NOACTIVATE | Win32ControlInfoFetchHelper.SWP_NOSIZE | Win32ControlInfoFetchHelper.SWP_NOMOVE);
            //this.ShowInTaskbar = false;
           

            
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.TransparencyKey = Color.DarkRed;
            //this.BackColor = Color.DarkRed;
           // this.BackgroundImage = global::TollAssistUI.Properties.Resources.启动_修改;

            this.timerHide.Interval = 3000;
            this.timerHide.Enabled = true;

            SetBits(global::TollAssistUI.Properties.Resources.启动_修改);
        }

        //private int count = 30;
        private void timerHide_Tick(object sender, EventArgs e)
        {
            //this.Opacity -= 0.03f;
            //count--;
            //if (count == 0) 
            //{
            //    this.Close();
            //}
            this.timerHide.Enabled = false;
            this.Close();
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
    }


    // ///

    ///// Summary description for BitmapRegion.
    /////
    //public class BitmapRegion
    //{
    //    public BitmapRegion()
    //    { }

    //    ///
    //    /// Create and apply the region on the supplied control
    //    /// 创建支持位图区域的控件（目前有button和form）
    //    ///
    //    /// The Control object to apply the region to控件
    //    /// The Bitmap object to create the region from位图
    //    public static void CreateControlRegion(Control control, Bitmap bitmap)
    //    {
    //        // Return if control and bitmap are null
    //        //判断是否存在控件和位图
    //        if (control == null || bitmap == null)
    //            return;
    //        // Set our control''s size to be the same as the bitmap
    //        //设置控件大小为位图大小
    //        control.Width = bitmap.Width;
    //        control.Height = bitmap.Height;
    //        // Check if we are dealing with Form here
    //        //当控件是form时
    //        if (control is System.Windows.Forms.Form)
    //        {
    //            // Cast to a Form object
    //            //强制转换为FORM
    //            Form form = (Form)control;
    //            // Set our form''s size to be a little larger that the bitmap just
    //            // in case the form''s border style is not set to none in the first place
    //            //当FORM的边界FormBorderStyle不为NONE时，应将FORM的大小设置成比位图大小稍大一点
    //            form.Width = control.Width;
    //            form.Height = control.Height;
    //            // No border
    //            //没有边界
    //            form.FormBorderStyle = FormBorderStyle.None;
    //            // Set bitmap as the background image
    //            //将位图设置成窗体背景图片
    //            form.BackgroundImage = bitmap;
    //            // Calculate the graphics path based on the bitmap supplied
    //            //计算位图中不透明部分的边界
    //            GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
    //            // Apply new region
    //            //应用新的区域
    //            form.Region = new Region(graphicsPath);
    //        }
    //        // Check if we are dealing with Button here
    //        //当控件是button时
    //        else if (control is System.Windows.Forms.Button)
    //        {
    //            // Cast to a button object
    //            //强制转换为 button
    //            Button button = (Button)control;
    //            // Do not show button text
    //            //不显示button text
    //            button.Text = "";
    //            // Change cursor to hand when over button
    //            //改变 cursor的style
    //            button.Cursor = Cursors.Hand;
    //            // Set background image of button
    //            //设置button的背景图片
    //            button.BackgroundImage = bitmap;
    //            // Calculate the graphics path based on the bitmap supplied
    //            //计算位图中不透明部分的边界
    //            GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
    //            // Apply new region
    //            //应用新的区域
    //            button.Region = new Region(graphicsPath);
    //        }
    //    }
    //    ///
    //    /// Calculate the graphics path that representing the figure in the bitmap
    //    /// excluding the transparent color which is the top left pixel.
    //    /// //计算位图中不透明部分的边界
    //    ///
    //    /// The Bitmap object to calculate our graphics path from
    //    /// Calculated graphics path
    //    private static GraphicsPath CalculateControlGraphicsPath(Bitmap bitmap)
    //    {
    //        // Create GraphicsPath for our bitmap calculation
    //        //创建 GraphicsPath
    //        GraphicsPath graphicsPath = new GraphicsPath();
    //        // Use the top left pixel as our transparent color
    //        //使用左上角的一点的颜色作为我们透明色
    //        Color colorTransparent = bitmap.GetPixel(0, 0);
    //        // This is to store the column value where an opaque pixel is first found.
    //        // This value will determine where we start scanning for trailing opaque pixels.
    //        //第一个找到点的X
    //        int colOpaquePixel = 0;
    //        // Go through all rows (Y axis)
    //        // 偏历所有行（Y方向）
    //        for (int row = 0; row < bitmap.Height; row++)
    //        {
    //            // Reset value
    //            //重设
    //            colOpaquePixel = 0;
    //            // Go through all columns (X axis)
    //            //偏历所有列（X方向）
    //            for (int col = 0; col < bitmap.Width; col++)
    //            {
    //                // If this is an opaque pixel, mark it and search for anymore trailing behind
    //                //如果是不需要透明处理的点则标记，然后继续偏历
    //                if (bitmap.GetPixel(col, row) != colorTransparent)
    //                {
    //                    // Opaque pixel found, mark current position
    //                    //记录当前
    //                    colOpaquePixel = col;
    //                    // Create another variable to set the current pixel position
    //                    //建立新变量来记录当前点
    //                    int colNext = col;
    //                    // Starting from current found opaque pixel, search for anymore opaque pixels
    //                    // trailing behind, until a transparent   pixel is found or minimum width is reached
    //                    ///从找到的不透明点开始，继续寻找不透明点,一直到找到或则达到图片宽度
    //                    for (colNext = colOpaquePixel; colNext < bitmap.Width; colNext++)
    //                        if (bitmap.GetPixel(colNext, row) == colorTransparent)
    //                            break;
    //                    // Form a rectangle for line of opaque   pixels found and add it to our graphics path
    //                    //将不透明点加到graphics path
    //                    graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));
    //                    // No need to scan the line of opaque pixels just found
    //                    col = colNext;
    //                }
    //            }
    //        }
    //        // Return calculated graphics path
    //        return graphicsPath;
    //    }
    //}



}
