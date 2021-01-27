using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;

namespace PartyScreenEnhancements
{
    public static class Logging
    {
        public static readonly string LOG_FILE = Directories.MOD_DIR + "PSE.log";

        public static void Initialise()
        {
            // Just want to reset the log file after every startup to prevent bloat, but allow debugging.
            File.Delete(LOG_FILE);
            File.Create(LOG_FILE);
        }


        public static void Log(Levels level, string message)
        {
            File.AppendText($"[{level.ToString()}] {message}");
        }


        public enum Levels
        {
            DEBUG,
            ERROR,
        }
    }
}
