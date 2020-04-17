using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using PartyScreenEnhancements.ViewModel.Settings.Options;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.ViewModel.Settings
{
    public class SettingGeneralPaneVM : TaleWorlds.Library.ViewModel
    {

        private MBBindingList<GenericOptionDataVM> _options;
        private ExtraSettings _settings;

        public SettingGeneralPaneVM()
        {
            this.Name = "General";
            this._options = new MBBindingList<GenericOptionDataVM>();
            this._settings = PartyScreenConfig.ExtraSettings;
            AddSettingVms();
            
        }

        public void AddSettingVms()
        {
            _options.Add(new BooleanOptionDataVM(_settings.DisplayCategoryNumbers, "Display Category Numbers", "Display how many infantry, archers, and cavalry you have", (value) => _settings.DisplayCategoryNumbers = value));
            _options.Add(new BooleanOptionDataVM(_settings.RecruitByDefault, "Recruit By Default", "Determines all prisoners are eligible for recruitment with the Recruit All button.\n If turned off you'll have to CTRL + click each prisoner's recruitment button to allow their recruitment.", (value) => _settings.RecruitByDefault = value));
            _options.Add(new BooleanOptionDataVM(_settings.ShowGeneralLogMessage, "Display Log Messages", "Display log messages on the bottom left detailing how many units were upgraded/recruited.", (value) => _settings.ShowGeneralLogMessage = value));
        }

        public new void OnFinalize()
        {
            this._options = null;
            this._settings = null;
            PartyScreenConfig.Save();
        }

        [DataSourceProperty]
        public string Name { get; set; }

        [DataSourceProperty]
        public MBBindingList<GenericOptionDataVM> Options
        {
            get => _options;

            set
            {
                if (value != this._options)
                {
                    this._options = value;
                    base.OnPropertyChanged(nameof(Options));
                }
            }
        }

    }
}
