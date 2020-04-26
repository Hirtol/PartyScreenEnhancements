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
        private PartyEnhancementsVM _parent;

        public TransferWoundedTroopsVM(PartyEnhancementsVM parent, PartyVM partyVm, bool shouldShow)
        {
            _parent = parent;
            _partyVm = partyVm;
            _mainPartyList = partyVm?.MainPartyTroops;
            this._shouldShowTransferWounded = shouldShow;
            this._woundedHint = new HintViewModel("Transfer All Wounded");
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _partyVm = null;
            _parent = null;
            _mainPartyList = null;
        }

        private void ExecuteTransferWounded()
        {
            var enumerator = new PartyCharacterVM[_mainPartyList.Count];
            _mainPartyList?.CopyTo(enumerator, 0);

            foreach (PartyCharacterVM character in enumerator)
            {
                if (character?.WoundedCount > 0)
                {
                    if(character.IsTroopTransferrable)
                    {
                        int wounded = Math.Min(character.WoundedCount, character.Number);
                        character.OnTransfer(character, -1, wounded, character.Side);
                        character.InitializeUpgrades();
                    }
                }
            }

            this._partyVm?.ExecuteRemoveZeroCounts();
            _parent.RefreshValues();
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
