using System;
using System.ComponentModel;
using System.Linq;
using HarmonyLib;
using PartyScreenEnhancements.Saving;
using PartyScreenEnhancements.ViewModel.Settings;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

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
        private RecruitTillLimitVM _recruitLimitVM;

        private GauntletLayer _settingLayer;
        private readonly GauntletPartyScreen _parentScreen;
        private IGauntletMovie _currentMovie;

        private HintViewModel _settingsHint;


        public PartyEnhancementsVM(PartyVM partyVM, PartyScreenLogic partyScreenLogic, GauntletPartyScreen parentScreen)
        {
            _partyVM = partyVM;
            _partyScreenLogic = partyScreenLogic;
            _parentScreen = parentScreen;
            _settingsHint = new HintViewModel(new TextObject("Settings"));

            _sortTroopsVM = new SortAllTroopsVM(_partyVM, _partyScreenLogic);
            _upgradeTroopsVM = new UpgradeAllTroopsVM(this, _partyVM, _partyScreenLogic);
            _recruitPrisonerVm = new RecruitPrisonerVM(this, _partyVM, _partyScreenLogic);
            _unitTallyVm = new UnitTallyVM(partyVM.MainPartyTroops, partyVM.OtherPartyTroops, partyScreenLogic, _partyScreenLogic?.LeftOwnerParty?.MobileParty?.IsGarrison ?? false);
            _transferWounded = new TransferWoundedTroopsVM(this, partyVM, _partyScreenLogic?.LeftOwnerParty?.MobileParty?.IsGarrison ?? false);
            _recruitLimitVM = new RecruitTillLimitVM(_partyVM,_partyScreenLogic);

            _partyScreenLogic.AfterReset += AfterReset;
            _partyScreenLogic.Update += UpdateLabel;
            PartyScreenConfig.ExtraSettings.PropertyChanged += OnEnableChange;

            RefreshValues();
            UpdateLabel(null);
        }

        public void UpdateLabel(PartyScreenLogic.PartyCommand command)
        {
            try
            {
                if (!PartyScreenConfig.ExtraSettings.ShouldShowCompletePartyNumber) return;

                var _otherParty = _partyVM.OtherPartyTroops;
                var _mainParty = _partyVM.MainPartyTroops;

                if (_mainParty != null && !_mainParty.IsEmpty() &&
                    (_partyScreenLogic.RightOwnerParty?.PartySizeLimit > 0))
                {
                    _partyVM.MainPartyTroopsLbl =
                        PopulatePartyList(_mainParty, _partyScreenLogic.RightOwnerParty.PartySizeLimit);
                }

                if (_otherParty != null && !_otherParty.IsEmpty() &&
                    (_partyScreenLogic.LeftOwnerParty?.PartySizeLimit > 0))
                {
                    _partyVM.OtherPartyTroopsLbl =
                        PopulatePartyList(_otherParty, _partyScreenLogic.LeftOwnerParty.PartySizeLimit);
                }
            }
            catch (Exception e)
            {
                Logging.Log(Logging.Levels.ERROR, $"Update Label: {e}");
                Utilities.DisplayMessage($"PSE UpdateLabel Exception: {e}");
            }
        }

        private string PopulatePartyList(MBBindingList<PartyCharacterVM> list, int sizeLimit)
        {
            int troopNumb = list.Sum(character => Math.Max(0, character.Troop.Number));
            return $"({troopNumb} / {sizeLimit})";
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
            _partyScreenLogic.Update -= UpdateLabel;
            PartyScreenConfig.ExtraSettings.PropertyChanged -= OnEnableChange;

            _unitTallyVm.OnFinalize();
            _recruitLimitVM.OnFinalize();
            _recruitPrisonerVm.OnFinalize();
            _upgradeTroopsVM.OnFinalize();
            _sortTroopsVM.OnFinalize();

            _unitTallyVm = null;
            _recruitLimitVM = null;
            _recruitPrisonerVm = null;
            _upgradeTroopsVM = null;
            _sortTroopsVM = null;
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

        public void OnEnableChange(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(PartyScreenConfig.ExtraSettings
                .ShouldShowCompletePartyNumber)))
            {
                Traverse.Create(_partyVM).Method("RefreshPartyInformation").GetValue();
                UpdateLabel(null);
            }
        }

        [DataSourceProperty]
        public bool AnyOtherPopupOpen
        {
            get => _partyVM.IsAnyPopUpOpen;
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

        [DataSourceProperty]
        public RecruitTillLimitVM RecruitTillLimit
        {
            get => _recruitLimitVM;
            set
            {
                if (value != _recruitLimitVM)
                {
                    _recruitLimitVM = value;
                    OnPropertyChanged(nameof(RecruitTillLimit));
                }
            }
        }
    }
}