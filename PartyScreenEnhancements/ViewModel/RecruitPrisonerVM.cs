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
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel
{
    public class RecruitPrisonerVM : TaleWorlds.Library.ViewModel
    {
        private MBBindingList<PartyCharacterVM> _mainPartyPrisoners;
        private PartyScreenLogic _partyLogic;
        private PartyVM _partyVM;
        private PartyEnhancementsVM _parent;

        private HintViewModel _recruitHint;

        public RecruitPrisonerVM(PartyEnhancementsVM parent, PartyVM partyVm, PartyScreenLogic logic)
        {
            this._parent = parent;
            this._partyVM = partyVm;
            this._partyLogic = logic;
            this._mainPartyPrisoners = this._partyVM.MainPartyPrisoners;
            this._recruitHint = new HintViewModel("Recruit All Prisoners.\nClick with CTRL pressed to ignore party size limits");
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            this._mainPartyPrisoners = null;
            _partyLogic = null;
            _partyVM = null;
            _parent = null;
        }

        //TODO: Switch to cleaner RecruitByDefault=false logic.
        public void RecruitAll()
        {
            bool shouldIgnoreLimit = ScreenManager.TopScreen.DebugInput.IsControlDown();
            int amountUpgraded = 0;

            var enumerator = new PartyCharacterVM[_mainPartyPrisoners.Count];
            _mainPartyPrisoners.CopyTo(enumerator, 0);

            foreach (PartyCharacterVM prisoner in enumerator)
            {
                if (prisoner == null) continue;

                int remainingPartySize = _partyLogic.RightOwnerParty.PartySizeLimit - _partyLogic
                                             .MemberRosters[(int) PartyScreenLogic.PartyRosterSide.Right]
                                             .TotalManCount;
                if (remainingPartySize > 0 || shouldIgnoreLimit)
                {
                    if (prisoner.IsTroopRecruitable)
                    {
                        _partyVM.CurrentCharacter = prisoner;

                        if (PartyScreenConfig.PrisonersToRecruit.TryGetValue(prisoner.Character.StringId, out int val))
                        {
                            if (val == -1 && PartyScreenConfig.ExtraSettings.RecruitByDefault)
                                continue;
                        }
                        else if (!PartyScreenConfig.ExtraSettings.RecruitByDefault) continue;

                        RecruitPrisoner(prisoner, 
                            shouldIgnoreLimit ? prisoner.NumOfRecruitablePrisoners : remainingPartySize,
                            ref amountUpgraded);
                    }
                }
                else
                {
                    if (PartyScreenConfig.ExtraSettings.ShowGeneralLogMessage)
                        InformationManager.DisplayMessage(
                            new InformationMessage($"Party size limit reached, {amountUpgraded} recruited!"));
                    return;
                }
            }

            if (PartyScreenConfig.ExtraSettings.ShowGeneralLogMessage)
                InformationManager.DisplayMessage(new InformationMessage($"Recruited {amountUpgraded} prisoners"));

            _parent.RefreshValues();
        }

        private void RecruitPrisoner(PartyCharacterVM character, int remainingSize, ref int amount)
        {
            if (!this._partyLogic.IsPrisonerRecruitable(character.Type, character.Character, character.Side)) return;

            var numberOfRecruitables = character.NumOfRecruitablePrisoners;
            var number = Math.Min(character.NumOfRecruitablePrisoners, remainingSize);

            if(number > 0)
            {
                PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
                partyCommand.FillForRecruitTroop(character.Side, character.Type,
                    character.Character, number);

                this._partyLogic.AddCommand(partyCommand);

                // Currently the game subtracts 1 every time you recruit a prisoner, no matter how many you actually recruit
                // (Even if you press Shift to recruit 5!)
                // Therefore this fix is necessary for now.
                Campaign.Current.GetCampaignBehavior<IRecruitPrisonersCampaignBehavior>()?.SetRecruitableNumber(character.Character, numberOfRecruitables - number);
                amount += number;
                character.UpdateRecruitable();
            }
        }

        [DataSourceProperty]
        public HintViewModel RecruitHint
        {
            get => _recruitHint;
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
