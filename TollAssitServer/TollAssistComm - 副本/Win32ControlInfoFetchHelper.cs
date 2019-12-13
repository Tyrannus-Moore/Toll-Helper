using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TollAssistComm
{
    public static class Win32ControlInfoFetchHelper
    {
        #region WIN32 API

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;
        }


        public delegate int EnumChildProc(IntPtr hwnd, int lParam);

        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT p);

        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, POINT p, uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT p);

        [DllImport("user32.dll")]
        public static extern IntPtr RealChildWindowFromPoint(IntPtr hwndParent, POINT ptParentClientCoords);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT rect);

        [DllImport("user32.dll")]
        public static extern int GetClientRect(IntPtr hwnd, ref RECT rect);

        [DllImport("user32.dll")]
        public static extern int GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll")]
        public static extern int GetCursorPos(ref POINT p);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder sb, int maxCount);

        [DllImport("Oleacc.dll")]
        public static extern IntPtr GetProcessHandleFromHwnd(IntPtr hwnd);

        [DllImport("Kernel32.dll")]
        public static extern int GetProcessId(IntPtr process);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, ref uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        private const uint GA_PARENT = 1;
        private const uint GA_ROOT = 2;
        private const uint GA_ROOTOWNER = 3;





        //private const uint LIST_MODULES_32BIT = 0x01;
        //private const uint LIST_MODULES_64BIT = 0x02;
        //private const uint LIST_MODULES_ALL = 0x03;
        //private const uint PROCESS_VM_READ = (0x0010);
        //private const uint PROCESS_QUERY_INFORMATION = (0x0400);
        //private const uint PROCESS_QUERY_LIMITED_INFORMATION = (0x1000);
        //private const uint PROCESS_ALL_ACCESS = (0x000F0000) | (0x00100000) | (0xFFFF);

        //[DllImport("Kernel32.dll")]
        //private static extern IntPtr OpenProcess(uint dwDesiredAccess, uint bInheritHandle, int dwProcessId);

        //[DllImport("Kernel32.dll")]
        //private static extern int CloseHandle(IntPtr hObject);

        //[DllImport("Psapi.dll")]
        //private static extern int EnumProcessModulesEx(IntPtr hProcess, IntPtr[] lphModule, int cb, ref int lpcbNeeded, uint dwFilterFlag);


        //[DllImport("Psapi.dll")]
        //private static extern uint GetModuleFileNameExA(IntPtr hProcess, IntPtr hModule, StringBuilder sb, uint nSize);

        //[DllImport("Kernel32.dll")]
        //private static extern int QueryFullProcessImageNameA(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);


        //[DllImport("Psapi.dll")]
        //private static extern int GetProcessImageFileNameA(IntPtr hProcess, StringBuilder lpExeName, uint lpdwSize);

        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();



        public const int HWND_BOTTOM = 1;
        public const int HWND_NOTOPMOST = -2;
        public const int HWND_TOP = 0;
        public const int HWND_TOPMOST = -1;

        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_NOSIZE= 0x0001;
        public const uint SWP_NOMOVE = 0x0002;

        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();


        private const uint WM_GETTEXT = 0x000D;
        private const uint CWP_ALL = 0x0000;
        private const uint CWP_SKIPINVISIBLE = 0x0001;
        private const uint CWP_SKIPDISABLED = 0x0002;
        private const uint CWP_SKIPTRANSPARENT = 0x0004;


        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int GetWindowTextEx(IntPtr hwnd, uint msg, uint wParam, StringBuilder sb);

        #endregion


        /// <summary>
        /// 根据屏幕坐标获取控件的句柄和文本控件的文本串
        /// </summary>
        /// <param name="point">控件所在的屏幕坐标</param>
        /// <param name="handle">控件的句柄</param>
        /// <param name="txt">控件的文本内容</param>
        /// <returns></returns>
        public static bool GetControlInfo(System.Drawing.Point point, out IntPtr handle, out string txt) 
        {

            POINT p = new POINT() { x = point.X, y = point.Y };
            IntPtr hwnd = WindowFromPoint(p);
            if (hwnd == IntPtr.Zero)
            {
                handle = IntPtr.Zero;
                txt = string.Empty;
                return false;
            }


            IntPtr rs_hwnd = GetChildHwnd(hwnd, p);
            
            if (rs_hwnd == IntPtr.Zero)
                rs_hwnd = hwnd;

            handle = rs_hwnd;

           return GetText(handle,out txt);
        }

        /// <summary>
        /// 根据屏幕坐标获取控件的句柄
        /// </summary>
        /// <param name="point">控件所在的屏幕坐标</param>
        /// <returns></returns>
        public static IntPtr GetControlByPos(System.Drawing.Point point) 
        {
            POINT p = new POINT() { x = point.X, y = point.Y };
            IntPtr hwnd = WindowFromPoint(p);
            if (hwnd == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }


            IntPtr rs_hwnd = GetChildHwnd(hwnd, p);

            if (rs_hwnd == IntPtr.Zero)
                rs_hwnd = hwnd;

            return  rs_hwnd;

        }

        /// <summary>
        /// 根据窗口句柄获取进程的路径
        /// </summary>
        /// <param name="hwnd">进程所拥有的窗口的句柄</param>
        /// <returns></returns>
        public static string GetProcessPathByHwnd(IntPtr hwnd) 
        {
            //IntPtr processHandle = GetProcessHandleFromHwnd(hwnd);
            //if (processHandle == IntPtr.Zero)
            //    return string.Empty;

            //int processId = GetProcessId(processHandle);
            //if (processId == 0)
            //    return string.Empty;

            uint processId = 0;
            IntPtr rootHwnd = GetAncestor(hwnd, GA_ROOT);
            if (rootHwnd == IntPtr.Zero)
            {
                rootHwnd = GetAncestor(hwnd, GA_ROOTOWNER);
            }
            if (rootHwnd != IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref processId);
              
            }

            return TollAssistComm.Helper.GetProcessPath((int)processId);

        }

        ///// <summary>
        ///// 获取进程的路径
        ///// </summary>
        ///// <param name="processId"></param>
        ///// <param name="processPath"></param>
        ///// <returns></returns>
        //public static string GetProcessPath(int processId)
        //{
        //    string processPath = null;

        //    IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, 0, processId);
        //    if (processHandle == IntPtr.Zero)
        //    {
        //        return processPath;
        //    }

        //    StringBuilder pathSBuilder = new StringBuilder(1024);

        //    //尝试采用以下方式获取程序路径
        //    pathSBuilder.Clear();
        //    uint lpdwSize = 1024;
        //    if (Environment.OSVersion.Version.Major >= 6) //WIN7及以上系统
        //    {

        //        if (QueryFullProcessImageNameA(processHandle, 0, pathSBuilder, ref lpdwSize) != 0)//获取路径成功
        //        {
        //            processPath = pathSBuilder.ToString();
        //            goto EXIT;
        //        }
        //    }
        //    else
        //    {
        //        if (GetProcessImageFileNameA(processHandle, pathSBuilder, lpdwSize) != 0)//获取路径成功
        //        {
        //            //此函数获取的是驱动器路径，还需要再次转换
        //            //TODO

        //            processPath = pathSBuilder.ToString();
        //            goto EXIT;
        //        }
        //    }

        //    //尝试采用以下方式再次获取路径

        //    IntPtr[] lphModule = new IntPtr[1024];
        //    int lpcbNeeded = 0;

        //    if (EnumProcessModulesEx(processHandle, lphModule, IntPtr.Size * lphModule.Length, ref lpcbNeeded, LIST_MODULES_ALL) == 0)//枚举模块失败
        //    {
        //        //uint er = GetLastError();
        //        goto EXIT;
        //    }

        //    if (GetModuleFileNameExA(processHandle, lphModule[0], pathSBuilder, 1024) != 0) //获取成功
        //    {
        //        processPath = pathSBuilder.ToString();
        //    }

        //EXIT:
        //    CloseHandle(processHandle);
        //    return processPath;
        //}


        public static IntPtr GetChildHwnd(IntPtr parentHwnd, POINT p)
        {
            if (parentHwnd == IntPtr.Zero) return parentHwnd;

            RECT rect = new RECT();
            //获取父窗体的所在位置
            if (GetWindowRect(parentHwnd, ref rect) != 0)
            {
                //获取点击点的相对父控件的坐标
                POINT clientP = new POINT();
                clientP.x = p.x - rect.left;
                clientP.y = p.y - rect.top;

                //获取子窗体句柄
                IntPtr _parentHwnd = parentHwnd;
                IntPtr hwnd = ChildWindowFromPointEx(parentHwnd, clientP, CWP_ALL);

                if (hwnd != IntPtr.Zero && hwnd != parentHwnd)
                {
                    return GetChildHwnd(hwnd, p);
                }
                else
                {
                    return hwnd;
                }

            }
            else
            {
                return parentHwnd;
            }
        }

        private static bool GetText(IntPtr hwnd,out string txt)
        {
            StringBuilder textContent = new StringBuilder(255);
            int cnt = GetWindowTextEx(hwnd, WM_GETTEXT, 255, textContent);
            if (cnt > 0)
            {
                txt = textContent.ToString();
                return true;
            }
            else
            {
                txt = string.Empty;
                return false;
            }

        }

    }

    #region API声明
    public  class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 x, Int32 y)
            {
                cx = x;
                cy = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public const byte AC_SRC_OVER = 0;
        public const Int32 ULW_ALPHA = 2;
        public const byte AC_SRC_ALPHA = 1;

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int DeleteObject(IntPtr hObj);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr ExtCreateRegion(IntPtr lpXform, uint nCount, IntPtr rgnData);
    }
    #endregion

}
