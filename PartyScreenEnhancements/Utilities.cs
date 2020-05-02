using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PartyScreenEnhancements
{
    public static class Utilities
    {

        public static void DisplayMessage(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message, Color.White));
        }

    }
}
