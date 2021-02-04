using System;
using System.Collections.Generic;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.ViewModel
{
    public class UpgradeAllTroopsVM : TaleWorlds.Library.ViewModel
    {
        private const int HALF_HALF_VALUE = 2;

        private MBBindingList<PartyCharacterVM> _mainPartyList;
        private PartyEnhancementsVM _parent;
        private PartyScreenLogic _partyLogic;
        private PartyVM _partyVM;

        private HintViewModel _upgradeHint;


        public UpgradeAllTroopsVM(PartyEnhancementsVM parent, PartyVM partyVm, PartyScreenLogic logic)
        {
            this._parent = parent;
            this._partyVM = partyVm;
            this._partyLogic = logic;
            this._mainPartyList = _partyVM.MainPartyTroops;

            this._upgradeHint = new HintViewModel(new TextObject("Upgrade All Troops\nRight click to upgrade only paths set by you"));
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _mainPartyList = null;
            _partyLogic = null;
            _parent = null;
            _partyVM = null;
        }

        private void UpgradeAllTroopsPath(int shouldUseOnlyDictInt)
        {
            var totalUpgrades = 0;
            var toUpgrade = new Dictionary<PartyCharacterVM, int>();
            var shouldUseOnlyDict = shouldUseOnlyDictInt == 1;

            try
            {
                foreach (PartyCharacterVM character in _mainPartyList)
                {
                    if (character == null) continue;

                    if (PartyScreenConfig.PathsToUpgrade.TryGetValue(character.Character.StringId, out var upgradePath))
                    {
                        if (upgradePath != -1)
                            toUpgrade.Add(character, upgradePath);
                    }
                    else if (!shouldUseOnlyDict)
                    {
                        if (PartyScreenConfig.ExtraSettings.HalfHalfUpgrades && character.IsUpgrade1Available &&
                            character.IsUpgrade2Available)
                        {
                            toUpgrade.Add(character, HALF_HALF_VALUE);
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

                _parent.RefreshValues();

                if (PartyScreenConfig.ExtraSettings.ShowGeneralLogMessage)
                    InformationManager.DisplayMessage(new InformationMessage($"Upgraded {totalUpgrades} troops!"));
            }
            catch (Exception e)
            {
                Utilities.DisplayMessage($"PSE UpgradeTroops exception: {e}");
                Logging.Log(Logging.Levels.ERROR, $"Upgrade All Troops: {e}");
            }
        }

        private void Upgrade(PartyCharacterVM character, int upgradeIndex, ref int totalUpgrades)
        {
            //Somehow, for some people, character seems to be null at random times. Haven't been able to reproduce it so far
            //So this simple null check will have to stay.
            if (character == null) return;
            
            // Sanity check in case troop trees change due to game update or mod configs.
            if ((upgradeIndex == 0 && !character.IsUpgrade1Exists) ||
                (upgradeIndex == 1 && !character.IsUpgrade2Exists))
            {
                Utilities.DisplayMessage($"Tried to upgrade { character.Name } to a troop that doesn't exist! Please reset your upgrade preferences.");
                return;
            }

            var anyInsufficient =
                upgradeIndex == 0 ? character.IsUpgrade1Insufficient : character.IsUpgrade2Insufficient;

            anyInsufficient = upgradeIndex == HALF_HALF_VALUE
                ? character.IsUpgrade1Insufficient || character.IsUpgrade2Insufficient
                : anyInsufficient;

            if (!anyInsufficient)
            {
                if (character.Character.UpgradeTargets.Length > upgradeIndex || upgradeIndex == HALF_HALF_VALUE)
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
                //HALF_HALF upgrade
                if (upgradeTargetType == PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget3)
                {
                    //Not sure how necessary this is, but better to be safe.
                    var upgOne = Math.Min(character.NumOfTarget1UpgradesAvailable,
                        character.NumOfUpgradeableTroops / 2);
                    var upgTwo = Math.Min(character.NumOfTarget2UpgradesAvailable,
                        character.NumOfUpgradeableTroops - upgOne);

                    totalUpgrades += upgOne + upgTwo;

                    if (upgOne > 0)
                        SendCommand(character, upgOne, PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget1);

                    if (upgTwo > 0)
                        SendCommand(character, upgTwo, PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget2);
                    return;
                }

                var val = upgradeTargetType == PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget1
                    ? character.NumOfTarget1UpgradesAvailable
                    : character.NumOfTarget2UpgradesAvailable;
                if (val > 0)
                {
                    totalUpgrades += val;
                    SendCommand(character, val, upgradeTargetType);
                }
            }
        }

        private void SendCommand(PartyCharacterVM character, int amount,
            PartyScreenLogic.PartyCommand.UpgradeTargetType target)
        {
            var partyCommand = new PartyScreenLogic.PartyCommand();
            partyCommand.FillForUpgradeTroop(character.Side, character.Type, character.Character, amount, target);
            _partyVM.CurrentCharacter = character;
            _partyLogic.AddCommand(partyCommand);
        }

        [DataSourceProperty]
        public HintViewModel UpgradeHint
        {
            get => _upgradeHint;
            set
            {
                if (value != _upgradeHint)
                {
                    _upgradeHint = value;
                    OnPropertyChanged(nameof(UpgradeHint));
                }
            }
        }
    }
}