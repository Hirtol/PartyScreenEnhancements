using System.IO;
using PartyScreenEnhancements.Saving;

namespace PartyScreenEnhancements
{
    public static class Logging
    {
        public static readonly string LOG_FILE = Directories.GetConfigPathForFile("PSE.log");

        public static void Initialise()
        {
            // Just want to reset the log file after every startup to prevent bloat, but allow debugging.
            File.Delete(LOG_FILE);
            File.Create(LOG_FILE);
        }


        public static void Log(Levels level, string message)
        {
            File.AppendAllText(LOG_FILE, $"[{level}] {message}\n");
        }


        public enum Levels
        {
            DEBUG,
            ERROR,
        }
    }
}
