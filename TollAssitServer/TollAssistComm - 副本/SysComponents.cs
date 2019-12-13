using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TollAssistComm
{
    /// <summary>
    /// 系统组件
    /// </summary>
    public static class SysComponents
    {
        /// <summary>
        /// 注册组件
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="component"></param>
        /// <returns></returns>
		public static bool RegComponent(string componentName,Object component)
        {
            if (!SysComponents.mComponents.ContainsKey(componentName)) 
            {
                SysComponents.mComponents.Add(componentName, component);
                return true;
            }
            return false;
        }
		
        /// <summary>
        /// 反注册组件(将组件移除)
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns></returns>
		public static bool UnRegComponent(string componentName)
        {
            if (SysComponents.mComponents.ContainsKey(componentName))
            {
               return SysComponents.mComponents.Remove(componentName);
            }
            return false;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns>成功返回指定名称的组件</returns>
        public static Object GetComponent(string componentName) 
        {
            if (SysComponents.mComponents.ContainsKey(componentName))
            {
                return SysComponents.mComponents[componentName];
            }

            return false;
        }

        
		private static Dictionary<string,Object> mComponents=new Dictionary<string,object>();//组件集合
    }


    
}
