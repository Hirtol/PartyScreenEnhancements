using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class BasicTypeComparer : PartySort
    {
        private Dictionary<string, Func<PartyCharacterVM, PartyCharacterVM, bool>> _compDictionary;
        public BasicTypeComparer(PartySort equalSorter, bool descending) : base(equalSorter, @descending, null)
        {
        }

        public BasicTypeComparer()
        {
        }

        public override string GetHintText()
        {
            return "Compares units based on their unit type (Infantry, Archers, Mounted).\nFormation Type Order does something similar so for more control use that.\nAscending order Cavalry -> Archers -> Infantry.\nDescending order is Infantry -> Archers -> Cavalry";
        }

        public override string GetName()
        {
            return "Unit Type Comparer";
        }

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            if (Descending ? x.Character.IsMounted && !y.Character.IsMounted : x.Character.IsInfantry && !y.Character.IsInfantry) return -1;

            if ((x.Character.IsInfantry && y.Character.IsInfantry) || (x.Character.IsMounted && y.Character.IsMounted) || (x.Character.IsArcher && y.Character.IsArcher)) return EqualSorter?.Compare(x, y) ?? 0;

            if (Descending ? x.Character.IsArcher && y.Character.IsInfantry : x.Character.IsArcher && y.Character.IsMounted) return -1;

            if (Descending ? x.Character.IsArcher && y.Character.IsMounted : x.Character.IsArcher && y.Character.IsInfantry) return 1;

            if (Descending ? x.Character.IsInfantry && !y.Character.IsInfantry : x.Character.IsMounted && !y.Character.IsMounted) return 1;

            return -1;
        }

        public override void FillCustomList()
        {
            base.FillCustomList();
            this._compDictionary = new Dictionary<string, Func<PartyCharacterVM, PartyCharacterVM, bool>>();
        }
    }
}
