using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class TypeComparer : PartySort
    {
        public TypeComparer(PartySort equalSorter, bool descending) : base(equalSorter, descending)
        {
        }

        internal TypeComparer()
        {

        }

        public override string GetHintText()
        {
            return
                "Compares units based on their Formation Class.\nAscending order is low to high.\nDescending order is high to low.";
        }

        public override string GetName()
        {
            return "Formation Type Comparer";
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