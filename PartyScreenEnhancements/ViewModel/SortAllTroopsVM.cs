using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly PartyEnhancementsVM _parent;
        private HintViewModel _sortHint;
        public SortAllTroopsVM(PartyEnhancementsVM parent)
        {
            this._parent = parent;
            this._partyVM = parent.EnhancementPartyVM;
            this._partyLogic = parent.EnhancementPartyLogic;
            this._mainPartyList = this._partyVM.MainPartyTroops;
            this._mainPartyPrisoners = this._partyVM.MainPartyPrisoners;
            this._sortHint = new HintViewModel("Sort Troops\nCtrl Click to sort just main party");
        }

        public void SortTroops()
        {
            var settings = PartyScreenConfig.ExtraSettings;
            SortAnyParty(_mainPartyList, _partyLogic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right], settings.PartySorter);

            if(!ScreenManager.TopScreen?.DebugInput.IsControlDown() ?? true)
            {
                SortAnyParty(_mainPartyPrisoners,
                    _partyLogic.PrisonerRosters[(int) PartyScreenLogic.PartyRosterSide.Right],
                    settings.SeparateSortingProfiles ? settings.PrisonerSorter : settings.PartySorter);
                if (_partyLogic.LeftOwnerParty?.MobileParty?.IsActive ?? false)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Sorting Garrison!"));
                    SortAnyParty(_partyVM.OtherPartyTroops,
                        _partyLogic.MemberRosters[(int) PartyScreenLogic.PartyRosterSide.Left],
                        settings.SeparateSortingProfiles ? settings.GarrisonSorter : settings.PartySorter);
                }
            }
        }
        private static void SortAnyParty(MBBindingList<PartyCharacterVM> toSort, TroopRoster rosterToSort, PartySort sorter)
        {
            toSort.Sort(sorter);
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
            get
            {
                return _sortHint;
            }
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
