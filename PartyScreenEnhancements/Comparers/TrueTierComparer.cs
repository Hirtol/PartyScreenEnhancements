using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class TrueTierComparer : PartySort
    {
        /// <summary>
        ///     Creates a <code>IComparer</code> instance which sorts Characters in the Party list based on their tier in game.
        /// </summary>
        /// <param name="descending">Whether to sort on descending order or ascending. (top to bottom)</param>
        /// <param name="equalSorter">An additional sorter that could be used for parts where two characters are equal</param>
        public TrueTierComparer(PartySort equalSorter, bool descending) : base(descending, equalSorter)
        {
        }

        internal TrueTierComparer()
        {

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