using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommHandler;
using TollAssistComm;
using System.Runtime.InteropServices;




namespace TollAssistUI
{
    public partial class FrmCFG : Form, IConntectionStatChanged
    {
     



        public FrmCFG(ISysConfigChanged note)
        {
            InitializeComponent();

            this.DoConntectionStatChanged=ConntectionStatChanged;
            this.DoConntectionStatChanged_GCHandle = GCHandle.Alloc(this.DoConntectionStatChanged);

            this.mISysConfigChanged = note;
        }

        private void pnlContactWay_MouseDown(object sender, MouseEventArgs e)
        {
            Panel pnl = sender as Panel;
            pnl.BorderStyle = BorderStyle.Fixed3D;
        }

        private void pnlContactWay_MouseEnter(object sender, EventArgs e)
        {
            Panel pnl = sender as Panel;
            //if (pnl.BorderStyle == BorderStyle.None)
            //    pnl.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pnlContactWay_MouseLeave(object sender, EventArgs e)
        {
            Panel pnl = sender as Panel;
            //if (pnl.BorderStyle == BorderStyle.FixedSingle)
            //    pnl.BorderStyle = BorderStyle.None;
        }

        private void pnlContactWay_MouseUp(object sender, MouseEventArgs e)
        {
            Panel pnl = sender as Panel;
            //pnl.BorderStyle = BorderStyle.FixedSingle;
            pnl.BorderStyle = BorderStyle.None;
        }


        private bool isMove = false;
        private Point? last = null;//上次的位置
        private Point formPoint = new Point();
        private void pnlTitel_MouseDown(object sender, MouseEventArgs e)
        {
            isMove = (e.Button == System.Windows.Forms.MouseButtons.Left);
            if (isMove)
            {
                formPoint = this.Location;
                last = Cursor.Position;
            }
        }

        private void pnlTitel_MouseMove(object sender, MouseEventArgs e)
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

        private void pnlTitel_MouseUp(object sender, MouseEventArgs e)
        {
            isMove = false;

        }

        private void pnlClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.Hide();
        }


        private UCDescInfo mUCDescInfo = new UCDescInfo();//设置选项提示文本控件
        private UCDrag mUCDrag = new UCDrag();//设置选项拖动靶标控件
        private UCOverLoadSet mUCOverLoadSet = new UCOverLoadSet();//超载提示设置界面
        private UCPlateNumberSet mUCPlateNumberSet = new UCPlateNumberSet();//号牌设置界面
        private UCSiteSet mUCSiteSet = new UCSiteSet();//站点设置界面
        private UCTipInfoSet mUCTipInfoSet = new UCTipInfoSet();//提示信息设置界面


        private ISysConfigChanged mISysConfigChanged = null;//系统配置改变通知

        //站点设置按钮被点击
        private void pnlSiteSet_MouseClick(object sender, MouseEventArgs e)
        {
            this.pnlSetContent.Controls.Clear();
            this.mUCSiteSet.Dock = DockStyle.Fill;
            this.pnlSetContent.Controls.Add(mUCSiteSet);

            this.lblSetTip.Text = "站点设置";

            this.pnlDragHit.Visible = false;
            this.mUCDrag.SetDragHitType(DragHitType.None, null);

            this.mUCDescInfo.SetTips(string.Empty);
        }

        //车牌设置按钮被点击
        private void pnlPlateNumberSet_MouseClick(object sender, MouseEventArgs e)
        {
            //备注：功能取消 20171217 PM
//            MessageBox.Show("暂无此功能!");
//            return;

            this.pnlSetContent.Controls.Clear();
            this.mUCPlateNumberSet.Dock = DockStyle.Fill;
            this.pnlSetContent.Controls.Add(mUCPlateNumberSet);

            this.lblSetTip.Text = "车牌设置";

            this.mUCDescInfo.SetTips(string.Empty);
        }

        //提示信息按钮被点击
        private void pnlTipSet_MouseClick(object sender, MouseEventArgs e)
        {
            this.pnlSetContent.Controls.Clear();
            this.mUCTipInfoSet.Dock = DockStyle.Fill;
            this.pnlSetContent.Controls.Add(mUCTipInfoSet);

            this.lblSetTip.Text = "提示信息设置";

            this.mUCDescInfo.SetTips(string.Empty);
        }

        //超载告警按钮被点击
        private void pnlOverLoadSet_MouseClick(object sender, MouseEventArgs e)
        {
            //备注：功能取消 20171217 PM
            MessageBox.Show("暂无此功能!");
            return;


            this.pnlSetContent.Controls.Clear();
            this.mUCOverLoadSet.Dock = DockStyle.Fill;
            this.pnlSetContent.Controls.Add(mUCOverLoadSet);

            this.lblSetTip.Text = "超载告警设置";

            this.mUCDescInfo.SetTips(string.Empty);
        }

        //加载提示文本
        private void LoadTipsTextToControl()
        {
            this.pnlTipsInfo.Controls.Clear();
            this.mUCDescInfo.Dock = DockStyle.Fill;
            this.pnlTipsInfo.Controls.Add(mUCDescInfo);
        }

        //加载靶标控件
        private void LoadDragImageToControl()
        {
            this.pnlDragHit.Controls.Clear();
            this.mUCDrag.Dock = DockStyle.Fill;
            this.pnlDragHit.Controls.Add(mUCDrag);
        }

        /// <summary>
        /// 注册设置控件相关事件
        /// </summary>
        private void RegSetControlEvent()
        {
            this.mUCOverLoadSet.OverloadPostionSelectedEvent += new Action(mUCOverLoadSet_OverloadPostionSelectedEvent);
            this.mUCOverLoadSet.OtherControlSelectedEvent += new Action(mUCOverLoadSet_OtherControlSelectedEvent);
            this.mUCOverLoadSet.SelectControlDescEvent += new Action<string>(mUCOverLoadSet_SelectControlDescEvent);


            this.mUCPlateNumberSet.PlatteNumberPosSelected += new Action(mUCPlateNumberSet_PlatteNumberPosSelected);
            this.mUCPlateNumberSet.CarTypePosSelected += new Action(mUCPlateNumberSet_CarTypePosSelected);
            this.mUCPlateNumberSet.OtherControlSelectedEvent += new Action(mUCOverLoadSet_OtherControlSelectedEvent);
            this.mUCPlateNumberSet.SelectControlDescEvent += new Action<string>(mUCOverLoadSet_SelectControlDescEvent);

            this.mUCTipInfoSet.TipsPosSelected += new Action(mUCTipInfoSet_TipsPosSelected);
            this.mUCTipInfoSet.OtherControlSelectedEvent += new Action(mUCOverLoadSet_OtherControlSelectedEvent);
            this.mUCTipInfoSet.SelectControlDescEvent += new Action<string>(mUCOverLoadSet_SelectControlDescEvent);
            this.mUCTipInfoSet.ResFormVisibleEvent += mUCTipInfoSet_ResFormVisibleEvent;
            this.mUCTipInfoSet.TipsFontSizeChangedEvent += new Action<int>(mUCTipInfoSet_TipsFontSizeChangedEvent);

            this.mUCSiteSet.SelectControlDescEvent += new Action<string>(mUCOverLoadSet_SelectControlDescEvent);


            this.mUCDrag.DragCompledEvent += new Action<Point, object>(mUCDrag_DragCompledEvent);
        }

        /// <summary>
        /// 提示字体大小改变
        /// </summary>
        /// <param name="obj"></param>
        void mUCTipInfoSet_TipsFontSizeChangedEvent(int obj)
        {
            if (this.mISysConfigChanged != null) 
            {
                this.mISysConfigChanged.TipsFontSizeChanged(obj);
            }
        }

        /// <summary>
        /// 资源窗口的可见性发生改变
        /// </summary>
        /// <param name="obj"></param>
        void mUCTipInfoSet_ResFormVisibleEvent(bool obj)
        {
            if (this.mISysConfigChanged != null) 
            {
                this.mISysConfigChanged.ResFormVisible(obj);
            }
        }

        //当设置项被点击后，需要弹出该项的描述信息
        void mUCOverLoadSet_SelectControlDescEvent(string obj)
        {
            this.mUCDescInfo.SetTips(obj);
        }

        /// <summary>
        /// 初始状态显示
        /// </summary>
        private void InitLoadSet() 
        {
            LoadTipsTextToControl();
            LoadDragImageToControl();
            //注册设置控件相关事件
            RegSetControlEvent();

            this.pnlDragHit.Visible = false;
            this.pnlSiteSet_MouseClick(null, null);


            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmCFG::pnlSaveSet_MouseClick()=>获取SysConfig实例失败");
                return;
            }

            this.mUCTipInfoSet.LoadBySysConfig(sysConfig);
            this.mUCSiteSet.LoadBySysConfig(sysConfig);
            this.mUCPlateNumberSet.LoadBySysConfig(sysConfig);
            this.mUCOverLoadSet.LoadBySysConfig(sysConfig);

        }

        //拖动操作完成
        void mUCDrag_DragCompledEvent(Point pos, object ObjSetParam)
        {
            if (ObjSetParam == null || pos.IsEmpty)
                return;
            SetParam setParam = (SetParam)ObjSetParam;
            switch (setParam)
            {
                case SetParam.TipsPostion:
                    {
                        this.mUCTipInfoSet.SetTipsPostion(pos);
                        break;
                    }
                case SetParam.OverloadPostion:
                    {
                        this.mUCOverLoadSet.SetOverloadPos(pos);
                        break;
                    }
                case SetParam.PlatteNumberPostion:
                    {
                        this.mUCPlateNumberSet.SetPlattePos(pos);
                        break;
                    }
                case SetParam.CarTypePostion:
                    {
                        this.mUCPlateNumberSet.SetCarTypePos(pos);
                        break;
                    }
            }
        }

        //其他控件被选择
        void mUCOverLoadSet_OtherControlSelectedEvent()
        {
            this.pnlDragHit.Visible = false;
            this.mUCDrag.SetDragHitType(DragHitType.None, null);
        }

        //当前正在进行提示位置的设置
        void mUCTipInfoSet_TipsPosSelected()
        {
            this.pnlDragHit.Visible = true;
            this.mUCDrag.SetDragHitType(DragHitType.HitPostion, SetParam.TipsPostion);
        }

        //当前正在进行车型控件位置的设置
        void mUCPlateNumberSet_CarTypePosSelected()
        {
            this.pnlDragHit.Visible = true;
            this.mUCDrag.SetDragHitType(DragHitType.HitControl, SetParam.CarTypePostion);
        }

        //当前正在进行号牌控件位置的设置
        void mUCPlateNumberSet_PlatteNumberPosSelected()
        {
            this.pnlDragHit.Visible = true;
            this.mUCDrag.SetDragHitType(DragHitType.HitControl, SetParam.PlatteNumberPostion);
        }

        //当前正在进行超载提示控件的设置
        void mUCOverLoadSet_OverloadPostionSelectedEvent()
        {
            this.pnlDragHit.Visible = true;
            this.mUCDrag.SetDragHitType(DragHitType.HitControl, SetParam.OverloadPostion);
        }

        private void FrmCFG_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            InitLoadSet();
        }

        private Action<bool> DoConntectionStatChanged = null;
        private GCHandle DoConntectionStatChanged_GCHandle ;
        public void ConntectionStatChanged(bool connected)
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(this.DoConntectionStatChanged, connected);
            }
            else 
            {
                if (connected)
                {
                    if (this.lblConnStat.Text != "已连接") //防止多次重复设置
                    {
                        this.picConnStat.Image = global::TollAssistUI.Properties.Resources.ylj;
                        this.lblConnStat.Text = "已连接";
                    }
                }
                else
                {

                    this.picConnStat.Image = global::TollAssistUI.Properties.Resources.ydk;//vincent

                    if (this.lblConnStat.Text != "已断开")//防止多次重复设置
                    {
                        this.picConnStat.Image = global::TollAssistUI.Properties.Resources.ydk;
                        this.lblConnStat.Text = "已断开";
                    }
                }
            }

            
        }

 
        private void pnlExitApplication_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void pnlHideForm_MouseClick(object sender, MouseEventArgs e)
        {
            this.Hide();
        }

        //保存设置
        private void pnlSaveSet_MouseClick(object sender, MouseEventArgs e)
        {
            //获取系统配置实例
            SysConfig sysConfig = SysComponents.GetComponent("SysConfig") as SysConfig;
            if (sysConfig == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmCFG::pnlSaveSet_MouseClick()=>获取SysConfig实例失败");
                return;
            }

            this.mUCTipInfoSet.SaveToSysConfig(sysConfig);
            this.mUCSiteSet.SaveToSysConfig(sysConfig);
            this.mUCPlateNumberSet.SaveToSysConfig(sysConfig);
            this.mUCOverLoadSet.SaveToSysConfig(sysConfig);

            SysConfig.SaveObjectToFile(sysConfig);

        }

        /// <summary>
        /// 手动刷新CompanyCodes
        /// </summary>
        public void ManualRefreshCompanyCodes() 
        {
            this.mUCSiteSet.LoadCompanyCodes();
        }

        private void pnlSetContent_Paint(object sender, PaintEventArgs e)
        {

        }
  
        #region 卓文君禁用alt+f4
        private void FrmCFG_KeyDown(object sender, KeyEventArgs e)
        {

            int i = 0;
            if (e.KeyCode == Keys.F4 && e.Modifiers == Keys.Alt)
            {
                e.Handled = true;
           
        }
        #endregion


        }

        private void pnlHideForm_Paint(object sender, PaintEventArgs e)
        {

        }
     
    }
    /// <summary>
    /// 描述当前正在设置的参数
    /// </summary>
    public enum SetParam
    {
        /// <summary>
        /// 未知参数
        /// </summary>
        None = 0,
        /// <summary>
        /// 当前正在设置提示信息控件坐标
        /// </summary>
        TipsPostion = 1,
        /// <summary>
        /// 当前正在设置车型控件位置坐标
        /// </summary>
        CarTypePostion = 2,
        /// <summary>
        /// 当前正在设置号牌控件位置坐标
        /// </summary>
        PlatteNumberPostion = 3,

        /// <summary>
        /// 当前正在设置超载提示控件位置坐标
        /// </summary>
        OverloadPostion = 4,
    }

    /// <summary>
    /// 系统连接状态通知
    /// </summary>
    public interface IConntectionStatChanged 
    {
        void ConntectionStatChanged(bool connected);
    }

    /// <summary>
    /// 系统配置信息改变
    /// </summary>
    public interface ISysConfigChanged 
    {
        /// <summary>
        /// 资源提示窗口的可见性发生改变
        /// </summary>
        /// <param name="visible">是否可见 </param>
        void ResFormVisible(bool visible);

        /// <summary>
        /// 字体大小变更
        /// </summary>
        /// <param name="fontSize">新的字体大小</param>
        void TipsFontSizeChanged(int fontSize);
    }
}
