using System;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class AlphabetComparer : PartySort
    {
        public AlphabetComparer(PartySort equalSorter, bool descending) : base(equalSorter, @descending, null)
        {
        }

        internal AlphabetComparer()
        {

        }

        public override string GetHintText()
        {
            return "Compares units based on their names.\nThis Comparer is should probably be the last one in the list to resolve any remaining conflicts.\nAscending order is A->Z.\nDescending order is Z->A";
        }

        public override string GetName()
        {
            return "Name Comparer";
        }

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            int result;
            if (Descending)
                result = StringComparer.CurrentCulture.Compare(y.Name.ToString(), x.Name.ToString());
            else
                result = StringComparer.CurrentCulture.Compare(x.Name.ToString(), y.Name.ToString());

            if (result == 0)
            {
                if (EqualSorter != null)
                {
                    return EqualSorter.Compare(x, y);
                }
            }

            return result;
        }
    }
}
