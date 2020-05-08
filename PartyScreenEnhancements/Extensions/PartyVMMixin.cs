using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PartyScreenEnhancements.Saving;
using PartyScreenEnhancements.ViewModel.HackedIn;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem.Save;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    [ViewModelMixin]
    public class PartyVMMixin : BaseViewModelMixin<PartyVM>
    {

        //TODO: Make own transfer (drag and drop) methods.
        // No need for the button clicks, as those should be handled by our eventhandler of ListChanged
        public delegate void VisualUpdateDelegate(MBBindingList<PSEWrapperVM> list);
        public event VisualUpdateDelegate Update;

        private PartyVM _viewModel;
        private PartyScreenLogic _logic;
        private MBBindingList<PSEWrapperVM> _categoryList;
        private MBBindingList<PSEWrapperVM> _privateCategoryList;
        private Dictionary<int, PartyCharacterVM> _indexToParty;


        public PartyVMMixin(PartyVM viewModel) : base(viewModel)
        {
            PartyScreenConfig.TroopCategoryBindings.Add("sturgian_recruit", "Normal Category");
            this._categoryList = new MBBindingList<PSEWrapperVM>();
            this._privateCategoryList = new MBBindingList<PSEWrapperVM>();
            this._indexToParty = new Dictionary<int, PartyCharacterVM>();

            if (_vm.TryGetTarget(out PartyVM vm))
            {
                _viewModel = vm;
                _logic = GetPrivate<PartyScreenLogic>("_partyScreenLogic");
            }
            else
            {
                Utilities.DisplayMessage("Something went wrong when establishing the PSE View, Could not get PartyVM from reference.\nPlease report this issue and disable the mod for now.");
                FileLog.Log("Something went wrong when establishing the PSE View, Could not get PartyVM from reference.\nPlease report this issue and disable the mod for now.");
                return;
            }

            _wrapper = new PSEListWrapperVM(this, _viewModel);
            this.Update += UpdateLabel;
            InitialiseCategories();

            //(_viewModel.MainPartyTroops as IMBBindingList).ListChanged += PartyVMMixin_ListChanged;
            
            
            // _viewModel.MainPartyTroops.ApplyActionOnAllItems(character => _categoryList.Add(new PSEWrapperVM(character)));
            // //_categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "", CreateTroopLabel, Category.SYSTEM)));
            // _categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "Normal Category", CreateTroopLabel, Category.USER_DEFINED)));
            
        }

        public void OnShiftTroop(PartyCharacterVM characterVm, int newIndex)
        {
            if(characterVm.Side == PartyScreenLogic.PartyRosterSide.None) return;
            _viewModel.CurrentCharacter = characterVm;
            if (ValidateShift(characterVm, newIndex))
            {
                var character = characterVm.Character;
                if (characterVm.Type == PartyScreenLogic.TroopType.Member)
                {
                    int num = this.MemberRosters[(int)command.RosterSide].FindIndexOfTroop(character);
                    TroopRosterElement elementCopyAtIndex = this.MemberRosters[(int)command.RosterSide].GetElementCopyAtIndex(num);
                    int num2 = (num < command.Index) ? (command.Index - 1) : command.Index;
                    this.MemberRosters[(int)command.RosterSide].AddToCounts(character, -elementCopyAtIndex.Number, false, -elementCopyAtIndex.WoundedNumber, 0, true, num);
                    this.MemberRosters[(int)command.RosterSide].AddToCounts(character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, elementCopyAtIndex.Xp, true, (this.MemberRosters[(int)command.RosterSide].Count < num2) ? -1 : num2);
                }
                else
                {
                    Utilities.DisplayMessage("You may not shift prisoners!");
                    throw new NotImplementedException("You may not shift prisoners!");
                }


                this.Update?.Invoke(GetPartyList(characterVm.Side));
            }
        }

        public void OnTransferTroop(PartyCharacterVM character, int newIndex, int characterNumber, PartyScreenLogic.PartyRosterSide characterSide, string targetList)
        {
            throw new NotImplementedException();
        }

        public void WrapperShift(PSEWrapperVM wrapper, int newIndex)
        {
            throw new NotImplementedException();
        }

        //TODO: Clean this one up

        private bool ValidateShift(PartyCharacterVM character, int index)
        {
            if (character.Character == CharacterObject.PlayerCharacter) return false;
            int num;
            if (character.Type == PartyScreenLogic.TroopType.Member)
            {
                num = _logic.MemberRosters[(int)character.Side].FindIndexOfTroop(character.Character);
                return num != -1 &&  index != 0 && num != index;
            }
            num = _logic.PrisonerRosters[(int)character.Side].FindIndexOfTroop(character.Character);
            return num != -1 && num != index;
        }

        private MBBindingList<PSEWrapperVM> GetPartyList(PartyScreenLogic.PartyRosterSide side)
        {
            if (side == PartyScreenLogic.PartyRosterSide.Right)
            {
                return _categoryList;
            }
            else
            {
                throw new NotImplementedException("PSE encountered unknown side");
            }
        }

        public void UpdateLabel(MBBindingList<PSEWrapperVM> list)
        {

        }

        //TODO: FIX REMOVE
        // private void PartyVMMixin_ListChanged(object sender, ListChangedEventArgs e)
        // {
        //     var partyList = sender as MBBindingList<PartyCharacterVM>;
        //
        //     switch (e.ListChangedType)
        //     {
        //         case ListChangedType.ItemAdded:
        //             if(partyList != null)
        //             {
        //                 var character = partyList[e.NewIndex];
        //                 var categoryAdd = FindRelevantCategory(character?.Character?.StringId);
        //
        //                 _indexToParty.Clear();
        //
        //                 for (var i = 0; i < partyList.Count; i++)
        //                 {
        //                     _indexToParty.Add(i, _viewModel.MainPartyTroops[i]);
        //                 }
        //
        //                 if (categoryAdd != null)
        //                 {
        //                     categoryAdd.TroopList.Add(character);
        //                 }
        //                 else
        //                 {
        //                     var test = new PSEWrapperVM(character);
        //                     CategoryList.Add(test);
        //                 }
        //             }
        //             break;
        //         case ListChangedType.ItemChanged:
        //             Utilities.DisplayMessage("PSE Unsupported operation just occured, please notify Mod Dev.");
        //             break;
        //         case ListChangedType.ItemDeleted:
        //             var removedChar = _indexToParty[e.NewIndex];
        //             var categoryRemove = FindRelevantCategory(removedChar?.Character?.StringId);
        //
        //             if (categoryRemove != null)
        //             {
        //                 categoryRemove.TroopList.Remove(removedChar);
        //             }
        //             else
        //             {
        //                 var test = new PSEWrapperVM(removedChar);
        //                 CategoryList.Remove(test);
        //             }
        //
        //             _indexToParty.Remove(e.NewIndex);
        //             break;
        //         case ListChangedType.Reset:
        //         case ListChangedType.Sorted:
        //             _categoryList.Clear();
        //             _indexToParty.Clear();
        //             InitialiseCategories();
        //             break;
        //     }
        // }

        //TODO: Patch <AutoScrollablePanelWidget Id="MainPartyScrollablePanel" WidthSizePolicy="Fixed" HeightSizePolicy="StretchToParent" SuggestedWidth="!PartyToggle.Width" HorizontalAlignment="Left" VerticalAlignment="Bottom" MarginLeft="!SidePanel.ScrollablePanel.MarginHorizontal" MarginTop="!SidePanel.ScrollablePanel.MarginTop" MarginBottom="!SidePanel.ScrollablePanel.MarginBottom" AcceptDrop="true" AutoHideScrollBars="true" ClipRect="MyClipRect" Command.Drop="ExecuteTransferWithParameters" CommandParameter.Drop="MainParty" InnerPanel="MyClipRect\MainPartyInnerPanel" VerticalScrollbar="..\MainPartyScrollbar\Scrollbar">
        // Actually, if we automatically intercept ListChangedType.Add it shouldn't matter!

        // [DataSourceMethod]
        // public void ExecutePSETransferWithParameters(TaleWorlds.Library.ViewModel party, int index, string targetTag)
        // {
        //     Utilities.DisplayMessage("Hello World" + party);
        //     if (party is PartyCharacterVM character)
        //     {
        //     
        //     }
        //     else if(party is PSEWrapperVM wrapper)
        //     {
        //     
        //     }
        // }

        [DataSourceMethod]
        public void ExecutePSETransferWithParameters()
        {
            Utilities.DisplayMessage("Hello World");
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

        [DataSourceProperty]
        public PSEListWrapperVM PSEListWrapper
        {
            get => _wrapper;
            set
            {
                if (value != _wrapper)
                {
                    _wrapper = value;
                    _viewModel.OnPropertyChanged(nameof(PSEListWrapper));
                }
            }
        }

        private PSEListWrapperVM _wrapper;
    }
}
