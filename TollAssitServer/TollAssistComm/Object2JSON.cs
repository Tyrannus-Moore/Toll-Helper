using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;

namespace JSONTool
{
    /// <summary>
    /// 实体对象于JSON字符串之间的转换
    /// </summary>
    public sealed class Object2JSON
    {
        /// <summary>
        /// 将对象序列化为JSON格式字符串
        /// </summary>
        /// <typeparam name="T">需要序列化的对象类型</typeparam>
        /// <param name="obj">需要序列化的对象</param>
        /// <param name="jsonString">返回序列化后的对象</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public static bool ObjectToJSONString<T>(T obj, out string jsonString, out string error) where T : class
        {
            jsonString = null;
            error = null;
            if (obj == null) 
            {
                error = "输入参数obj不能为null";
                return false;
            }
            try
            {
                DataContractJsonSerializer tool = new DataContractJsonSerializer(typeof(T));
                using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
                {
                    tool.WriteObject(memStream, obj);
                    memStream.Position = 0;
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(memStream)) //此处默认utf8编码格式读取数据
                    {
                        jsonString = reader.ReadToEnd();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("执行ObjectToJSONString()发生异常:{0}", ex.Message);
            }
            return false;
        }

        /// <summary>
        /// JSON字符串到对象实体转换
        /// </summary>
        /// <typeparam name="T">需要反序列化的对象类型</typeparam>
        /// <param name="jsonString"> JSON字符串</param>
        /// <param name="obj">反序列化后的对象</param>
        /// <param name="error">错误消息</param>
        /// <returns>成功返回true</returns>
        public static bool JSONStringToObject<T>(string jsonString, out T obj, out string error) where T:class
        {
            obj = null;
            error = null;
            if (string.IsNullOrWhiteSpace(jsonString)) 
            {
                error = "输入参数jsonString不能为空或null";
                return false;
            }
            try
            {
                DataContractJsonSerializer tool = new DataContractJsonSerializer(typeof(T));
                using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
                {
                    byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                    memStream.Write(utf8Bytes, 0, utf8Bytes.Length);
                    memStream.Position = 0;
                    obj = tool.ReadObject(memStream) as T;
                }

                return true;
            }
            catch (Exception ex)
            {
                error = error = string.Format("执行JSONStringToObject()发生异常:{0}", ex.Message);
            }

            return false;
        }
    }
}
