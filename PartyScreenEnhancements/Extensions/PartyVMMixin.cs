using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using UIExtenderLib;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    [ViewModelMixin]
    public class PartyVMMixin : BaseViewModelMixin<PartyVM>
    {
        private const int _rightSide = (int) PartyScreenLogic.PartyRosterSide.Right;
        private const int _leftSide = (int) PartyScreenLogic.PartyRosterSide.Left;

        //TODO: Make own transfer (drag and drop) methods.
        // No need for the button clicks, as those should be handled by our eventhandler of ListChanged
        public delegate void VisualUpdateDelegate(MBBindingList<PSEWrapperVM> list);
        public event VisualUpdateDelegate Update;

        private PartyVM _viewModel;
        private PartyScreenLogic _logic;
        private PSEListWrapperVM _wrapper;

        private MBBindingList<PSEWrapperVM> _mainPartyWrappers;
        private MBBindingList<PSEWrapperVM> _privateCategoryList;

        private IList<PartyCharacterVM> _indexToParty;

        private bool _reset;


        public PartyVMMixin(PartyVM viewModel) : base(viewModel)
        {
            this._reset = false;
            this._mainPartyWrappers = new MBBindingList<PSEWrapperVM>();
            this._privateCategoryList = new MBBindingList<PSEWrapperVM>();
            this._indexToParty = new List<PartyCharacterVM>();
            
            this.CategoryRosters = new List<MBBindingList<PSEWrapperVM>>() { _mainPartyWrappers };

            if (_vm.TryGetTarget(out PartyVM vm))
            {
                _viewModel = vm;
                _logic = new Traverse(_viewModel).Field<PartyScreenLogic>("_partyScreenLogic").Value;
            }
            else
            {
                Utilities.DisplayMessage("Something went wrong when establishing the PSE View, Could not get PartyVM from reference.\nPlease report this issue and disable the mod for now.");
                FileLog.Log("Something went wrong when establishing the PSE View, Could not get PartyVM from reference.\nPlease report this issue and disable the mod for now.");
                return;
            }

            this._wrapper = new PSEListWrapperVM(this, _viewModel);
            this.Update += UpdateLabels;
            InitialiseCategories();

            (_viewModel.MainPartyTroops as IMBBindingList).ListChanged += PartyVMMixin_ListChanged;
            
            
            // _viewModel.MainPartyTroops.ApplyActionOnAllItems(character => _categoryList.Add(new PSEWrapperVM(character)));
            // //_categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "", CreateTroopLabel, Category.SYSTEM)));
            // _categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "Normal Category", CreateTroopLabel, Category.USER_DEFINED)));
            
        }

        private void RefreshTopInformation()
        {
            _viewModel.MainPartyTotalWeeklyCostLbl = MobileParty.MainParty.GetTotalWage(1f, null).ToString();
            _viewModel.MainPartyTotalGoldLbl = Hero.MainHero.Gold.ToString();
            _viewModel.MainPartyTotalMoraleLbl = ((int)MobileParty.MainParty.Morale).ToString("##.0");
            _viewModel.MainPartyTotalSpeedLbl = CampaignUIHelper.FloatToString(MobileParty.MainParty.ComputeSpeed());
        }

        //TODO: Fix cast exception when selecting PartyCharacterVM on the left side after transfering from the right.
        public void OnTransferTroop(PartyCharacterVM character, int newIndex, int characterNumber,
            PartyScreenLogic.PartyRosterSide characterSide, PartyCategoryVM fromCategory, string targetList)
        {
            if(newIndex < 0) return;

            var characterWrapper = new PSEWrapperVM(character);

            PartyScreenConfig.TroopCategoryBindings.Remove(character.Character.StringId);

            if (fromCategory != null)
                fromCategory.TroopList.Remove(character);
            else
            {
                _mainPartyWrappers.Remove(characterWrapper);
            }

            // To Category
            if (targetList.StartsWith(PartyCategoryVM.CATEGORY_LABEL_PREFIX))
            {
                PartyCategoryVM targetCategory = GetCategoryFromName(targetList);

                if (targetCategory == null)
                {
                    this._mainPartyWrappers.Add(characterWrapper);
                    Utilities.DisplayMessage("PSE Attempted to add to category which doesn't exist!");
                }
                else
                {
                    PartyScreenConfig.TroopCategoryBindings.Add(character.Character.StringId, targetCategory.Label);

                    if (characterSide == PartyScreenLogic.PartyRosterSide.Left)
                    {
                        //TODO: Make transfer respect your decided Index
                        var indexToUse = newIndex;
                        if (indexToUse <= 0)
                            indexToUse = 1;
                        character.OnTransfer(character, indexToUse, characterNumber, characterSide);
                        character.ThrowOnPropertyChanged();
                        this.RefreshTopInformation();
                        _viewModel.ExecuteRemoveZeroCounts();
                        return;
                    }

                    InsertIntoBindingList(character, newIndex + 1, targetCategory.TroopList);

                    character.ThrowOnPropertyChanged();
                    this.RefreshTopInformation();
                }
            }
            // To Main List
            else
            {
                if (newIndex == 0)
                    newIndex = -1;

                newIndex++;

                if (ValidateShift(character, newIndex) && character.Type == PartyScreenLogic.TroopType.Member && characterSide == PartyScreenLogic.PartyRosterSide.Left)
                {
                    var newSide = characterSide == PartyScreenLogic.PartyRosterSide.Right
                        ? PartyScreenLogic.PartyRosterSide.Left
                        : PartyScreenLogic.PartyRosterSide.Right;

                    if (newSide == PartyScreenLogic.PartyRosterSide.Right)
                    {
                        character.OnTransfer(character, newIndex, characterNumber, characterSide);
                        _viewModel.CurrentCharacter = character;
                        return;
                    }

                    character.ThrowOnPropertyChanged();
                    this.RefreshTopInformation();
                }
                else
                {
                    OnShiftTroop(character, newIndex);
                }
            }

            Update?.Invoke(_mainPartyWrappers);
        }

        public void OnShiftTroop(PartyCharacterVM characterVm, int newIndex)
        {
            if (characterVm.Side == PartyScreenLogic.PartyRosterSide.None) return;
            _viewModel.CurrentCharacter = characterVm;

            if (ValidateShift(characterVm, newIndex))
            {
                if (characterVm.Type == PartyScreenLogic.TroopType.Member)
                {
                    var sideList = GetPartyList(characterVm.Side);

                    InsertIntoBindingList(new PSEWrapperVM(characterVm), newIndex, sideList);

                    characterVm.ThrowOnPropertyChanged();
                    this.RefreshTopInformation();
                }
                else
                {
                    Utilities.DisplayMessage("You may not shift prisoners!");
                    throw new NotImplementedException("You may not shift prisoners!");
                }
            }
        }

        public void CategoryShift(PartyCategoryVM category, int newIndex)
        {
            if(!IsValidIndex(newIndex)) return;

            var sideList = GetPartyList(PartyScreenLogic.PartyRosterSide.Right);

            InsertIntoBindingList(new PSEWrapperVM(category), newIndex, sideList);

            this.RefreshTopInformation();
        }

        public void WrapperShift(PSEWrapperVM wrapper, int newIndex)
        {
            var sideList = GetPartyList(PartyScreenLogic.PartyRosterSide.Right);

            InsertIntoBindingList(wrapper, newIndex, sideList);

            this.RefreshTopInformation();
        }

        public void InsertIntoBindingList<T>(T model, int newIndex, MBBindingList<T> list)
        {
            var indexOfTroop = list.IndexOf(model);
            
            if(indexOfTroop != -1)
                list.RemoveAt(indexOfTroop);

            if (list.Count < newIndex)
            {
                list.Add(model);
            }
            else
            {
                int index = (indexOfTroop < newIndex) ? (newIndex - 1) : newIndex;
                list.Insert(index, model);
            }
        }

        private MBBindingList<PSEWrapperVM> GetPartyList(PartyScreenLogic.PartyRosterSide side)
        {
            if (side == PartyScreenLogic.PartyRosterSide.Right)
            {
                return _mainPartyWrappers;
            }
            else
            {
                throw new NotImplementedException("PSE encountered unknown side");
            }
        }

        public void UpdateLabels(MBBindingList<PSEWrapperVM> list)
        {
            var enumerable = list.Where(wrapper => wrapper.IsCategory);
            foreach (var wrapper in enumerable)
            {
                (wrapper.WrapperViewModel as PartyCategoryVM).UpdateLabel();
            }
        }

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
                         
                         if(e.NewIndex >= _indexToParty.Count)
                            _indexToParty.Add(character);
                         else
                             _indexToParty.Insert(e.NewIndex, character);

                         if (categoryAdd != null)
                         {
                             categoryAdd.TroopList.Add(character);
                             categoryAdd.UpdateLabel();
                         }
                         else
                         {
                             if (e.NewIndex >= MainPartyWrappers.Count)
                                 MainPartyWrappers.Add(new PSEWrapperVM(character));
                             else
                             {
                                 if(e.NewIndex > 0)
                                     MainPartyWrappers.Insert(e.NewIndex - 1, new PSEWrapperVM(character));
                                 else
                                     MainPartyWrappers.Insert(e.NewIndex, new PSEWrapperVM(character));
                             }
                                 
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
                         categoryRemove.UpdateLabel();
                     }
                     else
                     {
                         MainPartyWrappers.Remove(new PSEWrapperVM(removedChar));
                     }
        
                     _indexToParty.RemoveAt(e.NewIndex);

                     break;
                 case ListChangedType.Reset:
                     Utilities.DisplayMessage("Hey, reset!");
                     _reset = true;
                     _mainPartyWrappers.Clear();
                     _indexToParty.Clear();
                     _privateCategoryList.Clear();
                     InitialiseCategories();
                     break;
                case ListChangedType.Sorted:
                    Utilities.DisplayMessage("Hey, Sort!");
                    _mainPartyWrappers.Clear();
                     _indexToParty.Clear();
                     _privateCategoryList.Clear();
                     InitialiseCategories();
                     break;
             }
         }

         public void InitialiseCategories()
        {
            for (var i = 0; i < _viewModel.MainPartyTroops.Count; i++)
            {
                _indexToParty.Add(_viewModel.MainPartyTroops[i]);
            }

            var names = PartyScreenConfig.TroopCategoryBindings.Values.Distinct();

            if(_privateCategoryList.IsEmpty())
            {
                foreach (var name in names)
                {
                    _privateCategoryList.Add(new PSEWrapperVM(new PartyCategoryVM(new MBBindingList<PartyCharacterVM>(),
                        name,
                        "MainPartyTroops")));
                }
            }

            foreach (PartyCharacterVM character in _viewModel.MainPartyTroops)
            {
                var id = character?.Character?.StringId ?? "NULL";
                var relevantCategory = FindRelevantCategory(id);

                if(relevantCategory != null)
                {
                    if(!relevantCategory.TroopList.Contains(character))
                    {
                        var wrappedCategory = new PSEWrapperVM(relevantCategory);
                        relevantCategory.TroopList.Add(character);
                        relevantCategory.UpdateLabel();
                        if(!_mainPartyWrappers.Contains(wrappedCategory))
                            _mainPartyWrappers.Add(wrappedCategory);
                    }
                }
                else
                {
                    _mainPartyWrappers.Add(new PSEWrapperVM(character));
                }
            }

            //TODO: Save order of the _categoryList and reapply here.
            _privateCategoryList.ApplyActionOnAllItems(wrapper =>
            {
                if (!_mainPartyWrappers.Contains(wrapper)) 
                    _mainPartyWrappers.Add(wrapper);
            });
        }

        public PartyCategoryVM FindRelevantCategory(string characterId)
        {
            if (!_mainPartyWrappers.IsEmpty() && PartyScreenConfig.TroopCategoryBindings.TryGetValue(characterId, out var category))
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

        public override void OnFinalize()
        {
            base.OnFinalize();
            if(!_logic.IsCancelActive())
                PropagateLayout();
            else
                Utilities.DisplayMessage("Canceled!");
            PartyScreenConfig.Save();

            _mainPartyWrappers = null;
            _viewModel = null;
            _privateCategoryList = null;
            _indexToParty = null;
            _logic = null;
            _wrapper = null;
        }

        private PartyCategoryVM GetCategoryFromName(string targetList)
        {
            return this._privateCategoryList.FirstOrDefault(wrapper =>
                (wrapper.WrapperViewModel as PartyCategoryVM).TransferLabel.Equals(targetList))?.WrapperViewModel as PartyCategoryVM;
        }

        //TODO: Clean this one up
        private bool ValidateShift(PartyCharacterVM character, int index)
        {
            if (character.Character == CharacterObject.PlayerCharacter) return false;
            int num;
            if (character.Type == PartyScreenLogic.TroopType.Member)
            {
                num = _logic.MemberRosters[(int)character.Side].FindIndexOfTroop(character.Character);
                return num != -1 && IsValidIndex(index);
            }

            return false;
        }

        private bool IsValidIndex(int index)
        {
            return index > 0;
        }

        private void PropagateLayout()
        {
            var _mainRoster = _logic.MemberRosters[_rightSide];
            var _leftRoster = _logic.MemberRosters[_leftSide];

            _mainRoster.Clear();
            PropagateRelevantRoster(_mainRoster, _mainPartyWrappers);
        }

        private void PropagateRelevantRoster(TroopRoster roster, MBBindingList<PSEWrapperVM> wrappers)
        {
            foreach (var wrap in wrappers)
            {
                if (wrap.WrapperViewModel is PartyCategoryVM category)
                {
                    for (var i = 0; i < category.TroopList.Count; i++)
                    {
                        AddToRoster(category.TroopList[i], roster);
                    }
                }
                else if (wrap.WrapperViewModel is PartyCharacterVM character)
                {
                    AddToRoster(character, roster);
                }
            }
        }

        private void AddToRoster(PartyCharacterVM character, TroopRoster roster)
        {
            roster.AddToCounts(character.Troop.Character, character.Troop.Number, false, character.Troop.WoundedNumber,
                character.Troop.Xp);
        }

        [DataSourceProperty]
        public MBBindingList<PSEWrapperVM> MainPartyWrappers
        {
            get => _mainPartyWrappers;
            set
            {
                if (value != _mainPartyWrappers && _vm.TryGetTarget(out var pvm))
                {
                    _mainPartyWrappers = value;
                    pvm.OnPropertyChanged(nameof(MainPartyWrappers));
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

        public IList<MBBindingList<PSEWrapperVM>> CategoryRosters { get; set; }
    }
}
