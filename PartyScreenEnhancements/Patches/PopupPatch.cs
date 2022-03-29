using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Patches
{
    // Couldn't figure out how the new PopUps were actually added (as they're not Gauntlet layers as far as I could tell).
    // Still, we want to hide our UI when we show those new pop ups, so this manual refresh will have to do.
    [HarmonyPatch(typeof(PartyTroopManagerVM))]
    public class PopupPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("OpenPopUp")]
        public static void PostfixOpen(ref PartyTroopManagerVM __instance)
        {
            PartyEnhancementLayerPatch.enhancementVm.OnPropertyChanged(nameof(PartyEnhancementLayerPatch.enhancementVm
                .AnyOtherPopupOpen));

            PartyEnhancementLayerPatch.enhancementVm.OpenPopupViewEnhancements(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch("ExecuteDone")]
        public static void PostfixClose()
        {
            PartyEnhancementLayerPatch.enhancementVm.ClosePopupViewEnhancements();
            PartyEnhancementLayerPatch.enhancementVm.OnPropertyChanged(nameof(PartyEnhancementLayerPatch.enhancementVm
                .AnyOtherPopupOpen));
        }

        [HarmonyPostfix]
        [HarmonyPatch("ConfirmCancel")]
        public static void PostfixCancel()
        {
            PartyEnhancementLayerPatch.enhancementVm.ClosePopupViewEnhancements();
            PartyEnhancementLayerPatch.enhancementVm.OnPropertyChanged(nameof(PartyEnhancementLayerPatch.enhancementVm
                .AnyOtherPopupOpen));
        }
    }
}