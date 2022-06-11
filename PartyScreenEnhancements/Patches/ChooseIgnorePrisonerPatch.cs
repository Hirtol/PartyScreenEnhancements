using HarmonyLib;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace PartyScreenEnhancements.Patches
{
    [HarmonyPatch(typeof(PartyCharacterVM), "ExecuteRecruitTroop")]
    public class ChooseIgnorePrisonerPatch
    {
        public static bool Prefix(ref PartyCharacterVM __instance)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen && Utilities.IsControlDown() && Utilities.IsShiftDown())
            {
                if (!PartyScreenConfig.PrisonersToRecruit.ContainsKey(__instance.Character.StringId))
                {
                    PartyScreenConfig.PrisonersToRecruit.Add(__instance.Character.StringId, -1);

                    if (PartyScreenConfig.ExtraSettings.RecruitByDefault)
                        DisplayRemoved(__instance);
                    else
                        DisplayAllowed(__instance);
                }
                else
                {
                    PartyScreenConfig.PrisonersToRecruit.Remove(__instance.Character.StringId);

                    if (PartyScreenConfig.ExtraSettings.RecruitByDefault)
                        DisplayAllowed(__instance);
                    else
                        DisplayRemoved(__instance);
                }
                PartyScreenConfig.Save();
                return false;
            }

            return true;
        }

        private static void DisplayAllowed(PartyCharacterVM __instance)
        {
            InformationManager.DisplayMessage(new InformationMessage(
                $"Allowed recruiting of {__instance.Name}",
                Color.ConvertStringToColor("#0bbd0bFF")));
        }

        private static void DisplayRemoved(PartyCharacterVM __instance)
        {
            InformationManager.DisplayMessage(new InformationMessage(
                $"Disallowed recruiting of {__instance.Character.Name}",
                Color.ConvertStringToColor("#a83123FF")));
        }

    }
}
