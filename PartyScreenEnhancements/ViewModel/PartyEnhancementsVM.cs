using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.ViewModel.Settings;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
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
        

        public PartyEnhancementsVM(PartyVM partyVM, PartyScreenLogic partyScreenLogic, GauntletPartyScreen parentScreen)
        {
            this._partyVM = partyVM;
            this._partyScreenLogic = partyScreenLogic;
            this._sortTroopsVM = new SortAllTroopsVM(partyVM, partyScreenLogic);
            this._upgradeTroopsVM = new UpgradeAllTroopsVM(partyScreenLogic, partyVM);
            this._recruitPrisonerVm = new RecruitPrisonerVM(partyVM, partyScreenLogic);
            this._parentScreen = parentScreen;
        }

        public void OpenSettingView()
        {
            if (_settingLayer == null)
            {
                _settingLayer = new GauntletLayer(200);
                _settingScreenVm = new SettingScreenVM(this);
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
        protected SettingScreenVM _settingScreenVm;

    }
}
