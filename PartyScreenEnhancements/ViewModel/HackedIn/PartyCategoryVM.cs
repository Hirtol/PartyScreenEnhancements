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
        private string _name;
        private string _transferLabel;
        private string _troopNumberLabel;
        

        public PartyCategoryVM(MBBindingList<PartyCharacterVM> sublist, string name, Func<MBBindingList<PartyCharacterVM>, int, string> troopUpdate, Category category)
        {
            this._subList = sublist;
            this._name = name;
            this._troopNumberLabel = troopUpdate(sublist, sublist.Count);
            this._transferLabel = "PSE_" + _name;
            this.Category = category;
        }

        public Category Category { get; set; }

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
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    base.OnPropertyChanged(nameof(Label));
                }
            }
        }

        [DataSourceProperty]
        public string TroopNumberLabel
        {
            get => _troopNumberLabel;
            set
            {
                if (value != _troopNumberLabel)
                {
                    _troopNumberLabel = value;
                    base.OnPropertyChanged(nameof(TroopNumberLabel));
                }
            }
        }


        [DataSourceProperty]
        public string TransferLabel
        {
            get => _transferLabel;
        }

        [DataSourceProperty]
        public bool IsHeaderVisible
        {
            get => this.Category != Category.SYSTEM;
        }
    }
}
