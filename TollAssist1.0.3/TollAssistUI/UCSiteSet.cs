using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommHandler;
using IDbHandler;
using TollAssistComm;
using Ice_Servant_Factory;
using System.Management;

namespace TollAssistUI
{
    public partial class UCSiteSet : UserControl
    {
        public UCSiteSet()
        {
            InitializeComponent();

            //IP地址验证
            ipErrorProvider.SetIconAlignment(this.txtLocalHostIP, ErrorIconAlignment.MiddleRight);
            ipErrorProvider.SetIconPadding(this.txtLocalHostIP, 2);
            ipErrorProvider.BlinkRate = 1000;
            ipErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.AlwaysBlink;
        }

        /// <summary>
        /// 选择控件的描述文本
        /// </summary>
        public event Action<string> SelectControlDescEvent;

        //IP地址验证
        System.Windows.Forms.ErrorProvider ipErrorProvider = new  System.Windows.Forms.ErrorProvider();
       


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


        /// <summary>
        /// 将此控件中的所有参数保存到系统配置中
        /// </summary>
        public void SaveToSysConfig(SysConfig sysConfig)
        {
            if (!string.IsNullOrWhiteSpace(this.cboCompanyCode.Text))
                sysConfig.mCompanyCode = this.cboCompanyCode.Text;

            if (!string.IsNullOrWhiteSpace(this.cboPlazeCode.Text))
                sysConfig.mPlazCode = this.cboPlazeCode.Text;

            if (!string.IsNullOrWhiteSpace(this.cboLanName.Text))
                sysConfig.mLanName = this.cboLanName.Text;

            if (!string.IsNullOrWhiteSpace(this.numLanNum.Value.ToString()))
                sysConfig.mLanNum = (int)this.numLanNum.Value;

            if (!string.IsNullOrWhiteSpace(this.txtLocalHostIP.Text) && Helper.ValidateIPAddress(this.txtLocalHostIP.Text))
                sysConfig.mLocalHostIPAddress = this.txtLocalHostIP.Text;

            if (!string.IsNullOrWhiteSpace(this.txtServerIP.Text)&&Helper.ValidateIPAddress(this.txtServerIP.Text))
                sysConfig.mServerIPAddress = this.txtServerIP.Text;

            //20171217 add
            //上传收费节点信息到数据库
            System.Threading.ThreadPool.QueueUserWorkItem(stat => 
            {
                ASSISTICE.TollNode node = new ASSISTICE.TollNode();
                node.companycode = sysConfig.mCompanyCode;
                node.plazcode = sysConfig.mPlazCode;
                node.lanname = sysConfig.mLanName;
                node.lannum = sysConfig.mLanNum;
                string host_mac = getMacAddr_Local();

                UploadNodeInfo(host_mac, node);

            });

        }

        private void UploadNodeInfo(string id,ASSISTICE.TollNode node) 
        {
            ClientProxyWrapper<ASSISTICE.ICarQueryPrx> proxy = proxy = SysComponents.GetComponent("ASSISTICE.ICarQueryPrx") as ClientProxyWrapper<ASSISTICE.ICarQueryPrx>;
            if (proxy == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "UCSiteSet::UploadNodeInfo()=>获取代理ICarQueryPrx失败");

            }
            else
            {
                string error;
                //执行上传操作
                ICarQueryProxyWrapper wrapper = new ICarQueryProxyWrapper(proxy.prx);
                if (!wrapper.UploadTollNode(id,node, out error))
                {
                    LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "UCSiteSet::UploadNodeInfo()=>调用BatchUpload失败:{0}", error);
                }
            }
        }

        /// <summary> 
        /// 获取网卡物理地址 
        /// </summary> 
        /// <returns></returns> 
        public static string getMacAddr_Local()
        {
            string madAddr = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc2 = mc.GetInstances();
            foreach (ManagementObject mo in moc2)
            {
                if (Convert.ToBoolean(mo["IPEnabled"]) == true)
                {
                    madAddr = mo["MacAddress"].ToString();
                    madAddr = madAddr.Replace(':', '-');
                }
                mo.Dispose();
            }
            return madAddr;
        }

         /// <summary>
        /// 从配置文件读取信息并显示到UI中
        /// </summary>
        /// <param name="sysConfig"></param>
        public void LoadBySysConfig(SysConfig sysConfig) 
        {
            this.cboCompanyCode.Text = sysConfig.mCompanyCode;
            this.cboPlazeCode.Text = sysConfig.mPlazCode;
            this.cboLanName.Text = sysConfig.mLanName;
            this.numLanNum.Value = sysConfig.mLanNum;
            this.txtLocalHostIP.Text = sysConfig.mLocalHostIPAddress;
            this.txtServerIP.Text = sysConfig.mServerIPAddress;

        }

        private void cboCompanyCode_MouseClick(object sender, MouseEventArgs e)
        {
            SendSelectedItemDesc(sender);
        }

        private void UCSiteSet_Load(object sender, EventArgs e)
        {
            LoadCompanyCodes();
        }

        //加载路公司下拉列表
        public void LoadCompanyCodes()
        {
            string sql = "select * from station where lb='1'";//lb==1表示路公司代码

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadCompanyCodes()=>获取SqliteHandler实例失败");
                return;
            }

            string error;
            List<ASSISTICE.Station> stations = sqliteHandler.SearcherStation(sql, out error);
            if (stations == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "FrmSeverMain::LoadCompanyCodes()=>调用SearcherStation异常:{0}", error);
                return;
            }
            if (stations.Count == 0)
                return;

            this.cboCompanyCode.Items.Clear();
            foreach (ASSISTICE.Station item in stations)
            {
                this.cboCompanyCode.Items.Add(string.Format("{0}{1}", item.bm, item.mc));//bm+mc
            }

            if (this.cboCompanyCode.Items.Count > 0)
                this.cboCompanyCode.SelectedIndex = 0;

        }

        //加载站点列表
        private void LoadPlazeCodes()
        {
            if (string.IsNullOrWhiteSpace(this.cboCompanyCode.Text))
                return;

            if (this.cboCompanyCode.Text.Length < 3)
                return;
            //提取bm
            string bm = this.cboCompanyCode.Text.Substring(0, 3);

            string sql = string.Format("select * from station where lgs='{0}'", bm);

            //获取sqlite查询实例
            SqliteHandler sqliteHandler = SysComponents.GetComponent("SqliteHandler") as SqliteHandler;
            if (sqliteHandler == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.DEBUG, "FrmSeverMain::LoadCompanyCodes()=>获取SqliteHandler实例失败");
                return;
            }

            string error;
            List<ASSISTICE.Station> stations = sqliteHandler.SearcherStation(sql, out error);
            if (stations == null)
            {
                LogerPrintWrapper.Print(LOGCS.LogLevel.ERROR, "FrmSeverMain::LoadCompanyCodes()=>调用SearcherStation异常:{0}", error);
                return;
            }
            if (stations.Count == 0)
                return;

            this.cboPlazeCode.Items.Clear();
            foreach (ASSISTICE.Station item in stations)
            {
                this.cboPlazeCode.Items.Add(string.Format("{0}{1}", item.bm, item.mc));//bm+mc
            }

            if (this.cboPlazeCode.Items.Count > 0)
                this.cboPlazeCode.SelectedIndex = 0;
        }

        private void cboCompanyCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadPlazeCodes();
        }

        
       


        //验证IP地址
        private void txtLocalHostIP_Validated(object sender, EventArgs e)
        {

            TextBox txtBox = sender as TextBox;

            this.ipErrorProvider.SetIconAlignment(txtBox, ErrorIconAlignment.MiddleRight);
            this.ipErrorProvider.SetIconPadding(txtBox, 2);
            if (Helper.ValidateIPAddress(txtBox.Text))
            {
                this.ipErrorProvider.SetError(txtBox, "");
            }
            else 
            {
                this.ipErrorProvider.SetError(txtBox, "无效的IP地址");
            }
        }

    }
}
