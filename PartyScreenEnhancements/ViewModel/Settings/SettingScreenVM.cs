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
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.Settings
{
    public class SettingScreenVM : TaleWorlds.Library.ViewModel
    {

        private PartyEnhancementsVM _partyEnhancementsVm;

        private SettingSorterPaneVM _sorterPane;
        private SettingGeneralPaneVM _generalPane;

        public SettingScreenVM(PartyEnhancementsVM parent)
        {
            this._partyEnhancementsVm = parent;

            this._sorterPane = new SettingSorterPaneVM();
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
        }

        public void ExecuteCloseSettings()
        {
            _partyEnhancementsVm.CloseSettingView();
            _sorterPane.OnFinalize();
            _generalPane.OnFinalize();
            this.OnFinalize();
        }
        public new void OnFinalize()
        {
            base.OnFinalize();
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
