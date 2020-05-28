using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Extensions;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.HackedIn
{
    public class PartyCategoryVM : TaleWorlds.Library.ViewModel
    {
        public const string CATEGORY_LABEL_PREFIX = "PSE_CATEGORY_";

        private MBBindingList<PartyCharacterVM> _subList;

        //TODO: Fix  the listpanel being extended due to formation selector. will need custom parent widget to only expand based on the formation
        // list expansion, as well as the basic id="SecondaryList"
        private SelectorVM<SelectorItemVM> _currentFormationSelector;
        private string _name;
        private string _transferLabel;
        private string _troopNumberLabel;

        public PartyCategoryVM(MBBindingList<PartyCharacterVM> sublist, CategoryInformation information, string parentTag, IEnumerable<SelectorItemVM> formationList)
        {
            this._subList = sublist;
            this.Information = information;
            this._currentFormationSelector = new SelectorVM<SelectorItemVM>(new List<string>(), information.SelectedFormation, ExecuteSetAllTroopsToFormation);
            this._currentFormationSelector.ItemList.AddRange(formationList);
            this._name = information.Name;
            this._transferLabel = CATEGORY_LABEL_PREFIX + _name;
            this.ParentTag = parentTag;
            UpdateLabel();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();

            this.Information = null;
            this.TroopList = null;
            this.CharacterFormationSelector = null;
        }

        public void Rename(string newName)
        {
            this.Label = newName;
        }

        public void ExecuteSetAllTroopsToFormation(SelectorVM<SelectorItemVM> vm)
        {
            this.Information.SelectedFormation = CharacterFormationSelector.SelectedIndex;

            if ((FormationClass) vm.SelectedIndex != FormationClass.Unset)
            {
                foreach (PartyCharacterVM character in _subList)
                {
                    character.Character.CurrentFormationClass = (FormationClass) vm.SelectedIndex;
                }
            }
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


        public CategoryInformation Information { get; private set; }

        public string ParentTag { get; set; }


        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> CharacterFormationSelector
        {
            get => this._currentFormationSelector;
            set
            {
                if (value != this._currentFormationSelector)
                {
                    this._currentFormationSelector = value;
                    base.OnPropertyChanged(nameof(CharacterFormationSelector));
                }
            }
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
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    this.Information.Name = value;
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
