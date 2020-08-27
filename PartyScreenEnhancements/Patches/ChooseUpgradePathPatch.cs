using HarmonyLib;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.Patches
{
    /// <summary>
    ///     Simple patch to intercept Upgrade calls so that people can select a path for the Upgrade All function to use.
    ///     Doesn't necessarily need Harmony, you could also just replace the PartyUpgradeButtonWidget and the PartyVM, but
    ///     I wanted to avoid doing that this time around.
    /// </summary>
    [HarmonyPatch(typeof(PartyCharacterVM), "Upgrade")]
    public class ChooseUpgradePathPatch
    {
        public static bool Prefix(int upgradeIndex, ref PartyCharacterVM __instance)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen && Utilities.IsControlDown() && Utilities.IsShiftDown())
            {
                if (PartyScreenConfig.PathsToUpgrade.TryGetValue(__instance.Character.StringId, out var upgradeValue))
                {
                    if (upgradeValue == upgradeIndex)
                    {
                        PartyScreenConfig.PathsToUpgrade.Remove(__instance.Character.StringId);
                        InformationManager.DisplayMessage(new InformationMessage(
                            $"Removed the set upgrade path for {__instance.Name}",
                            Color.ConvertStringToColor("#a83123FF")));
                    }
                    else
                    {
                        if (upgradeValue == -1)
                        {
                            PartyScreenConfig.PathsToUpgrade.Remove(__instance.Character.StringId);
                            InformationManager.DisplayMessage(new InformationMessage(
                                $"Allowed single-path upgrading of {__instance.Name} to {__instance.Character.UpgradeTargets[upgradeIndex].Name}",
                                Color.ConvertStringToColor("#0bbd0bFF")));
                        }
                        else
                        {
                            PartyScreenConfig.PathsToUpgrade[__instance.Character.StringId] = upgradeIndex;
                            InformationManager.DisplayMessage(new InformationMessage(
                                $"Changed the upgrade target of {__instance.Name} to {__instance.Character.UpgradeTargets[upgradeIndex].Name}",
                                Color.ConvertStringToColor("#0bbd0bFF")));
                        }
                    }
                }
                else
                {
                    if (__instance.Character.UpgradeTargets.Length == 1)
                    {
                        PartyScreenConfig.PathsToUpgrade.Add(__instance.Character.StringId, -1);
                        InformationManager.DisplayMessage(new InformationMessage(
                            $"Disallowed single-path upgrading of {__instance.Name}",
                            Color.ConvertStringToColor("#a83123FF")));
                    }
                    else
                    {
                        PartyScreenConfig.PathsToUpgrade.Add(__instance.Character.StringId, upgradeIndex);
                        InformationManager.DisplayMessage(new InformationMessage(
                            $"Set the upgrade target of {__instance.Name} to {__instance.Character.UpgradeTargets[upgradeIndex].Name}",
                            Color.ConvertStringToColor("#0bbd0bFF")));
                    }
                }

                PartyScreenConfig.Save();
                return false;
            }

            return true;
        }
    }
}