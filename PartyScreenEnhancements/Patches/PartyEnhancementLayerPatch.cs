using HarmonyLib;
using PartyScreenEnhancements.ViewModel;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace PartyScreenEnhancements.Patches
{
    /// <summary>
    /// Simple patch in order to create the required overlay on top of the PartyScreen
    /// </summary>
    [HarmonyPatch(typeof(ScreenBase))]
    public class PartyEnhancementLayerPatch
    {
        internal static GauntletLayer screenLayer;
        internal static PartyEnhancementsVM enhancementVm;

        [HarmonyPatch("AddLayer")]
        public static void Postfix(ref ScreenBase __instance)
        {
            if (__instance is not GauntletPartyScreen partyScreen || screenLayer != null) return;

            var traverser = Traverse.Create(partyScreen);
            PartyVM partyVM = traverser.Field<PartyVM>("_dataSource").Value;

            // Some other mods might hook PartyVM constructor methods, and insert their own layers during that initialisation.
            // In theory that would be fine, as the Party Screen Layer doesn't get added until *after* PartyVM construction.
            // But, the GauntletPartyScreen *is* already the top screen, and thus the guard above gets passed, even though PartyVM isn't yet constructed.
            // Therefore we need a guard here to protect against an uninitialised PartyVM.
            if (partyVM == null) return;

            screenLayer = new GauntletLayer(10);
            PartyState partyState = traverser.Field<PartyState>("_partyState").Value;

            enhancementVm = new PartyEnhancementsVM(partyVM, partyState.PartyScreenLogic, partyScreen);
            screenLayer.LoadMovie("PartyScreenEnhancements", enhancementVm);
            screenLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            partyScreen.AddLayer(screenLayer);
        }

        [HarmonyPatch("RemoveLayer")]
        public static void Prefix(ref ScreenBase __instance, ref ScreenLayer layer)
        {
            if (__instance is not GauntletPartyScreen || screenLayer == null ||
                !layer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("PartyHotKeyCategory"))) return;
            
            __instance.RemoveLayer(screenLayer);
            enhancementVm?.OnFinalize();
            enhancementVm = null;
            screenLayer = null;
        }
    }
}