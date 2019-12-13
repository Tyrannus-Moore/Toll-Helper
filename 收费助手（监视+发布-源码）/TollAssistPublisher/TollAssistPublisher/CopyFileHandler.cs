using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommTools
{
    /// <summary>
    /// 文件拷贝完成通知
    /// </summary>
    public interface ICopyFileCompled
    {

        /// <summary>
        /// 文件拷贝完成通知
        /// </summary>
        /// <param name="flag">拷贝结果，成功与否</param>
        /// <param name="src">源文件</param>
        /// <param name="dest">目标文件</param>
        /// <param name="error">错误消息</param>
        void CopyFileCompled(bool flag, string src, string dest, string error);
    }

    public sealed class CopyFileHandler
    {
        private ICopyFileCompled mICopyFileCompled;
        public CopyFileHandler(ICopyFileCompled notify)
        {
            this.mICopyFileCompled = notify;
        }

        /// <summary>
        /// 确定一个path指向的是文件还是其他
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsFile(string path)
        {
            return System.IO.File.Exists(path);
        }

        /// <summary>
        /// 确定一个path指向的是文件夹还是其他
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsDirectory(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        private void SendNotify(bool flag, string src, string dest, string error)
        {
            if (this.mICopyFileCompled != null)
                this.mICopyFileCompled.CopyFileCompled(flag, src, dest, error);
        }

        /// <summary>
        /// 拷贝文件或文件夹下内容到指定的目录下；
        /// srcFolder为目录则拷贝其下文件或目录到destFolder,此时destFolder必须为目录;srcFolder为文件则destFolder可以为目录或者文件，为文件时直接拷贝，为目录则拷贝到其下
        /// </summary>
        /// <param name="srcFolder">拷贝源</param>
        /// <param name="destFolder">拷贝目的地</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool CopyFile(string srcFolder, string destFolder, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(srcFolder) || string.IsNullOrWhiteSpace(destFolder))
            {
                return false;
            }

            //有效性判断=>srcFolder必须存在
            if (!System.IO.Directory.Exists(srcFolder))
            {
                if (!System.IO.File.Exists(srcFolder))
                {
                    error = string.Format("指定的路径:{0}无效", srcFolder);
                    return false;
                }
            }

            if (System.IO.File.Exists(srcFolder)) //确定srcFolder为文件
            {
                if (!IsFile(destFolder))//destFolder为路径
                {
                    string fileName = System.IO.Path.GetFileName(srcFolder);//获取文件名
                    string destFile = System.IO.Path.Combine(destFolder, fileName);

                    if (!System.IO.Directory.Exists(destFolder))
                        System.IO.Directory.CreateDirectory(destFolder);

                    try
                    {
                        System.IO.File.Copy(srcFolder, destFile, true);//拷贝
                        SendNotify(true, srcFolder, destFile, null);
                    }
                    catch (Exception ex)
                    {
                        SendNotify(false, srcFolder, destFile, ex.Message);
                    }
                }
                else //destFolder为文件
                {
                    string destDir = System.IO.Path.GetDirectoryName(destFolder);

                    if (!System.IO.Directory.Exists(destDir))
                        System.IO.Directory.CreateDirectory(destDir);

                    try
                    {
                        System.IO.File.Copy(srcFolder, destFolder, true);//拷贝
                        SendNotify(true, srcFolder, destFolder, null);
                    }
                    catch (Exception ex)
                    {
                        SendNotify(false, srcFolder, destFolder, ex.Message);
                    }
                }

                return true;
            }
            else //srcFolder为文件夹,此种情况destFolder必须为路径
            {
                if (!IsFile(destFolder))//destFolder为路径
                {

                    if (!System.IO.Directory.Exists(destFolder))
                        System.IO.Directory.CreateDirectory(destFolder);

                    string[] files = System.IO.Directory.GetFiles(srcFolder);
                    foreach (string file in files)
                    {
                        if (!CopyFile(file, destFolder, out error))
                        {
                            return false;
                        }
                    }
                    string[] dirs = System.IO.Directory.GetDirectories(srcFolder);
                    foreach (string dir in dirs)
                    {

                        if (!CopyFile(dir, System.IO.Path.Combine(destFolder, dir.Substring(dir.LastIndexOf("\\") + 1)), out error))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else //destFolder为文件
                {
                    error = string.Format("{0}不是有效的路径", destFolder);
                    return false;
                }
            }

        }
    }
}
