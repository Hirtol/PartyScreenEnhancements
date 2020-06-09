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
        private PartyVMMixin _primary;
        private PartyVM _partyVm;

        public PSEListWrapperVM(PartyVMMixin mixin, PartyVM partyVm)
        {
            _primary = mixin;
            _partyVm = partyVm;
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            this._primary = null;
            this._partyVm = null;
        }

        //TODO: Add category shift.
        // Be careful of transfer from left to right.
        public void ExecutePSETransferWithParameters(TaleWorlds.Library.ViewModel party, int index, string targetTag)
        {
            Utilities.DisplayMessage("Hello World " + party + " at index: " + index + " with tag " + targetTag);
            if (party is PartyCharacterVM character)
            {
                var currentCategory = _primary.FindRelevantCategory(character.Character?.StringId ?? "NULL");
                PartyScreenLogic.PartyRosterSide side = character.Side;
                PartyScreenLogic.PartyRosterSide partyRosterSide = targetTag.StartsWith("MainParty") ? PartyScreenLogic.PartyRosterSide.Right : PartyScreenLogic.PartyRosterSide.Left;

                // To Main party (head label)
                if (targetTag == "MainParty")
                {
                    index = -1;
                }

                //From category
                if (currentCategory != null && !currentCategory.Label.Equals(targetTag))
                {
                    if(!targetTag.StartsWith("MainParty"))
                    {
                        Utilities.DisplayMessage("Shift Category");
                        _primary.OnShiftCategoryTroop(character, index, targetTag);
                        return;
                    }
                    else
                    {
                        Utilities.DisplayMessage("Transfer From Category");
                        _primary.OnTransferTroop(character, index, character.Number, character.Side, currentCategory, targetTag);
                        return;
                    }
                }

                // To category
                if (targetTag.StartsWith(PartyCategoryVM.CATEGORY_LABEL_PREFIX))
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
