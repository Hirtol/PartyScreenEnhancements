using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Patches
{
    [HarmonyPatch(typeof(PartyVM))]
    public class QuickUpgradePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ExecuteOpenUpgradePopUp")]
        public static bool PrefixOpenUpgrade(ref PartyVM __instance)
        {
            if (Utilities.IsControlDown())
            {
                // Upgrade all troops, don't just upgrade paths set by user.
                PartyEnhancementLayerPatch.enhancementVm.UpgradeAllTroops.UpgradeAllTroopsPath(0);
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ExecuteOpenRecruitPopUp")]
        public static bool PrefixOpenRecruit(ref PartyVM __instance)
        {
            if (Utilities.IsControlDown())
            {
                // Upgrade all troops, don't just upgrade paths set by user.
                PartyEnhancementLayerPatch.enhancementVm.RecruitAllPrisoners.RecruitAll();
                return false;
            }

            return true;
        }
    }
}