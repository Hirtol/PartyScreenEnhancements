using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class UpgradeableComparer : PartySort
    {

        public UpgradeableComparer(PartySort equalSorter, bool descending = true) : base(equalSorter, descending)
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

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            if (Descending
                ? x.Troop.NumberReadyToUpgrade > y.Troop.NumberReadyToUpgrade
                : y.Troop.NumberReadyToUpgrade > x.Troop.NumberReadyToUpgrade) return -1;


            if (y.Troop.NumberReadyToUpgrade == x.Troop.NumberReadyToUpgrade) return EqualSorter?.Compare(x, y) ?? 0;

            return 1;
        }
    }
}
