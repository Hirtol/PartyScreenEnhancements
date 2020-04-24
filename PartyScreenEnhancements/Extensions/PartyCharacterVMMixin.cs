using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    //OR PATCH METHODS FOR HINT MANIPULATION, CUSTOM FUNCTIONALITY, ETC
    //TODO: Find way to patch in PartyCharacterVM support in UIExtender Lib 
    // [ViewModelMixin]
    // public class PartyCharacterVMMixin : BaseViewModelMixin<PartyCharacterVM>
    // {
    //     public PartyCharacterVMMixin(PartyCharacterVM vm) : base(vm)
    //     {
    //         this.SpecialUpgradeHint1 = new HintViewModel();
    //     }
    //
    //     [DataSourceMethod]
    //     public void SpecialUpgrade(int upgradeIndex)
    //     {
    //
    //     }
    //
    //
    //     [DataSourceProperty]
    //     public HintViewModel SpecialUpgradeHint1
    //     {
    //         get => _upgradeHint1;
    //         set
    //         {
    //             if (value != _upgradeHint1 && _vm.TryGetTarget(out var character))
    //             {
    //                 _upgradeHint1 = value;
    //                 character.OnPropertyChanged(nameof(SpecialUpgradeHint1));
    //             }
    //         }
    //     }
    //
    //     [DataSourceProperty]
    //     public HintViewModel SpecialUpgradeHint2
    //     {
    //         get => _upgradeHint2;
    //         set
    //         {
    //             if (value != _upgradeHint2 && _vm.TryGetTarget(out var character))
    //             {
    //                 _upgradeHint2 = value;
    //                 character.OnPropertyChanged(nameof(SpecialUpgradeHint2));
    //             }
    //         }
    //     }
    //
    //     private HintViewModel _upgradeHint1;
    //     private HintViewModel _upgradeHint2;
    //
    // }
}
