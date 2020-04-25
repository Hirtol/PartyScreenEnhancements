using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace PartyScreenEnhancements.ViewModel.Settings.Options
{
    /**
     * Almost an exact replica of the official BooleanOptionDataVM.
     * The fact that they use hard baked enums made it less of a chore to just roll my own OptionVM than use theirs.
     */
    public class BooleanOptionDataVM : GenericOptionDataVM
    {
        public BooleanOptionDataVM(bool initialValue, string name, string description, Action<bool> setter) : base(name, description, 0)
        {
            this._optionValue = initialValue;
            this.OptionValueAsBoolean = this._optionValue;
            this._setter = setter;
            this.ImageIDs = new string[]
            {
                name.ToString() + "_0",
                name.ToString() + "_1"
            };
        }

        [DataSourceProperty]
        public bool OptionValueAsBoolean
        {
            get
            {
                return this._optionValue;
            }
            set
            {
                if (value != this._optionValue)
                {
                    this._optionValue = value;
                    _setter(value);
                    base.OnPropertyChanged(nameof(OptionValueAsBoolean));
                }
            }
        }

        private bool _optionValue;
        private Action<bool> _setter;
    }
}
