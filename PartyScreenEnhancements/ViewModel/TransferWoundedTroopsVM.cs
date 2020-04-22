using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel
{
    public class TransferWoundedTroopsVM : TaleWorlds.Library.ViewModel
    {

        private PartyVM _partyVm;
        private MBBindingList<PartyCharacterVM> _mainPartyList;

        public TransferWoundedTroopsVM(PartyVM partyVm, MBBindingList<PartyCharacterVM> mainPartyList, bool shouldShow)
        {
            _partyVm = partyVm;
            _mainPartyList = mainPartyList;
            this._shouldShowTransferWounded = shouldShow;
            this._woundedHint = new HintViewModel("Transfer All Wounded");
        }

        private void ExecuteTransferWounded()
        {
            foreach (PartyCharacterVM character in _mainPartyList)
            {
                if (character.WoundedCount > 0)
                {
                    if(character.IsTroopTransferrable)
                    {
                        int wounded = character.WoundedCount;
                        character.OnTransfer(character, -1, wounded, character.Side);
                        character.ThrowOnPropertyChanged();
                        character.InitializeUpgrades();
                    }
                }
            }
            this._partyVm?.ExecuteRemoveZeroCounts();
        }

        [DataSourceProperty]
        public HintViewModel WoundedHint
        {
            get => _woundedHint;
            set
            {
                if (value != _woundedHint)
                {
                    _woundedHint = value;
                    base.OnPropertyChanged(nameof(WoundedHint));
                }
            }
        }

        [DataSourceProperty]
        public bool ShouldShowTransferWounded
        {
            get => _shouldShowTransferWounded;
            set
            {
                if (value != this._shouldShowTransferWounded)
                {
                    this._shouldShowTransferWounded = value;
                    base.OnPropertyChanged(nameof(ShouldShowTransferWounded));
                }
            }
        }


        private HintViewModel _woundedHint;
        private bool _shouldShowTransferWounded;
    }
}
