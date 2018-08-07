using System;
using System.Diagnostics;
using System.IO;

namespace MS_targeted
{
    public static class outputToLog
    {
        public static string logFile { get; set; }
        public static TextWriter output;
        private static Stopwatch countTime;

        public static void initializeLogFile(string _logfile)
        {
            logFile = _logfile;
            countTime = new Stopwatch();
            countTime.Start();
            output = new StreamWriter(@"" + _logfile);
            output.WriteLine("MS_targeted analysis started " + DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss"));
        }

        public static void closeLogFile()
        {
            output.WriteLine("MS_targeted analysis finished " + DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss"));
            countTime.Stop();
            output.WriteLine(string.Format("Elapsed time {0:00}h {1:00}min {2:00}sec {3:00}msec",
                countTime.Elapsed.Hours, countTime.Elapsed.Minutes, countTime.Elapsed.Seconds, countTime.Elapsed.Milliseconds / 10));
            output.Close();
        }

        public static void WriteLine(string line)
        {
            Console.WriteLine(line);
            output.WriteLine(line);
        }

        public static void WriteErrorLine(string line)
        {
            Console.WriteLine("ERROR: " + line + "! Exiting!");
            output.WriteLine(line);
            closeLogFile();
            Environment.Exit(0);
        }

        public static void WriteWarningLine(string line)
        {
            Console.WriteLine("WARNING: " + line + "!");
            output.WriteLine(line);
        }
    }
}

