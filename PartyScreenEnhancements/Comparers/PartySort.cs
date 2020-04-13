using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public abstract class PartySort : IComparer<CharacterObject>
    {
        public int Compare(CharacterObject x, CharacterObject y)
        {
            if (x.IsHero || y.IsHero)
            {
                return 1;
            }
            else
            {
                return localCompare(x, y);
            }
        }

        protected abstract int localCompare(CharacterObject x, CharacterObject y);
    }
}
