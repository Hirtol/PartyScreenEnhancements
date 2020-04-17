using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.Patches
{
    [HarmonyPatch(typeof(PartyCharacterVM), "ExecuteRecruitTroop")]
    public class ChooseIgnorePrisonerPatch
    {

        public static bool Prefix(ref PartyCharacterVM __instance)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen screen && screen.DebugInput.IsControlDown())
            {
                if (!PartyScreenConfig.PrisonersToRecruit.ContainsKey(__instance.Character.StringId))
                {
                    PartyScreenConfig.PrisonersToRecruit.Add(__instance.Character.StringId, -1);

                    if(PartyScreenConfig.ExtraSettings.RecruitByDefault)
                        displayRemoved(__instance);
                    else
                        displayAllowed(__instance);
                }
                else
                {
                    PartyScreenConfig.PrisonersToRecruit.Remove(__instance.Character.StringId);

                    if (PartyScreenConfig.ExtraSettings.RecruitByDefault)
                        displayAllowed(__instance);
                    else
                        displayRemoved(__instance);
                }
                PartyScreenConfig.Save();
                return false;
            }

            return true;
        }

        private static void displayAllowed(PartyCharacterVM __instance)
        {
            InformationManager.DisplayMessage(new InformationMessage(
                $"Allowed recruiting of {__instance.Name}",
                Color.ConvertStringToColor("#0bbd0bFF")));
        }

        private static void displayRemoved(PartyCharacterVM __instance)
        {
            InformationManager.DisplayMessage(new InformationMessage(
                $"Disallowed recruiting of {__instance.Character.Name}",
                Color.ConvertStringToColor("#a83123FF")));
        }

    }
}
