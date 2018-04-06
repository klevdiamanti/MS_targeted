using System;
using System.IO;

namespace MS_targeted
{
    public static class outputToLog
    {
        public static TextWriter output;

        public static void initializeLogFile(string logfile)
        {
            output = new StreamWriter(@"" + logfile);
        }

        public static void closeLogFile()
        {
            output.Close();
        }

        public static void WriteLine(string line)
        {
            Console.WriteLine(line);
            output.WriteLine(line);
        }

        public static void WriteErrorLine(string line)
        {
            Console.WriteLine(line);
            output.WriteLine(line);
            closeLogFile();
            Environment.Exit(0);
        }
    }
}

