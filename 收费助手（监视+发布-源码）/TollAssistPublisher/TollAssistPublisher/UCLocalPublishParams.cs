using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TollAssistPublisher
{
    /// <summary>
    /// 本地发布参数界面
    /// </summary>
    public partial class UCLocalPublishParams : UserControl
    {
        public UCLocalPublishParams()
        {
            InitializeComponent();
            this.mFolderBrowserDialog.ShowNewFolderButton = false;
        }

        /// <summary>
        /// 获取路径
        /// </summary>
        /// <returns></returns>
        public string GetPath() 
        {
            return this.txtPath.Text;
        }

        /// <summary>
        /// 设置路径
        /// </summary>
        /// <param name="path"></param>
        public void SetPath(string path) 
        {
            this.txtPath.Text = path;
        }

        private FolderBrowserDialog mFolderBrowserDialog = new FolderBrowserDialog();

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (this.mFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtPath.Text = this.mFolderBrowserDialog.SelectedPath;
            }
            
        }



    }
}
