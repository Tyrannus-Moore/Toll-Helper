using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommTools;
using System.Runtime.InteropServices;

namespace TollAssistPublisher
{

   


    public partial class FrmPublisher : Form,ICopyFileCompled
    {

        public FrmPublisher()
        {
            InitializeComponent();

            this.mCopyFileHandler = new CopyFileHandler(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private FolderBrowserDialog mFolderBrowserDialog = new FolderBrowserDialog();
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (this.mFolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                this.txtResourcePath.Text = this.mFolderBrowserDialog.SelectedPath;
            }
        }


        private UCLocalPublishParams mUCLocalPublishParams = new UCLocalPublishParams();
        private UCRemotePublishParams mUCRemotePublishParams = new UCRemotePublishParams();
        private CopyFileHandler mCopyFileHandler = null;

        private void rdoLocalPublish_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdoButton = sender as RadioButton;
            switch (rdoButton.Text) 
            {
                case "本地发布": 
                    {
                        this.pnlParam.Controls.Clear();
                        this.pnlParam.Controls.Add(this.mUCLocalPublishParams);
                        break;
                    }
                case "远程发布": 
                    {
                        this.pnlParam.Controls.Clear();
                        this.pnlParam.Controls.Add(this.mUCRemotePublishParams);
                        break;
                    }
            }

        }

        private void FrmPublisher_Load(object sender, EventArgs e)
        {
           
            this.rdoLocalPublish.Checked = true;//默认选择本地发布
            this.mFolderBrowserDialog.ShowNewFolderButton = false;//文件夹对话框只能选择不能新建
        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            this.richInfo.Clear();
            //1.验证用户资源文件夹是否有效
            if (!System.IO.Directory.Exists(this.txtResourcePath.Text.Trim())) 
            {
                UpdateDisplayList(false,string.Format("资源路径:{0}无效\r\n发布终止",this.txtResourcePath.Text));
                return;
            }

            string srcDir=this.txtResourcePath.Text.Trim();//需要发布的资源文件夹路径

            string publishDir = string.Empty;//发布地址

            //获取发布地址
            if (this.rdoLocalPublish.Checked)//本地发布
            {
                //获取发布路径
                publishDir = this.mUCLocalPublishParams.GetPath();
            }
            else //远程发布
            {
                //获取发布路径
                publishDir = string.Format(@"{0}\{1}", this.mUCRemotePublishParams.GetRemoteAddress(), this.mUCRemotePublishParams.GetShareFolderName());

                if (!NetworkShareFileHelper.OpenConnection(publishDir, this.mUCRemotePublishParams.GetRemoteUserName(), this.mUCRemotePublishParams.GetRemotePassword())) 
                {
                    UpdateDisplayList(false, string.Format("不能打开到:{0}的连接\r\n发布终止", publishDir));
                    this.btnPublish.Enabled = true;
                    return;
                }

            }

            this.btnPublish.Enabled = false;
            //开始发布
            BeginPublish(srcDir, publishDir);

        }

        /// <summary>
        /// 开始发布
        /// </summary>
        /// <param name="srcDir">要发布的资源所在文件夹</param>
        /// <param name="publishDir">发布目的文件夹</param>
        private void BeginPublish(string srcDir, string publishDir)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(stat =>
            {
                //2.验证发布路径是否有效
                if (!System.IO.Directory.Exists(publishDir))
                {
                    UpdateDisplayList(false, string.Format("发布路径:{0}无效\r\n发布终止", publishDir));
                    DoPublishCompled();
                    return;
                }

                //3.在发布路径下生成资源文件夹(yyyyMMddHHmmss)
                string publishFolderName;//发布文件夹名称
                string publishFolderDir;//发布文件夹完整路径
                string ResourcePath;//Resource文件夹完整路径

                try
                {
                    //生成一个可用的发布文件夹
                    for (publishFolderName = DateTime.Now.ToString("yyyyMMddHHmmss"); System.IO.Directory.Exists(publishFolderName);
                        System.Threading.Thread.Sleep(1000),
                        publishFolderName = DateTime.Now.ToString("yyyyMMddHHmmss")) ;
                    publishFolderDir = System.IO.Path.Combine(publishDir, publishFolderName);//发布文件夹完整路径
                    System.IO.Directory.CreateDirectory(publishFolderDir);//生成资源文件夹(yyyyMMddHHmmss)
                    UpdateDisplayList(true, string.Format("创建发布文件夹:{0}成功", publishFolderName));
                    //4.在资源文件夹(yyyyMMddHHmmss)下生成Resource文件夹
                    ResourcePath = System.IO.Path.Combine(publishFolderDir, "Resource");//Resource文件夹完整路径
                    System.IO.Directory.CreateDirectory(ResourcePath);//生成资源文件夹(yyyyMMddHHmmss)
                    UpdateDisplayList(true, string.Format("创建Resource文件夹:{0}成功", "Resource"));
                }
                catch (Exception e)
                {

                    UpdateDisplayList(false, string.Format("文件夹操作出现异常:{0}\r\n发布终止", e.Message));
                    DoPublishCompled();
                    return;
                }

                System.Threading.ThreadPool.QueueUserWorkItem(state =>
                {
                    //5.将用户资源文件夹的内容拷贝到Resource文件夹下
                    string error;
                    bool ret = this.mCopyFileHandler.CopyFile(srcDir, ResourcePath, out error);
                    if (!ret)
                    {
                        UpdateDisplayList(ret, string.Format("用户资源文件夹的内容拷贝到Resource文件夹下失败:{0}", error));
                        goto THREADEXIT;
                    }


                    //6.将拷贝的文件信息相对路径列表保存到资源文件夹(yyyyMMddHHmmss)下的List.txt中
                    ret = SaveFilePathToPublishFolderOfListFile(srcDir, publishFolderDir, out error);
                    if (!ret)
                    {
                        UpdateDisplayList(ret, string.Format("将拷贝的文件信息相对路径列表保存到资源文件夹(yyyyMMddHHmmss)下的List.txt中失败:{0}", error));
                        goto THREADEXIT;
                    }

                    //7.将资源文件夹(yyyyMMddHHmmss)名称和发布序号(时间值)写入到发布路径下的PublishRecords.txt中
                    ret = WritePublishRecordToFile(publishDir, publishFolderName, out error);
                    if (!ret)
                    {
                        UpdateDisplayList(ret, string.Format("资源文件夹(yyyyMMddHHmmss)名称和发布序号(时间值)写入到发布路径下的PublishRecords.txt中失败:{0}", error));
                    }

                THREADEXIT:
                    //8.完成发布
                    string overInfo = ret ? "【发布成功】" : "【发布失败】";
                    UpdateDisplayList(ret, overInfo);

                    DoPublishCompled();

                }, (object)null);


            }, (object)null);
        }

        /// <summary>
        /// 发布完成
        /// </summary>
        private void DoPublishCompled() 
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(DoPublishCompled));
            }
            else 
            {
                if (this.rdoRemotePublish.Checked) 
                {
                    //获取发布路径
                    string publishDir = string.Format(@"{0}\{1}", this.mUCRemotePublishParams.GetRemoteAddress(), this.mUCRemotePublishParams.GetShareFolderName());
                    NetworkShareFileHelper.DisConnection(publishDir);
                }
                this.btnPublish.Enabled = true;
            }
        }

        /// <summary>
        /// 将发布记录写入到发布文件夹下的PublishRecords.txt中
        /// </summary>
        /// <param name="publishDir">发布路径</param>
        /// <param name="publishFolderName">资源文件夹名称(yyyyMMddHHmmss)</param>
        /// <param name="error">错误消息</param>
        /// <returns></returns>
        private  bool WritePublishRecordToFile(string publishDir, string publishFolderName,out string error)
        {
            error = string.Empty;
            try
            {
                string publishRecord = string.Format("{0} {1}", (uint)(Helper.GetDateValue(DateTime.Now) / 1000), publishFolderName);//每条记录的格式为:mSerialNumber mResourceFolderName
                string publishRecordPath = System.IO.Path.Combine(publishDir, "PublishRecords.txt");//PublishRecords.txt文件的完整路径
                using (System.IO.FileStream fs = new System.IO.FileStream(publishRecordPath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.Default))
                    {

                        sw.WriteLine(publishRecord);
                    }

                }
            }
            catch (Exception e)
            {

                error = e.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 保存发布的文件相对路径信息列表到发布文件夹下的List.txt文件中
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="resourceFolderrDir"></param>
        /// <returns></returns>
        private bool SaveFilePathToPublishFolderOfListFile(string srcDir, string publishFolderDir,out string error)
        {
            error = string.Empty;
            try
            {
                string[] fileInfoDirs = System.IO.Directory.GetFiles(srcDir, "*.*", System.IO.SearchOption.AllDirectories);
                var fileInfoList = fileInfoDirs.Select(fileInfoDir => fileInfoDir.Substring(srcDir.Length + 1));

                string listPath = System.IO.Path.Combine(publishFolderDir, "List.txt");//构造List.txt的完整路径

                //写入文件信息相对路径到List.txt中
                using (System.IO.FileStream fs = new System.IO.FileStream(listPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.Default))
                    {
                        foreach (var item in fileInfoList)
                        {
                            sw.WriteLine(item);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新显示列表
        /// </summary>
        /// <param name="info"></param>
        private void UpdateDisplayList(bool flag, string info)
        {
            if (this.richInfo.InvokeRequired)
            {
                this.richInfo.Invoke(new Action<bool, string>(UpdateDisplayList), flag, info);
            }
            else
            {
                this.richInfo.AppendText("\r\n");
                int start = this.richInfo.TextLength;
                this.richInfo.AppendText(info);
                if (!flag)
                {
                    
                    this.richInfo.Select(start, info.Length);
                    this.richInfo.SelectionColor = Color.Red;
                }

                this.richInfo.Select(this.richInfo.TextLength, 0);
                this.richInfo.ScrollToCaret();
            }
        }

        //文件拷贝信息
        void ICopyFileCompled.CopyFileCompled(bool flag, string src, string dest, string error)
        {
            string info = string.Format("将文件{0}拷贝到{1}{2}", src, dest, flag ? "成功" : string.Format("失败:{0}", error));
            this.UpdateDisplayList(flag, info);
        }

    }



  
}
