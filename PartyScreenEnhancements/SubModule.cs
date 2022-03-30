using HarmonyLib;
using PartyScreenEnhancements.Saving;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace PartyScreenEnhancements
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            var harmony = new Harmony("top.hirtol.patch.partyenhancements");
            harmony.PatchAll();

            try
            {
                Directories.Initialize();
                Logging.Initialise();
                PartyScreenConfig.Initialize();
            }
            catch (Exception e)
            {
                FileLog.Log($"PSE Config Load Exception: {e}");
            }

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
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
        }
    }
}