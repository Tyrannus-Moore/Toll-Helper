using System.Runtime.InteropServices;
using System;
public class StructConvert
	{
		/// <summary>
        /// ���ֽ�����ת��Ϊ����
        /// </summary>
        /// <param name="buffer">�����������ݵ�����</param>
        /// <param name="offset">��ʼת��λ��</param>
        /// <param name="size">ת���ĳ���</param>
        /// <param name="head">����</param>
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
        /// ���ֽ�����ת��Ϊ����(�ڲ���ʹ�ö��⻺����)
        /// </summary>
        /// <param name="buffer">�����������ݵ�����</param>
        /// <param name="offset">��ʼת��λ��</param>
        /// <param name="size">ת���ĳ���</param>
        /// <param name="head">����</param>
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
        /// ������ת��Ϊ�ֽ�����
        /// </summary>
        /// <param name="head">����</param>
        /// <param name="buffer">��Ŷ��������</param>
        /// <param name="offset">ָ����ŵ�����Ŀ�ʼλ��</param>
        /// <param name="size">����</param>
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
        /// ������ת��Ϊ�ֽ�����(�ڲ���ʹ�ö��⻺����)
        /// </summary>
        /// <param name="head">����</param>
        /// <param name="buffer">��Ŷ��������</param>
        /// <param name="offset">ָ����ŵ�����Ŀ�ʼλ��</param>
        /// <param name="size">����</param>
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