using System;
using System.Runtime.InteropServices;

namespace KOK3.Unpacker
{
    class LZO1X
    {
        private static readonly Boolean _Is64Bit = DetectIs64Bit();

        private static Boolean DetectIs64Bit()
        {
            return Marshal.SizeOf(IntPtr.Zero) == 8;
        }

        private static class Native32
        {
            [DllImport("lzo1x_32.dll", EntryPoint = "#67", CallingConvention = CallingConvention.StdCall)]
            internal static extern Int32 NativeCompress(Byte[] inbuf, UInt32 inlen, Byte[] outbuf, ref UInt32 outlen, Byte[] workbuf);

            [DllImport("lzo1x_32.dll", EntryPoint = "#68", CallingConvention = CallingConvention.StdCall)]
            internal static extern Int32 NativeDecompress(Byte[] inbuf, UInt32 inlen, Byte[] outbuf, ref UInt32 outlen);
        }

        private static class Native64
        {
            [DllImport("lzo1x_64.dll", EntryPoint = "#67", CallingConvention = CallingConvention.StdCall)]
            internal static extern Int32 NativeCompress(Byte[] inbuf, UInt32 inlen, Byte[] outbuf, ref UInt32 outlen, Byte[] workbuf);

            [DllImport("lzo1x_64.dll", EntryPoint = "#68", CallingConvention = CallingConvention.StdCall)]
            internal static extern Int32 NativeDecompress(Byte[] inbuf, UInt32 inlen, Byte[] outbuf, ref UInt32 outlen);
        }

        private const Int32 lzo_sizeof_dict_t = 2;
        private const Int32 LZO1X_1_MEM_COMPRESS = (16384 * lzo_sizeof_dict_t);

        private static byte[] CompressWork = new byte[LZO1X_1_MEM_COMPRESS];

        public static Int32 iDecompress(Byte[] lpScrBuffer, UInt32 dwZSize, Byte[] lpDstBuffer, ref UInt32 dwSize)
        {
            if (_Is64Bit == true)
            {
                return Native64.NativeDecompress(lpScrBuffer, dwZSize, lpDstBuffer, ref dwSize);
            }

            return Native32.NativeDecompress(lpScrBuffer, dwZSize, lpDstBuffer, ref dwSize);
        }
    }
}
