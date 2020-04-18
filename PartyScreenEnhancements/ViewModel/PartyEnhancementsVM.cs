using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.ViewModel.Settings;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel
{
    public class PartyEnhancementsVM : TaleWorlds.Library.ViewModel
    {

        protected readonly PartyVM _partyVM;
        protected readonly PartyScreenLogic _partyScreenLogic;
        

        private GauntletLayer _settingLayer;
        private GauntletPartyScreen _parentScreen;
        private GauntletMovie _currentMovie;

        private HintViewModel _settingsHint;
        

        public PartyEnhancementsVM(PartyVM partyVM, PartyScreenLogic partyScreenLogic, GauntletPartyScreen parentScreen)
        {
            this._partyVM = partyVM;
            this._partyScreenLogic = partyScreenLogic;
            this._sortTroopsVM = new SortAllTroopsVM(this);
            this._upgradeTroopsVM = new UpgradeAllTroopsVM(this);
            this._recruitPrisonerVm = new RecruitPrisonerVM(this);
            this._unitTallyVm = new UnitTallyVM(partyVM.MainPartyTroops);
            this._parentScreen = parentScreen;
            this._settingsHint = new HintViewModel("Settings");
            this._partyScreenLogic.AfterReset += AfterReset;
        }

        public void AfterReset(PartyScreenLogic logic)
        {
            this.RefreshValues();
        }
        public new void RefreshValues()
        {
            base.RefreshValues();
            this._unitTallyVm.RefreshValues();
        }

        public new void OnFinalize()
        {
            this._partyScreenLogic.AfterReset -= AfterReset;
        }

        public void OpenSettingView()
        {
            if (_settingLayer == null)
            {
                _settingLayer = new GauntletLayer(200);
                _settingScreenVm = new SettingScreenVM(this, _parentScreen);
                _currentMovie = _settingLayer.LoadMovie("PartyEnhancementSettings", _settingScreenVm);
                _settingLayer.IsFocusLayer = true;
                ScreenManager.TrySetFocus(_settingLayer);
                this._settingLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
                _parentScreen.AddLayer(_settingLayer);
                this._settingLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            }
        }

        public void CloseSettingView()
        {
            if (_settingLayer != null)
            {
                _settingLayer.ReleaseMovie(_currentMovie);
                _parentScreen.RemoveLayer(_settingLayer);
                _settingLayer.InputRestrictions.ResetInputRestrictions();
                _settingLayer = null;
                _settingScreenVm = null;
                this.RefreshValues();
            }
        }

        public bool IsHotKeyPressed(string hotkey)
        {
            if (this._settingLayer != null)
            {
                return this._settingLayer.Input.IsHotKeyReleased(hotkey);
            }

            return false;
        }

        [DataSourceProperty]
        public HintViewModel SettingHint
        {
            get
            {
                return this._settingsHint;
            }
            set
            {
                if (value != this._settingsHint)
                {
                    this._settingsHint = value;
                    base.OnPropertyChanged(nameof(SettingHint));
                }
            }
        }

        [DataSourceProperty]
        public UpgradeAllTroopsVM UpgradeAllTroops
        {
            get
            {
                return _upgradeTroopsVM;
            }
            set
            {
                if (value != this._upgradeTroopsVM)
                {
                    this._upgradeTroopsVM = value;
                    base.OnPropertyChanged("UpgradeAllTroops");
                }
            }
        }

        [DataSourceProperty]
        public RecruitPrisonerVM RecruitAllPrisoners
        {
            get
            {
                return _recruitPrisonerVm;
            }
            set
            {
                if (value != this._recruitPrisonerVm)
                {
                    this._recruitPrisonerVm = value;
                    base.OnPropertyChanged("RecruitAllPrisoners");
                }
            }
        }

        [DataSourceProperty]
        public SortAllTroopsVM SortAllTroops
        {
            get
            {
                return _sortTroopsVM;
            }
            set
            {
                if (value != this._sortTroopsVM)
                {
                    this._sortTroopsVM = value;
                    base.OnPropertyChanged("SortAllTroops");
                }
            }
        }

        [DataSourceProperty]
        public UnitTallyVM UnitTally
        {
            get
            {
                return _unitTallyVm;
            }
            set
            {
                if (value != this._unitTallyVm)
                {
                    this._unitTallyVm = value;
                    base.OnPropertyChanged(nameof(UnitTally));
                }
            }
        }

        [DataSourceProperty]
        public PartyVM EnhancementPartyVM
        {
            get
            {
                return this._partyVM;
            }
        }

        [DataSourceProperty]
        public PartyScreenLogic EnhancementPartyLogic
        {
            get
            {
                return this._partyScreenLogic;
            }
        }

        private SortAllTroopsVM _sortTroopsVM;
        private UpgradeAllTroopsVM _upgradeTroopsVM;
        private RecruitPrisonerVM _recruitPrisonerVm;
        private SettingScreenVM _settingScreenVm;
        private UnitTallyVM _unitTallyVm;

    }
}
