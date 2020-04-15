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
            _partyEnhancementsVm = null;
            _possibleSettingList = null;
            _settingList = null;
        }

        public void TransferSorter(SettingSortVM sorter, SettingSide side)
        {
            InformationManager.DisplayMessage(new InformationMessage($"Transfer from character: {sorter.Name} - {side}"));
        }

        public void ExecuteCloseSettings()
        {
            this.OnFinalize();
            _partyEnhancementsVm.CloseSettingView();
        }

        public void ExecuteListTransfer(SettingSortVM sorter, int index, string targetTag)
        {
            InformationManager.DisplayMessage(new InformationMessage($"Transfer from list: {sorter.Name} - {index} - {targetTag}"));
        }

        private void InitialiseSettingLists()
        {
            InitialisePossibleSettingList();
            InitialiseSettingList();
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
