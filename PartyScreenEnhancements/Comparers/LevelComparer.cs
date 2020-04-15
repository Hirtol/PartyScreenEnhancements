using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class LevelComparer : PartySort
    {
        public LevelComparer(PartySort equalSorter, bool descending) : base(descending, equalSorter)
        {
        }

        public LevelComparer()
        {
        }

        public override string GetHintText()
        {
            return "Compares units based on their Level.\nAscending order is low to high.\nDescending order is high to low.";
        }

        public override string GetName()
        {
            return "Level Comparer";
        }

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (Descending ? x.Level > y.Level : y.Level > x.Level) return -1;

            if (x.Level == y.Level) return EqualSorter?.Compare(x, y) ?? 0;

            return 1;
        }
    }
}
