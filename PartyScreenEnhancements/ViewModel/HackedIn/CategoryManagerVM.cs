using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Saving;
using PartyScreenEnhancements.ViewModel.Settings.SortingOrders;
using PartyScreenEnhancements.ViewModel.Settings.Tabs;
using PartyScreenEnhancements.ViewModel.Settings.Tabs.Miscellaneous;
using PartyScreenEnhancements.ViewModel.Settings.Tabs.Sorting;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.HackedIn
{
    public class CategoryManagerVM : TaleWorlds.Library.ViewModel
    {

        private Action _parentCloseLayer;

        public CategoryManagerVM(Action parentCloseLayer)
        {
            _parentCloseLayer = parentCloseLayer;

            if (Game.Current != null)
                Game.Current.AfterTick =
                    (Action<float>) Delegate.Combine(Game.Current.AfterTick, new Action<float>(this.AfterTick));
        }

        public void AfterTick(float dt)
        {
            //TODO: Add IsHotKeyPressed("Exit") back in
            // if (_partyEnhancementsVm.IsHotKeyPressed("Exit"))
            // {
            //     ExecuteCloseSettings();
            // }
        }

        public void ExecuteCloseSettings()
        {
            _parentCloseLayer();
            this.OnFinalize();
        }

        public new void OnFinalize()
        {
            base.OnFinalize();
            PartyScreenConfig.Save();
            if (Game.Current != null)
                Game.Current.AfterTick =
                    (Action<float>) Delegate.Remove(Game.Current.AfterTick, new Action<float>(this.AfterTick));
        }
    }
}
