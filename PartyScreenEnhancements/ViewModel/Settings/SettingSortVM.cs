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

        private Action<SettingSortVM, SettingSide> _transferCallBack;

        private SettingSide _side;

        public SettingSortVM(PartySort sortingComparer, Action<SettingSortVM, SettingSide> transferCallBack, SettingSide side)
        {
            this.SortingComparer = sortingComparer;
            this.SettingHint = new HintViewModel(SortingComparer.GetHintText());
            this._transferHint = new HintViewModel($"Click to transfer to the {side.GetOtherSide().ToString().ToLower()} side!");
            this.IsTransferable = true;
            this._transferCallBack = transferCallBack;
            this._side = side;
        }

        public void ExecuteSetSelected()
        {
            InformationManager.DisplayMessage(new InformationMessage("Hey, selected " + SortingComparer.GetName()));
        }

        public void TransferSides()
        {
            this._transferCallBack(this, this._side);
        }


        [DataSourceProperty]
        public PartySort SortingComparer { get; }

        [DataSourceProperty]
        public string Name => SortingComparer.GetName();

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
                }
            }
        }

    }

}
