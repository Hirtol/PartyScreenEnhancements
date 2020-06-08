using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    [PrefabExtension("PartyScreen", "descendant::PartyHeaderToggleWidget[@Id='MainPartyTroopsToggleWidget']")]
    public class CategoryAdditionPartyPatch : PrefabExtensionInsertPatch
    {
        public override int Position => 3;
        public override string Name => "CategoryAddition";
    }
}
