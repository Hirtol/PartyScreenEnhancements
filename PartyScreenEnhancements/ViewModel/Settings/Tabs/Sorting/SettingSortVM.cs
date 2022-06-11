using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.ViewModel.Settings.Sorting;
using System;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.ViewModel.Settings.Tabs.Sorting
{
    public class SettingSortVM : TaleWorlds.Library.ViewModel
    {
        private bool _isTransferable;

        private HintViewModel _settingHint;
        private HintViewModel _transferHint;
        private HintViewModel _ascDescHint;

        private Action<SettingSortVM, SettingSide> _transferCallBack;
        private Action<SettingSortVM> _openSubSetting;

        private SettingSide _side;

        public SettingSortVM(PartySort sortingComparer, Action<SettingSortVM, SettingSide> transferCallBack, SettingSide side, Action<SettingSortVM> openSubSetting)
        {
            SortingComparer = sortingComparer;
            SettingHint = new HintViewModel(new TextObject(SortingComparer.GetHintText()));
            _transferHint = new HintViewModel(new TextObject($"Click to transfer to the {side.GetOtherSide().ToString().ToLower()} side!"));
            AscDescHint = new HintViewModel(new TextObject($"Current Mode: {(IsDescending ? "Descending" : "Ascending")}"));

            IsTransferable = true;
            IsDescending = SortingComparer.Descending;

            _transferCallBack = transferCallBack;
            _openSubSetting = openSubSetting;
            _side = side;
        }

        public void ExecuteChangeDirection()
        {
            IsDescending = !IsDescending;
            SortingComparer.Descending = IsDescending;
        }

        public void ExecuteOpenSubSetting()
        {
            if (SortingComparer.HasCustomSettings())
            {
                _openSubSetting(this);
            }
        }

        public void TransferSides()
        {
            _transferCallBack(this, _side);
        }

        public void UpdateValues(SettingSide newSide)
        {
            Side = newSide;
        }

        [DataSourceProperty]
        public PartySort SortingComparer { get; set; }

        [DataSourceProperty]
        public string Name => SortingComparer.GetName();

        [DataSourceProperty]
        public bool HasCustomSetting => SortingComparer.HasCustomSettings();

        [DataSourceProperty]
        public bool IsDescending
        {
            get => SortingComparer.Descending;
            set
            {
                if (value != SortingComparer.Descending)
                {
                    SortingComparer.Descending = value;
                    base.OnPropertyChanged(nameof(IsDescending));
                    AscDescHint.HintText = new TextObject($"Current Mode: {(value ? "Descending" : "Ascending")}");
                }
            }
        }

        [DataSourceProperty]
        public bool IsTransferable
        {
            get => _isTransferable;
            set
            {
                if (value != _isTransferable)
                {
                    _isTransferable = value;
                    base.OnPropertyChanged(nameof(IsTransferable));
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel AscDescHint
        {
            get => _ascDescHint;
            set
            {
                if (value != _ascDescHint)
                {
                    _ascDescHint = value;
                    base.OnPropertyChanged(nameof(AscDescHint));
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel SettingHint
        {
            get => _settingHint;
            set
            {
                if (value != _settingHint)
                {
                    _settingHint = value;
                    base.OnPropertyChanged(nameof(SettingHint));
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel TransferHint
        {
            get => _transferHint;
            set
            {
                if (value != _transferHint)
                {
                    _transferHint = value;
                    base.OnPropertyChanged(nameof(TransferHint));
                }
            }
        }

        [DataSourceProperty]
        public SettingSide Side
        {
            get => _side;
            set
            {
                if (value != _side)
                {
                    _side = value;
                    base.OnPropertyChanged(nameof(Side));
                    TransferHint.HintText = new TextObject($"Click to transfer to the {Side.GetOtherSide().ToString().ToLower()} side!");
                }
            }
        }

    }

}
