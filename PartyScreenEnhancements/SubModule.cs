using HarmonyLib;
using PartyScreenEnhancements.Saving;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.TwoDimension;
using UIExtenderLib;

namespace PartyScreenEnhancements
{
    public class SubModule : MBSubModuleBase
    {
        private UIExtender _extender;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            PartyScreenConfig.Initialize();
            _extender = new UIExtender("PartyScreenEnhancements");
            _extender.Register();

            var harmony = new Harmony("top.hirtol.patch.partyenhancements");
            harmony.PatchAll();


            //UIResourceManager.UIResourceDepot.StartWatchingChangesInDepot();
        }

        protected override void OnApplicationTick(float dt)
        {
            //UIResourceManager.UIResourceDepot.CheckForChanges();
        }

        protected override void OnSubModuleUnloaded()
        {
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            _extender.Verify();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
        }
    }
}