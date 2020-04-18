using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.Saving;
using PartyScreenEnhancements.ViewModel.Settings.SortingOrders;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings
{
    public class SettingScreenVM : TaleWorlds.Library.ViewModel
    {

        private PartyEnhancementsVM _partyEnhancementsVm;

        private SettingSorterPaneVM _sorterPane;
        private SettingGeneralPaneVM _generalPane;

        private GauntletLayer _subSettingLayer;
        private GauntletPartyScreen _parentScreen;
        private GauntletMovie _currentMovie;
        private SettingSortingOrderScreenVM _subScreen;

        public SettingScreenVM(PartyEnhancementsVM parent, GauntletPartyScreen parentScreen)
        {
            this._partyEnhancementsVm = parent;
            this._parentScreen = parentScreen;
            this._sorterPane = new SettingSorterPaneVM(this);
            this._generalPane = new SettingGeneralPaneVM();

            if(Game.Current != null)
                Game.Current.AfterTick = (Action<float>)Delegate.Combine(Game.Current.AfterTick, new Action<float>(this.AfterTick));
        }

        public void AfterTick(float dt)
        {
            if (_partyEnhancementsVm.IsHotKeyPressed("Exit"))
            {
                ExecuteCloseSettings();
            }

            if (_subSettingLayer != null && _subSettingLayer.Input.IsHotKeyReleased("Exit"))
            {
                _subScreen.ExecuteCloseSettings();
            }
        }

        public void ExecuteCloseSettings()
        {
            _partyEnhancementsVm.CloseSettingView();
            _sorterPane.OnFinalize();
            _generalPane.OnFinalize();
            this.OnFinalize();
        }

        public void OpenSubSetting(SettingSortVM sortVm)
        {
            if (_subSettingLayer == null)
            {
                _subSettingLayer = new GauntletLayer(300);
                _subScreen = new SettingSortingOrderScreenVM(this, sortVm.SortingComparer);
                _currentMovie = _subSettingLayer.LoadMovie("PartyEnhancementSortingSettings", _subScreen);
                _subSettingLayer.IsFocusLayer = true;
                ScreenManager.TrySetFocus(_subSettingLayer);
                this._subSettingLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
                _parentScreen.AddLayer(_subSettingLayer);
                this._subSettingLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            }
        }

        public void CloseSubSetting()
        {
            if (_subSettingLayer != null)
            {
                _subSettingLayer.ReleaseMovie(_currentMovie);
                _parentScreen.RemoveLayer(_subSettingLayer);
                _subSettingLayer.InputRestrictions.ResetInputRestrictions();
                _subSettingLayer = null;
                _subScreen = null;
                this.RefreshValues();
            }
        }

        public new void OnFinalize()
        {
            base.OnFinalize();
            PartyScreenConfig.Save();
            if (Game.Current != null)
                Game.Current.AfterTick = (Action<float>)Delegate.Remove(Game.Current.AfterTick, new Action<float>(this.AfterTick));

            _partyEnhancementsVm = null;
            _sorterPane = null;
            _generalPane = null;
            
        }

        [DataSourceProperty]
        public SettingSorterPaneVM SorterPane
        {
            get => _sorterPane;
            set
            {
                if (value != this._sorterPane)
                {
                    this._sorterPane = value;
                    base.OnPropertyChanged(nameof(SorterPane));
                }
            }
        }

        [DataSourceProperty]
        public SettingGeneralPaneVM GeneralPane
        {
            get => _generalPane;
            set
            {
                if (value != this._generalPane)
                {
                    this._generalPane = value;
                    base.OnPropertyChanged(nameof(GeneralPane));
                }
            }
        }
    }
}
