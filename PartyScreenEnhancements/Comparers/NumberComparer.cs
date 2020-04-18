using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class NumberComparer : PartySort
    {
        public NumberComparer(PartySort equalSorter, bool descending) : base(equalSorter, @descending, null)
        {
        }

        public NumberComparer()
        {
        }

        public override string GetHintText()
        {
            return "Compares units based on the amount of troops you currently have.\nAscending order is low to high.\nDescending order is high to low.";
        }

        public override string GetName()
        {
            return "Troop Number Comparer";
        }

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            if (Descending ? x.Number > y.Number : y.Number > x.Number) return -1;

            if (x.Number == y.Number) return EqualSorter?.Compare(x, y) ?? 0;

            return 1;
        }
    }
}
