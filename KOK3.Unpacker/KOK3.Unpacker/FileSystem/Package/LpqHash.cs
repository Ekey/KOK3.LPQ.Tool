using System;
using System.Text;

namespace KOK3.Unpacker
{
    class LpqHash
    {
        public UInt32 dwHashIndex { get; set; }
        public UInt32 dwHashA { get; set; }
        public UInt32 dwHashB { get; set; }
        public Int32 dwFileID { get; set; }
        public Int32 dwIndexID { get; set; }

        public static UInt32[] lpStormBuffer = new UInt32[1280];

        public static void iInitializeCryptography()
        {
            Int32 i;
            UInt32 dwSeed = 0x00100001;
            UInt32 dwIndex1 = 0;
            UInt32 dwIndex2 = 0;

            for (dwIndex1 = 0; dwIndex1 < 256; dwIndex1++)
            {
                for (dwIndex2 = dwIndex1, i = 0; i < 5; i++, dwIndex2 += 256)
                {
                    UInt32 dwTemp1, dwTemp2;

                    dwSeed = (dwSeed * 125 + 3) % 0x2aaaab;
                    dwTemp1 = (dwSeed & 0xffff) << 0x10;

                    dwSeed = (dwSeed * 125 + 3) % 0x2aaaab;
                    dwTemp2 = (dwSeed & 0xffff);

                    lpStormBuffer[dwIndex2] = (dwTemp1 | dwTemp2);
                }
            }
        }

        public static UInt32 iGetHash(String m_FileName, Int32 dwHashType)
        {
            UInt32 dwSeed1 = 0x7FED7FED;
            UInt32 dwSeed2 = 0xEEEEEEEE;

            var m_NameData = Encoding.ASCII.GetBytes(m_FileName.ToUpper());
            for (Int32 i = 0; i < m_FileName.Length; i++)
            {
                dwSeed1 = lpStormBuffer[dwHashType + m_NameData[i]] ^ (dwSeed1 + dwSeed2);
                dwSeed2 = m_NameData[i] + dwSeed1 + dwSeed2 + (dwSeed2 << 5) + 3;
            }

            return dwSeed1;
        }
    }
}