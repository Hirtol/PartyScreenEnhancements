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
        public const string CATEGORY_LABEL_PREFIX = "PSE_CATEGORY_";

        private MBBindingList<PartyCharacterVM> _subList;
        private string _name;
        private string _transferLabel;
        private string _troopNumberLabel;
        

        public PartyCategoryVM(MBBindingList<PartyCharacterVM> sublist, string name, string parentTag)
        {
            this._subList = sublist;
            this._name = name;
            this._transferLabel = CATEGORY_LABEL_PREFIX + _name;
            this.ParentTag = parentTag;
            UpdateLabel();

        }

        public void UpdateLabel()
        {
            int totalTroops = _subList.Sum(character => Math.Max(0, character.Troop.Number));
            int healthyTroops = _subList.Sum(character => Math.Max(0, character.Troop.Number - character.WoundedCount));
            int wounded = _subList.Sum(item =>
            {
                if (item.Number < item.WoundedCount)
                {
                    return 0;
                }
                return item.WoundedCount;
            });
            this.TroopNumberLabel = $"({healthyTroops} + {wounded}w / {totalTroops})";
        }


        public string ParentTag { get; set; }

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
            set
            {
                if (value != _transferLabel)
                {
                    _transferLabel = value;
                    base.OnPropertyChanged(nameof(TransferLabel));
                }
            }
        }

        [DataSourceProperty]
        public bool IsHeaderVisible
        {
            get => true;
        }
    }
}
