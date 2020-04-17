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
                if ((!PartyScreenConfig.PrisonersToRecruit.ContainsKey(__instance.Character.StringId) && PartyScreenConfig.ExtraSettings.RecruitByDefault) || (PartyScreenConfig.PrisonersToRecruit.ContainsKey(__instance.Character.StringId) && !PartyScreenConfig.ExtraSettings.RecruitByDefault))
                {
                    PartyScreenConfig.PrisonersToRecruit.Add(__instance.Character.StringId, -1);
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"Disallowed recruiting of {__instance.Character.Name}",
                        Color.ConvertStringToColor("#a83123FF")));
                }
                else
                {
                    PartyScreenConfig.PrisonersToRecruit.Remove(__instance.Character.StringId);
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"Allowed recruiting of {__instance.Name}",
                        Color.ConvertStringToColor("#0bbd0bFF")));
                }
                PartyScreenConfig.Save();
                return false;
            }

            return true;
        }

    }
}
