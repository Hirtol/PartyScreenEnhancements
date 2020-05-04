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
        private ResourceDepot resourceDepot = UIResourceManager.UIResourceDepot;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // InitializeSprites();
            //
            // AddSprites("pse_icons");

            ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            SpriteData spriteData = UIResourceManager.SpriteData;
            SpriteData spriteData2 = new SpriteData("PSESpriteData");
            spriteData2.Load(uiresourceDepot);
            TaleWorlds.TwoDimension.Texture item = new TaleWorlds.TwoDimension.Texture(new EngineTexture(TaleWorlds.Engine.Texture.CreateTextureFromPath("../../Modules/PartyScreenEnhancements/GUI/SpriteSheets/pse_icons/", "pse_icons_1.png")));
            spriteData.SpriteCategories.Add("pse_icons", spriteData2.SpriteCategories["pse_icons"]);
            spriteData.SpritePartNames.Add("pse_sort_icon", spriteData2.SpritePartNames["pse_sort_icon"]);
            spriteData.SpriteNames.Add("pse_sort_icon", new SpriteGeneric("pse_sort_icon", spriteData2.SpritePartNames["pse_sort_icon"]));
            spriteData.SpritePartNames.Add("pse_empty_button", spriteData2.SpritePartNames["pse_empty_button"]);
            spriteData.SpriteNames.Add("pse_empty_button", new SpriteGeneric("pse_empty_button", spriteData2.SpritePartNames["pse_empty_button"]));
            SpriteCategory spriteCategory = spriteData.SpriteCategories["pse_icons"];
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
            var test = UIResourceManager.SpriteData.SpriteCategories;

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