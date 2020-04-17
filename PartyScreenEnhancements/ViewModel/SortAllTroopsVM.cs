using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel
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
            _mainPartyList.Sort(PartyScreenConfig.Sorter);

            _partyLogic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right].Clear();


            foreach (var character in _mainPartyList)
            {
                _partyLogic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right].AddToCounts(
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
