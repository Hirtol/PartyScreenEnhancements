using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace PartyScreenEnhancements.ViewModel
{
    public class UnitTallyVM : TaleWorlds.Library.ViewModel
    {
        private string _infantryLabel;
        private string _archersLabel;
        private string _cavalryLabel;
        private string _horseArcherLabel;

        private string _infantryGarrisonLabel;
        private string _archersGarrisonLabel;
        private string _cavalryGarrisonLabel;
        private string _horseArcherGarrisonLabel;

        private bool _isEnabled;
        private bool _shouldShowGarrison;
        private MBBindingList<PartyCharacterVM> _mainPartyList;
        private MBBindingList<PartyCharacterVM> _otherPartyList;
        private PartyScreenLogic _logic;

        public UnitTallyVM(MBBindingList<PartyCharacterVM> mainPartyList, MBBindingList<PartyCharacterVM> otherParty, PartyScreenLogic logic, bool shouldShowGarrison)
        {
            PartyScreenConfig.ExtraSettings.PropertyChanged += OnEnableChange;
            this.InfantryLabel = "Infantry: NaN";
            this.ArchersLabel = "Archers: NaN";
            this.CavalryLabel = "Cavalry: NaN";
            this.HorseArcherLabel = "Horse Archers: NaN";

            this.InfantryGarrisonLabel = "Infantry: NaN";
            this.ArchersGarrisonLabel = "Archers: NaN";
            this.CavalryGarrisonLabel = "Cavalry: NaN";
            this._horseArcherGarrisonLabel = "Horse Archers: NaN";

            this._mainPartyList = mainPartyList;
            this._otherPartyList = otherParty;
            this.IsEnabled = PartyScreenConfig.ExtraSettings.DisplayCategoryNumbers;
            this.ShouldShowGarrison = shouldShowGarrison;

            this._logic = logic;
            _logic.UpdateDelegate = Delegate.Combine(_logic.UpdateDelegate, new PartyScreenLogic.PresentationUpdate(RefreshDelegate)) as PartyScreenLogic.PresentationUpdate;
        }

        public override void OnFinalize()
        {
            base.OnFinalize();

            _logic.UpdateDelegate = Delegate.Remove(_logic.UpdateDelegate, new PartyScreenLogic.PresentationUpdate(RefreshDelegate)) as PartyScreenLogic.PresentationUpdate;

            this._mainPartyList = null;
            this._otherPartyList = null;
            this._logic = null;

        }

        public void RefreshDelegate(PartyScreenLogic.PartyCommand command)
        {
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
                int infantry = 0, archers = 0, cavalry = 0, horseArchers = 0;

                foreach (PartyCharacterVM character in _mainPartyList)
                {
                    if (character?.Character != null)
                    {
                        if (character.Character.IsMounted && character.Character.IsArcher)
                            horseArchers += character.Number;
                        else if (character.Character.IsMounted) cavalry += character.Number;
                        else if (character.Character.IsArcher) archers += character.Number;
                        else if (character.Character.IsInfantry) infantry += character.Number;

                    }
                }

                InfantryLabel = $"Infantry: {infantry}";
                ArchersLabel = $"Archers: {archers}";
                CavalryLabel = $"Cavalry: {cavalry}";
                HorseArcherLabel = $"Horse Archers: {horseArchers}";
            }

            if (ShouldShowGarrison && _otherPartyList != null)
            {
                int infantry = 0, archers = 0, cavalry = 0, horseArchers = 0;

                foreach (PartyCharacterVM character in _otherPartyList)
                {
                    if (character?.Character != null)
                    {
                        if (character.Character.IsMounted && character.Character.IsArcher)
                            horseArchers += character.Number;
                        else if (character.Character.IsMounted) cavalry += character.Number;
                        else if (character.Character.IsArcher) archers += character.Number;
                        else if (character.Character.IsInfantry) infantry += character.Number;

                    }
                }

                InfantryGarrisonLabel = $"Infantry: {infantry}";
                ArchersGarrisonLabel = $"Archers: {archers}";
                CavalryGarrisonLabel = $"Cavalry: {cavalry}";
                HorseArcherGarrisonLabel = $"Horse Archers: {horseArchers}";
            }
        }

        [DataSourceProperty]
        public bool IsEnabled
        {
            get => this._isEnabled;
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
        public bool ShouldShowGarrison
        {
            get => this._shouldShowGarrison && this._isEnabled;
            set
            {
                if (value != this._shouldShowGarrison)
                {
                    this._shouldShowGarrison = value;
                    base.OnPropertyChanged(nameof(ShouldShowGarrison));
                }
            }
        }

        // Party Labels

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

        [DataSourceProperty]
        public string HorseArcherLabel
        {
            get
            {
                return this._horseArcherLabel;
            }
            set
            {
                if (value != this._horseArcherLabel)
                {
                    this._horseArcherLabel = value;
                    base.OnPropertyChanged(nameof(HorseArcherLabel));
                }
            }
        }

        // Garrison Labels

        [DataSourceProperty]
        public string InfantryGarrisonLabel
        {
            get
            {
                return this._infantryGarrisonLabel;
            }
            set
            {
                if (value != this._infantryGarrisonLabel)
                {
                    this._infantryGarrisonLabel = value;
                    base.OnPropertyChanged(nameof(InfantryGarrisonLabel));
                }
            }
        }

        [DataSourceProperty]
        public string ArchersGarrisonLabel
        {
            get
            {
                return this._archersGarrisonLabel;
            }
            set
            {
                if (value != this._archersGarrisonLabel)
                {
                    this._archersGarrisonLabel = value;
                    base.OnPropertyChanged(nameof(ArchersGarrisonLabel));
                }
            }
        }

        [DataSourceProperty]
        public string CavalryGarrisonLabel
        {
            get
            {
                return this._cavalryGarrisonLabel;
            }
            set
            {
                if (value != this._cavalryGarrisonLabel)
                {
                    this._cavalryGarrisonLabel = value;
                    base.OnPropertyChanged(nameof(CavalryGarrisonLabel));
                }
            }
        }

        [DataSourceProperty]
        public string HorseArcherGarrisonLabel
        {
            get
            {
                return this._horseArcherGarrisonLabel;
            }
            set
            {
                if (value != this._horseArcherGarrisonLabel)
                {
                    this._horseArcherGarrisonLabel = value;
                    base.OnPropertyChanged(nameof(HorseArcherGarrisonLabel));
                }
            }
        }
    }
}
