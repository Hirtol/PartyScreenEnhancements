using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class TrueTierComparer : PartySort
    {
        public TrueTierComparer(PartySort equalSorter, bool descending) : base(descending, equalSorter)
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

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (Descending ? x.Tier < y.Tier : x.Tier > y.Tier)
            {
                return 1;
            }
            if (x.Tier == y.Tier)
            {
                if (EqualSorter != null)
                {
                    return EqualSorter.Compare(x, y);
                }
                return 0;
            }else
            {
                return -1;
            }
        }
    }
}