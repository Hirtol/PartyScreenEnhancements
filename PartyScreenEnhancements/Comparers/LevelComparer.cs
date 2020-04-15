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
        public LevelComparer(bool descending, PartySort equalSorter) : base(descending, equalSorter)
        {
        }

        public LevelComparer()
        {
        }

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (Descending ? x.Level > y.Level : y.Level > x.Level) return -1;

            if (x.Level == y.Level) return EqualSorter?.Compare(x, y) ?? 0;

            return 1;
        }
    }
}
