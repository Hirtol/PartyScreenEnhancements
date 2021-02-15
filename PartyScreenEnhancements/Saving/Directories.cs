using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages.FromClient.ToLobbyServer;

namespace PartyScreenEnhancements.Saving
{
    public static class Directories
    {
        public static readonly string MOD_DIR = TaleWorlds.Engine.Utilities.GetConfigsPath() + "Mods" + Path.DirectorySeparatorChar;

        public static void Initialize()
        {
            Directory.CreateDirectory(MOD_DIR);
        }
    }
}
