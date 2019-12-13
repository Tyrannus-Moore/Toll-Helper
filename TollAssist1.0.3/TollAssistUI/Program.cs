using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TollAssistUI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           // Application.Run(new FrmCFG());

            #region 单例进程调用实例

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            if (EnableStart())
            {

                Application.Run(new FrmMain());
               //Application.Run(new FrmReport());
            }
            else
            {
                MessageBox.Show("进程已在运行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion

        }


        #region 以下接口均在需要控制进程为单例的时候调用

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                gMutex.ReleaseMutex();
                gMutex.Dispose();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        private static System.Threading.Mutex gMutex = new System.Threading.Mutex(false, "1a2eea10-6943-46a6-84d7-dd45adcff452");
        /// <summary>
        /// 描述是否允许启动应用程序
        /// [内部通过互斥量的方式进行互斥保证只能运行一个进程]
        /// </summary>
        /// <returns></returns>
        private static bool EnableStart()
        {
            try
            {
                return gMutex.WaitOne(2000);
            }
            catch (Exception)
            {

                return false;
            }
        }

        #endregion
    }
}
