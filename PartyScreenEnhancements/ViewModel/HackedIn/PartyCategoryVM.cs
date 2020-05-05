using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.HackedIn
{
    public class PartyCategoryVM : TaleWorlds.Library.ViewModel
    {
        private MBBindingList<PartyCharacterVM> _subList;

        public PartyCategoryVM(MBBindingList<PartyCharacterVM> sublist)
        {
            this._subList = sublist;
        }


        [DataSourceProperty]
        public MBBindingList<PartyCharacterVM> TroopList
        {
            get => _subList;
            set
            {
                if (value != this._subList)
                {
                    _subList = value;
                    base.OnPropertyChanged(nameof(TroopList));
                }
            }
        }

        [DataSourceProperty]
        public string Label
        {
            get => "Testing World!";
            set
            {
                
            }
        }
    }
}
