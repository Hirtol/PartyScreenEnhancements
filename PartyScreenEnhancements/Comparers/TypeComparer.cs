using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class TypeComparer : PartySort
    {
        private readonly bool _descending;
        private readonly PartySort _equalSorter;

        public TypeComparer(PartySort equalSorter, bool descending)
        {
            _equalSorter = equalSorter;
            _descending = descending;
        }

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (_descending
                ? x.CurrentFormationClass < y.CurrentFormationClass
                : x.CurrentFormationClass > y.CurrentFormationClass) return 1;

            if (y.CurrentFormationClass == x.CurrentFormationClass)
            {
                if (_equalSorter != null)
                    return _equalSorter.Compare(x, y);
                return 0;
            }

            return -1;
        }
    }
}