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

        private string _displayText;

        public SettingSortingOrderVM(string displayText)
        {
            this.SettingHint = new HintViewModel(displayText);
            this._displayText = displayText;
        }


        [DataSourceProperty] public string Name => _displayText;

        [DataSourceProperty]
        public HintViewModel SettingHint
        {
            get { return _settingHint; }
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
