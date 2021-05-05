using System.IO;
using System.Text;
using PartyScreenEnhancements.Saving;

namespace PartyScreenEnhancements
{
    public static class Logging
    {
        public static void Initialise()
        {
            // Just want to reset the log file after every startup to prevent bloat, but allow debugging.
            FileManager.DeleteLog();
            FileManager.SaveConfig("");
        }


        public static void Log(Levels level, string message)
        {
            // My god this is an awful solution, but we thankfully only log on errors.
            // I haven't been able to find an easy AppendText for TaleWorld's new way of doing file io.
            var builder = new StringBuilder(FileManager.ReadLog());
            builder.AppendLine($"[{level}] {message} {message.Length}");
            FileManager.SaveLog(builder.ToString());
        }


        public enum Levels
        {
            DEBUG,
            ERROR,
        }
    }
}
