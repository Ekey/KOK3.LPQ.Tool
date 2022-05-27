using System;

namespace KOK3.Unpacker
{
    class LpqEntry
    {
        public Int32 dwUnknown1 { get; set; } // 1
        public UInt32 dwAttributes { get; set; }
        public Int32 dwUnknown2 { get; set; } // 1
        public UInt32 dwOffset { get; set; }
        public Int32 dwCompressedSize { get; set; }
        public Int32 dwDecompressedSize { get; set; }
        public Int32 dwFileID { get; set; }
        public UInt32 dwFileHash { get; set; }
        public Int32 dwFileTime { get; set; }
    }
}
