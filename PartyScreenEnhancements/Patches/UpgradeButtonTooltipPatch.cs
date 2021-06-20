using System.Runtime.CompilerServices;
using HarmonyLib;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.Patches
{
    [HarmonyPatch(typeof(PartyCharacterVM), "UpdateUpgradeHint")]
    public class UpgradeButtonTooltipPatch
    {
        private const string UPGRADE_TOOLTIP = "\nHold [CTRL] and [SHIFT] to select as preferred upgrade path";

        public static void Postfix(ref PartyCharacterVM __instance, int index)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen && PartyScreenConfig.ExtraSettings.PathSelectTooltips)
            {
                if (index == 0)
                {
                    if (!__instance.Upgrade1Hint?.HintText.Contains(UPGRADE_TOOLTIP) ?? false)
                        __instance.Upgrade1Hint.HintText = new TextObject(__instance.Upgrade1Hint.HintText + UPGRADE_TOOLTIP);
                } else if (index == 1)
                {
                    if (!__instance.Upgrade2Hint?.HintText.Contains(UPGRADE_TOOLTIP) ?? false)
                        __instance.Upgrade2Hint.HintText = new TextObject(__instance.Upgrade2Hint.HintText + UPGRADE_TOOLTIP);
                }
            }
        }
    }
}
