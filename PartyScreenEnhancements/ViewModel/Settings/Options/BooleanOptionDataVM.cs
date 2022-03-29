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
            _optionValue = initialValue;
            OptionValueAsBoolean = _optionValue;
            _setter = setter;
            ImageIDs = new string[]
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
                return _optionValue;
            }
            set
            {
                if (value != _optionValue)
                {
                    _optionValue = value;
                    _setter(value);
                    base.OnPropertyChanged(nameof(OptionValueAsBoolean));
                }
            }
        }

        private bool _optionValue;
        private Action<bool> _setter;
    }
}
