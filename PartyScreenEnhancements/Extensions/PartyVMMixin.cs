using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PartyScreenEnhancements.Saving;
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

        //TODO: Make own transfer (drag and drop) methods.
        // No need for the button clicks, as those should be handled by our eventhandler of ListChanged

        private PartyVM _viewModel;
        private MBBindingList<PSEWrapperVM> _categoryList;
        private MBBindingList<PSEWrapperVM> _privateCategoryList;
        private Dictionary<int, PartyCharacterVM> _indexToParty;


        public PartyVMMixin(PartyVM viewModel) : base(viewModel)
        {
            this._categoryList = new MBBindingList<PSEWrapperVM>();
            this._privateCategoryList = new MBBindingList<PSEWrapperVM>();
            this._indexToParty = new Dictionary<int, PartyCharacterVM>();

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

            InitialiseCategories();

            (_viewModel.MainPartyTroops as IMBBindingList).ListChanged += PartyVMMixin_ListChanged;
            // _viewModel.MainPartyTroops.ApplyActionOnAllItems(character => _categoryList.Add(new PSEWrapperVM(character)));
            // //_categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "", CreateTroopLabel, Category.SYSTEM)));
            // _categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "Normal Category", CreateTroopLabel, Category.USER_DEFINED)));
        }

        //TODO: FIX REMOVE
        private void PartyVMMixin_ListChanged(object sender, ListChangedEventArgs e)
        {
            var partyList = sender as MBBindingList<PartyCharacterVM>;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if(partyList != null)
                    {
                        var character = partyList[e.NewIndex];
                        var categoryAdd = FindRelevantCategory(character?.Character?.StringId);

                        _indexToParty.Clear();

                        for (var i = 0; i < partyList.Count; i++)
                        {
                            _indexToParty.Add(i, _viewModel.MainPartyTroops[i]);
                        }

                        if (categoryAdd != null)
                        {
                            categoryAdd.TroopList.Add(character);
                        }
                        else
                        {
                            var test = new PSEWrapperVM(character);
                            CategoryList.Add(test);
                        }
                    }
                    break;
                case ListChangedType.ItemChanged:
                    Utilities.DisplayMessage("PSE Unsupported operation just occured, please notify Mod Dev.");
                    break;
                case ListChangedType.ItemDeleted:
                    var removedChar = _indexToParty[e.NewIndex];
                    var categoryRemove = FindRelevantCategory(removedChar?.Character?.StringId);

                    if (categoryRemove != null)
                    {
                        categoryRemove.TroopList.Remove(removedChar);
                    }
                    else
                    {
                        var test = new PSEWrapperVM(removedChar);
                        CategoryList.Remove(test);
                    }

                    _indexToParty.Remove(e.NewIndex);
                    break;
                case ListChangedType.Reset:
                case ListChangedType.Sorted:
                    _categoryList.Clear();
                    _indexToParty.Clear();
                    InitialiseCategories();
                    break;
            }
        }


        public void ExecutePSETransferWithParameters(PSEWrapperVM party, int index, string targetTag)
        {

        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _categoryList = null;
            _viewModel = null;
            _privateCategoryList = null;
            _indexToParty = null;
        }

        public override void OnRefresh()
        {
            base.OnRefresh();
            
        }

        public void InitialiseCategories()
        {
            for (var i = 0; i < _viewModel.MainPartyTroops.Count; i++)
            {
                _indexToParty.Add(i, _viewModel.MainPartyTroops[i]);
            }

            Utilities.DisplayMessage("Init called");
            var names = PartyScreenConfig.TroopCategoryBindings.Values.Distinct();

            if(_privateCategoryList.IsEmpty())
            {
                foreach (var name in names)
                {
                    _privateCategoryList.Add(new PSEWrapperVM(new PartyCategoryVM(new MBBindingList<PartyCharacterVM>(),
                        name,
                        CreateTroopLabel, Category.USER_DEFINED)));
                }
            }

            foreach (PartyCharacterVM character in _viewModel.MainPartyTroops)
            {
                var id = character?.Character?.StringId ?? "NULL";
                var relevantCategory = FindRelevantCategory(id);

                if(relevantCategory != null)
                {
                    if(!relevantCategory.TroopList.Contains(character))
                    relevantCategory.TroopList.Add(character);
                }
                else
                {
                    _categoryList.Add(new PSEWrapperVM(character));
                }
            }

            //TODO: Save order of the _categoryList and reapply here.
            _privateCategoryList.ApplyActionOnAllItems(wrapper => _categoryList.Add(wrapper));
        }

        public PartyCategoryVM FindRelevantCategory(string id)
        {
            if (!_categoryList.IsEmpty() && PartyScreenConfig.TroopCategoryBindings.TryGetValue(id, out var category))
            {
                return _privateCategoryList.FirstOrDefault(wrapper =>
                {
                    if (wrapper.WrapperViewModel is PartyCategoryVM cat)
                    {
                        return cat.Label.Equals(category);
                    }

                    return false;
                })?.WrapperViewModel as PartyCategoryVM;
            }

            return null;
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
