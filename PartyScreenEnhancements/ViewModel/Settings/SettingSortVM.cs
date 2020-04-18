using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Comparers;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings
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
            this.SortingComparer = sortingComparer;
            this.SettingHint = new HintViewModel(SortingComparer.GetHintText());
            this._transferHint = new HintViewModel($"Click to transfer to the {side.GetOtherSide().ToString().ToLower()} side!");
            this.AscDescHint = new HintViewModel($"Current Mode: {(IsDescending ? "Descending" : "Ascending")}");
            this.IsTransferable = true;
            this.IsDescending = SortingComparer.Descending;
            this._transferCallBack = transferCallBack;
            this._openSubSetting = openSubSetting;
            this._side = side;
        }

        public void ExecuteChangeDirection()
        {
            this.IsDescending = !this.IsDescending;
            SortingComparer.Descending = this.IsDescending;
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
            this._transferCallBack(this, this._side);
        }

        public void UpdateValues(SettingSide newSide)
        {
            this.Side = newSide;
        }

        [DataSourceProperty]
        public PartySort SortingComparer
        {
            get;
            set;
        }

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
                if (value != this.SortingComparer.Descending)
                {
                    this.SortingComparer.Descending = value;
                    base.OnPropertyChanged(nameof(IsDescending));
                    this.AscDescHint.HintText = $"Current Mode: {(value ? "Descending" : "Ascending")}";
                }
            }
        }

        [DataSourceProperty]
        public bool IsTransferable
        {
            get { return _isTransferable; }
            set
            {
                if (value != this._isTransferable)
                {
                    this._isTransferable = value;
                    base.OnPropertyChanged(nameof(IsTransferable));
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel AscDescHint
        {
            get { return _ascDescHint; }
            set
            {
                if (value != this._ascDescHint)
                {
                    this._ascDescHint = value;
                    base.OnPropertyChanged(nameof(AscDescHint));
                }
            }
        }

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

        [DataSourceProperty]
        public HintViewModel TransferHint
        {
            get { return _transferHint; }
            set
            {
                if (value != this._transferHint)
                {
                    this._transferHint = value;
                    base.OnPropertyChanged(nameof(TransferHint));
                }
            }
        }

        [DataSourceProperty]
        public SettingSide Side
        {
            get { return _side; }
            set
            {
                if (value != this._side)
                {
                    this._side = value;
                    base.OnPropertyChanged(nameof(Side));
                    this.TransferHint.HintText = $"Click to transfer to the {Side.GetOtherSide().ToString().ToLower()} side!";
                }
            }
        }

    }

}
