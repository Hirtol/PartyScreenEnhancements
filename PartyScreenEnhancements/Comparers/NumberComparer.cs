using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class NumberComparer : PartySort
    {
        public NumberComparer(PartySort equalSorter, bool descending) : base(equalSorter, descending)
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

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (Descending ? x.Level > y.Level : y.Level > x.Level) return -1;

            if (x.Level == y.Level) return EqualSorter?.Compare(x, y) ?? 0;

            return 1;
        }
    }
}
