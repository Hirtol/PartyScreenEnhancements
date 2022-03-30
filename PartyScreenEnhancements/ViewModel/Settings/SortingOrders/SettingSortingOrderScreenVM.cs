using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.Saving;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings.SortingOrders
{
    public class SettingSortingOrderScreenVM : TaleWorlds.Library.ViewModel
    {
        private SettingScreenVM _settingScreen;
        private PartySort _sorter;

        private MBBindingList<SettingSortingOrderVM> _sortingOrder;

        public SettingSortingOrderScreenVM(SettingScreenVM parent, PartySort sorter)
        {
            _settingScreen = parent;
            _sorter = sorter;
            _sortingOrder = new MBBindingList<SettingSortingOrderVM>();
            Name = sorter.GetName();

            InitializeList();
        }

        public void ExecuteCloseSettings()
        {
            _settingScreen.CloseSubSetting();
            OnFinalize();
        }

        public new void OnFinalize()
        {
            base.OnFinalize();
            SaveList();
            PartyScreenConfig.Save();

            _sortingOrder = null;
            _settingScreen = null;
            _sorter = null;

        }

        public void ExecuteListTransfer(SettingSortingOrderVM sorter, int index, string targetTag)
        {
            _sortingOrder.Remove(sorter);
            if (index > _sortingOrder.Count)
                _sortingOrder.Insert(index - 1, sorter);
            else
                _sortingOrder.Insert(index, sorter);
        }

        private void InitializeList()
        {
            if (_sorter.CustomSettingsList == null || _sorter.CustomSettingsList.IsEmpty())
            {
                _sorter.FillCustomList();
            }
            _sorter.CustomSettingsList?.ForEach(item =>
            {
                SortingOrder.Add(new SettingSortingOrderVM(item));
            });
        }

        private void SaveList()
        {
            _sorter.CustomSettingsList.Clear();
            _sortingOrder.ApplyActionOnAllItems(item => _sorter.CustomSettingsList.Add(item.Name));
        }

        [DataSourceProperty]
        public MBBindingList<SettingSortingOrderVM> SortingOrder
        {
            get => _sortingOrder;
            set
            {
                if (value != _sortingOrder)
                {
                    _sortingOrder = value;
                    base.OnPropertyChanged(nameof(SortingOrder));
                }
            }
        }

        [DataSourceProperty]
        public string Name { get; set; }
    }
}
