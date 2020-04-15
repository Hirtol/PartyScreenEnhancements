using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings
{
    public class SettingScreenVM : TaleWorlds.Library.ViewModel
    {

        private MBBindingList<SettingSortVM> _possibleSettingList;
        private MBBindingList<SettingSortVM> _settingList;
        private PartyEnhancementsVM _partyEnhancementsVm;


        public SettingScreenVM(PartyEnhancementsVM parent)
        {
            PossibleSettingList = new MBBindingList<SettingSortVM>();
            SettingList = new MBBindingList<SettingSortVM>();
            this._partyEnhancementsVm = parent;

            InitialiseSettingLists();
        }

        public void OnFinalize()
        {
            SaveSettingList();

            _partyEnhancementsVm = null;
            _possibleSettingList = null;
            _settingList = null;
        }

        public void TransferSorter(SettingSortVM sorter, SettingSide side)
        {
            ExecuteTransfer(sorter, -1, side.GetOtherSide());
            InformationManager.DisplayMessage(new InformationMessage($"Transfer from character: {sorter.Name} - {side.GetOtherSide()}"));
        }

        public void ExecuteCloseSettings()
        {
            _partyEnhancementsVm.CloseSettingView();
            this.OnFinalize();
        }

        public void ExecuteListTransfer(SettingSortVM sorter, int index, string targetTag)
        {
            if (targetTag == "SettingList")
            {
                if(sorter.Side != SettingSide.RIGHT)
                    ExecuteTransfer(sorter, index, SettingSide.RIGHT);
                else
                {

                    _settingList.Remove(sorter);
                    if (index > _settingList.Count)
                    {
                        _settingList.Add(sorter);
                    }
                    else
                    {
                        _settingList.Insert(index, sorter);
                    }
                }
            }
            else if (targetTag == "PossibleSettingList")
            {
                if(sorter.Side != SettingSide.LEFT)
                    ExecuteTransfer(sorter, index, SettingSide.LEFT);
                else
                {
                    _possibleSettingList.Remove(sorter);
                    if (index > _possibleSettingList.Count)
                    {
                        _possibleSettingList.Add(sorter);
                    }
                    else
                    {
                        _possibleSettingList.Insert(index, sorter);
                    }
                }
            }
            InformationManager.DisplayMessage(new InformationMessage($"Transfer from list: {sorter.Name} - {index} - {targetTag}"));
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

        public void SaveSettingList()
        {
            if(_settingList.Count > 0)
            {
                PartyScreenConfig.Sorter = GetFullPartySorter(0);
            }
            else
            {
                PartyScreenConfig.Sorter = new AlphabetComparer(null, false);
            }
            PartyScreenConfig.SaveSorter();
        }

        private PartySort GetFullPartySorter(int n)
        {
            // PartySort result = null;
            // for (int i = _settingList.Count-1; i != 0; i--)
            // {
            //     if (result == null)
            //     {
            //         result = _settingList[i].SortingComparer;
            //         continue;
            //     }
            //
            //     result = (_settingList[i].SortingComparer = result);
            // }
            //
            // return result;
            if (n == _settingList.Count-1)
            {
                return _settingList[n].SortingComparer;
            }
            
            var toReturn = _settingList[n].SortingComparer;
            toReturn.EqualSorter = GetFullPartySorter(n + 1);
            return toReturn;
        }

        private void InitialiseSettingLists()
        {
            InitialisePossibleSettingList();
            InitialiseSettingList();

            foreach (SettingSortVM settingSortVm in _settingList)
            {
                for (int i = 0; i < _possibleSettingList.Count; i++)
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
            _possibleSettingList.Add(new SettingSortVM(new AlphabetComparer(null, false), TransferSorter, SettingSide.LEFT));
            _possibleSettingList.Add(new SettingSortVM(new BasicTypeComparer(null, false), TransferSorter, SettingSide.LEFT));
            _possibleSettingList.Add(new SettingSortVM(new TypeComparer(null, false), TransferSorter, SettingSide.LEFT));
            _possibleSettingList.Add(new SettingSortVM(new LevelComparer(null, true), TransferSorter, SettingSide.LEFT));
            _possibleSettingList.Add(new SettingSortVM(new TrueTierComparer(null, true), TransferSorter, SettingSide.LEFT));
        }

        private void InitialiseSettingList()
        {
            for (var currentSort = PartyScreenConfig.Sorter; currentSort != null; currentSort = currentSort.EqualSorter)
            {
                var settingSortVM = new SettingSortVM(currentSort, TransferSorter, SettingSide.RIGHT);
                _settingList.Add(settingSortVM);
            }
        }


        [DataSourceProperty]
        public MBBindingList<SettingSortVM> PossibleSettingList
        {
            get
            {
                return this._possibleSettingList;
            }
            set
            {
                if (value != this._possibleSettingList)
                {
                    this._possibleSettingList = value;
                    base.OnPropertyChanged(nameof(PossibleSettingList));
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<SettingSortVM> SettingList
        {
            get
            {
                return this._settingList;
            }
            set
            {
                if (value != this._settingList)
                {
                    this._settingList = value;
                    base.OnPropertyChanged(nameof(SettingList));
                }
            }
        }

    }
}
