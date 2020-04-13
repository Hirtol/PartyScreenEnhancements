﻿using HarmonyLib;
using System;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace PartyScreenEnhancements
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            var harmony = new Harmony("top.hirtol.patch");
            harmony.PatchAll();

            UIResourceManager.UIResourceDepot.StartWatchingChangesInDepot();
        }

        protected override void OnApplicationTick(float dt)
        {
            UIResourceManager.UIResourceDepot.CheckForChanges();
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
