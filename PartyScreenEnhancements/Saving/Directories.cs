using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyScreenEnhancements.Saving
{
    public static class Directories
    {
        public static readonly string MOD_DIR = TaleWorlds.Engine.Utilities.GetConfigsPath() + "Mods" + Path.DirectorySeparatorChar;
    }
}
