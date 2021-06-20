using HarmonyLib;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.Patches
{
    [HarmonyPatch(typeof(PartyVM), "RefreshCurrentCharacterInformation")]
    public class UpgradeButtonTooltipPatch
    {
        private const string UPGRADE_TOOLTIP = "\nHold [CTRL] and [SHIFT] to select as preferred upgrade path";

        public static void Postfix(ref PartyVM __instance)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen && PartyScreenConfig.ExtraSettings.PathSelectTooltips)
            {
                var current_char = __instance.CurrentCharacter;
                if(current_char == null) return;

                // Pretty dirty way to do this, but ㄟ( ▔, ▔ )ㄏ it'll work for now.
                if (!current_char.Upgrade1Hint?.HintText.Contains(UPGRADE_TOOLTIP) ?? false) 
                    __instance.CurrentCharacter.Upgrade1Hint.HintText = new TextObject(__instance.CurrentCharacter.Upgrade1Hint.HintText.ToString() + UPGRADE_TOOLTIP);
                if (!current_char.Upgrade2Hint?.HintText.Contains(UPGRADE_TOOLTIP) ?? false)
                    __instance.CurrentCharacter.Upgrade2Hint.HintText = new TextObject(__instance.CurrentCharacter.Upgrade2Hint.HintText.ToString() + UPGRADE_TOOLTIP);
            }
        }

    }
}
