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
            this._mainParty = new SettingSorterPaneVM(_parent);
            this._mainPrisoners = new SettingSorterPaneVM(_parent);
            this._mainGarrison = new SettingSorterPaneVM(_parent);
            this._name = "Sorters";
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
        public MBBindingList<SettingSorterPaneVM> SorterList
        {
            get => _sorterPanes;
            set
            {
                if (value != _sorterPanes)
                {
                    _sorterPanes = value;
                    base.OnPropertyChanged(nameof(SorterList));
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
