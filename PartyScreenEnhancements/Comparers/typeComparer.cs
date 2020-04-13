using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace PartyScreenEnhancements.Comparers
{
    public class TypeComparer : PartySort
    {

        private readonly PartySort _equalSorter;
        private bool _descending;

        public TypeComparer(PartySort equalSorter, bool descending)
        {
            this._equalSorter = equalSorter;
            this._descending = descending;
        }

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (_descending ? x.CurrentFormationClass < y.CurrentFormationClass : x.CurrentFormationClass > y.CurrentFormationClass)
            {
                return 1;
            }
            else if (y.CurrentFormationClass == x.CurrentFormationClass)
            {
                if (_equalSorter != null)
                    return _equalSorter.Compare(x, y);
                else
                    return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
