using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.HackedIn
{

    /// <summary>
    /// Necessary due to UIExtenderLib not supporting dynamic method calls with parameters
    /// </summary>
    public class PSEListWrapperVM : TaleWorlds.Library.ViewModel
    {
        private readonly PartyVMMixin _primary;
        private readonly PartyVM _partyVm;


        public PSEListWrapperVM(PartyVMMixin mixin, PartyVM partyVm)
        {
            _primary = mixin;
            _partyVm = partyVm;
        }

        //TODO: Fix transfer logic from main list to category and vice versa.
        // Be careful of transfer from left to right.
        public void ExecutePSETransferWithParameters(TaleWorlds.Library.ViewModel party, int index, string targetTag)
        {
            Utilities.DisplayMessage("Hello World " + party + " at index: " + index + " with tag " + targetTag);
            if (party is PartyCharacterVM character)
            {
                PartyScreenLogic.PartyRosterSide side = character.Side;
                PartyScreenLogic.PartyRosterSide partyRosterSide = targetTag.StartsWith("MainParty") ? PartyScreenLogic.PartyRosterSide.Right : PartyScreenLogic.PartyRosterSide.Left;
                if (targetTag == "MainParty")
                {
                    index = -1;
                }
                else if (targetTag.EndsWith("Prisoners") != character.IsPrisoner)
                {
                    index = -1;
                }
                if (side != partyRosterSide && character.IsTroopTransferrable)
                {
                    _primary.OnTransferTroop(character, index, character.Number, character.Side, targetTag);
                    _partyVm.ExecuteRemoveZeroCounts();
                    return;
                }
                if (side == PartyScreenLogic.PartyRosterSide.Right)
                {
                    _primary.OnShiftTroop(character, index);
                }
            }
            else if (party is PartyCategoryVM category)
            {
                _primary.CategoryShift(category, index);
            }
            else if(party is PSEWrapperVM wrapper)
            {
                _primary.WrapperShift(wrapper, index);
            }
        }

        

        // public void ExecutePSETransferWithParameters(PartyCharacterVM party, int index, string targetTag)
        // {
        //     Utilities.DisplayMessage("Hello World " + party + " at index: " + index + " with tag " + targetTag);
        //     if (party is PartyCharacterVM character)
        //     {
        //
        //     }
        // }
        //
        // public void ExecutePSETransferWithParameters(PSEWrapperVM party, int index, string targetTag)
        // {
        //     Utilities.DisplayMessage("Hello World " + party + " at index: " + index + " with tag " + targetTag);
        //     if (party is PSEWrapperVM wrapper)
        //     {
        //
        //     }
        // }

        [DataSourceProperty]
        public MBBindingList<PSEWrapperVM> CategoryList
        {
            get => _primary.CategoryList;
        }
    }
}
