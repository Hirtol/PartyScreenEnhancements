using System.IO;

namespace PartyScreenEnhancements.Saving
{
    public static class Directories
    {
        public static readonly string MOD_DIR =
            TaleWorlds.Engine.Utilities.GetConfigsPath() + "Mods" + Path.DirectorySeparatorChar;

        public static void Initialize()
        {
            Directory.CreateDirectory(MOD_DIR);
        }
    }
}