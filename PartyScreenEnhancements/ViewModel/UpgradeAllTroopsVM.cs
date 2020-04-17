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
    public class UpgradeAllTroopsVM : TaleWorlds.Library.ViewModel
    {
        private readonly MBBindingList<PartyCharacterVM> _mainPartyList;
        private readonly PartyScreenLogic _partyLogic;
        private readonly PartyVM _partyVM;
        private readonly PartyEnhancementsVM _parent;
        private HintViewModel _upgradeHint;


        public UpgradeAllTroopsVM(PartyEnhancementsVM parent)
        {
            _parent = parent;
            _partyLogic = parent.EnhancementPartyLogic;
            _partyVM = parent.EnhancementPartyVM;
            _mainPartyList = _partyVM.MainPartyTroops;
            _upgradeHint = new HintViewModel("Upgrade All Troops");
        }

        private void UpgradeAllTroopsPath()
        {
            var totalUpgrades = 0;
            var toUpgrade = new Dictionary<PartyCharacterVM, int>();

            foreach (PartyCharacterVM character in _mainPartyList)
            {
                if(character != null)
                {
                    if (PartyScreenConfig.ExtraSettings.HalfHalfUpgrades && character.IsUpgrade1Available &&
                        character.IsUpgrade2Available)
                    {
                        toUpgrade.Add(character, 2);
                    }
                    else if (PartyScreenConfig.PathsToUpgrade.TryGetValue(character.Character.StringId,
                        out var upgradePath))
                    {
                        if (upgradePath != -1)
                            toUpgrade.Add(character, upgradePath);
                    }
                    else if (character.IsUpgrade1Available && !character.IsUpgrade2Available)
                    {
                        toUpgrade.Add(character, 0);
                    }
                    else if (!character.IsUpgrade1Available && character.IsUpgrade2Available)
                    {
                        toUpgrade.Add(character, 1);
                    }
                }
            }

            foreach (var keyValuePair in toUpgrade)
            {
                Upgrade(keyValuePair.Key, keyValuePair.Value, ref totalUpgrades);
            }

            _mainPartyList.ApplyActionOnAllItems(partyCharacterVm => partyCharacterVm?.InitializeUpgrades());

            _parent.RefreshValues();

            if(PartyScreenConfig.ExtraSettings.ShowGeneralLogMessage)
                InformationManager.DisplayMessage(new InformationMessage($"Upgraded {totalUpgrades} troops!"));
        }

        private void Upgrade(PartyCharacterVM character, int upgradeIndex, ref int totalUpgrades)
        {
            //Somehow, for some people, character seems to be null at random times. Haven't been able to reproduce it so far
            //So this simple null check will have to stay.
            if (character == null) return;

            var anyInsufficient =
                upgradeIndex == 0 ? character.IsUpgrade1Insufficient : character.IsUpgrade2Insufficient;
            anyInsufficient = upgradeIndex == 2
                ? character.IsUpgrade1Insufficient || character.IsUpgrade2Insufficient
                : anyInsufficient;
            if (!anyInsufficient)
            {
                if (character.Character.UpgradeTargets.Length > upgradeIndex || upgradeIndex == 2)
                {
                    ExecuteUpgrade((PartyScreenLogic.PartyCommand.UpgradeTargetType) upgradeIndex, character,
                        ref totalUpgrades);
                }
            }
        }

        private void ExecuteUpgrade(PartyScreenLogic.PartyCommand.UpgradeTargetType upgradeTargetType,
            PartyCharacterVM character, ref int totalUpgrades)
        {
            character.InitializeUpgrades();

            if (character.Side == PartyScreenLogic.PartyRosterSide.Right &&
                character.Type == PartyScreenLogic.TroopType.Member)
            {
                var val = upgradeTargetType == PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget1
                    ? character.NumOfTarget1UpgradesAvailable
                    : character.NumOfTarget2UpgradesAvailable;
                if (val > 0)
                {
                    totalUpgrades += val;
                    var partyCommand = new PartyScreenLogic.PartyCommand();
                    partyCommand.FillForUpgradeTroop(character.Side, character.Type, character.Character, val,
                        upgradeTargetType);

                    _partyVM.CurrentCharacter = character;
                    _partyLogic.AddCommand(partyCommand);
                }
            }
        }


        [DataSourceProperty]
        public HintViewModel UpgradeHint
        {
            get
            {
                return _upgradeHint;
            }
            set
            {
                if (value != this._upgradeHint)
                {
                    this._upgradeHint = value;
                    base.OnPropertyChanged(nameof(UpgradeHint));
                }
            }
        }

    }


   


}
