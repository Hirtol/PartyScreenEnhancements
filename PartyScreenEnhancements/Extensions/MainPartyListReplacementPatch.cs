using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    [PrefabExtension("PartyScreen", "descendant::Widget[@Id='MainPartyTroopsList']")]
    public class MainPartyListReplacementPatch : PrefabExtensionReplacePatch
    {
        public override string Name => "MainListReplacement";
    }
}
