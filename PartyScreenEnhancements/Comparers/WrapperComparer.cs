using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using PartyScreenEnhancements.ViewModel.HackedIn;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class WrapperComparer : IComparer<PSEWrapperVM>
    {

        private PartySort _otherSorter;

        public WrapperComparer(PartySort otherSorter)
        {
            _otherSorter = otherSorter;
        }

        public int Compare(PSEWrapperVM x, PSEWrapperVM y)
        {
            if (x.IsCategory || y.IsCategory) 
                return 0;
            else 
                return _otherSorter.Compare(x.WrapperViewModel as PartyCharacterVM, y.WrapperViewModel as PartyCharacterVM);
        }
    }
}
