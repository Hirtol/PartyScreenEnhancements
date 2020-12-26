using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.Patches;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

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
            this._partyVM = partyVm;
            this._partyLogic = logic;
            this._mainPartyList = this._partyVM.MainPartyTroops;
            this._mainPartyPrisoners = this._partyVM.MainPartyPrisoners;
            this._sortHint = new HintViewModel("Sort Troops\nCtrl Click to sort just main party");
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
                Utilities.DisplayMessage($"PSE Sorting Unit Exception: {e}");
            }
        }
        private static void SortAnyParty(MBBindingList<PartyCharacterVM> toSort, PartyBase party, TroopRoster rosterToSort, PartySort sorter)
        {
            if (rosterToSort == null || rosterToSort.IsEmpty() || toSort == null || toSort.IsEmpty()) return;
            var leaderOfParty = party?.Leader;
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
                if (value != this._sortHint)
                {
                    this._sortHint = value;
                    base.OnPropertyChanged(nameof(SortHint));
                }
            }
        }
    }
}
