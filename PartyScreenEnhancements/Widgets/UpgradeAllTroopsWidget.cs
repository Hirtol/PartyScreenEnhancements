using System.Collections.Generic;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace PartyScreenEnhancements
{
    /**
     * The Widget normally shouldn't contain logic, considering the MVVM pattern.
     * However, considering that I'd otherwise have to modify PartyVM I'd rather break design pattern than have to override the main, and important, VM.
     */
    public class UpgradeAllTroopsWidget : ButtonWidget
    {
        private readonly MBBindingList<PartyCharacterVM> _mainPartyList;
        private readonly PartyScreenLogic _partyLogic;
        private readonly PartyVM _partyVM;

        public UpgradeAllTroopsWidget(UIContext context) : base(context)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen)
            {
                _partyVM = (PartyVM) ((GauntletPartyScreen) ScreenManager.TopScreen).GetField("_dataSource");
                _partyLogic = (PartyScreenLogic) _partyVM.GetField("_partyScreenLogic");
                _mainPartyList = _partyVM.MainPartyTroops;
            }

            EventFire += EventHandler;
        }

        private void EventHandler(Widget widget, string eventName, object[] args)
        {
            if (IsVisible)
            {
                if (eventName == "HoverBegin")
                {
                    InformationManager.AddHintInformation("Upgrade All Troops");
                }

                if (eventName == "HoverEnd")
                {
                    InformationManager.HideInformations();
                }
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            UpgradeAllTroopsPath();
        }


        private void UpgradeAllTroopsPath()
        {
            var toUpgrade = new Dictionary<PartyCharacterVM, int>();

            foreach (PartyCharacterVM character in _mainPartyList)
            {
                if (character.IsUpgrade1Available && !character.IsUpgrade2Available)
                {
                    toUpgrade.Add(character, 0);
                }
                else if (!character.IsUpgrade1Available && character.IsUpgrade2Available)
                {
                    toUpgrade.Add(character, 1);
                }
                else if (character.IsUpgrade1Available && character.IsUpgrade2Available)
                {
                    if (PartyScreenConfig.PathsToUpgrade.ContainsKey(character.Character.StringId))
                    {
                        toUpgrade.Add(character, PartyScreenConfig.PathsToUpgrade[character.Character.StringId]);
                    }
                }
            }

            var totalUpgrades = 0;

            foreach (var keyValuePair in toUpgrade)
            {
                Upgrade(keyValuePair.Key, keyValuePair.Value, ref totalUpgrades);
            }

            _mainPartyList.ApplyActionOnAllItems(partyCharacterVm => partyCharacterVm.InitializeUpgrades());
            InformationManager.DisplayMessage(
                new InformationMessage($"Upgraded {totalUpgrades} troops to the next level!"));
        }

        private void Upgrade(PartyCharacterVM character, int upgradeIndex, ref int totalUpgrades)
        {
            var anyInsufficient =
                upgradeIndex == 0 ? character.IsUpgrade1Insufficient : character.IsUpgrade2Insufficient;
            if (!anyInsufficient)
            {
                if (character.Character.UpgradeTargets.Length > upgradeIndex)
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
    }
}