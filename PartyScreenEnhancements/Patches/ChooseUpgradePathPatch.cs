using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using Debug = TaleWorlds.Library.Debug;

namespace PartyScreenEnhancements.Patches
{
    /// <summary>
    /// Simple patch to intercept Upgrade calls so that people can select a path for the Upgrade All function to use.
    /// Doesn't necessarily need Harmony, you could also just replace the PartyUpgradeButtonWidget and the PartyVM, but
    /// I wanted to avoid doing that this time around.
    /// </summary>
    [HarmonyPatch(typeof(PartyCharacterVM), "Upgrade")]
    public class ChooseUpgradePathPatch
    {
        public static bool Prefix(int upgradeIndex, ref PartyCharacterVM __instance)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen)
            {
                var screen = (GauntletPartyScreen) ScreenManager.TopScreen;
                if (screen.DebugInput.IsControlDown())
                {
                    if(upgradeIndex < __instance.Character.UpgradeTargets.Length)
                    {
                        if (PartyScreenConfig.PathsToUpgrade.ContainsKey(__instance.Character.StringId) && PartyScreenConfig.PathsToUpgrade[__instance.Character.StringId] == upgradeIndex)
                        {
                            PartyScreenConfig.PathsToUpgrade.Remove(__instance.Character.StringId);
                            InformationManager.DisplayMessage(new InformationMessage(
                                $"Removed the set upgrade path for {__instance.Name}", Color.ConvertStringToColor("#a83123FF")));
                        }
                        else
                        {
                            PartyScreenConfig.PathsToUpgrade.Add(__instance.Character.StringId, upgradeIndex);
                            InformationManager.DisplayMessage(new InformationMessage(
                                $"Set the upgrade target of {__instance.Name} to {__instance.Character.UpgradeTargets[upgradeIndex].Name}", Color.ConvertStringToColor("#0bbd0bFF")));
                        }
                        return false;
                    }
                    else
                    {
                        Debug.PrintError("Error, upgradeIndex was greater than the allow UpgradeTargets Length!");
                    }
                }
            }

            return true;
        }

    }
}
