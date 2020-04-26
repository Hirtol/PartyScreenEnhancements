using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PartyScreenEnhancements.Comparers;
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
        private readonly MBBindingList<PartyCharacterVM> _mainPartyList;
        private readonly MBBindingList<PartyCharacterVM> _mainPartyPrisoners;
        private readonly PartyScreenLogic _partyLogic;
        private readonly PartyVM _partyVM;

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

        public void SortTroops()
        {
            var settings = PartyScreenConfig.ExtraSettings;

            SortAnyParty(_mainPartyList, _partyLogic.MemberRosters[_rightSide], settings.PartySorter);

            if(!ScreenManager.TopScreen?.DebugInput.IsControlDown() ?? true)
            {
                SortAnyParty(_mainPartyPrisoners,
                    _partyLogic.PrisonerRosters[_rightSide],
                    settings.SeparateSortingProfiles ? settings.PrisonerSorter : settings.PartySorter);

                SortAnyParty(_partyVM.OtherPartyPrisoners,
                    _partyLogic.PrisonerRosters[_leftSide], 
                    settings.SeparateSortingProfiles ? settings.PrisonerSorter : settings.PartySorter);

                if (_partyLogic.LeftOwnerParty?.MobileParty != null)
                {
                    bool useGarrisonSorter = _partyLogic.LeftOwnerParty.MobileParty.IsGarrison &&
                                             settings.SeparateSortingProfiles;

                    SortAnyParty(_partyVM.OtherPartyTroops,
                        _partyLogic.MemberRosters[_leftSide],
                        useGarrisonSorter ? settings.GarrisonSorter : settings.PartySorter);
                }
                else
                {
                    SortAnyParty(_partyVM.OtherPartyTroops,
                        _partyLogic.MemberRosters[_leftSide],
                        settings.PartySorter);
                }
            }

            if (!_mainPartyList.IsEmpty() && (!_mainPartyList[0]?.Troop.Character?.IsPlayerCharacter ?? false))
            {
                InformationManager.DisplayMessage(new InformationMessage("Your player character is no longer at the top of the list due to sorting, do NOT save your game and notify the mod manager"));
            }
        }
        private static void SortAnyParty(MBBindingList<PartyCharacterVM> toSort, TroopRoster rosterToSort, PartySort sorter)
        {
            if(rosterToSort == null || rosterToSort.IsEmpty() || toSort == null || toSort.IsEmpty()) return;

            toSort.Sort(sorter);

            if(!toSort.IsOrdered(sorter))
            {
                //FileLog.Log($"Attempted sort on party {toSort.Count} with sorter {sorter} but the result wasn't ordered!");
                toSort.Sort(sorter);
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
