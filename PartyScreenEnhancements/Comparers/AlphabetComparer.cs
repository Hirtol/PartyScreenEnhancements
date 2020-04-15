using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class AlphabetComparer : PartySort
    {
        public AlphabetComparer(PartySort equalSorter, bool descending) : base(descending, equalSorter)
        {
        }

        internal AlphabetComparer()
        {

        }

        public override string GetHintText()
        {
            return "Compares units based on their names.\nThis Comparer is advised as your last one in the list to resolve any remaining conflicts.\nAscending order is A->Z.\nDescending order is Z->A";
        }

        public override string GetName()
        {
            return "Name Comparer";
        }

        protected override int localCompare(CharacterObject x, CharacterObject y)
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
