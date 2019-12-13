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
    public partial class UCDrag : UserControl
    {
        public UCDrag()
        {
            InitializeComponent();
            this.mHitCursor = new Cursor("eye.cur");
        }

        private void UCDrag_Load(object sender, EventArgs e)
        {
            this.picBoxDrag.Image = global::TollAssistUI.Properties.Resources.drag.ToBitmap();
        }

        private Cursor mOldCursor;
        private Cursor mHitCursor;
        private bool mouseDown = false;

        private DragHitType mDragHitType = DragHitType.None;
        private Object mUserParam = null;

        /// <summary>
        /// 拖动完成事件
        /// </summary>
        public event Action<Point,Object> DragCompledEvent;

        /// <summary>
        /// 设置当前的拖动命中类型
        /// </summary>
        /// <param name="type"></param>
        public void SetDragHitType(DragHitType type,Object userParam) 
        {
            this.mDragHitType = type;
            this.mUserParam = userParam;
        }

        private void picBoxDrag_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.mDragHitType == DragHitType.None)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mOldCursor = Cursor.Current;
                Cursor.Current = this.mHitCursor;
                mouseDown = true;
                this.picBoxDrag.Image = global::TollAssistUI.Properties.Resources.drag2.ToBitmap();
               if(this.mDragHitType== DragHitType.HitControl)
                    this.timerHit.Enabled = true;
            }
          
        }



        private void picBoxDrag_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown) 
            {
                Cursor.Current = mOldCursor;
                mouseDown = false;
                this.picBoxDrag.Image = global::TollAssistUI.Properties.Resources.drag.ToBitmap();
                if (this.timerHit.Enabled)
                    this.timerHit.Enabled = false;

                switch (this.mDragHitType)
                {
                    case DragHitType.HitControl: 
                        {
                            HideRect();
                            if (this.DragCompledEvent != null)
                                this.DragCompledEvent(Cursor.Position, this.mUserParam);
                            break;
                        }
                    case DragHitType.HitPostion:
                        {
                            if (this.DragCompledEvent != null)
                                this.DragCompledEvent(Cursor.Position, this.mUserParam);
                            break;
                        }
                }
            }
        }


        private FrmSelectBox frm = new FrmSelectBox();

        private void ShowRect(Point p)
        {
            frm.StartPosition = FormStartPosition.Manual;
            frm.Location = p;
            frm.Show();
            frm.Refresh();//必须刷新一次
        }

        private void HideRect()
        {
            frm.Hide();
        }

        private void BindShowRect(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return;

            TollAssistComm.Win32ControlInfoFetchHelper.RECT rect = new TollAssistComm.Win32ControlInfoFetchHelper.RECT();
            //获取窗体的所在位置
            if (TollAssistComm.Win32ControlInfoFetchHelper.GetWindowRect(hwnd, ref rect) == 0) return;
            Point p = new Point(rect.left, rect.top);
            
            frm.ClientSize = new Size(rect.right - rect.left, rect.bottom - rect.top);

            ShowRect(p);
        }


        private void timerHit_Tick(object sender, EventArgs e)
        {
            if (!mouseDown)
                return;
            TollAssistComm.Win32ControlInfoFetchHelper.POINT t = new TollAssistComm.Win32ControlInfoFetchHelper.POINT();
            if (TollAssistComm.Win32ControlInfoFetchHelper.GetCursorPos(ref t) != 0)
            {
                IntPtr hwnd = TollAssistComm.Win32ControlInfoFetchHelper.WindowFromPoint(t);
                if (hwnd == IntPtr.Zero)
                {
                    HideRect();
                    return;
                }

                IntPtr rs_hwnd = TollAssistComm.Win32ControlInfoFetchHelper.GetChildHwnd(hwnd, t);

                if (rs_hwnd == IntPtr.Zero)
                    rs_hwnd = hwnd;

                BindShowRect(rs_hwnd);
            }
            else
            {
                HideRect(); 
            }

        }


    }



    /// <summary>
    /// 拖动靶标需要进行的动作
    /// </summary>
    public enum DragHitType 
    {
        /// <summary>
        /// 不命中
        /// </summary>
        None=0,
        /// <summary>
        /// 框选控件
        /// </summary>
        HitControl=1,
        /// <summary>
        /// 选择一个位置
        /// </summary>
        HitPostion=2,
    }
}
