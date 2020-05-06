using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PartyScreenEnhancements.ViewModel.HackedIn;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    [ViewModelMixin]
    public class PartyVMMixin : BaseViewModelMixin<PartyVM>
    {

        private PartyVM _viewModel;
        private MBBindingList<PSEWrapperVM> _categoryList;


        public PartyVMMixin(PartyVM viewModel) : base(viewModel)
        {
            this._categoryList = new MBBindingList<PSEWrapperVM>();

            if (_vm.TryGetTarget(out PartyVM vm))
            {
                _viewModel = vm;
            }
            else
            {
                Utilities.DisplayMessage("Something went wrong when establishing the PSE View, Could not get PartyVM from reference.\nPlease report this issue and disable the mod for now.");
                FileLog.Log("Something went wrong when establishing the PSE View, Could not get PartyVM from reference.\nPlease report this issue and disable the mod for now.");
                return;
            }

            //(_categoryList as IMBBindingList).ListChanged
            _viewModel.MainPartyTroops.ApplyActionOnAllItems(character => _categoryList.Add(new PSEWrapperVM(character)));
            //_categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "", CreateTroopLabel, Category.SYSTEM)));
            _categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "Normal Category", CreateTroopLabel, Category.USER_DEFINED)));
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _categoryList = null;
            _viewModel = null;
        }

        public override void OnRefresh()
        {
            base.OnRefresh();
            
        }

        public string CreateTroopLabel(MBBindingList<PartyCharacterVM> list, int limit = 0)
        {
            int total = list.Sum(item => item.Number);
            int healthy = list.Sum(item => Math.Max(0, item.Number - item.WoundedCount));
            int wounded = list.Sum(item =>
            {
                if (item.Number < item.WoundedCount)
                {
                    return 0;
                }
                return item.WoundedCount;
            });
            MBTextManager.SetTextVariable("COUNT", healthy, false);
            MBTextManager.SetTextVariable("WEAK_COUNT", wounded, false);
            if (total != 0)
            {
                MBTextManager.SetTextVariable("MAX_COUNT", total, false);
                if (wounded > 0)
                {
                    MBTextManager.SetTextVariable("PARTY_LIST_TAG", "", false);
                    MBTextManager.SetTextVariable("WEAK_COUNT", wounded, false);
                    return GameTexts.FindText("str_party_list_label_with_weak", null).ToString();
                }
                MBTextManager.SetTextVariable("PARTY_LIST_TAG", "", false);
                return GameTexts.FindText("str_party_list_label", null).ToString();
            }

            if (wounded > 0)
            {
                return GameTexts.FindText("str_party_list_label_with_weak_without_max", null).ToString();
            }
            return healthy.ToString();
        }

        [DataSourceProperty]
        public MBBindingList<PSEWrapperVM> CategoryList
        {
            get => _categoryList;
            set
            {
                if (value != _categoryList && _vm.TryGetTarget(out var pvm))
                {
                    _categoryList = value;
                    pvm.OnPropertyChanged(nameof(CategoryList));
                }
            }
        }

    }
}
