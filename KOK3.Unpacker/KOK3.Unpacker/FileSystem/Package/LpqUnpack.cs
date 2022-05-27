using System;
using System.IO;
using System.Collections.Generic;

namespace KOK3.Unpacker
{
    class LpqUnpack
    {
        static List<LpqHash> m_HashTable = new List<LpqHash>();
        static List<LpqEntry> m_EntryTable = new List<LpqEntry>();

        static Int32 iReadPad(FileStream TFileReader)
        {
            Int32 dwCheck = 0;
            do
            {
                if (TFileReader.Position == TFileReader.Length)
                {
                    return 0;
                }
                else if (TFileReader.Position + 4 == TFileReader.Length)
                {
                    return 0;
                }
                else
                {
                    dwCheck = TFileReader.ReadInt32();
                }
            }
            while (dwCheck == 0 || dwCheck == -1 || dwCheck == -2);

            TFileReader.Position -= 4;

            return 1;
        }

        public static void iDoIt(String m_HeaderFile, String m_DstFolder)
        {
            //Initialze hashing cryptography
            LpqHash.iInitializeCryptography();

            //Loading project list
            LpqHashList.iLoadProject();

            using (FileStream THeaderStream = File.OpenRead(m_HeaderFile))
            {

                var m_Header = new LpqHeader();
                m_Header.dwArchiveSize = THeaderStream.ReadInt32();
                m_Header.dwFlags = THeaderStream.ReadInt32();
                m_Header.dwHash = THeaderStream.ReadUInt32();
                m_Header.dwVersion = THeaderStream.ReadInt32();
                m_Header.dwHeaderSize = THeaderStream.ReadInt32();
                m_Header.dwBlockFiles = THeaderStream.ReadInt32();
                m_Header.dwReserved = THeaderStream.ReadInt32();
                m_Header.dwTotalFiles = THeaderStream.ReadInt32();
                m_Header.dwBlockTableSize = THeaderStream.ReadInt32();
                m_Header.dwBlockTableSize -= 56 - 4;

                if (m_Header.dwVersion != 8)
                {
                    Utils.iSetError("[ERROR]: Invalid version of LPQ archive file");
                    return;
                }

                THeaderStream.Seek(m_Header.dwHeaderSize, SeekOrigin.Begin);

                m_EntryTable.Clear();
                for (Int32 i = 0; i < m_Header.dwTotalFiles; i++)
                {
                    Int32 dwUnknown1 = THeaderStream.ReadInt32();
                    UInt32 dwAttributes = THeaderStream.ReadUInt32();
                    Int32 dwUnknown2 = THeaderStream.ReadInt32();
                    UInt32 dwOffset = THeaderStream.ReadUInt32();
                    Int32 dwCompressedSize = THeaderStream.ReadInt32();
                    Int32 dwDecompressedSize = THeaderStream.ReadInt32();
                    Int32 dwFileID = THeaderStream.ReadInt32();
                    UInt32 dwFileHash = THeaderStream.ReadUInt32();
                    Int32 dwFileTime = THeaderStream.ReadInt32();

                    var m_Entry = new LpqEntry
                    {
                        dwUnknown1 = dwUnknown1,
                        dwAttributes = dwAttributes,
                        dwUnknown2 = dwUnknown2,
                        dwOffset = dwOffset,
                        dwCompressedSize = dwCompressedSize,
                        dwDecompressedSize = dwDecompressedSize,
                        dwFileID = dwFileID,
                        dwFileHash = dwFileHash,
                        dwFileTime = dwFileTime
                    };

                    m_EntryTable.Add(m_Entry);
                }

                THeaderStream.Position += 4;

                m_HashTable.Clear();
                do
                //for (Int32 i = 0; i < m_Header.dwTotalFiles; i++)
                {
                    //Reading hash table
                    Int32 dwResult = iReadPad(THeaderStream);
                    if (dwResult == 0)
                    {
                        break;
                    }

                    UInt32 dwHashIndex = THeaderStream.ReadUInt32();
                    UInt32 dwHashA = THeaderStream.ReadUInt32();
                    UInt32 dwHashB = THeaderStream.ReadUInt32();
                    Int32 dwFileID = THeaderStream.ReadInt32();
                    Int32 dwIndexID = THeaderStream.ReadInt32();

                    var m_Hash = new LpqHash
                    {
                        dwHashIndex = dwHashIndex,
                        dwHashA = dwHashA,
                        dwHashB = dwHashB,
                        dwFileID = dwFileID,
                        dwIndexID = dwIndexID,
                    };

                    m_HashTable.Add(m_Hash);
                }
                while (THeaderStream.Position != THeaderStream.Length);

                String m_ArchiveFile = m_HeaderFile.Replace(".bhl", ".lpq");

                if (!File.Exists(m_ArchiveFile))
                {
                    Utils.iSetError("[ERROR]: Input archive -> " + m_ArchiveFile + " <- does not exist");
                    return;
                }

                using (FileStream TArchiveStream = File.OpenRead(m_ArchiveFile))
                {
                    for (Int32 i = 0; i < m_HashTable.Count; i++)
                    {
                        String m_FileName = LpqHashList.iGetNameFromHashList(m_HashTable[i].dwHashA, m_HashTable[i].dwHashB);
                        String m_FullPath = m_DstFolder + m_FileName;
                        Utils.iCreateDirectory(m_FullPath);

                        Console.WriteLine("[UNPACKING]: {0}", m_FileName);

                        TArchiveStream.Seek(m_EntryTable[m_HashTable[i].dwFileID].dwOffset, SeekOrigin.Begin);
                        if (m_EntryTable[i].dwCompressedSize == m_EntryTable[m_HashTable[i].dwFileID].dwDecompressedSize)
                        {
                            var lpSrcBuffer = TArchiveStream.ReadBytes(m_EntryTable[m_HashTable[i].dwFileID].dwCompressedSize);
                            File.WriteAllBytes(m_FullPath, lpSrcBuffer);
                        }
                        else
                        {
                            UInt32 dwSrcSize = (UInt32)m_EntryTable[m_HashTable[i].dwFileID].dwCompressedSize;
                            UInt32 dwDestSize = (UInt32)m_EntryTable[m_HashTable[i].dwFileID].dwDecompressedSize;

                            var lpSrcBuffer = TArchiveStream.ReadBytes(m_EntryTable[m_HashTable[i].dwFileID].dwCompressedSize);
                            Byte[] lpDstBuffer = new Byte[m_EntryTable[m_HashTable[i].dwFileID].dwDecompressedSize];

                            LZO1X.iDecompress(lpSrcBuffer, dwSrcSize, lpDstBuffer, ref dwDestSize);

                            File.WriteAllBytes(m_FullPath, lpDstBuffer);
                        }
                    }

                    TArchiveStream.Dispose();
                }
            }
        }
    }
}