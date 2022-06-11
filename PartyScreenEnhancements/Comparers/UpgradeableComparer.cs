using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;

namespace PartyScreenEnhancements.Comparers
{
    public class UpgradeableComparer : PartySort
    {

        public UpgradeableComparer(PartySort equalSorter, bool descending = true) : base(equalSorter, @descending, null)
        {
        }

        internal UpgradeableComparer()
        {

        }

        public override string GetHintText()
        {
            return
                "Compares units based on the number of upgrades available.\nNote, this includes units which can't currently upgrade due to lack of resources, but do have the experience required.\nAscending order is low to high.\nDescending order is high to low.";
        }

        public override string GetName()
        {
            return "Upgradable Comparer";
        }

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            if (Descending
                ? x.NumOfReadyToUpgradeTroops > y.NumOfReadyToUpgradeTroops
                : y.NumOfReadyToUpgradeTroops > x.NumOfReadyToUpgradeTroops) return -1;


            if (y.NumOfReadyToUpgradeTroops == x.NumOfReadyToUpgradeTroops)
                return EqualSorter?.Compare(x, y) ?? 0;

            return 1;
        }
    }
}
