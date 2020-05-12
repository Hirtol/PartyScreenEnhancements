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

        //TODO: Add category shift.
        // Be careful of transfer from left to right.
        public void ExecutePSETransferWithParameters(TaleWorlds.Library.ViewModel party, int index, string targetTag)
        {
            Utilities.DisplayMessage("Hello World " + party + " at index: " + index + " with tag " + targetTag);
            if (party is PartyCharacterVM character)
            {
                var currentCategory = _primary.FindRelevantCategory(character?.Character?.StringId ?? "NULL");
                PartyScreenLogic.PartyRosterSide side = character.Side;
                PartyScreenLogic.PartyRosterSide partyRosterSide = targetTag.StartsWith("MainParty") ? PartyScreenLogic.PartyRosterSide.Right : PartyScreenLogic.PartyRosterSide.Left;

                // To Main party (head label)
                if (targetTag == "MainParty")
                {
                    index = -1;
                }
                // Prisoner, irrelevant for main troops.
                else if (targetTag.EndsWith("Prisoners") != character.IsPrisoner)
                {
                    index = -1;
                }
                // To category
                if (targetTag.StartsWith(PartyCategoryVM.CATEGORY_LABEL_PREFIX) || currentCategory != null)
                {
                    Utilities.DisplayMessage("Transfer Category");
                    _primary.OnTransferTroop(character, index, character.Number, character.Side, currentCategory, targetTag);
                    return;
                }
                // If different side (left to right, or vice-versa)
                if (side != partyRosterSide && character.IsTroopTransferrable)
                {
                    Utilities.DisplayMessage("Transfer other");
                    _primary.OnTransferTroop(character, index, character.Number, character.Side, currentCategory, targetTag);
                    _partyVm.ExecuteRemoveZeroCounts();
                    return;
                }
                // If we're on the same side as before, to account for in-category shift, TODO: Make better to account for left to right
                if (side == PartyScreenLogic.PartyRosterSide.Right)
                {
                    Utilities.DisplayMessage("Shift");
                    _primary.OnShiftTroop(character, index);
                }
            }
            else if (party is PartyCategoryVM category)
            {
                if(category.ParentTag.Equals(targetTag))
                    _primary.CategoryShift(category, index);
                else
                    Utilities.DisplayMessage("Category side switching is not implemented!");
            }
            else if(party is PSEWrapperVM wrapper)
            {
                _primary.WrapperShift(wrapper, index);
            }
        }

        [DataSourceProperty]
        public MBBindingList<PSEWrapperVM> MainPartyWrappers
        {
            get => _primary.MainPartyWrappers;
        }
    }
}
