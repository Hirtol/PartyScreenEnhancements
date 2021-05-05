using System;
using System.IO;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using Path = System.IO.Path;

namespace PartyScreenEnhancements.Saving
{
    public static class FileManager
    {
        public static readonly string MOD_DIR_RELATIVE = EngineFilePaths.ConfigsDirectoryName + Path.DirectorySeparatorChar + "Mods";
        public static PlatformDirectoryPath DEFAULT_MOD_DIR => new PlatformDirectoryPath(PlatformFileType.User, MOD_DIR_RELATIVE);
        public static PlatformFilePath DEFAULT_LOG_FILE => new PlatformFilePath(DEFAULT_MOD_DIR, "PSE.log");
        public static PlatformFilePath DEFAULT_CONF_FILE => new PlatformFilePath(DEFAULT_MOD_DIR, "PartyScreenEnhancements.xml");

        public static void Initialize()
        {
            
        }

        public static void SaveConfig(string data)
        {
            FileHelper.SaveFileString(DEFAULT_CONF_FILE, data);
        }

        public static string ReadConfig()
        {
            return FileHelper.GetFileContentString(DEFAULT_CONF_FILE);
        }

        public static bool ConfigExists()
        {
            return FileHelper.FileExists(DEFAULT_CONF_FILE);
        }

        public static void SaveLog(string data)
        {
            FileHelper.SaveFileString(DEFAULT_LOG_FILE, data);
        }

        public static string ReadLog()
        {
            return FileHelper.GetFileContentString(DEFAULT_LOG_FILE);
        }

        public static void DeleteLog()
        {
            FileHelper.DeleteFile(DEFAULT_LOG_FILE);
        }
    }
}