using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class TypeComparer : PartySort
    {
        public TypeComparer(PartySort equalSorter, bool descending) : base(descending, equalSorter)
        {
        }

        internal TypeComparer()
        {

        }

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (Descending
                ? x.CurrentFormationClass < y.CurrentFormationClass
                : x.CurrentFormationClass > y.CurrentFormationClass) return 1;

            if (y.CurrentFormationClass == x.CurrentFormationClass)
            {
                if (EqualSorter != null)
                    return EqualSorter.Compare(x, y);
                return 0;
            }

            return -1;
        }
    }
}