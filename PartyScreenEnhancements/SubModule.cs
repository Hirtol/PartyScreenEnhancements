using System;
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
        private SpriteCategory test;
        ResourceDepot resourceDepot = UIResourceManager.UIResourceDepot;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            InitializeSprites();

            //AddSprites("PartyEnhancementSpriteData");

            ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            SpriteData spriteData = UIResourceManager.SpriteData;
            SpriteData spriteData2 = new SpriteData("PartyEnhancementSpriteData");
            spriteData2.Load(uiresourceDepot);
            TaleWorlds.TwoDimension.Texture item = new TaleWorlds.TwoDimension.Texture(new EngineTexture(TaleWorlds.Engine.Texture.CreateTextureFromPath("../../Modules/PartyScreenEnhancements/GUI/SpriteSheets/enhancement_icons/", "enhancement_icons_1.png")));
            spriteData.SpriteCategories.Add("enhancement_icons", spriteData2.SpriteCategories["enhancement_icons"]);
            spriteData.SpritePartNames.Add("enhancement_sort_icons_1", spriteData2.SpritePartNames["enhancement_sort_icons_1"]);
            spriteData.SpriteNames.Add("enhancement_sort_icons_1", new SpriteGeneric("enhancement_sort_icons_1", spriteData2.SpritePartNames["enhancement_sort_icons_1"]));
            spriteData.SpritePartNames.Add("enhancement_empty_button", spriteData2.SpritePartNames["enhancement_empty_button"]);
            spriteData.SpriteNames.Add("enhancement_empty_button", new SpriteGeneric("enhancement_empty_button", spriteData2.SpritePartNames["enhancement_empty_button"]));
            SpriteCategory spriteCategory = spriteData.SpriteCategories["enhancement_icons"];
            spriteCategory.SpriteSheets.Add(item);
            spriteCategory.Load(resourceContext, uiresourceDepot);
            UIResourceManager.BrushFactory.Initialize();

            // SpriteCategory spriteCategory = UIResourceManager.SpriteData.SpriteCategories["enhancement_icons_1"];
            // spriteCategory.Load(UIResourceManager.ResourceContext, resourceDepot);
            //
            // var texture = TaleWorlds.Engine.Texture.LoadTextureFromPath("enhancement_icons_1.png",
            //     $"{BasePath.Name}Modules/PartyScreenEnhancements/GUI/SpriteSheets");
            // texture.PreloadTexture();
            // var texture2D = new TaleWorlds.TwoDimension.Texture(new EngineTexture(texture));
            // UIResourceManager.SpriteData.SpriteCategories["enhancement_icons_1"].SpriteSheets[1 - 1] = texture2D;

            _extender = new UIExtender("PartyScreenEnhancements");
            _extender.Register();

            var harmony = new Harmony("top.hirtol.patch.partyenhancements");
            harmony.PatchAll();

            try
            {
                PartyScreenConfig.Initialize();
            }
            catch (Exception e)
            {
                FileLog.Log($"PSE Config Load Exception: {e}");
            }

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
            base.OnBeforeInitialModuleScreenSetAsRoot();

            _extender.Verify();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
        }

        public void AddSprites(string spriteSheet, int sheetId = 1)
        {
            SpriteCategory spriteCategory = UIResourceManager.SpriteData.SpriteCategories[spriteSheet];
            spriteCategory.Load(UIResourceManager.ResourceContext, resourceDepot);
            var texture = TaleWorlds.Engine.Texture.LoadTextureFromPath($"{spriteSheet}_{sheetId}.png",
                $"{BasePath.Name}Modules/PartyScreenEnhancements/GUI/SpriteSheets/{spriteSheet}");
            texture.PreloadTexture();
            var texture2D = new TaleWorlds.TwoDimension.Texture(new EngineTexture(texture));
            UIResourceManager.SpriteData.SpriteCategories[spriteSheet].SpriteSheets[sheetId - 1] = texture2D;
        }

        private void InitializeSprites()
        {
            resourceDepot.AddLocation("Modules/PartyScreenEnhancements/GUI/");
            resourceDepot.CollectResources();
            UIResourceManager.SpriteData.Load(resourceDepot);
        }
    }
}