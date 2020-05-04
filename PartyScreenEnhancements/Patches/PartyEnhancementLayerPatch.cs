using HarmonyLib;
using PartyScreenEnhancements.ViewModel;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

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
            if (__instance is GauntletPartyScreen partyScreen && screenLayer == null)
            {
                screenLayer = new GauntletLayer(100);

                var traverser = Traverse.Create(partyScreen);
                PartyVM partyVM = traverser.Field<PartyVM>("_dataSource").Value;
                PartyState partyState = traverser.Field<PartyState>("_partyState").Value;
                
                enhancementVm = new PartyEnhancementsVM(partyVM, partyState.PartyScreenLogic, partyScreen);
                screenLayer.LoadMovie("PartyScreenEnhancements", enhancementVm);
                screenLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
                partyScreen.AddLayer(screenLayer);

                //TODO: Move this to PartyCharacterVM Mixin for (hopefully) better performance
                //This is necessarry for UpdateCurrentCharacterUpgrades (in PartyVM) to be called to instantiate Hints
                foreach (PartyCharacterVM partyVmMainPartyTroop in partyVM.MainPartyTroops)
                {
                    partyVM.CurrentCharacter = partyVmMainPartyTroop;
                }
            }
        }

        [HarmonyPatch("RemoveLayer")]
        public static void Prefix(ref ScreenBase __instance, ref ScreenLayer layer)
        {
            if (__instance is GauntletPartyScreen && screenLayer != null && layer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("PartyHotKeyCategory")))
            {
                __instance.RemoveLayer(screenLayer);
                enhancementVm.OnFinalize();
                enhancementVm = null;
                screenLayer = null;
            }
        }
    }
}