using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Mono.CompilerServices.SymbolWriter;
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

            InitialiseSprites();

            try
            {
                PartyScreenConfig.Initialize();
            }
            catch (Exception e)
            {
                FileLog.Log($"PSE Config Load Exception: {e}");
            }

            if (PartyScreenConfig.ExtraSettings.ShowVisualAdditions)
            {
                _extender = new UIExtender("PartyScreenEnhancements");
                _extender.Register();
            }

            var harmony = new Harmony("top.hirtol.patch.partyenhancements");
            harmony.PatchAll();

            UIResourceManager.UIResourceDepot.StartWatchingChangesInDepot();
        }

        protected override void OnApplicationTick(float dt)
        {
            UIResourceManager.UIResourceDepot.CheckForChanges();
        }

        

        private IEnumerable<DictionaryEntry> CastDict(IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                yield return entry;
            }
        }

        protected override void OnSubModuleUnloaded()
        {
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            if (PartyScreenConfig.ExtraSettings.ShowVisualAdditions)
                _extender?.Verify();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
        }

        public void InitialiseSprites()
        {
            var oldSpriteData = UIResourceManager.SpriteData;
            var newSpriteData = new SpriteData("PSESpriteData");
            newSpriteData.Load(UIResourceManager.UIResourceDepot);
            var item = new Texture(new EngineTexture(TaleWorlds.Engine.Texture.CreateTextureFromPath("../../Modules/PartyScreenEnhancements/GUI/SpriteSheets/pse_icons/", "pse_icons_1.png")));

            newSpriteData.SpriteCategories.ToList().ForEach(kvp => oldSpriteData.SpriteCategories.Add(kvp.Key, kvp.Value));
            newSpriteData.SpritePartNames.ToList().ForEach(kvp => oldSpriteData.SpritePartNames.Add(kvp.Key, kvp.Value));
            newSpriteData.SpriteNames.ToList().ForEach(kvp => oldSpriteData.SpriteNames.Add(kvp.Key, kvp.Value));

            var spriteCategory = oldSpriteData.SpriteCategories["pse_icons"];
            spriteCategory.SpriteSheets.Add(item);
            spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
            UIResourceManager.BrushFactory.Initialize();
        }
    }
}