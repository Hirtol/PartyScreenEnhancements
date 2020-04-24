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

    // [PrefabExtension("MapBar", "descendant::ListPanel[@Sprite='mapbar_left_canvas']/Children")]
    // public class MapBarExtension : PrefabExtensionInsertPatch
    // {
    //     public override int Position => 3;
    //     public override string Name => "TestingTesting";
    // }
    //
    // [ViewModelMixin]
    // public class MapNavigationVMExtension : BaseViewModelMixin<MapNavigationVM>
    // {
    //     private HintViewModel _campHint = new HintViewModel("Establish camp");
    //
    //     [DataSourceProperty] public bool IsCampEnabled => MobileParty.MainParty.Party.Settlement == null;
    //     [DataSourceProperty] public HintViewModel CampHintText => _campHint;
    //
    //     public MapNavigationVMExtension(MapNavigationVM vm) : base(vm)
    //     {
    //     }
    //
    //     [DataSourceMethod]
    //     public void ExecuteOpenCamp()
    //     {
    //         Trace.WriteLine("Hello");
    //     }
    // }
}
