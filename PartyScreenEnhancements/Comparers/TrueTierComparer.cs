using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class TrueTierComparer : PartySort
    {
        public TrueTierComparer(PartySort equalSorter, bool descending) : base(equalSorter, @descending, null)
        {
        }

        internal TrueTierComparer()
        {

        }

        public override string GetHintText()
        {
            return
                "Compares units based on their Tier (which by default range from 1-6)\nAscending order is low to high.\nDescending order is high to low.";
        }

        public override string GetName()
        {
            return "Tier Comparer";
        }

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            if (Descending ? x.Character.Tier < y.Character.Tier : x.Character.Tier > y.Character.Tier)
            {
                return 1;
            }

            if (x.Character.Tier == y.Character.Tier)
            {
                return EqualSorter?.Compare(x, y) ?? 0;
            }

            return -1;
        }
    }
}