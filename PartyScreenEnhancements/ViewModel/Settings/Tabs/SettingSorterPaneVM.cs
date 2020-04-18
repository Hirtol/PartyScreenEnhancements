using System.Collections.Generic;
using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.Saving;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings
{
    public class SettingSorterPaneVM : TaleWorlds.Library.ViewModel
    {
        private readonly SettingScreenVM _parent;

        private MBBindingList<SettingSortVM> _possibleSettingList;
        private MBBindingList<SettingSortVM> _settingList;

        public SettingSorterPaneVM(SettingScreenVM parent)
        {
            _parent = parent;
            PossibleSettingList = new MBBindingList<SettingSortVM>();
            SettingList = new MBBindingList<SettingSortVM>();
            Name = "Sorters";

            InitialiseSettingLists();
        }

        [DataSourceProperty]
        public MBBindingList<SettingSortVM> PossibleSettingList
        {
            get => _possibleSettingList;
            set
            {
                if (value != _possibleSettingList)
                {
                    _possibleSettingList = value;
                    OnPropertyChanged(nameof(PossibleSettingList));
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<SettingSortVM> SettingList
        {
            get => _settingList;
            set
            {
                if (value != _settingList)
                {
                    _settingList = value;
                    OnPropertyChanged(nameof(SettingList));
                }
            }
        }


        [DataSourceProperty] public string Name { get; set; }

        public void TransferSorter(SettingSortVM sorter, SettingSide side)
        {
            ExecuteTransfer(sorter, -1, side.GetOtherSide());
        }

        //TODO: FIX TRANSFER WITH 3 ITEMS IN LIST -> DRAG TOP TO VERY BOTTOM AND IT TRIES INDEX 3???. IF YOU TRY IN BETWEEN IT PUTS IT LAST IN LIST????
        public void ExecuteListTransfer(SettingSortVM sorter, int index, string targetTag)
        {
            if (targetTag == "SettingList")
            {
                if (sorter.Side != SettingSide.RIGHT)
                    ExecuteTransfer(sorter, index, SettingSide.RIGHT);
                else
                {
                    _settingList.Remove(sorter);
                    if (index > _settingList.Count)
                        _settingList.Insert(index - 1, sorter);
                    else
                        _settingList.Insert(index, sorter);
                }
            }
            else if (targetTag == "PossibleSettingList")
            {
                if (sorter.Side != SettingSide.LEFT)
                    ExecuteTransfer(sorter, index, SettingSide.LEFT);
                else
                {
                    _possibleSettingList.Remove(sorter);
                    if (index > _possibleSettingList.Count)
                        _possibleSettingList.Insert(index - 1, sorter);
                    else
                        _possibleSettingList.Insert(index, sorter);
                }
            }
        }

        public new void OnFinalize()
        {
            base.OnFinalize();
            SaveSettingList();

            _possibleSettingList = null;
            _settingList = null;
        }

        public void SaveSettingList()
        {
            if (_settingList.Count > 0)
            {
                PartyScreenConfig.Sorter = GetFullPartySorter(0);
            }
            else
            {
                PartyScreenConfig.Sorter = new AlphabetComparer(null, false);
            }

            PartyScreenConfig.SaveSorter();
        }

        private void ExecuteTransfer(SettingSortVM sorter, int index, SettingSide sideToMoveTo)
        {
            if (sideToMoveTo == SettingSide.LEFT)
            {
                sorter.UpdateValues(sideToMoveTo);
                _possibleSettingList.Insert(index != -1 ? index : _possibleSettingList.Count, sorter);
                _settingList.Remove(sorter);
            }
            else if (sideToMoveTo == SettingSide.RIGHT)
            {
                sorter.UpdateValues(sideToMoveTo);
                _settingList.Insert(index != -1 ? index : _settingList.Count, sorter);
                _possibleSettingList.Remove(sorter);
            }
        }

        private PartySort GetFullPartySorter(int n)
        {
            if (n == _settingList.Count - 1)
            {
                return _settingList[n].SortingComparer;
            }

            PartySort toReturn = _settingList[n].SortingComparer;
            toReturn.EqualSorter = GetFullPartySorter(n + 1);
            return toReturn;
        }

        private void InitialiseSettingLists()
        {
            InitialisePossibleSettingList();
            InitialiseSettingList();

            foreach (SettingSortVM settingSortVm in _settingList)
            {
                for (var i = 0; i < _possibleSettingList.Count; i++)
                {
                    if (_possibleSettingList[i].SortingComparer.GetType() == settingSortVm.SortingComparer.GetType())
                    {
                        _possibleSettingList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        //TODO: Consider using reflection here instead of manual declaration
        private void InitialisePossibleSettingList()
        {
            _possibleSettingList.Add(new SettingSortVM(new AlphabetComparer(null, false), TransferSorter,
                SettingSide.LEFT, _parent.OpenSubSetting));
            _possibleSettingList.Add(new SettingSortVM(new BasicTypeComparer(null, false), TransferSorter,
                SettingSide.LEFT, _parent.OpenSubSetting));
            _possibleSettingList.Add(new SettingSortVM(new TypeComparer(null, false), TransferSorter, SettingSide.LEFT,
                _parent.OpenSubSetting));
            _possibleSettingList.Add(new SettingSortVM(new LevelComparer(null, true), TransferSorter, SettingSide.LEFT,
                _parent.OpenSubSetting));
            _possibleSettingList.Add(new SettingSortVM(new TrueTierComparer(null, true), TransferSorter,
                SettingSide.LEFT, _parent.OpenSubSetting));
            _possibleSettingList.Add(new SettingSortVM(new CultureComparer(null, false), TransferSorter,
                SettingSide.LEFT, _parent.OpenSubSetting));
            _possibleSettingList.Add(new SettingSortVM(new NumberComparer(null, true), TransferSorter, SettingSide.LEFT,
                _parent.OpenSubSetting));
            _possibleSettingList.Add(new SettingSortVM(new UpgradeableComparer(null), TransferSorter, SettingSide.LEFT,
                _parent.OpenSubSetting));
        }

        private void InitialiseSettingList()
        {
            for (PartySort currentSort = PartyScreenConfig.Sorter;
                currentSort != null;
                currentSort = currentSort.EqualSorter)
            {
                PartySort freshSorter;
                if (currentSort.HasCustomSettings())
                {
                    freshSorter =
                        currentSort.GetType()
                            .GetConstructor(new[] {typeof(PartySort), typeof(bool), typeof(List<string>)})
                            ?.Invoke(new object[]
                                {null, currentSort.Descending, currentSort.CustomSettingsList}) as PartySort;
                }
                else
                {
                    freshSorter = currentSort.GetType().GetConstructor(new[] {typeof(PartySort), typeof(bool)})
                        ?.Invoke(new object[] {null, currentSort.Descending}) as PartySort;
                }

                var settingSortVM =
                    new SettingSortVM(freshSorter, TransferSorter, SettingSide.RIGHT, _parent.OpenSubSetting);
                _settingList.Add(settingSortVM);
            }
        }
    }
}