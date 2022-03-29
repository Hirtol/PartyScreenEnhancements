using PartyScreenEnhancements.Saving;
using System;
using System.ComponentModel;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Library;

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
            InfantryLabel = "Infantry: NaN";
            ArchersLabel = "Archers: NaN";
            CavalryLabel = "Cavalry: NaN";
            HorseArcherLabel = "Horse Archers: NaN";

            InfantryGarrisonLabel = "Infantry: NaN";
            ArchersGarrisonLabel = "Archers: NaN";
            CavalryGarrisonLabel = "Cavalry: NaN";
            _horseArcherGarrisonLabel = "Horse Archers: NaN";

            _mainPartyList = mainPartyList;
            _otherPartyList = otherParty;
            IsEnabled = PartyScreenConfig.ExtraSettings.DisplayCategoryNumbers;
            ShouldShowGarrison = shouldShowGarrison;

            _logic = logic;
            _logic.UpdateDelegate = Delegate.Combine(_logic.UpdateDelegate, new PartyScreenLogic.PresentationUpdate(RefreshDelegate)) as PartyScreenLogic.PresentationUpdate;
        }

        public override void OnFinalize()
        {
            base.OnFinalize();

            _logic.UpdateDelegate = Delegate.Remove(_logic.UpdateDelegate, new PartyScreenLogic.PresentationUpdate(RefreshDelegate)) as PartyScreenLogic.PresentationUpdate;
            PartyScreenConfig.ExtraSettings.PropertyChanged -= OnEnableChange;

            _mainPartyList = null;
            _otherPartyList = null;
            _logic = null;

        }

        public void RefreshDelegate(PartyScreenLogic.PartyCommand command)
        {
            RefreshValues();
        }

        public void OnEnableChange(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(PartyScreenConfig.ExtraSettings
                .DisplayCategoryNumbers)))
            {
                IsEnabled = PartyScreenConfig.ExtraSettings.DisplayCategoryNumbers;
            }
        }

        public new void RefreshValues()
        {
            try
            {
                base.RefreshValues();
                if (IsEnabled)
                {
                    int infantry = 0, archers = 0, cavalry = 0, horseArchers = 0;

                    foreach (PartyCharacterVM character in _mainPartyList)
                    {
                        if (character?.Character != null)
                        {
                            if (character.Character.IsMounted && character.Character.IsRanged)
                                horseArchers += character.Number;
                            else if (character.Character.IsMounted) cavalry += character.Number;
                            else if (character.Character.IsRanged) archers += character.Number;
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
                            if (character.Character.IsMounted && character.Character.IsRanged)
                                horseArchers += character.Number;
                            else if (character.Character.IsMounted) cavalry += character.Number;
                            else if (character.Character.IsRanged) archers += character.Number;
                            else if (character.Character.IsInfantry) infantry += character.Number;

                        }
                    }

                    InfantryGarrisonLabel = $"Infantry: {infantry}";
                    ArchersGarrisonLabel = $"Archers: {archers}";
                    CavalryGarrisonLabel = $"Cavalry: {cavalry}";
                    HorseArcherGarrisonLabel = $"Horse Archers: {horseArchers}";
                }
            }
            catch (Exception e)
            {
                Logging.Log(Logging.Levels.ERROR, $"Unit Tally: {e}");
                Utilities.DisplayMessage($"PSE Unit Tally Label Update Exception {e}");
            }
        }

        [DataSourceProperty]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    base.OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        [DataSourceProperty]
        public bool ShouldShowGarrison
        {
            get => _shouldShowGarrison && _isEnabled;
            set
            {
                if (value != _shouldShowGarrison)
                {
                    _shouldShowGarrison = value;
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
                return _infantryLabel;
            }
            set
            {
                if (value != _infantryLabel)
                {
                    _infantryLabel = value;
                    base.OnPropertyChanged(nameof(InfantryLabel));
                }
            }
        }

        [DataSourceProperty]
        public string ArchersLabel
        {
            get
            {
                return _archersLabel;
            }
            set
            {
                if (value != _archersLabel)
                {
                    _archersLabel = value;
                    base.OnPropertyChanged(nameof(ArchersLabel));
                }
            }
        }

        [DataSourceProperty]
        public string CavalryLabel
        {
            get
            {
                return _cavalryLabel;
            }
            set
            {
                if (value != _cavalryLabel)
                {
                    _cavalryLabel = value;
                    base.OnPropertyChanged(nameof(CavalryLabel));
                }
            }
        }

        [DataSourceProperty]
        public string HorseArcherLabel
        {
            get
            {
                return _horseArcherLabel;
            }
            set
            {
                if (value != _horseArcherLabel)
                {
                    _horseArcherLabel = value;
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
                return _infantryGarrisonLabel;
            }
            set
            {
                if (value != _infantryGarrisonLabel)
                {
                    _infantryGarrisonLabel = value;
                    base.OnPropertyChanged(nameof(InfantryGarrisonLabel));
                }
            }
        }

        [DataSourceProperty]
        public string ArchersGarrisonLabel
        {
            get
            {
                return _archersGarrisonLabel;
            }
            set
            {
                if (value != _archersGarrisonLabel)
                {
                    _archersGarrisonLabel = value;
                    base.OnPropertyChanged(nameof(ArchersGarrisonLabel));
                }
            }
        }

        [DataSourceProperty]
        public string CavalryGarrisonLabel
        {
            get
            {
                return _cavalryGarrisonLabel;
            }
            set
            {
                if (value != _cavalryGarrisonLabel)
                {
                    _cavalryGarrisonLabel = value;
                    base.OnPropertyChanged(nameof(CavalryGarrisonLabel));
                }
            }
        }

        [DataSourceProperty]
        public string HorseArcherGarrisonLabel
        {
            get
            {
                return _horseArcherGarrisonLabel;
            }
            set
            {
                if (value != _horseArcherGarrisonLabel)
                {
                    _horseArcherGarrisonLabel = value;
                    base.OnPropertyChanged(nameof(HorseArcherGarrisonLabel));
                }
            }
        }
    }
}
