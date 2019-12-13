using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace TollAssistComm
{
    public static class Helper
    {
        private static int loopVale = 0;
        private static DateTime utcTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 利用时间值+回滚值生成long值
        /// </summary>
        /// <returns></returns>
        public static long GetID()
        {
            int sinceSeconds = (int)((DateTime.Now - utcTime).TotalSeconds);
            int incrementVar = System.Threading.Interlocked.Increment(ref loopVale);
            byte[] mem_value = new byte[sizeof(long)];
            byte[] sinceSeconds_buff = BitConverter.GetBytes(sinceSeconds);
            byte[] incrementVar_buff = BitConverter.GetBytes(incrementVar);
            Array.Copy(sinceSeconds_buff, 0, mem_value, 0, sinceSeconds_buff.Length);
            Array.Copy(incrementVar_buff, 0, mem_value, sinceSeconds_buff.Length, incrementVar_buff.Length);

            return BitConverter.ToInt64(mem_value, 0);
        }

        /// <summary>
        /// 返回从1970-1-1起开始经过的时间毫秒值(UTC)
        /// </summary>
        /// <param name="currTime"></param>
        /// <returns></returns>
        public static long GetDateValue(DateTime currTime)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            TimeSpan span = currTime.Subtract(new TimeSpan(8, 0, 0)) - dt;
            return (long)span.TotalMilliseconds;
        }

        /// <summary>
        /// 返回东八区表示的时间日期对象
        /// </summary>
        /// <param name="currTime">从1970-1-1起开始经过的时间毫秒值(UTC)</param>
        /// <returns></returns>
        public static DateTime GetDateTime(long currTime)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            TimeSpan span = TimeSpan.FromMilliseconds(currTime);
            TimeSpan china_span = new TimeSpan(8, 0, 0);
            TimeSpan total_span = span.Add(china_span);

            return dt.Add(total_span);

        }

        /// <summary>
        /// 获取当前进程的执行路径不包括进程名称
        /// [不推荐使用]
        /// </summary>
        /// <returns>进程路径</returns>
        public static string GetCurrentProcessPath() 
        {

            string fileName=System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            fileName = fileName.Substring(0, fileName.LastIndexOf('\\'));

            return fileName;

        }




        #region WIN32API

        private const uint LIST_MODULES_32BIT = 0x01;
        private const uint LIST_MODULES_64BIT = 0x02;
        private const uint LIST_MODULES_ALL = 0x03;
        private const uint PROCESS_VM_READ = (0x0010);
        private const uint PROCESS_QUERY_INFORMATION = (0x0400);
        private const uint PROCESS_QUERY_LIMITED_INFORMATION = (0x1000);
        private const uint PROCESS_ALL_ACCESS = (0x000F0000) | (0x00100000) | (0xFFFF);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, uint bInheritHandle, int dwProcessId);

        [DllImport("Kernel32.dll")]
        private static extern int CloseHandle(IntPtr hObject);

        [DllImport("Psapi.dll")]
        private static extern int EnumProcessModulesEx(IntPtr hProcess, IntPtr[] lphModule, int cb, ref int lpcbNeeded, uint dwFilterFlag);


        [DllImport("Psapi.dll")]
        private static extern uint GetModuleFileNameExA(IntPtr hProcess, IntPtr hModule, StringBuilder sb, uint nSize);

        [DllImport("Kernel32.dll")]
        private static extern int QueryFullProcessImageNameA(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

        [DllImport("Psapi.dll")]
        private static extern int GetProcessImageFileNameA(IntPtr hProcess, StringBuilder lpExeName, uint lpdwSize);

        [DllImport("Kernel32.dll")]
        private static extern uint QueryDosDeviceA(string deviceName, StringBuilder targetPath, uint ucchMax); 

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        #endregion

        /// <summary>
        /// 获取进程的路径
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="processPath"></param>
        /// <returns></returns>
        public static string GetProcessPath(int processId)
        {
            string processPath = null;

            IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, 0, processId);
            if (processHandle == IntPtr.Zero)
            {
                return processPath;
            }

            StringBuilder pathSBuilder = new StringBuilder(1024);

            //尝试采用以下方式获取程序路径
            pathSBuilder.Clear();
            uint lpdwSize = 1024;

            if (Environment.OSVersion.Version.Major >= 6) //WIN7及以上系统
            {

                if (QueryFullProcessImageNameA(processHandle, 0, pathSBuilder, ref lpdwSize) != 0)//获取路径成功
                {
                    processPath = pathSBuilder.ToString();
                    goto EXIT;
                }
            }
            else 
            {
                if (GetProcessImageFileNameA(processHandle, pathSBuilder, lpdwSize) != 0)//获取路径成功
                {
                    Console.WriteLine("GetProcessImageFileNameA=>{0}", pathSBuilder.ToString());
                    string devicePath = pathSBuilder.ToString();
                    //此函数获取的是驱动器路径，还需要再次转换
                    string[] logicalDevNames = Environment.GetLogicalDrives();
                    string deviceName = null;
                    foreach (string logicalDevName in logicalDevNames)
                    {
                        deviceName = logicalDevName.Substring(0, logicalDevName.IndexOf(':')+1);
                        pathSBuilder.Clear();
                        if (QueryDosDeviceA(deviceName, pathSBuilder, (uint)pathSBuilder.Capacity) > 0) 
                        {
                            if (devicePath.IndexOf(pathSBuilder.ToString()) == 0) //找到目标
                            {
                                processPath = deviceName + devicePath.Substring(pathSBuilder.Length);
                                Console.WriteLine("processPath={0}", processPath);
                                goto EXIT;
                            }
                        }
                    }


                    //processPath = pathSBuilder.ToString();
                    //goto EXIT;
                }
            }

           

            //尝试采用以下方式再次获取路径
           
            IntPtr[] lphModule = new IntPtr[1024];
            int lpcbNeeded = 0;

            if (EnumProcessModulesEx(processHandle, lphModule, IntPtr.Size * lphModule.Length, ref lpcbNeeded, LIST_MODULES_ALL) == 0)//枚举模块失败
            {
                //uint er = GetLastError();
                goto EXIT;
            }

            if (GetModuleFileNameExA(processHandle, lphModule[0], pathSBuilder, 1024) != 0) //获取成功
            {
                processPath = pathSBuilder.ToString();
            }

        EXIT:
            CloseHandle(processHandle);
            return processPath;
        }

        /// <summary>
        /// 获取本机首个IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetHostIPOfFrist() 
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostent = Dns.GetHostByName(hostName);
            IPAddress[] addrs = hostent.AddressList;

            string strIp = string.Empty;

            if (addrs.Length > 0) 
            {
                strIp = addrs[0].ToString();
            }

            return strIp;

        }

        /// <summary>
        /// 验证IP地址是否合法
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool ValidateIPAddress(string ipAddress)
        {
            Regex validipregex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            return ((!string.IsNullOrWhiteSpace(ipAddress)) && validipregex.IsMatch(ipAddress.Trim())) ? true : false;
        }

        ///// <summary>
        ///// 为指定的程序创建快捷方式,并返回创建的快捷方式的完整路径
        ///// </summary>
        ///// <param name="path">可执行文件的完整路径</param>
        ///// <param name="shortcutName">快捷方式的名称</param>
        ///// <param name="shortcutPath">快捷方式的完整路径</param>
        ///// <returns>如果快捷方式已经存在或者输入参数无效则返回false</returns>
        //public static bool CreateShortcut(string path, string shortcutName, out string shortcutPath)
        //{
        //    shortcutPath = null;
        //    if (string.IsNullOrWhiteSpace(path))
        //        return false;

        //    if (!System.IO.File.Exists(path))
        //        return false;
        //    string extensionName = System.IO.Path.GetExtension(path);
        //    if (string.IsNullOrWhiteSpace(extensionName) || extensionName != ".exe")
        //        return false;
        //    string exeName = System.IO.Path.GetFileNameWithoutExtension(path);
        //    if (string.IsNullOrWhiteSpace(exeName))
        //        return false;

        //    if (string.IsNullOrWhiteSpace(shortcutName))
        //    {
        //        shortcutName = exeName;
        //    }

        //    string exeDir = System.IO.Path.GetDirectoryName(path);
        //    string shortcutFullPath = System.IO.Path.Combine(exeDir, shortcutName + ".lnk");

        //    //if (System.IO.File.Exists(shortcutFullPath))
        //    //    return false;


        //    int windowStyle = 1;//1:普通 3:最大化 7:最小化

        //    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
        //    IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutFullPath);
        //    shortcut.TargetPath = path;
        //    shortcut.WorkingDirectory = exeDir;
        //    shortcut.WindowStyle = windowStyle;
        //    shortcut.Description = "Auto Create";
        //    shortcut.Save();

        //    shortcutPath = shortcutFullPath;

        //    return true;
        //}

        /// <summary>
        /// 将程序设置为自动启动
        /// 备注：如果已经存在该项不会添加，如果不存在则添加
        /// </summary>
        /// <param name="keyName">注册表项名称(程序名称)</param>
        /// <param name="path">要自动启动的程序的完整路径</param>
        public static bool SetAutoStart(string keyName, string path)
        {
            if (string.IsNullOrWhiteSpace(keyName) || string.IsNullOrWhiteSpace(path))
                return false;
            string regeditItemPath = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run";
            try
            {
                Object objPath = Registry.GetValue(regeditItemPath, keyName, null);
                if (objPath == null)
                {
                    //添加到注册表中
                    Registry.SetValue(regeditItemPath, keyName, path, RegistryValueKind.String);
                }
                else
                {
                    string strPath = objPath as string;
                    if (strPath != path)
                    {
                        //更新注册表
                        Registry.SetValue(regeditItemPath, keyName, path, RegistryValueKind.String);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        

    }
}
