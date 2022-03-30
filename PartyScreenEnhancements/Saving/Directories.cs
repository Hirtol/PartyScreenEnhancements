using System.IO;
using System.Reflection;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using Path = System.IO.Path;

namespace PartyScreenEnhancements.Saving
{
    public static class Directories
    {
        public static void Initialize()
        {
            Directory.CreateDirectory(Directories.GetConfigPath());
        }

        public static string GetConfigPathForFile(string filename)
        {
            return Path.Combine(Directories.GetConfigPath(), filename);
        }

        public static string GetConfigPath()
        {
            // Credits to Discord user @Sidies from the Modding Discord.
            PropertyInfo propertyInfo = Common.PlatformFileHelper.GetType().GetProperty("DocumentsPath", System.Reflection.BindingFlags.NonPublic
                                                                                                         | System.Reflection.BindingFlags.Instance);
            string documentsFilePath = (string)propertyInfo.GetValue(Common.PlatformFileHelper);

            documentsFilePath = Path.Combine(
                documentsFilePath,
                TaleWorlds.Engine.Utilities.GetApplicationName(),
                EngineFilePaths.ConfigsPath.Path,
                "Mods"
            );

            return documentsFilePath;
        }
    }
}