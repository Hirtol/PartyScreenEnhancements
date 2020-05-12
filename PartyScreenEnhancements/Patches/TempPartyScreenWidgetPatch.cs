using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;

namespace PartyScreenEnhancements.Patches
{

    [HarmonyPatch(typeof(PartyScreenWidget), "FindTupleWithTroopIDInList")]
    public class TempPartyScreenWidgetPatch
    {

        public static bool Prefix(string troopID, bool searchMainList, bool isPrisoner, ref PartyScreenWidget __instance, ref PartyTroopTupleWidget __result)
        {

            IEnumerable<PartyTroopTupleWidget> source;
            if (searchMainList)
            {
                source = (isPrisoner ? __instance.MainPrisonerList.Children.Cast<PartyTroopTupleWidget>() : EnumerateMainMemberList(__instance));
            }
            else
            {
                source = (isPrisoner ? __instance.OtherPrisonerList.Children.Cast<PartyTroopTupleWidget>() : __instance.OtherMemberList.Children.Cast<PartyTroopTupleWidget>());
            }

            __result = source.SingleOrDefault(i => i.CharacterID == troopID);

            // Temp patch until either transpiler to fix MainMemberList or mirror class PartyTroopTupleWidget to fix the method call to this to be to the 
            // mirror class of CustomPartyScreenWidget instead.
            return false;
        }

        private static IEnumerable<PartyTroopTupleWidget> EnumerateMainMemberList(PartyScreenWidget __instance)
        {
            IEnumerable<PartyTroopTupleWidget> result = new List<PartyTroopTupleWidget>();

            foreach (ListPanel listPanel in __instance.MainMemberList.Children.Cast<ListPanel>())
            {
                for (int i = 0; i < listPanel.ChildCount; i++)
                {
                    if (listPanel.Children[i] is PartyTroopTupleWidget troop)
                    {
                        result = result.Append(troop);
                    }
                }
                //result = result.Concat(listPanel.Children.Cast<PartyTroopTupleWidget>());
            }

            return result;
        }
    }
}
