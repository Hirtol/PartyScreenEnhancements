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
            if (CustomSettingsList == null || CustomSettingsList.IsEmpty())
            {
                FillCustomList();
            }
            foreach (var setting in CustomSettingsList)
            {
                if (x.Troop.Character.Culture.Name.Equals(y.Troop.Character.Culture.Name))
                    return EqualSorter?.Compare(x, y) ?? 0;

                bool xMatch = x.Troop.Character.Culture.Name?.ToString().Equals(setting) ?? false;
                bool yMatch = y.Troop.Character.Culture.Name?.ToString().Equals(setting) ?? false;

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
