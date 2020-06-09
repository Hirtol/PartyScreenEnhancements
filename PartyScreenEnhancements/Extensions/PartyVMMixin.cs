using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.Saving;
using PartyScreenEnhancements.ViewModel.HackedIn;
using PartyScreenEnhancements.ViewModel.Settings;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
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

        private HintViewModel _addCategoryHint;

        private GauntletLayer _addCategoryLayer;
        private readonly GauntletPartyScreen _parentScreen;
        private GauntletMovie _currentMovie;
        private CategoryManagerVM _categoryManager;

        private MBBindingList<PSEWrapperVM> _mainPartyWrappers;
        private MBBindingList<PSEWrapperVM> _privateCategoryList;

        private List<Tuple<string, TextObject>> _formationNames;
        private List<SelectorItemVM> _formationItemVms;

        private IList<PartyCharacterVM> _mainPartyIndexList;


        public PartyVMMixin(PartyVM viewModel) : base(viewModel)
        {
            _mainPartyWrappers = new MBBindingList<PSEWrapperVM>();
            _privateCategoryList = new MBBindingList<PSEWrapperVM>();
            _mainPartyIndexList = new List<PartyCharacterVM>();
            _addCategoryHint = new HintViewModel("Add Category");

            _parentScreen = ScreenManager.TopScreen as GauntletPartyScreen;

            CategoryRosters = new List<MBBindingList<PSEWrapperVM>> {_mainPartyWrappers};

            if (_vm.TryGetTarget(out PartyVM vm))
            {
                _viewModel = vm;
                _logic = new Traverse(_viewModel).Field<PartyScreenLogic>("_partyScreenLogic").Value;
            }
            else
            {
                Utilities.DisplayMessage(
                    "Something went wrong when establishing the PSE View, Could not get PartyVM from reference.\nPlease report this issue and disable the mod for now.");
                FileLog.Log(
                    "Something went wrong when establishing the PSE View, Could not get PartyVM from reference.\nPlease report this issue and disable the mod for now.");
                return;
            }

            _formationItemVms = new List<SelectorItemVM>(10);

            for (int i = 0; i < this.FormationNames.Count; i++)
            {
                _formationItemVms.Add(new SelectorItemVM(this.FormationNames[i].Item1, this.FormationNames[i].Item2));
            }

            this._wrapper = new PSEListWrapperVM(this, _viewModel);
            this.Update += UpdateLabels;
            InitialiseCategories();

            (_viewModel.MainPartyTroops as IMBBindingList).ListChanged += PartyVMMixin_ListChanged;

            


            // _viewModel.MainPartyTroops.ApplyActionOnAllItems(character => _categoryList.Add(new PSEWrapperVM(character)));
            // //_categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "", CreateTroopLabel, Category.SYSTEM)));
            // _categoryList.Add(new PSEWrapperVM(new PartyCategoryVM(_viewModel.MainPartyTroops, "Normal Category", CreateTroopLabel, Category.USER_DEFINED)));
        }

        [DataSourceMethod]
        public void ExecuteOpenCategoryManager()
        {
            try
            {
                if (_addCategoryLayer == null)
                {
                    _addCategoryLayer = new GauntletLayer(201);
                    _categoryManager = new CategoryManagerVM(CloseSettingView, OnCategoryAddition);
                    _currentMovie = _addCategoryLayer.LoadMovie("PSECategoryManager", _categoryManager);
                    _addCategoryLayer.IsFocusLayer = true;
                    ScreenManager.TrySetFocus(_addCategoryLayer);
                    _addCategoryLayer.Input.RegisterHotKeyCategory(
                        HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
                    _parentScreen.AddLayer(_addCategoryLayer);
                    _addCategoryLayer.InputRestrictions.SetInputRestrictions();
                }
            }
            catch (Exception e)
            {
                FileLog.Log($"PSE Exception upon opening SettingScreen: {e}");
                Utilities.DisplayMessage($"PSE Exception: {e}");
            }
        }

        public void OnRemoveCategory(PartyCategoryVM category)
        {
            PartyScreenConfig.ExtraSettings.CategoryInformationList.Remove(category.Information);
            foreach (var item in PartyScreenConfig.TroopCategoryBindings.Where(kvp => kvp.Value == category.Information.Name).ToList())
            {
                PartyScreenConfig.TroopCategoryBindings.Remove(item.Key);
            }
            _privateCategoryList.Remove(new PSEWrapperVM(category));
            this.PartyVMMixin_ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
            category.OnFinalize();
        }

        private void OnCategoryAddition(CategoryInformation categoryInformation)
        {
            var wrapper = new PSEWrapperVM(new PartyCategoryVM(new MBBindingList<PartyCharacterVM>(),
                categoryInformation,
                "MainPartyTroops", _formationItemVms, OnRemoveCategory));
            this._privateCategoryList.Add(wrapper);
            AddCategoryWrapperToParty(wrapper);
        }

        private void CloseSettingView()
        {
            if (_addCategoryLayer != null)
            {
                _addCategoryLayer.ReleaseMovie(_currentMovie);
                _parentScreen.RemoveLayer(_addCategoryLayer);
                _addCategoryLayer.InputRestrictions.ResetInputRestrictions();
                _addCategoryLayer = null;
                _categoryManager.OnFinalize();
                _categoryManager = null;
            }
        }

        private void RefreshTopInformation()
        {
            _viewModel.MainPartyTotalWeeklyCostLbl = MobileParty.MainParty.GetTotalWage().ToString();
            _viewModel.MainPartyTotalGoldLbl = Hero.MainHero.Gold.ToString();
            _viewModel.MainPartyTotalMoraleLbl = ((int) MobileParty.MainParty.Morale).ToString("##.0");
            _viewModel.MainPartyTotalSpeedLbl = CampaignUIHelper.FloatToString(MobileParty.MainParty.ComputeSpeed());
        }

        public void OnTransferTroop(PartyCharacterVM character, int newIndex, int characterNumber,
            PartyScreenLogic.PartyRosterSide characterSide, PartyCategoryVM fromCategory, string targetList)
        {
            if (newIndex < 0 || character.Character == CharacterObject.PlayerCharacter) return;

            var characterWrapper = new PSEWrapperVM(character);

            // To Category
            if (targetList.StartsWith(PartyCategoryVM.CATEGORY_LABEL_PREFIX))
            {
                RemoveCharacterFromLists(character, fromCategory, characterWrapper);

                PartyCategoryVM targetCategory = GetCategoryFromName(targetList);

                if (targetCategory == null)
                {
                    _mainPartyWrappers.Add(characterWrapper);
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
                        RefreshTopInformation();
                        _viewModel.ExecuteRemoveZeroCounts();
                        return;
                    }

                    InsertIntoBindingList(character, newIndex + 1, targetCategory.TroopList);

                    character.ThrowOnPropertyChanged();
                    RefreshTopInformation();
                }
            }
            // To Main List
            else
            {
                if (newIndex == 0)
                    newIndex = -1;

                newIndex++;

                if(!ValidateShift(character, newIndex))
                    return;

                RemoveCharacterFromLists(character, fromCategory, characterWrapper);

                if (character.Type == PartyScreenLogic.TroopType.Member && characterSide == PartyScreenLogic.PartyRosterSide.Left)
                {
                    character.OnTransfer(character, newIndex, characterNumber, characterSide);
                    _viewModel.CurrentCharacter = character;

                    character.ThrowOnPropertyChanged();
                    RefreshTopInformation();
                }
                else
                {
                    OnShiftTroop(character, newIndex);
                }
            }

            Update?.Invoke(_mainPartyWrappers);
        }

        private void RemoveCharacterFromLists(PartyCharacterVM character, PartyCategoryVM fromCategory,
            PSEWrapperVM characterWrapper)
        {
            PartyScreenConfig.TroopCategoryBindings.Remove(character.Character.StringId);

            if (fromCategory != null)
                fromCategory.TroopList.Remove(character);
            else
                _mainPartyWrappers.Remove(characterWrapper);
        }

        public void OnShiftCategoryTroop(PartyCharacterVM characterVm, int newIndex, string targetTag)
        {
            var category = GetCategoryFromName(targetTag);

            if (characterVm.Side == PartyScreenLogic.PartyRosterSide.None) return;
            _viewModel.CurrentCharacter = characterVm;

            if (newIndex >= 0)
            {
                if (characterVm.Type == PartyScreenLogic.TroopType.Member)
                {
                    var sideList = category.TroopList;

                    InsertIntoBindingList(characterVm, newIndex, sideList);

                    characterVm.ThrowOnPropertyChanged();
                    RefreshTopInformation();
                }
                else
                {
                    Utilities.DisplayMessage("You may not shift prisoners!");
                    throw new NotImplementedException("You may not shift prisoners!");
                }
            }
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
                    RefreshTopInformation();
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
            if (!IsValidIndex(newIndex)) return;

            var sideList = GetPartyList(PartyScreenLogic.PartyRosterSide.Right);

            InsertIntoBindingList(new PSEWrapperVM(category), newIndex, sideList);

            category.Information.CurrentIndexInMainList = newIndex;

            RefreshTopInformation();
        }

        public void WrapperShift(PSEWrapperVM wrapper, int newIndex)
        {
            var sideList = GetPartyList(PartyScreenLogic.PartyRosterSide.Right);

            InsertIntoBindingList(wrapper, newIndex, sideList);

            RefreshTopInformation();
        }

        public void InsertIntoBindingList<T>(T model, int newIndex, MBBindingList<T> list)
        {
            var indexOfTroop = list.IndexOf(model);

            if (indexOfTroop != -1)
                list.RemoveAt(indexOfTroop);

            if (list.Count < newIndex)
            {
                list.Add(model);
            }
            else
            {
                var index = indexOfTroop < newIndex ? newIndex - 1 : newIndex;
                list.Insert(index, model);
            }
        }

        private MBBindingList<PSEWrapperVM> GetPartyList(PartyScreenLogic.PartyRosterSide side)
        {
            if (side == PartyScreenLogic.PartyRosterSide.Right)
            {
                return _mainPartyWrappers;
            }

            throw new NotImplementedException("PSE encountered unknown side");
        }

        public void UpdateLabels(MBBindingList<PSEWrapperVM> list)
        {
            var enumerable = list.Where(wrapper => wrapper.IsCategory);
            foreach (PSEWrapperVM wrapper in enumerable)
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
                    if (partyList != null)
                    {
                        PartyCharacterVM character = partyList[e.NewIndex];
                        PartyCategoryVM categoryAdd = FindRelevantCategory(character?.Character?.StringId);

                        if (e.NewIndex >= _mainPartyIndexList.Count)
                            _mainPartyIndexList.Add(character);
                        else
                            _mainPartyIndexList.Insert(e.NewIndex, character);

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
                                if (e.NewIndex > 0)
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
                    PartyCharacterVM removedChar = _mainPartyIndexList[e.NewIndex];
                    PartyCategoryVM categoryRemove = FindRelevantCategory(removedChar?.Character?.StringId);

                    if (categoryRemove != null)
                    {
                        categoryRemove.TroopList.Remove(removedChar);
                        categoryRemove.UpdateLabel();
                    }
                    else
                    {
                        MainPartyWrappers.Remove(new PSEWrapperVM(removedChar));
                    }

                    _mainPartyIndexList.RemoveAt(e.NewIndex);

                    break;
                case ListChangedType.Reset:
                    Utilities.DisplayMessage("Hey, reset!");
                    _mainPartyWrappers.Clear();
                    _mainPartyIndexList.Clear();
                    _privateCategoryList.Clear();
                    InitialiseCategories();
                    break;
                case ListChangedType.Sorted:
                    //TODO: Consider making this an explicit method call from SortAllTroopsVM instead
                    // Currently this will only be called so long as the PartyVM's MainTroopList is unsorted.
                    // Which will, at worst, only be once per entry to the party view, and thus you can't sort multiple times.
                    SortLocalLists();
                    break;
            }
        }


        //TODO: Consider making own MBBindingList implementation so we avoid the need for reflection.
        // Will need to observe side effects, but so long as TaleWorlds use their IMBBindingList interface
        // it should be fine.
        public void SortLocalLists()
        {
            _mainPartyIndexList.Clear();
            InitialiseMirrorLists();

            var wrapperComparer = new WrapperComparer(PartyScreenConfig.ExtraSettings.PartySorter);

            var categoryPositions = _mainPartyWrappers.GetCategoryPositions();

            // Yes, it's faster to use reflection than take out the categories and re-insert them.
            // This is thanks to the (what I assume to be) incredibly slow Widget construction process.
            var traverser = new Traverse(_mainPartyWrappers);

            //TODO: Consider caching this upon construction of the VM.
            
            List<PSEWrapperVM> _mainList =
                traverser.Field("_list").GetValue<List<PSEWrapperVM>>();

            int n = 0;

            // Move the categories to the end of the list while remembering their previous position
            // This will allow us to soon move them back while keeping the list sorted.
            foreach (var keyValuePair in categoryPositions)
            {
                n++;
                _mainPartyWrappers.Swap(keyValuePair.Value, _mainPartyWrappers.Count-n);
                // Also sort the sublist.
                keyValuePair.Key.TroopList.Sort(PartyScreenConfig.ExtraSettings.PartySorter);
            }

            // Sort everything but the categories
            // The 'custom' sort call here is why we needed reflection, since MBBindingList doesn't have this.
            _mainList.Sort(0, _mainPartyWrappers.Count-n, wrapperComparer);

            // Move everything up to make space for the insertion
            foreach (var keyValuePair in categoryPositions)
            {
                var last = _mainList[_mainPartyWrappers.Count - n];

                for (int i = _mainPartyWrappers.Count - n; i > keyValuePair.Value; i--)
                    _mainList[i] = _mainList[i - 1];
                _mainPartyWrappers[keyValuePair.Value] = last;

                n--;
            }


            // Called to make the game update the view.
            traverser.Method("FireListChanged", new[] {typeof(ListChangedType), typeof(int)})
                .GetValue(ListChangedType.Sorted, -1);
        }

        public void InitialiseCategories()
        {
            InitialiseMirrorLists();
            var names = PartyScreenConfig.TroopCategoryBindings.Values.Distinct();

            if (_privateCategoryList.IsEmpty())
            {
                foreach (var categoryInformation in PartyScreenConfig.ExtraSettings.CategoryInformationList)
                {
                    _privateCategoryList.Add(new PSEWrapperVM(new PartyCategoryVM(new MBBindingList<PartyCharacterVM>(),
                        categoryInformation,
                        "MainPartyTroops", _formationItemVms, OnRemoveCategory)));
                }
            }

            foreach (PartyCharacterVM character in _viewModel.MainPartyTroops)
            {
                var id = character?.Character?.StringId ?? "NULL";
                PartyCategoryVM relevantCategory = FindRelevantCategory(id);

                if (relevantCategory != null)
                {
                    if (!relevantCategory.TroopList.Contains(character))
                    {
                        var wrappedCategory = new PSEWrapperVM(relevantCategory);
                        relevantCategory.TroopList.Add(character);
                        relevantCategory.UpdateLabel();
                        // if (!_mainPartyWrappers.Contains(wrappedCategory))
                        //     _mainPartyWrappers.Add(wrappedCategory);
                    }
                }
                else
                {
                    _mainPartyWrappers.Add(new PSEWrapperVM(character));
                }
            }

            _privateCategoryList.ApplyActionOnAllItems(AddCategoryWrapperToParty);
        }

        private void AddCategoryWrapperToParty(PSEWrapperVM wrapper)
        {
            if (!_mainPartyWrappers.Contains(wrapper))
            {
                var insertLocation =
                    ((PartyCategoryVM)wrapper.WrapperViewModel).Information.CurrentIndexInMainList;
                if (insertLocation > 0 && insertLocation < _mainPartyWrappers.Count)
                {
                    _mainPartyWrappers.Insert(insertLocation, wrapper);
                }
                else
                {
                    _mainPartyWrappers.Add(wrapper);
                }
            }
        }

        private void InitialiseMirrorLists()
        {
            for (var i = 0; i < _viewModel.MainPartyTroops.Count; i++)
            {
                _mainPartyIndexList.Add(_viewModel.MainPartyTroops[i]);
            }
        }

        public PartyCategoryVM FindRelevantCategory(string characterId)
        {
            if (!_mainPartyWrappers.IsEmpty() &&
                PartyScreenConfig.TroopCategoryBindings.TryGetValue(characterId, out var category))
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

            (_viewModel.MainPartyTroops as IMBBindingList).ListChanged -= PartyVMMixin_ListChanged;
            this.Update -= UpdateLabels;

            //TODO: Fix the fact that this is always called ?????
            if (!_logic.IsCancelActive())
                PropagateLayout();
            else
                Utilities.DisplayMessage("Canceled!");
            
            PropagateLayout();

            //Has to be called after propagate
            _privateCategoryList.ApplyActionOnAllItems(wrapper => ((PartyCategoryVM) wrapper.WrapperViewModel).OnFinalize());

            PartyScreenConfig.Save();

            _wrapper.OnFinalize();

            _mainPartyWrappers = null;
            _viewModel = null;
            _privateCategoryList = null;
            _mainPartyIndexList = null;
            _logic = null;
            _wrapper = null;
        }

        private PartyCategoryVM GetCategoryFromName(string targetList)
        {
            return _privateCategoryList.FirstOrDefault(wrapper =>
                    ((PartyCategoryVM) wrapper.WrapperViewModel).TransferLabel.Equals(targetList))
                ?.WrapperViewModel as PartyCategoryVM;
        }

        //TODO: Clean this one up
        private bool ValidateShift(PartyCharacterVM character, int index)
        {
            if (character.Character == CharacterObject.PlayerCharacter) return false;
            int num;
            if (character.Type == PartyScreenLogic.TroopType.Member)
            {
                num = _logic.MemberRosters[(int) character.Side].FindIndexOfTroop(character.Character);
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
            TroopRoster _mainRoster = _logic.MemberRosters[_rightSide];
            TroopRoster _leftRoster = _logic.MemberRosters[_leftSide];

            _mainRoster.Clear();
            PropagateRelevantRoster(_mainRoster, _mainPartyWrappers);
        }

        private void PropagateRelevantRoster(TroopRoster roster, MBBindingList<PSEWrapperVM> wrappers)
        {
            for (int i = 0; i < wrappers.Count; i++)
            {
                var wrap = wrappers[i];
                if (wrap.WrapperViewModel is PartyCategoryVM category)
                {
                    for (var j = 0; j < category.TroopList.Count; j++)
                    {
                        AddToRoster(category.TroopList[i], roster);
                    }

                    category.Information.CurrentIndexInMainList = i;
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

        public List<Tuple<string, TextObject>> FormationNames
        {
            get
            {
                if (this._formationNames == null)
                {
                    int num = 8;
                    this._formationNames = new List<Tuple<string, TextObject>>(num + 1);
                    for (int i = 0; i < num; i++)
                    {
                        string item = "<img src=\"PartyScreen\\FormationIcons\\" + (i + 1) + "\"/>";
                        TextObject item2 = GameTexts.FindText("str_troop_group_name", i.ToString());
                        this._formationNames.Add(new Tuple<string, TextObject>(item, item2));
                    }
                }
                return this._formationNames;
            }
        }

        [DataSourceProperty]
        public MBBindingList<PSEWrapperVM> MainPartyWrappers
        {
            get => _mainPartyWrappers;
            set
            {
                if (value != _mainPartyWrappers && _vm.TryGetTarget(out PartyVM pvm))
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

        [DataSourceProperty]
        public HintViewModel AddCategoryHint {
            get => _addCategoryHint;
            set
            {
                if (value != this._addCategoryHint)
                {
                    this._addCategoryHint = value;
                    _viewModel.OnPropertyChanged(nameof(AddCategoryHint));
                }
            }
        }

        public IList<MBBindingList<PSEWrapperVM>> CategoryRosters { get; set; }
    }

    public static class Extensions
    {
        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public static IDictionary<PartyCategoryVM, int> GetCategoryPositions(this MBBindingList<PSEWrapperVM> list)
        {
            var result = new Dictionary<PartyCategoryVM, int>(5);

            for (int i = 0; i < list.Count; i++)
            {
                if(list[i].IsCategory)
                    result.Add((list[i].WrapperViewModel as PartyCategoryVM), i);
            }

            return result;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> toAdd)
        {
            foreach (T item in toAdd)
            {
                list.Add(item);
            }
        }
    }
}