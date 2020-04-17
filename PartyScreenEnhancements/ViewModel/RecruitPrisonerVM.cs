using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MountAndBlade.CampaignBehaviors;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel
{
    public class RecruitPrisonerVM : TaleWorlds.Library.ViewModel
    {
        private readonly MBBindingList<PartyCharacterVM> _mainPartyPrisoners;
        private readonly PartyScreenLogic _partyLogic;
        private readonly PartyVM _partyVM;
        private HintViewModel _recruitHint;
        public RecruitPrisonerVM(PartyVM partyVm, PartyScreenLogic partyScreenLogic)
        {
            this._partyVM = partyVm;
            this._partyLogic = partyScreenLogic;
            this._mainPartyPrisoners = this._partyVM.MainPartyPrisoners;
            this._recruitHint = new HintViewModel("Recruit All Prisoners");
        }


        public void RecruitAll()
        {
            int amountUpgraded = 0;
            // Aah, concurrent modification exception, my old friend. Would be nice if the game gave a proper crash log >.>
            PartyCharacterVM[] enumerator = new PartyCharacterVM[_mainPartyPrisoners.Count];
            _mainPartyPrisoners.CopyTo(enumerator, 0);

            foreach (PartyCharacterVM prisoner in enumerator)
            {
                int remainingPartySize = _partyLogic.RightOwnerParty.PartySizeLimit - _partyLogic.MemberRosters[(int)PartyScreenLogic.PartyRosterSide.Right].TotalManCount;
                if(remainingPartySize > 0)
                {
                    if (prisoner.IsTroopRecruitable)
                    {
                        this._partyVM.CurrentCharacter = prisoner;
                        if (PartyScreenConfig.PrisonersToRecruit.TryGetValue(prisoner.Character.StringId, out int val))
                        {
                            if(val == -1 && PartyScreenConfig.ExtraSettings.RecruitByDefault)
                                continue;
                        }

                        RecruitPrisoner(prisoner, remainingPartySize, ref amountUpgraded);
                    }
                }
                else
                {
                    if(PartyScreenConfig.ExtraSettings.ShowGeneralLogMessage)
                        InformationManager.DisplayMessage(new InformationMessage($"Party size limit reached, {amountUpgraded} recruited!"));
                    return;
                }
            }
            if (PartyScreenConfig.ExtraSettings.ShowGeneralLogMessage)
                InformationManager.DisplayMessage(new InformationMessage($"Recruited {amountUpgraded} prisoners"));
        }

        private void RecruitPrisoner(PartyCharacterVM character, int remainingSize, ref int amount)
        {

            if (this._partyLogic.IsPrisonerRecruitable(character.Type, character.Character, character.Side))
            {
                //TODO: Remove this numberOfRecruitables, wait till the main game is fixed.
                int numberOfRecruitables = character.NumOfRecruitablePrisoners;
                int number = Math.Min(character.NumOfRecruitablePrisoners, remainingSize);

                if(number > 0)
                {
                    PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
                    partyCommand.FillForRecruitTroop(character.Side, character.Type,
                        character.Character, number);

                    this._partyLogic.AddCommand(partyCommand);

                    Campaign.Current.GetCampaignBehavior<IRecruitPrisonersCampaignBehavior>()?.SetRecruitableNumber(character.Character, numberOfRecruitables - number);
                    amount += number;
                    character.UpdateRecruitable();
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel RecruitHint
        {
            get
            {
                return _recruitHint;
            }
            set
            {
                if (value != this._recruitHint)
                {
                    this._recruitHint = value;
                    base.OnPropertyChanged(nameof(RecruitHint));
                }
            }
        }


    }
}
