using PartyScreenEnhancements.Saving;
using PartyScreenEnhancements.ViewModel.Settings.Options;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings.Tabs
{
    public class SettingGeneralPaneVM : TaleWorlds.Library.ViewModel
    {

        private MBBindingList<GenericOptionDataVM> _options;
        private ExtraSettings _settings;

        public SettingGeneralPaneVM()
        {
            Name = "General";
            _options = new MBBindingList<GenericOptionDataVM>();
            _settings = PartyScreenConfig.ExtraSettings;
            AddSettingVms();

        }

        public void AddSettingVms()
        {
            _options.Add(new BooleanOptionDataVM(_settings.PathSelectTooltips, "Additional Tooltips", "Add the additional tooltip to troop upgrade buttons telling you to use CTRL and SHIFT to select the upgrade paths", (value) => _settings.PathSelectTooltips = value));
            _options.Add(new BooleanOptionDataVM(_settings.ShouldShowCompletePartyNumber, "Show Combined Troop Total", "If enabled will change the party label on the top right to display the total amount of troops in your party, instead of healthy + wounded.", (value) => _settings.ShouldShowCompletePartyNumber = value));
            _options.Add(new BooleanOptionDataVM(_settings.KeepHeroesOnTop, "Keep Companions On Top", "If enabled sorts your companions to the top of the troop list, if disabled puts them on the bottom instead", (value) => _settings.KeepHeroesOnTop = value));
            _options.Add(new BooleanOptionDataVM(_settings.AutomaticSorting, "Automatic Sorting", "Automatically sort everything present upon opening the party screen, or when upgrading/recruiting units using the top buttons", (value) => _settings.AutomaticSorting = value));
            _options.Add(new BooleanOptionDataVM(_settings.SeparateSortingProfiles, "Separate Sorting Profiles", "Use different sorting rules for your main party/prisoners/garrisons", (value) => _settings.SeparateSortingProfiles = value));
            _options.Add(new BooleanOptionDataVM(_settings.EqualUpgrades, "Equally Distributed Upgrades", "Set the Upgrade All button to distribute the available upgrades equally for units with 2 or more upgrade choices.\nNote, any set path preferences will still be adhered to regardless of this setting.", (value) => _settings.EqualUpgrades = value));
            _options.Add(new BooleanOptionDataVM(_settings.DisplayCategoryNumbers, "Display Category Numbers", "Display how many Infantry, Archers, and Cavalry you have on the Party screen", (value) => _settings.DisplayCategoryNumbers = value));
            _options.Add(new BooleanOptionDataVM(_settings.RecruitByDefault, "Recruit By Default", "Determines whether all prisoners eligible for recruitment will be enlisted with the Recruit All button.\nIf turned off you'll have to CTRL + click each prisoner's recruitment button to allow their recruitment explicitly.\n\nNote any previously disallowed units will now be allowed", (value) => _settings.RecruitByDefault = value));
            _options.Add(new BooleanOptionDataVM(_settings.ShowGeneralLogMessage, "Display Log Messages", "Display log messages on the bottom left detailing how many units were upgraded/recruited.", (value) => _settings.ShowGeneralLogMessage = value));
        }

        public new void OnFinalize()
        {
            _options = null;
            _settings = null;
        }

        [DataSourceProperty]
        public string Name { get; set; }

        [DataSourceProperty]
        public MBBindingList<GenericOptionDataVM> Options
        {
            get => _options;

            set
            {
                if (value != _options)
                {
                    _options = value;
                    base.OnPropertyChanged(nameof(Options));
                }
            }
        }

    }
}
