using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace PartyScreenEnhancements.Comparers
{
    public class TypeComparer : PartySort
    {

        private readonly PartySort tierSorter = new TrueTierComparer(true);
        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if ( x.CurrentFormationClass > y.CurrentFormationClass)
            {
                return 1;
            }else if (y.CurrentFormationClass == x.CurrentFormationClass)
            {
                return tierSorter.Compare(x, y);
            }else
            {
                return -1;
            }
        }
    }
}
