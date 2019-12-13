using System.Runtime.InteropServices;
using System;
public class StructConvert
	{
		/// <summary>
        /// 将字节数组转换为对象
        /// </summary>
        /// <param name="buffer">包含对象数据的数组</param>
        /// <param name="offset">开始转换位置</param>
        /// <param name="size">转换的长度</param>
        /// <param name="head">对象</param>
        /// <returns></returns>
        public static bool ByteArrayToObject<T>(byte[] buffer, int offset, int size, out T head) where T:struct
        {
            head = default(T);
            int structSZ = Marshal.SizeOf(typeof(T));
            if (size < structSZ) return false;
            IntPtr ptr = Marshal.AllocHGlobal(structSZ);
            Marshal.Copy(buffer, offset, ptr, size);
            head = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return true;
        }

        /// <summary>
        /// 将字节数组转换为对象(内部不使用额外缓冲区)
        /// </summary>
        /// <param name="buffer">包含对象数据的数组</param>
        /// <param name="offset">开始转换位置</param>
        /// <param name="size">转换的长度</param>
        /// <param name="head">对象</param>
        /// <returns></returns>
        public static bool ByteArrayToObjectEx<T>(byte[] buffer, int offset, int size, out T head) where T:struct
        {
            head = default(T);
            int structSZ = Marshal.SizeOf(typeof(T));
            if (size < structSZ) return false;
            GCHandle g=GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr p = g.AddrOfPinnedObject();
            IntPtr obj_p=IntPtr.Add(p, offset);
            head=(T)Marshal.PtrToStructure(obj_p, typeof(T));
            g.Free();
            return true;
        }

        /// <summary>
        /// 将对象转换为字节数组
        /// </summary>
        /// <param name="head">对象</param>
        /// <param name="buffer">存放对象的数组</param>
        /// <param name="offset">指定存放到数组的开始位置</param>
        /// <param name="size">长度</param>
        /// <returns></returns>
        public static bool ObjectToBytes<T>(ref T head, byte[] buffer, int offset, int size) where T:struct
        {
            int structSZ = Marshal.SizeOf(typeof(T));
            if (size < structSZ) return false;
            IntPtr ptr = Marshal.AllocHGlobal(structSZ);
            Marshal.StructureToPtr(head, ptr, true);
            Marshal.Copy(ptr, buffer, offset, size);
            Marshal.FreeHGlobal(ptr);
            return true;
        }

        /// <summary>
        /// 将对象转换为字节数组(内部不使用额外缓冲区)
        /// </summary>
        /// <param name="head">对象</param>
        /// <param name="buffer">存放对象的数组</param>
        /// <param name="offset">指定存放到数组的开始位置</param>
        /// <param name="size">长度</param>
        /// <returns></returns>
        public static bool ObjectToBytesEx<T>(ref T head, byte[] buffer, int offset, int size) where T:struct
        {
            int structSZ = Marshal.SizeOf(typeof(T));
            if (size < structSZ) return false;
            GCHandle g = GCHandle.Alloc(head, GCHandleType.Pinned);
            IntPtr p = g.AddrOfPinnedObject();
            Marshal.Copy(p, buffer, offset, structSZ);
            g.Free();
            return true;
        }
	}