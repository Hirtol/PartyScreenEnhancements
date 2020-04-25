using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class TypeComparer : PartySort
    {
        public TypeComparer(PartySort equalSorter, bool descending) : base(equalSorter, @descending, null)
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

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            if (Descending
                ? x.Character.CurrentFormationClass < y.Character.CurrentFormationClass
                : x.Character.CurrentFormationClass > y.Character.CurrentFormationClass) return 1;

            if (y.Character.CurrentFormationClass == x.Character.CurrentFormationClass)
            {
                if (EqualSorter != null)
                    return EqualSorter.Compare(x, y);
                return 0;
            }

            return -1;
        }
    }
}