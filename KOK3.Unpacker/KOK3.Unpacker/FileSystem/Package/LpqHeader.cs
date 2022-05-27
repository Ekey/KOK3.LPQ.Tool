using System;

namespace KOK3.Unpacker
{
    class LpqHeader
    {
        public Int32 dwArchiveSize { get; set; }
        public Int32 dwFlags { get; set; } // 1
        public UInt32 dwHash { get; set; }
        public Int32 dwVersion { get; set; } // 8
        public Int32 dwHeaderSize { get; set; } // 56
        public Int32 dwBlockFiles { get; set; } // equals to dwTotalFiles
        public Int32 dwReserved { get; set; }
        public Int32 dwTotalFiles { get; set; } // equals to dwBlockFiles
        public Int32 dwBlockTableSize { get; set; }
        public Int32 dwHashTableSize { get; set; }
    }
}
