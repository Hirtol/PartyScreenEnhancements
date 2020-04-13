using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        private MBBindingList<PartyCharacterVM> _mainPartyList;
        private PartyVM _partyVM;
        private PartyScreenLogic _partyLogic;

        public UpgradeAllTroopsWidget(UIContext context) : base(context)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen)
			{
                this._partyVM = (PartyVM) ((GauntletPartyScreen)ScreenManager.TopScreen).GetField("_dataSource");
                this._partyLogic = (PartyScreenLogic) _partyVM.GetField("_partyScreenLogic");
                this._mainPartyList = _partyVM.MainPartyTroops;
            }
            this.EventFire += EventHandler;
        }

        private void EventHandler(Widget widget, string eventName, object[] args)
        {
            if (base.IsVisible)
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
            Dictionary<PartyCharacterVM, int> dict = new Dictionary<PartyCharacterVM, int>();

            foreach (PartyCharacterVM character in _mainPartyList)
            {
                if (character.IsUpgrade1Available && !character.IsUpgrade2Available)
                {
                    dict.Add(character, 0);
                }
                else if (!character.IsUpgrade1Available && character.IsUpgrade2Available)
                {
                    dict.Add(character, 1);
                }
                else if(character.IsUpgrade1Available && character.IsUpgrade2Available)
                {
                    if (PartyScreenConfig.PathsToUpgrade.ContainsKey(character.Character.StringId))
                    {
                        dict.Add(character, PartyScreenConfig.PathsToUpgrade[character.Character.StringId]);
                    }
                }
                
            }

            foreach (var keyValuePair in dict)
            {
                Upgrade(keyValuePair.Key, keyValuePair.Value);
            }
            _mainPartyList.ApplyActionOnAllItems(partyCharacterVm => partyCharacterVm.InitializeUpgrades());
            //InformationManager.DisplayMessage(new InformationMessage($"Upgraded {amountUpgrade} troops to the next level!"));
        }

        private void Upgrade(PartyCharacterVM character, int upgradeIndex)
        {
            int num = ((upgradeIndex == 0) ? character.IsUpgrade1Available : character.IsUpgrade2Available) ? 1 : 0;
            bool flag = (upgradeIndex == 0) ? character.IsUpgrade1Insufficient : character.IsUpgrade2Insufficient;
            if (num == 0 || flag)
            {
                return;
            }

            if(character.Character.UpgradeTargets.Length > upgradeIndex)
            {
                ExecuteUpgrade((PartyScreenLogic.PartyCommand.UpgradeTargetType) upgradeIndex, character);
            }
        }

        private void ExecuteUpgrade(PartyScreenLogic.PartyCommand.UpgradeTargetType upgradeTargetType, PartyCharacterVM character)
        {
            character.InitializeUpgrades();

            if (character.Side == PartyScreenLogic.PartyRosterSide.Right && character.Type == PartyScreenLogic.TroopType.Member)
            {
                int val = (upgradeTargetType == PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget1) ? character.NumOfTarget1UpgradesAvailable : character.NumOfTarget2UpgradesAvailable;
                if(val > 0)
                {
                    PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
                    partyCommand.FillForUpgradeTroop(character.Side, character.Type, character.Character, val,
                        upgradeTargetType);

                    _partyVM.CurrentCharacter = character;
                    this._partyLogic.AddCommand(partyCommand);
                }
            }
        }
    }
}
