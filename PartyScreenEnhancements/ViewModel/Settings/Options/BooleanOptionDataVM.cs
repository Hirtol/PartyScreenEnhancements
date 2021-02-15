using System;
using TaleWorlds.Library;

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
