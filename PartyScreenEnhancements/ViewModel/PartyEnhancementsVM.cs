using PartyScreenEnhancements.Saving;
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
    /// <summary>
    /// Primary VM for the overlay (includes buttons, unit tallies, etc)
    /// Holds references to all other VMs relevant
    /// </summary>
    public class PartyEnhancementsVM : TaleWorlds.Library.ViewModel
    {
        protected readonly PartyVM _partyVM;
        protected readonly PartyScreenLogic _partyScreenLogic;

        private SortAllTroopsVM _sortTroopsVM;
        private UpgradeAllTroopsVM _upgradeTroopsVM;
        private RecruitPrisonerVM _recruitPrisonerVm;
        private SettingScreenVM _settingScreenVm;
        private UnitTallyVM _unitTallyVm;
        private TransferWoundedTroopsVM _transferWounded;

        private GauntletLayer _settingLayer;
        private readonly GauntletPartyScreen _parentScreen;
        private GauntletMovie _currentMovie;

        private HintViewModel _settingsHint;


        public PartyEnhancementsVM(PartyVM partyVM, PartyScreenLogic partyScreenLogic, GauntletPartyScreen parentScreen)
        {
            _partyVM = partyVM;
            _partyScreenLogic = partyScreenLogic;
            _parentScreen = parentScreen;
            _settingsHint = new HintViewModel("Settings");

            _sortTroopsVM = new SortAllTroopsVM(_partyVM, _partyScreenLogic);
            _upgradeTroopsVM = new UpgradeAllTroopsVM(this, _partyVM, _partyScreenLogic);
            _recruitPrisonerVm = new RecruitPrisonerVM(this, _partyVM, _partyScreenLogic);
            _unitTallyVm = new UnitTallyVM(partyVM.MainPartyTroops, partyVM.OtherPartyTroops, partyScreenLogic, _partyScreenLogic?.LeftOwnerParty?.MobileParty?.IsGarrison ?? false);
            _transferWounded = new TransferWoundedTroopsVM(this, partyVM, _partyScreenLogic?.LeftOwnerParty?.MobileParty?.IsGarrison ?? false);

            _partyScreenLogic.AfterReset += AfterReset;

            RefreshValues();
        }

        public void AfterReset(PartyScreenLogic logic)
        {
            RefreshValues();
        }

        public new void RefreshValues()
        {
            base.RefreshValues();

            if (PartyScreenConfig.ExtraSettings.AutomaticSorting) _sortTroopsVM.SortTroops();

            _unitTallyVm.RefreshValues();
        }

        public new void OnFinalize()
        {
            _partyScreenLogic.AfterReset -= AfterReset;
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
                _settingLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
                _parentScreen.AddLayer(_settingLayer);
                _settingLayer.InputRestrictions.SetInputRestrictions();
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
                RefreshValues();
            }
        }

        public bool IsHotKeyPressed(string hotkey)
        {
            if (_settingLayer != null)
            {
                return _settingLayer.Input.IsHotKeyReleased(hotkey);
            }

            return false;
        }

        [DataSourceProperty]
        public HintViewModel SettingHint
        {
            get => _settingsHint;
            set
            {
                if (value != _settingsHint)
                {
                    _settingsHint = value;
                    OnPropertyChanged(nameof(SettingHint));
                }
            }
        }

        [DataSourceProperty]
        public UpgradeAllTroopsVM UpgradeAllTroops
        {
            get => _upgradeTroopsVM;
            set
            {
                if (value != _upgradeTroopsVM)
                {
                    _upgradeTroopsVM = value;
                    OnPropertyChanged(nameof(UpgradeAllTroops));
                }
            }
        }

        [DataSourceProperty]
        public RecruitPrisonerVM RecruitAllPrisoners
        {
            get => _recruitPrisonerVm;
            set
            {
                if (value != _recruitPrisonerVm)
                {
                    _recruitPrisonerVm = value;
                    OnPropertyChanged(nameof(RecruitAllPrisoners));
                }
            }
        }

        [DataSourceProperty]
        public SortAllTroopsVM SortAllTroops
        {
            get => _sortTroopsVM;
            set
            {
                if (value != _sortTroopsVM)
                {
                    _sortTroopsVM = value;
                    OnPropertyChanged(nameof(SortAllTroops));
                }
            }
        }

        [DataSourceProperty]
        public UnitTallyVM UnitTally
        {
            get => _unitTallyVm;
            set
            {
                if (value != _unitTallyVm)
                {
                    _unitTallyVm = value;
                    OnPropertyChanged(nameof(UnitTally));
                }
            }
        }

        [DataSourceProperty]
        public TransferWoundedTroopsVM TransferWoundedTroops
        {
            get => _transferWounded;
            set
            {
                if (value != _transferWounded)
                {
                    _transferWounded = value;
                    OnPropertyChanged(nameof(TransferWoundedTroops));
                }
            }
        }
    }
}