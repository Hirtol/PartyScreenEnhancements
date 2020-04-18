using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace PartyScreenEnhancements.Comparers
{
    public class CultureComparer : PartySort
    {
        public CultureComparer(PartySort equalSorter, bool descending, List<string> customSort = null) : base(equalSorter, @descending, customSort)
        {
            
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

        public override bool HasCustomSettings()
        {
            return true;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            var xName = x.Troop.Character.Culture.Name?.ToString();
            var yName = y.Troop.Character.Culture.Name?.ToString();

            if (xName == null || yName == null)
                return 1;

            if (xName.Equals(yName))
                return EqualSorter?.Compare(x, y) ?? 0;

            if (CustomSettingsList == null || CustomSettingsList.IsEmpty())
            {
                FillCustomList();
            }

            if (!CustomSettingsList.Contains(xName) || !CustomSettingsList.Contains(yName))
            {
                return !CustomSettingsList.Contains(xName) ? 1 : -1;
            }

            foreach (var setting in CustomSettingsList)
            {
                bool xMatch = xName.Equals(setting);
                bool yMatch = yName.Equals(setting);

                if (xMatch && !yMatch) return Descending ? -1 : 1;
                if (yMatch && !xMatch) return Descending ? 1 : -1;
            }

            return 1;
        }

        public override void FillCustomList()
        {
            base.FillCustomList();
            var cultures = Enum.GetValues(typeof(CultureCode));

            foreach (CultureCode cultureCode in cultures) 
            {
                CustomSettingsList.Add(cultureCode.ToString());
            }
            CustomSettingsList.Sort(StringComparer.CurrentCulture);
            CustomSettingsList.Reverse();
        }
    }
}
