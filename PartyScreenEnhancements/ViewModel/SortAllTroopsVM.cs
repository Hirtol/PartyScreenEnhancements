using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.Patches;
using PartyScreenEnhancements.Saving;
using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.ViewModel
{
    public class SortAllTroopsVM : TaleWorlds.Library.ViewModel
    {
        private MBBindingList<PartyCharacterVM> _mainPartyList;
        private MBBindingList<PartyCharacterVM> _mainPartyPrisoners;
        private PartyScreenLogic _partyLogic;
        private PartyVM _partyVM;

        private const int _leftSide = (int)PartyScreenLogic.PartyRosterSide.Left;
        private const int _rightSide = (int)PartyScreenLogic.PartyRosterSide.Right;

        private HintViewModel _sortHint;

        public SortAllTroopsVM(PartyVM partyVm, PartyScreenLogic logic)
        {
            _partyVM = partyVm;
            _partyLogic = logic;
            _mainPartyList = _partyVM.MainPartyTroops;
            _mainPartyPrisoners = _partyVM.MainPartyPrisoners;
            _sortHint = new HintViewModel(new TextObject("Sort Troops\nCtrl Click to sort just main party"));
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _mainPartyPrisoners = null;
            _mainPartyList = null;
            _partyLogic = null;
            _partyVM = null;
        }

        public void SortTroops()
        {
            var settings = PartyScreenConfig.ExtraSettings;

            try
            {

                SortAnyParty(_mainPartyList, _partyLogic.RightOwnerParty, _partyLogic.MemberRosters[_rightSide], settings.PartySorter);

                if (!Utilities.IsControlDown())
                {
                    SortAnyParty(_mainPartyPrisoners,
                        null,
                        _partyLogic.PrisonerRosters[_rightSide],
                        settings.SeparateSortingProfiles ? settings.PrisonerSorter : settings.PartySorter);

                    SortAnyParty(_partyVM.OtherPartyPrisoners,
                        null,
                        _partyLogic.PrisonerRosters[_leftSide],
                        settings.SeparateSortingProfiles ? settings.PrisonerSorter : settings.PartySorter);

                    if (_partyLogic.LeftOwnerParty?.MobileParty != null)
                    {
                        bool useGarrisonSorter = settings.SeparateSortingProfiles;
                        PartySort sorterToUse = useGarrisonSorter
                            ? settings.GarrisonAndAlliedPartySorter
                            : settings.PartySorter;

                        SortAnyParty(_partyVM.OtherPartyTroops, _partyLogic.LeftOwnerParty, _partyLogic.MemberRosters[_leftSide], sorterToUse);
                    }
                }

                if (!_mainPartyList.IsEmpty() && (!_mainPartyList[0]?.Troop.Character?.IsPlayerCharacter ?? false))
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        "Your player character is no longer at the top of the list due to sorting, do NOT save your game and notify the mod manager"));
                }
            }
            catch (Exception e)
            {
                Logging.Log(Logging.Levels.ERROR, $"Sorting: {e}");
                Utilities.DisplayMessage($"PSE Sorting Unit Exception: {e}");
            }
        }
        private static void SortAnyParty(MBBindingList<PartyCharacterVM> toSort, PartyBase party, TroopRoster rosterToSort, PartySort sorter)
        {
            if (rosterToSort == null || rosterToSort.Count == 0 || toSort == null || toSort.IsEmpty()) return;

            var leaderOfParty = party?.LeaderHero.CharacterObject;
            toSort.StableSort(sorter);

            if (leaderOfParty != null)
            {
                var index = toSort.FindIndex((character) => character.Character.Equals(leaderOfParty));
                PartyCharacterVM leaderVm = toSort[index];
                toSort.RemoveAt(index);
                toSort.Insert(0, leaderVm);
            }

            rosterToSort.Clear();

            foreach (PartyCharacterVM character in toSort)
            {
                rosterToSort.AddToCounts(
                    character.Troop.Character, character.Troop.Number, false, character.Troop.WoundedNumber,
                    character.Troop.Xp);
            }
        }

        [DataSourceProperty]
        public HintViewModel SortHint
        {
            get => _sortHint;
            set
            {
                if (value != _sortHint)
                {
                    _sortHint = value;
                    base.OnPropertyChanged(nameof(SortHint));
                }
            }
        }
    }
}
