using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommTools;

namespace TollAssistPublisher
{
    /// <summary>
    /// 远程发布参数界面
    /// </summary>
    public partial class UCRemotePublishParams : UserControl
    {
        public UCRemotePublishParams()
        {
            InitializeComponent();
        }

        public string GetRemoteAddress() 
        {
            return this.txtRemoteAddress.Text;
        }
        public void SetRemoteAddress(string remoteAddress) 
        {
            this.txtRemoteAddress.Text = remoteAddress;
        }

        public string GetRemoteUserName() 
        {
            return this.txtUserName.Text;
        }
        public void SetRemoteUserName(string userName) 
        {
            this.txtUserName.Text = userName;
        }


        public string GetRemotePassword() 
        {
            return this.txtPassword.Text;
        }
        public void SetRemotePassword(string password) 
        {
             this.txtPassword.Text = password;
        }

        public string GetShareFolderName()
        {
            return this.cboShareFolderPath.Text;
        }
        public void SetShareFolderName(string shareFolderName) 
        {
            this.cboShareFolderPath.Text = shareFolderName;
        }

        public bool RefreshFolderName(out string error) 
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(this.txtRemoteAddress.Text))
                return false;

            string publishDir =this.txtRemoteAddress.Text;

            try
            {
                bool ret = NetworkShareFileHelper.OpenConnection(publishDir, this.GetRemoteUserName(), this.GetRemotePassword());
                if (!ret) 
                {
                    error = string.Format("连接远程主机失败:{0}", publishDir);
                    return false;
                }
                string[] fullDirs = System.IO.Directory.GetDirectories(publishDir);
                this.cboShareFolderPath.Items.Clear();
                foreach (string item in fullDirs)
                {
                    this.cboShareFolderPath.Items.Add(item.Substring(publishDir.Length + 1));
                }

                if (this.cboShareFolderPath.Items.Count > 0)
                    this.cboShareFolderPath.SelectedIndex = 0;

                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("获取远程主机:{0}下的子目录异常:{1}", publishDir,ex.Message);
                return false;
            }
            finally 
            {
                NetworkShareFileHelper.DisConnection(this.GetRemoteAddress());
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string error;

            RefreshFolderName(out error);
        }


    }
}
