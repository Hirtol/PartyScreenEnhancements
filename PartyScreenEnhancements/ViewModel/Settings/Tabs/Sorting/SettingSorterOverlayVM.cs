using System.ComponentModel;
using PartyScreenEnhancements.Saving;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings.Tabs.Sorting
{
    public class SettingSorterOverlayVM : TaleWorlds.Library.ViewModel
    {
        private SettingSorterPaneVM _mainParty;
        private SettingSorterPaneVM _mainPrisoners;
        private SettingSorterPaneVM _mainGarrisonAllied;

        private string _name;
        private bool _hasSeparateSorting;

        public SettingSorterOverlayVM(SettingScreenVM _parent)
        {
            PartyScreenConfig.ExtraSettings.PropertyChanged += OnEnableChange;
            this._mainParty = new SettingSorterPaneVM(_parent, "Main Party", PartyScreenConfig.ExtraSettings.PartySorter, value => PartyScreenConfig.ExtraSettings.PartySorter = value);
            this._mainPrisoners = new SettingSorterPaneVM(_parent, "Prisoners", PartyScreenConfig.ExtraSettings.PrisonerSorter, value => PartyScreenConfig.ExtraSettings.PrisonerSorter = value);
            this._mainGarrisonAllied = new SettingSorterPaneVM(_parent, "Garrisons/Allied", PartyScreenConfig.ExtraSettings.GarrisonAndAlliedPartySorter, value => PartyScreenConfig.ExtraSettings.GarrisonAndAlliedPartySorter = value);
            this._name = "Sorters";
            this._hasSeparateSorting = PartyScreenConfig.ExtraSettings.SeparateSortingProfiles;
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            PartyScreenConfig.ExtraSettings.PropertyChanged -= OnEnableChange;
            _mainParty.OnFinalize();
            _mainPrisoners.OnFinalize();
            _mainGarrisonAllied.OnFinalize();

            _mainParty = null;
            _mainPrisoners = null;
            _mainGarrisonAllied = null;
            _name = null;
        }

        public void OnEnableChange(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(PartyScreenConfig.ExtraSettings
                .SeparateSortingProfiles)))
            {
                this.HasSeparateSorting = PartyScreenConfig.ExtraSettings.SeparateSortingProfiles;
            }
        }

    [DataSourceProperty]
        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    base.OnPropertyChanged(nameof(Name));
                }
            }
        }

        [DataSourceProperty]
        public SettingSorterPaneVM PartySorterPane
        {
            get => this._mainParty;
            set
            {
                if (value != _mainParty)
                {
                    _mainParty = value;
                    base.OnPropertyChanged(nameof(PartySorterPane));
                }
            }
        }

        [DataSourceProperty]
        public SettingSorterPaneVM PrisonerSorterPane
        {
            get => this._mainPrisoners;
            set
            {
                if (value != _mainPrisoners)
                {
                    _mainPrisoners = value;
                    base.OnPropertyChanged(nameof(PrisonerSorterPane));
                }
            }
        }

        [DataSourceProperty]
        public SettingSorterPaneVM GarrisonSorterPane
        {
            get => this._mainGarrisonAllied;
            set
            {
                if (value != _mainGarrisonAllied)
                {
                    _mainGarrisonAllied = value;
                    base.OnPropertyChanged(nameof(GarrisonSorterPane));
                }
            }
        }


        [DataSourceProperty]
        public bool HasSeparateSorting
        {
            get => _hasSeparateSorting;
            set
            {
                if (value != _hasSeparateSorting)
                {
                    _hasSeparateSorting = value;
                    base.OnPropertyChanged(nameof(HasSeparateSorting));
                }
            }
        }
    }
}
