using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Patches
{

    [PrefabExtension("PartyTroopTuple", "descendant::Widget[@Id='Main']/Children")]
    public class PartyScreenExtensions : PrefabExtensionInsertPatch
    {
        public override int Position => PositionLast;
        public override string Name => "TestingTesting";
    }
}
