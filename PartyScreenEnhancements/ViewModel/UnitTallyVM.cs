using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.ViewModel
{
    public class UnitTallyVM : TaleWorlds.Library.ViewModel
    {
        private string _infantryLabel;
        private string _archersLabel;
        private string _cavalryLabel;
        private bool _isEnabled;
        private MBBindingList<PartyCharacterVM> _mainPartyList;

        public UnitTallyVM(MBBindingList<PartyCharacterVM> mainPartyList)
        {
            PartyScreenConfig.ExtraSettings.PropertyChanged += OnEnableChange;
            this.InfantryLabel = "Infantry: NaN";
            this.ArchersLabel = "Archers: NaN";
            this.CavalryLabel = "Cavalry: NaN";
            this._mainPartyList = mainPartyList;
            this.IsEnabled = PartyScreenConfig.ExtraSettings.DisplayCategoryNumbers;
            this.RefreshValues();
        }

        public void OnEnableChange(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(PartyScreenConfig.ExtraSettings
                .DisplayCategoryNumbers)))
            {
                this.IsEnabled = PartyScreenConfig.ExtraSettings.DisplayCategoryNumbers;
            }
        }

        public new void RefreshValues()
        {
            base.RefreshValues();
            if(IsEnabled)
            {
                int infantry = 0, archers = 0, cavalry = 0;

                foreach (PartyCharacterVM character in _mainPartyList)
                {
                    if (character != null)
                    {
                        if (character.Character.IsMounted) cavalry += character.Number;
                        else if (character.Character.IsArcher) archers += character.Number;
                        else if (character.Character.IsInfantry) infantry += character.Number;

                    }
                }

                InfantryLabel = $"Infantry: {infantry}";
                ArchersLabel = $"Archers: {archers}";
                CavalryLabel = $"Cavalry: {cavalry}";
            }
        }

        [DataSourceProperty]
        public bool IsEnabled
        {
            get
            {
                return this._isEnabled;
            }
            set
            {
                if (value != this._isEnabled)
                {
                    this._isEnabled = value;
                    base.OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        [DataSourceProperty]
        public string InfantryLabel
        {
            get
            {
                return this._infantryLabel;
            }
            set
            {
                if (value != this._infantryLabel)
                {
                    this._infantryLabel = value;
                    base.OnPropertyChanged(nameof(InfantryLabel));
                }
            }
        }

        [DataSourceProperty]
        public string ArchersLabel
        {
            get
            {
                return this._archersLabel;
            }
            set
            {
                if (value != this._archersLabel)
                {
                    this._archersLabel = value;
                    base.OnPropertyChanged(nameof(ArchersLabel));
                }
            }
        }

        [DataSourceProperty]
        public string CavalryLabel
        {
            get
            {
                return this._cavalryLabel;
            }
            set
            {
                if (value != this._cavalryLabel)
                {
                    this._cavalryLabel = value;
                    base.OnPropertyChanged(nameof(CavalryLabel));
                }
            }
        }
    }
}
