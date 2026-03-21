using System;
using System.IO;
using System.Linq;

namespace Boostera
{
    internal class Logger
    {
        private string logFolder;
        private string logFileName;
        private int maxLines;
        private string logFilePath => Path.Combine(logFolder, logFileName);
        private readonly object logLock = new object();

        internal Logger(string logFolder, string logFileName, int maxLines)
        {   
            this.logFolder = logFolder;
            this.logFileName = logFileName;
            this.maxLines = maxLines;

            if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
        }

        internal void LogException(Exception ex)
        {
            lock (logLock)
            {
                try
                {
                    File.AppendAllText(logFilePath, ex.ToString());
                    TrimLog();
                }
                catch { }
            }
        }

        private void TrimLog()
        {
            lock (logLock)
            {
                try
                {
                    var lines = File.ReadLines(logFilePath).ToList();
                    if (lines.Count > maxLines)
                    {
                        File.WriteAllLines(logFilePath, lines.Skip(lines.Count - maxLines));
                    }
                }
                catch { }
            }
        }
    }
}
