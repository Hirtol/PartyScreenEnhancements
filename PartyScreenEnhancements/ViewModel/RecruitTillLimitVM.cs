using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel
{
    public class RecruitTillLimitVM : TaleWorlds.Library.ViewModel
    {

        private PartyVM _partyVm;
        private PartyScreenLogic _logic;
        private HintViewModel _limitHint;

        private bool _isEnabled;

        public RecruitTillLimitVM(PartyVM partyVm, PartyScreenLogic logic)
        {
            this._partyVm = partyVm;
            this._logic = logic;
            this.LimitHint = new HintViewModel("Transfer all units up to your party limit.\nRight click to transfer all prisoners up to your prisoner limit");
            this.IsEnabled = partyVm.OtherPartyTroops != null;
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            this._partyVm = null;
            this._logic = null;
        }

        public void ExecuteTransfer()
        {
            ExecutePrivateTransfer(_partyVm.OtherPartyTroops, _logic?.RightOwnerParty?.PartySizeLimit ?? 0, _logic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right]);
        }

        public void ExecutePrisonerTransfer()
        {
            ExecutePrivateTransfer(_partyVm.OtherPartyPrisoners, _logic?.RightOwnerParty?.PrisonerSizeLimit ?? 0, _logic.PrisonerRosters[(int) PartyScreenLogic.PartyRosterSide.Right]);
        }

        private void ExecutePrivateTransfer(MBBindingList<PartyCharacterVM> otherUnits, int sizeLimit, TroopRoster roster)
        {
            if (otherUnits == null || otherUnits.IsEmpty() || sizeLimit <= 0) return;

            try
            {

                var enumerator = new PartyCharacterVM[otherUnits.Count];
                otherUnits.CopyTo(enumerator, 0);

                foreach (PartyCharacterVM character in enumerator)
                {
                    if (character == null) continue;

                    int remainingPartySize = sizeLimit - roster.TotalManCount;

                    if (remainingPartySize > 0 && character.IsTroopTransferrable)
                    {
                        _partyVm.CurrentCharacter = character;
                        int toTransfer = Math.Min(remainingPartySize, character.Troop.Number);

                        character.OnTransfer(character, -1, toTransfer, character.Side);
                    }
                }

                _partyVm.ExecuteRemoveZeroCounts();
            }
            catch (Exception e)
            {
                Logging.Log(Logging.Levels.ERROR, $"Recruit Till Limit: {e}");
                Utilities.DisplayMessage($"PSE Transfer To Limit Exception: {e}");
            }
        }

        [DataSourceProperty]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value != _isEnabled)
                {
                    this._isEnabled = value;
                    base.OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel LimitHint
        {
            get => _limitHint;
            set
            {
                if (value != this._limitHint)
                {
                    this._limitHint = value;
                    base.OnPropertyChanged(nameof(LimitHint));
                }
            }
        }

    }
}
