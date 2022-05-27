using System;
using System.IO;

namespace KOK3.Unpacker
{
    class Program
    {
        private static String m_Title = "King of Kings 3 LPQ Unpacker";     

        static void Main(String[] args)
        {
            Console.Title = m_Title;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(m_Title);
            Console.WriteLine("(c) 2022 Ekey (h4x0r) / v{0}\n", Utils.iGetApplicationVersion());
            Console.ResetColor();

            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[Usage]");
                Console.WriteLine("    KOK3.Unpacker <m_File> <m_Directory>\n");
                Console.WriteLine("    m_File - Source of BHL/LQP file");
                Console.WriteLine("    m_Directory - Destination directory\n");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Examples]");
                Console.WriteLine("    KOK3.Unpacker E:\\Games\\King of Kings 3\\config.lpq D:\\Unpacked");
                Console.ResetColor();
                return;
            }

            String m_HeaderFile = Utils.iCheckFile(args[0]);
            String m_Output = Utils.iCheckArgumentsPath(args[1]);

            if (!File.Exists(m_HeaderFile))
            {
                Utils.iSetError("[ERROR]: Input file -> " + m_HeaderFile + " <- does not exist");
                return;
            }

            LpqUnpack.iDoIt(m_HeaderFile, m_Output);
        }
    }
}
