using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace KOK3.Unpacker
{
    class LpqHashList
    {
        static String m_Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static String m_ProjectFile = @"\Projects\FileNames.list";
        static String m_ProjectFilePath = m_Path + m_ProjectFile;
        static Dictionary<String, String> m_HashList = new Dictionary<String, String>();

        public static void iLoadProject()
        {
            String m_Line = null;

            if (!File.Exists(m_ProjectFilePath))
            {
                Utils.iSetError("[ERROR]: Unable to load project file " + m_ProjectFile);
            }
            else
            {
                Int32 i = 0;
                m_HashList.Clear();

                StreamReader TProjectFile = new StreamReader(m_ProjectFilePath);
                while ((m_Line = TProjectFile.ReadLine()) != null)
                {
                    UInt32 dwHashA = LpqHash.iGetHash(m_Line, 256);
                    UInt32 dwHashB = LpqHash.iGetHash(m_Line, 512);
                    String m_Hash = dwHashA.ToString("X8") + dwHashB.ToString("X8");

                    if (m_HashList.ContainsKey(m_Hash))
                    {
                        String m_Collision = null;
                        m_HashList.TryGetValue(m_Hash, out m_Collision);
                        Console.WriteLine("[COLLISION]: {0} <-> {1}", m_Collision, m_Line);
                    }

                    m_HashList.Add(m_Hash, m_Line);
                    i++;
                }

                TProjectFile.Close();
                Console.WriteLine("[INFO]: Project File Loaded: {0}", i);
                Console.WriteLine();
            }
        }

        public static String iGetNameFromHashList(UInt32 dwHashA, UInt32 dwHashB)
        {
            String m_FileName = null;
            String m_Hash = dwHashA.ToString("X8") + dwHashB.ToString("X8");

            if (m_HashList.ContainsKey(m_Hash))
            {
                m_HashList.TryGetValue(m_Hash, out m_FileName);
            }
            else
            {
                m_FileName = @"__Unknown\" + m_Hash;
            }

            return m_FileName;
        }
    }
}