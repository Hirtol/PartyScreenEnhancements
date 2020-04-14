using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewManagers
{
    public class SortAllTroopsVM : TaleWorlds.Library.ViewModel
    {
        private readonly MBBindingList<PartyCharacterVM> _mainPartyList;
        private readonly PartyScreenLogic _partyLogic;
        private readonly PartyVM _partyVM;
        private HintViewModel _sortHint;
        public SortAllTroopsVM(PartyVM partyVm, PartyScreenLogic partyScreenLogic)
        {
            this._partyVM = partyVm;
            this._partyLogic = partyScreenLogic;
            this._mainPartyList = this._partyVM.MainPartyTroops;
            this._sortHint = new HintViewModel("Sort Troops");
        }


        public void SortTroops()
        {
            var sortedList = new List<TroopRosterElement>();

            for (var i = 0; i < _partyLogic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right].Count; i++)
            {
                TroopRosterElement t = _partyLogic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right]
                    .GetElementCopyAtIndex(i);
                sortedList.Add(t);
            }

            _partyLogic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right].Clear();
            sortedList.Sort(new TroopComparer(PartyScreenConfig.Sorter));

            foreach (TroopRosterElement rosterElement in sortedList)
                _partyLogic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right].AddToCounts(
                    rosterElement.Character, rosterElement.Number, false, rosterElement.WoundedNumber,
                    rosterElement.Xp);

            // Other option, no need to reset 
            //_partyVM.Call("InitializeTroopLists");

            // Update the current View, not necessary for the state to be preserved.
            _mainPartyList.Sort(new VMComparer(PartyScreenConfig.Sorter));
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
                    base.OnPropertyChanged("SortHint");
                }
            }
        }
    }

    internal class VMComparer : IComparer<PartyCharacterVM>
    {
        private readonly IComparer<CharacterObject> _trueComparer;

        public VMComparer(IComparer<CharacterObject> trueComparer)
        {
            _trueComparer = trueComparer;
        }

        public int Compare(PartyCharacterVM x, PartyCharacterVM y)
        {
            return _trueComparer.Compare(x.Character, y.Character);
        }
    }

    internal class TroopComparer : IComparer<TroopRosterElement>
    {
        private readonly IComparer<CharacterObject> _trueComparer;

        public TroopComparer(IComparer<CharacterObject> trueComparer)
        {
            _trueComparer = trueComparer;
        }

        public int Compare(TroopRosterElement x, TroopRosterElement y)
        {
            return _trueComparer.Compare(x.Character, y.Character);
        }
    }
}
