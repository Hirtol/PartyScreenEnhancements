using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings.Tabs.Sorting
{
    public class SettingSorterOverlayVM : TaleWorlds.Library.ViewModel
    {
        private SettingSorterPaneVM _mainParty;
        private SettingSorterPaneVM _mainPrisoners;
        private SettingSorterPaneVM _mainGarrison;

        private string _name;

        public SettingSorterOverlayVM(SettingScreenVM _parent)
        {
            this._mainParty = new SettingSorterPaneVM(_parent, "Main Party", PartyScreenConfig.ExtraSettings.PartySorter, value => PartyScreenConfig.ExtraSettings.PartySorter = value);
            this._mainPrisoners = new SettingSorterPaneVM(_parent, "Prisoners", PartyScreenConfig.ExtraSettings.PrisonerSorter, value => PartyScreenConfig.ExtraSettings.PrisonerSorter = value);
            this._mainGarrison = new SettingSorterPaneVM(_parent, "Garrisons", PartyScreenConfig.ExtraSettings.GarrisonSorter, value => PartyScreenConfig.ExtraSettings.GarrisonSorter = value);
            this._name = "Sorters";
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _mainParty.OnFinalize();
            _mainPrisoners.OnFinalize();
            _mainGarrison.OnFinalize();

            _mainParty = null;
            _mainPrisoners = null;
            _mainGarrison = null;
            _name = null;
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
            get => this._mainGarrison;
            set
            {
                if (value != _mainGarrison)
                {
                    _mainGarrison = value;
                    base.OnPropertyChanged(nameof(GarrisonSorterPane));
                }
            }
        }


        [DataSourceProperty]
        public bool HasSeparateSorting
        {
            get => PartyScreenConfig.ExtraSettings.SeparateSortingProfiles;
            set
            {
                if (value != PartyScreenConfig.ExtraSettings.SeparateSortingProfiles)
                {
                    PartyScreenConfig.ExtraSettings.SeparateSortingProfiles = value;
                    base.OnPropertyChanged(nameof(HasSeparateSorting));
                }
            }
        }
    }
}
