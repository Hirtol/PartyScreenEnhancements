﻿using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.ViewModel.Settings
{
    public class SettingSortingOrderVM : TaleWorlds.Library.ViewModel
    {
        private HintViewModel _settingHint;

        public SettingSortingOrderVM(string displayText)
        {
            this.SettingHint = new HintViewModel(new TextObject(displayText));;
            this.Name = displayText;
        }

        [DataSourceProperty]
        public string Name { get; }

        [DataSourceProperty]
        public HintViewModel SettingHint
        {
            get => _settingHint;
            set
            {
                if (value != this._settingHint)
                {
                    this._settingHint = value;
                    base.OnPropertyChanged(nameof(SettingHint));
                }
            }
        }
    }
}
