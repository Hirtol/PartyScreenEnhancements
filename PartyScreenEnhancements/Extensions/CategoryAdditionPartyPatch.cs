using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    [PrefabExtension("PartyScreen", "descendant::PartyHeaderToggleWidget[@Id='MainPartyTroopsToggleWidget']/Children")]
    public class CategoryAdditionPartyPatch : PrefabExtensionInsertPatch
    {
        public override int Position => PositionLast;
        public override string Name => "CategoryAddition";
    }
}
