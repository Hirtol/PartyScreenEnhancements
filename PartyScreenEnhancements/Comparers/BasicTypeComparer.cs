using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class BasicTypeComparer : PartySort
    {
        public BasicTypeComparer(bool descending, PartySort equalSorter) : base(@descending, equalSorter)
        {
        }

        public BasicTypeComparer()
        {
        }

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (Descending ? x.IsMounted && !y.IsMounted : x.IsInfantry && !y.IsInfantry) return -1;

            if ((x.IsInfantry && y.IsInfantry) || (x.IsMounted && y.IsMounted) || (x.IsArcher && y.IsArcher)) return EqualSorter?.Compare(x, y) ?? 0;

            if (Descending ? x.IsArcher && y.IsInfantry : x.IsArcher && y.IsMounted) return -1;

            if (Descending ? x.IsArcher && !y.IsMounted : x.IsArcher && !y.IsInfantry) return 1;

            if (Descending ? x.IsInfantry && !y.IsInfantry : x.IsMounted && !y.IsMounted) return 1;

            return -1;
        }
    }
}
