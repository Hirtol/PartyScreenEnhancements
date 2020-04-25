using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Comparers;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings
{
    public class SettingSortingOrderVM : TaleWorlds.Library.ViewModel
    {
        private HintViewModel _settingHint;

        public SettingSortingOrderVM(string displayText)
        {
            this.SettingHint = new HintViewModel(displayText);
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
