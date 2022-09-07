using PartyScreenEnhancements.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.ViewModel
{
    public class UpgradeAllTroopsVM : TaleWorlds.Library.ViewModel
    {
        private MBBindingList<PartyCharacterVM> _mainPartyList;
        private PartyEnhancementsVM _parent;
        private PartyScreenLogic _partyLogic;
        private PartyVM _partyVM;

        private HintViewModel _upgradeHint;

        public UpgradeAllTroopsVM(PartyEnhancementsVM parent, PartyVM partyVm, PartyScreenLogic logic)
        {
            _parent = parent;
            _partyVM = partyVm;
            _partyLogic = logic;
            _mainPartyList = _partyVM.MainPartyTroops;

            _upgradeHint =
                new HintViewModel(new TextObject("Upgrade All Troops\nRight click to upgrade only paths set by you"));
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _mainPartyList = null;
            _partyLogic = null;
            _parent = null;
            _partyVM = null;
        }

        public void UpgradeAllTroopsPath(int shouldUseOnlyDictInt)
        {
            var totalUpgrades = 0;
            var toUpgrade = new Dictionary<PartyCharacterVM, UpgradeTarget>();
            var shouldUseOnlyDict = shouldUseOnlyDictInt == 1;

            try
            {
                foreach (PartyCharacterVM character in _mainPartyList)
                {
                    if (character == null || character.Upgrades.Count == 0) continue;

                    if (PartyScreenConfig.PathsToUpgrade.TryGetValue(character.Character.StringId, out var upgradePath))
                    {
                        if (upgradePath != -1)
                            toUpgrade.Add(character, new SpecificUpgradeTarget(upgradePath));
                    }
                    else if (!shouldUseOnlyDict)
                    {
                        if (character.Upgrades.Count >= 2)
                        {
                            if (PartyScreenConfig.ExtraSettings.EqualUpgrades)
                                toUpgrade.Add(character, new EqualDistributionTarget());
                        }
                        else if (character.Upgrades[0].IsAvailable)
                        {
                            toUpgrade.Add(character, new SpecificUpgradeTarget(0));
                        }
                    }
                }

                foreach (var keyValuePair in toUpgrade)
                {
                    totalUpgrades += Upgrade(keyValuePair.Key, keyValuePair.Value);
                }

                _parent.RefreshValues();
                _partyVM.ExecuteRemoveZeroCounts();

                if (PartyScreenConfig.ExtraSettings.ShowGeneralLogMessage)
                    InformationManager.DisplayMessage(new InformationMessage($"Upgraded {totalUpgrades} troops!"));
            }
            catch (Exception e)
            {
                Utilities.DisplayMessage($"PSE UpgradeTroops exception: {e}");
                Logging.Log(Logging.Levels.ERROR, $"Upgrade All Troops: {e}");

                foreach (var upgradeTarget in toUpgrade)
                {
                    Logging.Log(Logging.Levels.DEBUG, $"Key: {upgradeTarget.Key.Name} - Value: {upgradeTarget.Value}");
                }
            }
        }

        private int Upgrade(PartyCharacterVM character, UpgradeTarget upgradeTarget)
        {
            //Somehow, for some people, character seems to be null at random times. Haven't been able to reproduce it so far
            //So this simple null check will have to stay.
            if (character == null) return 0;

            bool allInsufficient;

            if (upgradeTarget is SpecificUpgradeTarget target)
            {
                // The upgrade target changed possible upgrade paths and therefore errors out, reset the upgrade preference
                if (target.targetIndex >= character.Upgrades.Count)
                {
                    Utilities.DisplayMessage(
                        $"Detected outdated upgrade preference, resetting upgrade preference for {character.Name}",
                        Colors.Red);
                    PartyScreenConfig.PathsToUpgrade.Remove(character.Character.StringId);
                    return 0;
                }

                allInsufficient = character.Upgrades[target.targetIndex].IsInsufficient;
            }
            else
            {
                allInsufficient = character.Upgrades.All(uTarget => uTarget.IsInsufficient);
            }

            if (!allInsufficient)
            {
                return ExecuteUpgrade(upgradeTarget, character);
            }

            return 0;
        }

        private int ExecuteUpgrade(UpgradeTarget upgradeTarget,
            PartyCharacterVM character)
        {
            character.InitializeUpgrades();

            if (character.Side == PartyScreenLogic.PartyRosterSide.Right &&
                character.Type == PartyScreenLogic.TroopType.Member)
            {
                if (upgradeTarget is EqualDistributionTarget)
                {
                    var upgradeableTroops = character.NumOfReadyToUpgradeTroops;
                    var upgradesCount = character.Upgrades.Count;

                    var upgradeCounts = character.Upgrades.Select(upgrade =>
                        Math.Min(upgrade.AvailableUpgrades, upgradeableTroops / upgradesCount)).ToList();

                    var remainingUpgrades = upgradeableTroops - upgradeCounts.Sum();

                    if (remainingUpgrades > 0)
                    {
                        // Distribute the remaining upgrades as best as possible (in case of uneven number of upgrades available)
                        for (int i = 0; i < upgradeCounts.Count; i++)
                        {
                            UpgradeTargetVM reprCharacter = character.Upgrades[i];
                            var toAdd = reprCharacter.AvailableUpgrades - upgradeCounts[i];

                            if (toAdd >= remainingUpgrades)
                            {
                                upgradeCounts[i] += remainingUpgrades;
                                remainingUpgrades = 0;
                            }
                            else
                            {
                                upgradeCounts[i] += toAdd;
                                remainingUpgrades -= toAdd;
                            }
                        }
                    }

                    for (int i = 0; i < upgradeCounts.Count; i++)
                    {
                        if (upgradeCounts[i] > 0)
                        {
                            SendCommand(character, upgradeCounts[i], i);
                        }
                    }

                    return upgradeCounts.Sum();
                }
                else if (upgradeTarget is SpecificUpgradeTarget target)
                {
                    var availableUpgrades = character.Upgrades[target.targetIndex].AvailableUpgrades;

                    if (availableUpgrades > 0)
                    {
                        SendCommand(character, availableUpgrades, target.targetIndex);
                    }

                    return availableUpgrades;
                }
            }

            return 0;
        }

        private void SendCommand(PartyCharacterVM character, int amount, int target)
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

        // Best approximation of Rust enums ._.
        private abstract class UpgradeTarget
        {
        }

        private class EqualDistributionTarget : UpgradeTarget
        {
        }

        private class SpecificUpgradeTarget : UpgradeTarget
        {
            public int targetIndex { get; }

            public SpecificUpgradeTarget(int targetIndex)
            {
                this.targetIndex = targetIndex;
            }


            public override string ToString()
            {
                return targetIndex.ToString();
            }
        }
    }
}