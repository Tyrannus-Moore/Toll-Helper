using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TollAssistComm
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NETRESOURCE
    {
        public uint dwScope;
        public uint dwType;
        public uint dwDisplayType;
        public uint dwUsage;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpLocalName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpRemoteName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpComment;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpProvider;
    }

    /// <summary>
    /// 网络共享文件帮助类
    /// </summary>
    public static class NetworkShareFileHelper
    {
        private const uint NO_ERROR = 0;
        private const uint ERROR_ALREADY_ASSIGNED = 85;
        private const uint RESOURCETYPE_ANY = 0;
        private const uint RESOURCETYPE_DISK = 1;
        private const uint RESOURCETYPE_PRINT = 2;
        private const uint CONNECT_INTERACTIVE = 8;

        private const uint CONNECT_UPDATE_PROFILE = 1;

        private const uint RESOURCE_GLOBALNET = 2;
        private const uint RESOURCEDISPLAYTYPE_SHARE = 3;
        private const uint RESOURCEUSAGE_CONNECTABLE = 1;

        [DllImport("Mpr.dll", CharSet = CharSet.Ansi)]
        public static extern uint WNetAddConnection2A(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, uint dwFlags);

        [DllImport("Mpr.dll", CharSet = CharSet.Ansi)]
        public static extern uint WNetCancelConnection2A(string lpName, uint dwFlags, uint fForce);

        /// <summary>
        /// 打开远程连接
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool OpenConnection(string remoteAddress, string userName, string password)
        {
            NETRESOURCE ns = new NETRESOURCE();
            ns.dwScope = RESOURCE_GLOBALNET;
            ns.dwDisplayType = RESOURCEDISPLAYTYPE_SHARE;
            ns.dwUsage = RESOURCEUSAGE_CONNECTABLE;
            ns.dwType = RESOURCETYPE_ANY;
            ns.lpLocalName = string.Empty;
            ns.lpRemoteName = remoteAddress;
            ns.lpProvider = string.Empty;
            uint ret = WNetAddConnection2A(ref ns, password, userName, CONNECT_INTERACTIVE);

            return ret == NO_ERROR;
            //return WNetAddConnection2A(ref ns, password, userName, CONNECT_INTERACTIVE) == NO_ERROR;
        }

        /// <summary>
        /// 关闭远程连接
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <returns></returns>
        public static bool DisConnection(string remoteAddress)
        {
            return WNetCancelConnection2A(remoteAddress, CONNECT_UPDATE_PROFILE, 0) == NO_ERROR;
        }


    }
}
