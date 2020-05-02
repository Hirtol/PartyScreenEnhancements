using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class WoundedComparer : PartySort
    {
        public WoundedComparer(PartySort equalSorter, bool descending) : base(equalSorter, @descending, null)
        {
        }

        public WoundedComparer()
        {
        }

        public override string GetHintText()
        {
            return "Compares units based on the amount of wounded troops they currently have.\nAscending order is low to high.\nDescending order is high to low.";
        }

        public override string GetName()
        {
            return "Wounded Comparer";
        }

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {

            if (Descending ? x.Troop.WoundedNumber > y.Troop.WoundedNumber : y.Troop.WoundedNumber > x.Troop.WoundedNumber) return -1;

            if (x.Troop.WoundedNumber == y.Troop.WoundedNumber) return EqualSorter?.Compare(x, y) ?? 0;

            return 1;
        }
    }
}
