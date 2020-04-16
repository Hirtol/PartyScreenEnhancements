using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class CultureComparer : PartySort
    {
        private StringComparer _nameComparer;
        public CultureComparer(PartySort equalSorter, bool descending) : base(equalSorter, descending)
        {
            this._nameComparer = StringComparer.CurrentCulture;
        }

        public CultureComparer()
        {
        }

        public override string GetHintText()
        {
            return "Compares units based on their Culture names.\nAscending order is A->Z.\nDescending order is Z->A";
        }

        public override string GetName()
        {
            return "Culture Comparer";
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            int result;
            if (Descending)
                result = _nameComparer.Compare(y.Character.Culture.Name.ToString(), x.Character.Culture.Name.ToString());
            else
                result = _nameComparer.Compare(x.Character.Culture.Name.ToString(), y.Character.Culture.Name.ToString());

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
