using HarmonyLib;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.Patches
{
    // Couldn't figure out how the new PopUps were actually added (as they're not Gauntlet layers as far as I could tell).
    // Still, we want to hide our UI when we show those new pop ups, so this manual refresh will have to do.
    [HarmonyPatch(typeof(PartyTroopManagerVM))]
    public class PopupPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("OpenPopUp")]
        public static void PostfixOpen()
        {
            PartyEnhancementLayerPatch.enhancementVm.OnPropertyChanged(nameof(PartyEnhancementLayerPatch.enhancementVm.AnyOtherPopupOpen));
        }

        [HarmonyPostfix]
        [HarmonyPatch("ExecuteDone")]
        public static void PostfixClose()
        {
            PartyEnhancementLayerPatch.enhancementVm.OnPropertyChanged(nameof(PartyEnhancementLayerPatch.enhancementVm.AnyOtherPopupOpen));
        }

        [HarmonyPostfix]
        [HarmonyPatch("ConfirmCancel")]
        public static void PostfixCancel()
        {
            PartyEnhancementLayerPatch.enhancementVm.OnPropertyChanged(nameof(PartyEnhancementLayerPatch.enhancementVm.AnyOtherPopupOpen));
        }

    }
}